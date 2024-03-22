use crate::state::*;

#[derive(Accounts)]
pub struct InitializeUser<'info> {
    #[account(
        init,
        seeds= [user_login.key().as_ref()],
        bump,
        space = 8 + 8 + 8,
        payer = user_login)]
    pub user_account: Account<'info, User>,

    #[account(mut)]
    pub user_login: Signer<'info>,

    pub system_program: Program<'info, System>,
    pub rent: Sysvar<'info, Rent>,
}

#[derive(Accounts)]
#[instruction(content: String)]
pub struct PlaceAsk<'info> {
    #[account(
        init, // this 'ask' account will be initialized
        seeds = [user_account.key().as_ref(), &user_account.running_ask_ordinal.to_le_bytes()], // unique ask address from user key and ordinal
        bump,
        payer = user, // 'user' account pays fees
        space = 8 + 4 + content.len() + 8)]
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

#[derive(Accounts)]
#[instruction(content: String, ordinal: u64)]
pub struct UpdateAsk<'info> {
    #[account(
        mut, // the content of the existing 'ask' account will be mutated
        seeds = [user_account.key().as_ref(), &ordinal.to_le_bytes()], // 'ask' account is identified by instruction parameters
        bump,
        realloc = 8 + 4 + content.len() + 8 ,
        realloc::zero = true,
        realloc::payer = user_account)] // 'user' account pays fees
    pub ask: Account<'info, Ask>,

    #[account(
        seeds = [user_login.key().as_ref()],
        bump)]
    pub user_account: Account<'info, User>,

    #[account(mut)]
    pub user_login: Signer<'info>, // signer of the transaction, implying the 'user_login' account

    pub system_program: Program<'info, System>,
}

#[derive(Accounts)]
#[instruction(ordinal: u64)]
pub struct CancelAsk<'info> {
    #[account(
        mut,
        close = user_login, // after the instruction is executed, the 'ask' account will be closed, and any remaining lamports will be transferred to the 'user' account.
        seeds = [user_account.key().as_ref(), &ordinal.to_le_bytes()],
        bump)]
    pub ask: Account<'info, Ask>,

    #[account(
        seeds = [user_login.key().as_ref()],
        bump)]
    pub user_account: Account<'info, User>,

    #[account(mut)]
    pub user_login: Signer<'info>,

    pub system_program: Program<'info, System>,
}
