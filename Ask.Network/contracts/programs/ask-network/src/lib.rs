use account::*;
use anchor_lang::prelude::*;
use anchor_lang::solana_program::entrypoint::*;
use anchor_spl::metadata::mpl_token_metadata;

use state::*;

mod account;
mod errors;
mod state;

declare_id!("EarWDrZeaMyMRuiWXVuFH2XKJ96Mg6W6h9rv51BCHgRD");

#[program]
pub mod ask_network {
    use super::*;

    const COMMUNITY_TREASURY_ADDRESS: &str = "DsXqkMYq54AdNoqjHg1f8R7JxPbzcssZSnXm11DDiwa6";

    pub fn initialize_user(ctx: Context<InitializeUser>) -> ProgramResult {
        msg!("Initializing user: {}", ctx.accounts.user.key());

        ctx.accounts.user_account.running_ask_ordinal = 0;

        // Anchor creates user's token account if needed

        Ok(())
    }

    pub fn initialize_token(_ctx: Context<InitializeToken>) -> Result<()> {
        msg!("Token mint initialized");
        Ok(())
    }

    pub fn initialize_treasury_claims(ctx: Context<InitializeTreasuryClaims>) -> Result<()> {
        msg!("Treasury claims initialized");

        ctx.accounts.treasury_claims_ordinal.claims_issued = 0;

        // Initialize Treasury Claim NFT Collection.
        // There only is a single collection NFT for all treasury claims.
        // This mints the singleton treasury claim SPL NFT to a program-owned ATA.
        anchor_spl::token::mint_to(
            CpiContext::new_with_signer(
                ctx.accounts.token_program.to_account_info(),
                anchor_spl::token::MintTo {
                    authority: ctx
                        .accounts
                        .treasury_claims_collection_authority
                        .to_account_info(),
                    to: ctx
                        .accounts
                        .treasury_claims_collection_ata
                        .to_account_info(),
                    mint: ctx
                        .accounts
                        .treasury_claims_collection_mint
                        .to_account_info(),
                },
                &[&[
                    b"treasury_claims_collection_authority",
                    &[ctx.bumps.treasury_claims_collection_authority],
                ]],
            ),
            1, // Amount of 1 for NFTs
        )?;

        // Create Treasury Claim Collection NFT Metadata
        anchor_spl::metadata::create_metadata_accounts_v3(
            CpiContext::new(
                ctx.accounts.metadata_program.to_account_info(),
                anchor_spl::metadata::CreateMetadataAccountsV3 {
                    // Metadata Account: This is the account where the metadata for the token will be stored.
                    // It must be writable because the function will initialize or modify this account with the metadata details.
                    metadata: ctx.accounts.metadata.to_account_info(),
                    // Mint Account: This is the mint account of the token (NFT) for which you're creating the metadata.
                    mint: ctx
                        .accounts
                        .treasury_claims_collection_mint
                        .to_account_info(),
                    // Mint Authority: This account has the authority to mint new tokens.
                    // It's required to sign the transaction as it's a critical operation involving the token properties.
                    mint_authority: ctx
                        .accounts
                        .treasury_claims_collection_authority
                        .to_account_info(),
                    // Update Authority: This account has the authority to update the metadata in the future.
                    // It's often the same as the mint authority, but it can be different.
                    // It must also be a signer to authorize this role.
                    update_authority: ctx
                        .accounts
                        .treasury_claims_collection_authority
                        .to_account_info(),
                    // Payer: This account pays for the transaction fees and any additional SOL needed to fund the new metadata account.
                    // It must be a signer because it is responsible for covering the costs of the transaction.
                    payer: ctx.accounts.signer.to_account_info(),
                    system_program: ctx.accounts.system_program.to_account_info(),
                    rent: ctx.accounts.rent.to_account_info(),
                },
            ),
            mpl_token_metadata::types::DataV2 {
                name: "ask.network Treasury Claims".to_string(),
                symbol: "ASK-T".to_string(),
                uri: "https://claims.ask.network/collection.json".to_string(),
                seller_fee_basis_points: 0,
                creators: None,
                collection: None, // This instruction defines the singleton Collection NFT, which is not part of any collection.
                uses: None,
            },
            false,
            true,
            // Setting the CollectionDetails field on this NFT makes it a Collection NFT.
            Some(mpl_token_metadata::types::CollectionDetails::V1 { size: 0 }),
        )?;

        Ok(())
    }

    pub fn place_ask(ctx: Context<PlaceAsk>, content: String) -> Result<()> {
        msg!(
            "User {} placed new ask: {}",
            ctx.accounts.user.key,
            &content
        );

        // Fill new ask with its content and index number
        ctx.accounts.ask.content = content;
        ctx.accounts.ask.ordinal = ctx.accounts.user_account.running_ask_ordinal;

        // Increment users ever-increasing ask counter
        ctx.accounts.user_account.running_ask_ordinal += 1;

        Ok(())
    }

