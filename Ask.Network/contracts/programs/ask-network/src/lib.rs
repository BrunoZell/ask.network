use account::*;
use anchor_lang::prelude::*;
use anchor_lang::solana_program::entrypoint::*;

mod account;
mod state;

declare_id!("4ktm3bQPuEfsyGRR95QrkRdcrfb268hGzgjDr9Y17FGE");

#[program]
pub mod ask_network {
    use super::*;

    pub fn initialize_user(ctx: Context<InitializeUser>) -> ProgramResult {
        msg!("Initializing user: {}", ctx.accounts.user.key());

        ctx.accounts.user_account.running_ask_ordinal = 0;

        // Anchor creates user's token account if needed

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
