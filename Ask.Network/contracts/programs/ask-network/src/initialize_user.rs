use anchor_lang::{
    prelude::*,
    solana_program::instruction,
    solana_program::entrypoint::*
};

use crate::state::*;

pub fn initialize_user(ctx: Context<InitializeUser>) -> ProgramResult {
    msg!("Initializing user: {}", ctx.accounts.user.key());

    ctx.accounts.user_account.running_ask_ordinal = 0;

    // Anchor creates user's token account if needed

    Ok(())
}

#[derive(Accounts)]
pub struct InitializeUser<'info> {
    #[account(
        init,
        seeds= [user.key().as_ref()],
        bump,
        space = 8 + 8 + 8,
        payer = user)]
    pub user_account: Account<'info, User>,

    #[account(mut)]
    pub user: Signer<'info>,

    pub system_program: Program<'info, System>,
    pub rent: Sysvar<'info, Rent>,
}
