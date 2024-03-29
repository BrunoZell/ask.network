use crate::state::*;
use anchor_lang::prelude::*;

#[derive(AnchorSerialize, AnchorDeserialize)]
pub struct SignUpUserArgs {}

#[derive(Accounts)]
pub struct SignUpUser<'info> {
    #[account(
        init,
        seeds= [user_login.key().as_ref()],
        bump,
        space = User::SIZE,
        payer = user_login)]
    pub user_account: Account<'info, User>,

    #[account(mut)]
    pub user_login: Signer<'info>,

    pub system_program: Program<'info, System>,
    pub rent: Sysvar<'info, Rent>,
}

impl SignUpUser<'_> {
    fn validate(&self, _args: &SignUpUserArgs) -> Result<()> {
        // fee and accs checked in invariant
        Ok(())
    }

    #[access_control(ctx.accounts.validate(&args))]
    pub fn handle(ctx: Context<Self>, args: SignUpUserArgs) -> Result<()> {
        msg!(
            "Initializing personal account named {} for login address {}",
            ctx.accounts.user_account.key(),
            ctx.accounts.user_login.key()
        );

        // Anchor creates user_account
        ctx.accounts.user_account.running_ask_ordinal = 0;

        Ok(())
    }
}
