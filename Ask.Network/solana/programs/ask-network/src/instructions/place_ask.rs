use crate::state::*;
use anchor_lang::prelude::*;

#[derive(AnchorSerialize, AnchorDeserialize)]
pub struct PlaceAskArgs {
    content: String,
}

#[derive(Accounts)]
#[instruction(args: PlaceAskArgs)]
pub struct PlaceAsk<'info> {
    #[account(
        init, // this 'ask' account will be initialized
        seeds = [user_account.key().as_ref(), &user_account.running_ask_ordinal.to_le_bytes()], // unique ask address from user key and ordinal
        bump,
        payer = user_login, // 'user' account pays fees
        space = 8 + 8 + 4 + args.content.len())]
    pub ask: Account<'info, Ask>,

    #[account(
        mut, // users running ask ordinal is incremented after ask placement
        seeds = [user_login.key().as_ref()], // 'user' account is derived from the users public key
        bump)]
    pub user_account: Account<'info, User>,

    #[account(mut)]
    pub user_login: Signer<'info>, // signer of the transaction, implying the 'user_login' account

    // Solana's built-in system program. Required for operations like account initialization.
    pub system_program: Program<'info, System>,
}

impl PlaceAsk<'_> {
    fn validate(&self, _args: &PlaceAskArgs) -> Result<()> {
        // fee and accs checked in invariant
        Ok(())
    }

    #[access_control(ctx.accounts.validate(&args))]
    pub fn handle(ctx: Context<Self>, args: PlaceAskArgs) -> Result<()> {
        msg!(
            "User {} placed new ask: {}",
            ctx.accounts.user_account.key(),
            &args.content
        );

        // Fill new ask with its content and index number
        ctx.accounts.ask.content = args.content;
        ctx.accounts.ask.ordinal = ctx.accounts.user_account.running_ask_ordinal;

        // Increment users ever-increasing ask counter
        ctx.accounts.user_account.running_ask_ordinal += 1;

        Ok(())
    }
}
