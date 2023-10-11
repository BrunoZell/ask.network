use anchor_lang::prelude::*;

declare_id!("4ktm3bQPuEfsyGRR95QrkRdcrfb268hGzgjDr9Y17FGE");

/// ###################
/// ####  Program  ####
/// ###################

#[program]
pub mod ask_network {
    use super::*;
    
    pub fn place_ask(ctx: Context<PlaceAsk>, content: String) -> Result<()> {
        msg!("User {} placed new ask: {}", ctx.accounts.user.key, content);

        // Fill new ask with its content and index number
        ctx.accounts.ask.content = content;
        ctx.accounts.ask.ordinal = ctx.accounts.user_account.running_ask_ordinal;
        
        // Increment users active ask counter
        ctx.accounts.user_account.running_ask_ordinal += 1;

        Ok(())
    }

    pub fn cancel_ask(ctx: Context<CancelAsk>, _ordinal: u64) -> Result<()> {
        msg!("Ask of user {} cancelled: {}", ctx.accounts.user.key, ctx.accounts.ask.content);
        Ok(())
    }
}

/// ###################
/// ####  Context  ####
/// ###################

#[derive(Accounts)]
#[instruction(content: String)]
pub struct PlaceAsk<'info> {
    #[account(
        init, // this 'ask' account will be initialized
        seeds = [user.key().as_ref(), &user_account.running_ask_ordinal.to_le_bytes()], // unique ask address from user key and ordinal
        bump,
        payer = user, // 'user' account pays fees
        space = 8 + 4 + content.len() + 8)]
    pub ask: Account<'info, Ask>,

    #[account(
        mut, // users running ask ordinal is incremented after ask placement
        seeds = [user.key().as_ref()], // 'user' account is derived from the users public key
        bump)]
    pub user_account: Account<'info, User>,
    
    #[account(mut)]
    pub user: Signer<'info>,

    // Solana's built-in system program. Required for operations like account initialization.
    pub system_program: Program<'info, System>,
}

#[derive(Accounts)]
#[instruction(ordinal: u64)]
pub struct CancelAsk<'info> {
    #[account(
        mut, 
        close = user, // after the instruction is executed, the 'ask' account will be closed, and any remaining lamports will be transferred to the 'user' account.
        seeds = [user.key().as_ref(), &ordinal.to_le_bytes()],
        bump)]
    pub ask: Account<'info, Ask>,

    #[account(mut)]
    pub user: Signer<'info>,

    pub system_program: Program<'info, System>,
}

/// #################
/// ####  State  ####
/// #################

#[account]
pub struct User {
    /// Total amount of asks the user has placed until now.
    pub running_ask_ordinal: u64,
}

#[account]
pub struct Ask {
    /// Plain-text payload of this Ask, freely definable by the user.
    /// This is to be translated into a causal query for matching with offers.
    pub content: String,  // 4 + len()

    /// A numeric index of this Ask local to the user. The tuple (user.key, ordinal)
    /// uniquely addresses an Ask. Keep in mind that Asks are mutable.
    pub ordinal: u64,       // 8
}
