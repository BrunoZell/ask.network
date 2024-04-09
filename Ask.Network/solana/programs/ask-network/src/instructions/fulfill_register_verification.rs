use crate::state::*;
use anchor_lang::prelude::*;

#[derive(AnchorSerialize, AnchorDeserialize)]
pub struct FulfillRegisterVerificationArgs {}

#[derive(Accounts)]
#[instruction(args: FulfillRegisterVerificationArgs)]
pub struct FulfillRegisterVerification<'info> {
    pub system_program: Program<'info, System>,
    pub rent: Sysvar<'info, Rent>,
}

impl FulfillRegisterVerification<'_> {
    fn validate(&self, _args: &FulfillRegisterVerificationArgs) -> Result<()> {
        // Todo: Implement account and argument validation
        Ok(())
    }

    #[access_control(ctx.accounts.validate(&args))]
    pub fn handle(ctx: Context<Self>, args: FulfillRegisterVerificationArgs) -> Result<()> {
        // Todo: Implement instruction
        Ok(())
    }
}
