use account::*;
use anchor_lang::prelude::*;
use anchor_lang::solana_program::entrypoint::*;
use anchor_spl::{
    associated_token::AssociatedToken,
    token::{Mint, Token, TokenAccount},
};
use errors::*;

mod account;
mod errors;
mod state;

declare_id!("4ktm3bQPuEfsyGRR95QrkRdcrfb268hGzgjDr9Y17FGE");

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

    pub fn update_ask(ctx: Context<UpdateAsk>, content: String, ordinal: u64) -> ProgramResult {
        msg!(
            "User {} updates ask from: {}",
            ctx.accounts.user.key,
            &ctx.accounts.ask.content
        );

        ctx.accounts.ask.content = content;

        msg!("New ask: {}", &ctx.accounts.ask.content);
        Ok(())
    }

    pub fn cancel_ask(ctx: Context<CancelAsk>, ordinal: u64) -> ProgramResult {
        msg!(
            "User {} cancelled ask: {}",
            ctx.accounts.user.key,
            &ctx.accounts.ask.content
        );

        Ok(())
    }
}
