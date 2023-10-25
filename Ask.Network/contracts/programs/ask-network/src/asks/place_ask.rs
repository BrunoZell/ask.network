use anchor_lang::prelude::*;
use anchor_lang::solana_program::entrypoint::*;

use super::state::Ask;
use crate::users::state::User;

pub fn place_ask(ctx: Context<PlaceAsk>, content: String) -> ProgramResult {
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

#[derive(Accounts)]
#[instruction(content: String)]
pub struct PlaceAsk<'info> {
    #[account(
        init, // this 'ask' account will be initialized
        seeds = [user.key().as_ref(), &user_account.running_ask_ordinal.to_le_bytes()], // unique ask address from user key and ordinal
        bump,
        payer = user, // 'user' account pays fees
        space = 8 + 4 + content.len() + 8)]
    pub ask: Account<'info, Ask>,

    #[account(
        mut, // users running ask ordinal is incremented after ask placement
        seeds = [user.key().as_ref()], // 'user' account is derived from the users public key
        bump)]
    pub user_account: Account<'info, User>,

    #[account(mut)]
    pub user: Signer<'info>, // signer of the transaction, implying the 'user' account

    // Solana's built-in system program. Required for operations like account initialization.
    pub system_program: Program<'info, System>,
}
