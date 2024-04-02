use crate::state::*;
use anchor_lang::prelude::*;

#[derive(AnchorSerialize, AnchorDeserialize)]
pub struct CompleteRegisterVerificationArgs {}

#[derive(Accounts)]
#[instruction(args: CompleteRegisterVerificationArgs)]
pub struct CompleteRegisterVerification<'info> {
    pub system_program: Program<'info, System>,
    pub rent: Sysvar<'info, Rent>,
}

impl CompleteRegisterVerification<'_> {
    fn validate(&self, _args: &CompleteRegisterVerificationArgs) -> Result<()> {
        // Todo: Implement account and argument validation
        Ok(())
    }

    #[access_control(ctx.accounts.validate(&args))]
    pub fn handle(ctx: Context<Self>, args: CompleteRegisterVerificationArgs) -> Result<()> {
        // Todo: Implement instruction
        Ok(())
    }
}