    pub fn update_ask(ctx: Context<UpdateAsk>, content: String, _ordinal: u64) -> ProgramResult {
        msg!(
            "User {} updates ask from: {}",
            ctx.accounts.user.key,
            &ctx.accounts.ask.content
        );

        ctx.accounts.ask.content = content;

        msg!("New ask: {}", &ctx.accounts.ask.content);
        Ok(())
    }

    pub fn cancel_ask(ctx: Context<CancelAsk>, _ordinal: u64) -> ProgramResult {
        msg!(
            "User {} cancelled ask: {}",
            ctx.accounts.user.key,
            &ctx.accounts.ask.content
        );

        Ok(())
    }

    pub fn deposit_sol(ctx: Context<DepositSol>, lamport_amount: u64) -> Result<()> {
        // Ensure the signing user has enough SOL
        if ctx.accounts.depositor.lamports() < lamport_amount {
            return err!(errors::ErrorCode::InsufficientFunds);
        }

        // Unsure the SOL destination address is the community treasury
        if ctx.accounts.community_treasury.key.to_string() != COMMUNITY_TREASURY_ADDRESS {
            return err!(errors::ErrorCode::InvalidCommunityTreasuryAddress);
        }

        // Transfer SOL from purchaser to community treasury
        anchor_lang::system_program::transfer(
            CpiContext::new(
                ctx.accounts.system_program.to_account_info(),
                anchor_lang::system_program::Transfer {
                    from: ctx.accounts.depositor.to_account_info().clone(),
                    to: ctx.accounts.community_treasury.clone(),
                },
            ),
            lamport_amount,
        )?;

        // Mint new treasury claim SPL NFT to the newly created associated token account of the depositor.
        // The token mint unique to this treasury claim and the depositors ATA is initialized automatically by Anchor.
        anchor_spl::token::mint_to(
            CpiContext::new_with_signer(
                ctx.accounts.token_program.to_account_info(),
                anchor_spl::token::MintTo {
                    authority: ctx.accounts.treasury_claims_authority.to_account_info(),
                    to: ctx.accounts.treasury_claim_ata.to_account_info(),
                    mint: ctx.accounts.treasury_claim_mint.to_account_info(),
                },
                &[&[
                    b"treasury_claims_authority",
                    &[ctx.bumps.treasury_claims_authority],
                ]],
            ),
            1, // Amount of 1 for NFTs
        )?;

        // Increment the singleton claims counter, for the next mind to have another unique ordinal.
        ctx.accounts.treasury_claims_ordinal.claims_issued += 1;

        // Retrieve the ordinal (claim number) for dynamic naming and URI
        // Assign claim ordinal only after increment. To make trasury claim IDs start at #1.
        let claim_number = ctx.accounts.treasury_claims_ordinal.claims_issued;

        // Generate dynamic name and URI based on the claim number
        let name = format!("ask.network Treasury Claim #{}", claim_number);
        let uri: String = format!("https://claims.ask.network/{}.json", claim_number);

        // Populate Treasury Claim account values. This is the main account of what makes a treasury claim.
        let clock = Clock::get()?;
        ctx.accounts.this_treasury_claim.ordinal =
            ctx.accounts.treasury_claims_ordinal.claims_issued;
        ctx.accounts.this_treasury_claim.unit_of_value = TreasuryCurrency::SOL;
        ctx.accounts.this_treasury_claim.deposit_amount = lamport_amount;
        ctx.accounts.this_treasury_claim.deposit_timestamp = clock.unix_timestamp;

        anchor_spl::metadata::create_metadata_accounts_v3(
            CpiContext::new(
                ctx.accounts.metadata_program.to_account_info(),
                anchor_spl::metadata::CreateMetadataAccountsV3 {
                    // Metadata Account: This is the account where the metadata for the token will be stored.
                    // It must be writable because the function will initialize or modify this account with the metadata details.
                    metadata: ctx.accounts.metadata.to_account_info(),
                    // Mint Account: This is the mint account of the token (NFT) for which you're creating the metadata.
                    mint: ctx.accounts.treasury_claim_mint.to_account_info(),
                    // Mint Authority: This account has the authority to mint new tokens.
                    // It's required to sign the transaction as it's a critical operation involving the token properties.
                    mint_authority: ctx.accounts.treasury_claims_authority.to_account_info(),
                    // Update Authority: This account has the authority to update the metadata in the future.
                    // It's often the same as the mint authority, but it can be different.
                    // It must also be a signer to authorize this role.
                    update_authority: ctx.accounts.treasury_claims_authority.to_account_info(),
                    // Payer: This account pays for the transaction fees and any additional SOL needed to fund the new metadata account.
                    // It must be a signer because it is responsible for covering the costs of the transaction.
                    payer: ctx.accounts.depositor.to_account_info(),
                    system_program: ctx.accounts.system_program.to_account_info(),
                    rent: ctx.accounts.rent.to_account_info(),
                },
            ),
            mpl_token_metadata::types::DataV2 {
                name: name,
                symbol: "ASK-T".to_string(),
                uri: uri,
                seller_fee_basis_points: 0,
                creators: None,
                collection: None,
                uses: None,
            },
            false, // Is mutable
            true,  // Update authority is signer
            None,  // Collection details
        )?;

        Ok(())
    }
}
