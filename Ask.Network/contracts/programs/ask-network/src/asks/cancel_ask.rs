use anchor_lang::prelude::*;
use anchor_lang::solana_program::entrypoint::*;

use super::state::Ask;
use crate::users::state::User;

pub fn cancel_ask(ctx: Context<CancelAsk>, ordinal: u64) -> ProgramResult {
    msg!("User {} cancelled ask: {}", ctx.accounts.user.key, &ctx.accounts.ask.content);

    Ok(())
}

#[derive(Accounts)]
#[instruction(ordinal: u64)]
pub struct CancelAsk<'info> {
    #[account(
        mut, 
        close = user, // after the instruction is executed, the 'ask' account will be closed, and any remaining lamports will be transferred to the 'user' account.
        seeds = [user.key().as_ref(), &ordinal.to_le_bytes()],
        bump)]
    pub ask: Account<'info, Ask>,

    #[account(mut)]
    pub user: Signer<'info>,

    #[account(
        seeds = [user.key().as_ref()],
        bump)]
    pub user_account: Account<'info, User>,

    pub system_program: Program<'info, System>,
}
