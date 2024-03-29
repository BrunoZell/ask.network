use crate::state::*;
use anchor_lang::prelude::*;

#[derive(AnchorSerialize, AnchorDeserialize)]
pub struct SignUpOrganizationArgs {
    alias: String,
}

#[derive(Accounts)]
#[instruction(args: SignUpOrganizationArgs)]
pub struct SignUpOrganization<'info> {
    #[account(
        init,
        seeds = [b"organization", &global.running_organization_ordinal.to_le_bytes()[..]],
        bump,
        space = 8 + (4 + args.alias.len()),
        payer = initial_member_login)]
    pub organization_account: Account<'info, Organization>,

    #[account(seeds= [b"user", initial_member_login.key().as_ref()], bump)]
    pub initial_member_account: Account<'info, User>,

    #[account(mut)]
    pub initial_member_login: Signer<'info>,

    #[account(mut, seeds = [b"global"], bump)]
    pub global: Account<'info, Global>,

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
            "Initializing organizational account named {} under accoount {} with the initial member account of {}",
            ctx.accounts.organization_account.alias,
            ctx.accounts.organization_account.key(),
            ctx.accounts.initial_member_login.key()
        );

        // Anchor creates the organization_account
        ctx.accounts.organization_account.alias = args.alias;

        // Increment global organization ordinal for next signup
        ctx.accounts.global.running_organization_ordinal += 0;

        Ok(())
    }
}
