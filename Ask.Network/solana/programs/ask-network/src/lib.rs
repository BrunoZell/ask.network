use accounts::*;
use anchor_lang::prelude::*;

declare_id!("4ktm3bQPuEfsyGRR95QrkRdcrfb268hGzgjDr9Y17FGE");

#[program]
pub mod ask_network {
    use super::*;

    // Initialization
    
    pub fn initialize_user(ctx: Context<InitializeUser>) -> Result<()> {
        msg!("Initializing user {} for login account {}", ctx.accounts.user_account.key(), ctx.accounts.user_login);

        ctx.accounts.user_account.running_ask_ordinal = 0;

        // Anchor creates user's token account if needed

        Ok(())
    }

    // Ask Management
    
    pub fn place_ask(ctx: Context<PlaceAsk>, content: String) -> Result<()> {
        msg!("User {} placed new ask: {}", ctx.accounts.user_account.key, &content);

        // Fill new ask with its content and index number
        ctx.accounts.ask.content = content;
        ctx.accounts.ask.ordinal = ctx.accounts.user_account.running_ask_ordinal;
        
        // Increment users ever-increasing ask counter
        ctx.accounts.user_account.running_ask_ordinal += 1;

        Ok(())
    }

    pub fn update_ask(ctx: Context<UpdateAsk>, content: String, _ordinal: u64) -> Result<()> {
        msg!("User {} updates ask from: {}", ctx.accounts.user_account.key, &ctx.accounts.ask.content);

        ctx.accounts.ask.content = content;
        
        msg!("New ask: {}", &ctx.accounts.ask.content);
        Ok(())
    }

    pub fn cancel_ask(ctx: Context<CancelAsk>, _ordinal: u64) -> Result<()> {
        msg!("User {} cancelled ask: {}", ctx.accounts.user_account.key, &ctx.accounts.ask.content);

        Ok(())
    }
}

