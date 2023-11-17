use account::*;
use anchor_lang::solana_program::entrypoint::*;
use anchor_lang::solana_program::program_pack::Pack;
use anchor_lang::{prelude::*, system_program};
use anchor_spl::token::{self, InitializeMint, Mint, MintTo};
use mpl_token_metadata::state::AssetData;
use mpl_token_metadata::state::PrintSupply::Zero;
use state::*;

mod account;
mod errors;
mod state;

declare_id!("EarWDrZeaMyMRuiWXVuFH2XKJ96Mg6W6h9rv51BCHgRD");

#[program]
pub mod ask_network {
    use anchor_spl::token::TokenAccount;
    use mpl_token_metadata::state::TokenStandard;

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

        // Mint new treasury claim NFT to the newly created associated token account of the depositor
        // ATA is initialized automatically by Anchor.
        // Also the mint unique to this treasury claim NFT is automatically initialized by Anchor
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
                    &[*ctx.bumps.get("treasury_claims_authority").unwrap()],
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
        let uri = format!("https://claims.ask.network/{}.json", claim_number);

        let clock = Clock::get()?;
        ctx.accounts.this_treasury_claim.ordinal =
            ctx.accounts.treasury_claims_ordinal.claims_issued;
        ctx.accounts.this_treasury_claim.unit_of_value = TreasuryCurrency::SOL;
        ctx.accounts.this_treasury_claim.deposit_amount = lamport_amount;
        ctx.accounts.this_treasury_claim.deposit_timestamp = clock.unix_timestamp;

        // Mint NFT Metadata
        mpl_token_metadata::instruction::create_metadata_accounts_v3(
            ctx.accounts.metadata_program.key(), // Program ID of the Metaplex Token Metadata program
            // Metadata Account: This is the account where the metadata for the token will be stored.
            // It must be writable because the function will initialize or modify this account with the metadata details.
            ctx.accounts.metadata.key(), // Metadata account (PDA)
            // Mint Account: This is the mint account of the token (NFT) for which you're creating the metadata.
            ctx.accounts.treasury_claim_mint.key(), // Mint account
            // Mint Authority: This account has the authority to mint new tokens.
            // It's required to sign the transaction as it's a critical operation involving the token properties.
            ctx.accounts.treasury_claims_authority.key(), // Mint authority
            // Payer: This account pays for the transaction fees and any additional SOL needed to fund the new metadata account.
            // It must be a signer because it is responsible for covering the costs of the transaction.
            ctx.accounts.depositor.key(), // Payer account
            // Update Authority: This account has the authority to update the metadata in the future.
            // It's often the same as the mint authority, but it can be different.
            // It must also be a signer to authorize this role.
            ctx.accounts.treasury_claim_mint.key(), // Update authority account
            "ask.network Treasury Claim #1".to_string(), // Name
            "ASK-T".to_string(),                    // Symbol
            "https://claims.ask.network/1.json".to_string(), // URI
            None,                                   // Creators
            0,                                      // Seller fee basis points
            true,                                   // Whether the primary sale happened
            true,                                   // Is mutable
            None,                                   // Collection
            None,                                   // Uses
            None,
        );

        Ok(())
    }
}
