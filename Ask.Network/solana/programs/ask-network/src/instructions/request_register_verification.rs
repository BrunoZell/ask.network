use crate::state::*;
use anchor_lang::prelude::*;

#[derive(AnchorSerialize, AnchorDeserialize)]
pub struct RequestRegisterVerificationArgs {}

#[derive(Accounts)]
#[instruction(args: RequestRegisterVerificationArgs)]
pub struct RequestRegisterVerification<'info> {
    pub system_program: Program<'info, System>,
    pub rent: Sysvar<'info, Rent>,
}

impl RequestRegisterVerification<'_> {
    fn validate(&self, _args: &RequestRegisterVerificationArgs) -> Result<()> {
        // Todo: Implement account and argument validation
        Ok(())
    }

    #[access_control(ctx.accounts.validate(&args))]
    pub fn handle(ctx: Context<Self>, args: RequestRegisterVerificationArgs) -> Result<()> {
        // Todo: Implement instruction
        Ok(())
    }
}
