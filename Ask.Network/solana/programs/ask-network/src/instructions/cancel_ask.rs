use crate::state::*;
use anchor_lang::prelude::*;

#[derive(AnchorSerialize, AnchorDeserialize)]
pub struct CancelAskArgs {
    ordinal: u64,
}

#[derive(Accounts)]
#[instruction(args: CancelAskArgs)]
pub struct CancelAsk<'info> {
    #[account(
        mut,
        close = user_login, // after the instruction is executed, the 'ask' account will be closed, and any remaining lamports will be transferred to the 'user' account.
        seeds = [user_account.key().as_ref(), &args.ordinal.to_le_bytes()],
        bump)]
    pub ask: Account<'info, Ask>,

    #[account(
        seeds = [user_login.key().as_ref()],
        bump)]
    pub user_account: Account<'info, User>,

    #[account(mut)]
    pub user_login: Signer<'info>,

    pub system_program: Program<'info, System>,
}

impl CancelAsk<'_> {
    fn validate(&self, _args: &CancelAskArgs) -> Result<()> {
        // Todo: Implement account and argument validation
        Ok(())
    }

    #[access_control(ctx.accounts.validate(&args))]
    pub fn handle(ctx: Context<Self>, args: CancelAskArgs) -> Result<()> {
        msg!(
            "User {} cancelled ask: {}",
            ctx.accounts.user_account.key(),
            &ctx.accounts.ask.content
        );

        Ok(())
    }
}
