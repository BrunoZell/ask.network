use crate::state::*;
use anchor_lang::prelude::*;

#[derive(AnchorSerialize, AnchorDeserialize)]
pub struct InitializeGlobalArgs {}

#[derive(Accounts)]
#[instruction(args: InitializeGlobalArgs)]
pub struct InitializeGlobal<'info> {
    #[account(
        init,
        seeds= [b"global"],
        bump,
        space = Global::SIZE,
        payer = signer)]
    pub global: Account<'info, Global>,

    #[account(mut)]
    pub signer: Signer<'info>,

    pub system_program: Program<'info, System>,
    pub rent: Sysvar<'info, Rent>,
}

impl InitializeGlobal<'_> {
    fn validate(&self, _args: &InitializeGlobalArgs) -> Result<()> {
        Ok(())
    }

    #[access_control(ctx.accounts.validate(&args))]
    pub fn handle(ctx: Context<Self>, args: InitializeGlobalArgs) -> Result<()> {
        ctx.accounts.global.running_account_ordinal = 0;

        Ok(())
    }
}
