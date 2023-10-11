use anchor_lang::prelude::*;

declare_id!("4ktm3bQPuEfsyGRR95QrkRdcrfb268hGzgjDr9Y17FGE");

#[program]
pub mod ask_network {
    use super::*;
    
    pub fn cancel_ask(ctx: Context<CancelAsk>, _ask_index: u64) -> Result<()> {
        msg!("Ask of user {} cancelled: {}", ctx.accounts.user.key, ctx.accounts.ask.content);
        Ok(())
    }
}

#[derive(Accounts)]
#[instruction(ask_index: u64)]
pub struct CancelAsk<'info> {
    #[account(
        mut, 
        close = user, // after the instruction is executed, the 'ask' account will be closed, and any remaining lamports will be transferred to the 'user' account.
        seeds = [user.key().as_ref(), &ask_index.to_le_bytes()],
        bump)]
    pub ask: Account<'info, Ask>,

    #[account(mut)]
    pub user: Signer<'info>,

    pub system_program: Program<'info, System>,
}

#[account]
pub struct Ask {
    /// Plain-text payload of this Ask, freely definable by the user.
    /// This is to be translated into a causal query for matching with offers.
    pub content: String,  // 4 + len()

    /// A numeric index of this Ask local to the user. The tuple (user.key, index)
    /// uniquely addresses an Ask. Keep in mind that Asks are mutable.
    pub index: u64,       // 8
}
