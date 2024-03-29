use crate::state::*;
use anchor_lang::prelude::*;

#[derive(AnchorSerialize, AnchorDeserialize)]
pub struct SignUpOrganizationArgs {}

#[derive(Accounts)]
pub struct SignUpOrganization<'info> {
    #[account(
        init,
        seeds= [initial_member_login.key().as_ref()],
        bump,
        space = Organization::SIZE,
        payer = initial_member_login)]
    pub organization_account: Account<'info, Organization>,

    #[account(mut)]
    pub initial_member_login: Signer<'info>,

    pub system_program: Program<'info, System>,
    pub rent: Sysvar<'info, Rent>,
}

impl SignUpOrganization<'_> {
    fn validate(&self, _args: &SignUpOrganizationArgs) -> Result<()> {
        // fee and accs checked in invariant
        Ok(())
    }

    #[access_control(ctx.accounts.validate(&args))]
    pub fn handle(ctx: Context<Self>, args: SignUpOrganizationArgs) -> Result<()> {
        msg!(
            "Initializing organizational account named {} with initial members login address {}",
            ctx.accounts.organization_account.key(),
            ctx.accounts.initial_member_login.key()
        );

        // Anchor creates organization_account
        ctx.accounts.organization_account.running_ask_ordinal = 0;

        Ok(())
    }
}
