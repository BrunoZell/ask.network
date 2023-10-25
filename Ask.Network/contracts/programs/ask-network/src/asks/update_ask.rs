use anchor_lang::prelude::*;
use anchor_lang::solana_program::entrypoint::*;

use super::state::Ask;
use crate::users::state::User;

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

#[derive(Accounts)]
#[instruction(content: String, ordinal: u64)]
pub struct UpdateAsk<'info> {
    #[account(
        mut, // the content of the existing 'ask' account will be mutated
        seeds = [user.key().as_ref(), &ordinal.to_le_bytes()], // 'ask' account is identified by instruction parameters
        bump,
        realloc = 8 + 4 + content.len() + 8 ,
        realloc::zero = true,
        realloc::payer = user)] // 'user' account pays fees
    pub ask: Account<'info, Ask>,

    #[account(
        seeds = [user.key().as_ref()],
        bump)]
    pub user_account: Account<'info, User>,

    #[account(mut)]
    pub user: Signer<'info>, // signer of the transaction, implying the 'user' account

    pub system_program: Program<'info, System>,
}
