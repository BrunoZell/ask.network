use account::*;
use anchor_lang::solana_program::entrypoint::*;
use anchor_lang::solana_program::program_pack::Pack;
use anchor_lang::{prelude::*, system_program};
use anchor_spl::token::{self, InitializeMint, Mint, MintTo};
use state::*;

mod account;
mod errors;
mod state;

declare_id!("EarWDrZeaMyMRuiWXVuFH2XKJ96Mg6W6h9rv51BCHgRD");

#[program]
pub mod ask_network {
    use anchor_spl::token::TokenAccount;

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

    pub fn acquire_token(ctx: Context<AcquireToken>, ask_amount: u64) -> Result<()> {
        // Calculate total cost of this purchase in SOL
        let lamport_amount = ask_amount / 1; // assume a 1:1 SOL/ASK purchase price for now

        // Ensure the signing user has enough SOL
        if ctx.accounts.user.lamports() < lamport_amount {
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
                    from: ctx.accounts.user.to_account_info().clone(),
                    to: ctx.accounts.community_treasury.clone(),
                },
            ),
            lamport_amount,
        )?;

        // Mint new $ASK tokens
        anchor_spl::token::mint_to(
            CpiContext::new_with_signer(
                ctx.accounts.token_program.to_account_info(),
                anchor_spl::token::MintTo {
                    authority: ctx.accounts.token_authority.to_account_info(),
                    to: ctx.accounts.user_token_account.to_account_info(),
                    mint: ctx.accounts.token_mint.to_account_info(),
                },
                &[&[b"authority", &[*ctx.bumps.get("authority").unwrap()]]],
            ),
            ask_amount,
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

        let clock = Clock::get()?;
        let claim = TreasuryClaim {
            ordinal: ctx.accounts.treasury_claims_ordinal.claims_issued, // After increment, to make this 1-based.
            unit_of_value: TreasuryCurrency::SOL,
            deposit_amount: lamport_amount,
            deposit_timestamp: clock.unix_timestamp,
        };

        Ok(())
    }
}
