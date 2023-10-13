use anchor_lang::prelude::*;
use anchor_spl::{
    associated_token::AssociatedToken,
    token::{Mint, Token, TokenAccount},
};

declare_id!("4ktm3bQPuEfsyGRR95QrkRdcrfb268hGzgjDr9Y17FGE");

/// ###################
/// ####  Program  ####
/// ###################

#[program]
pub mod ask_network {
    use super::*;

    // Initialization
    
    pub fn initialize_token(ctx: Context<InitializeToken>) -> Result<()> {
        msg!("Token mint initialized with supply of zero");

        Ok(())
    }

    pub fn initialize_user(ctx: Context<InitializeUser>) -> Result<()> {
        msg!("Initializing user: {}", ctx.accounts.user.key());

        ctx.accounts.user_account.running_ask_ordinal = 0;

        // Anchor creates user's token account if needed

        Ok(())
    }

    // Ask Management
    
    pub fn place_ask(ctx: Context<PlaceAsk>, content: String) -> Result<()> {
        msg!("User {} placed new ask: {}", ctx.accounts.user.key, &content);

        // Fill new ask with its content and index number
        ctx.accounts.ask.content = content;
        ctx.accounts.ask.ordinal = ctx.accounts.user_account.running_ask_ordinal;
        ctx.accounts.ask.stake = 0;
        
        // Increment users ever-increasing ask counter
        ctx.accounts.user_account.running_ask_ordinal += 1;

        Ok(())
    }

    pub fn update_ask(ctx: Context<UpdateAsk>, content: String, _ordinal: u64) -> Result<()> {
        msg!("User {} updates ask from: {}", ctx.accounts.user.key, &ctx.accounts.ask.content);

        ctx.accounts.ask.content = content;
        
        msg!("New ask: {}", &ctx.accounts.ask.content);
        Ok(())
    }

    pub fn cancel_ask(ctx: Context<CancelAsk>, _ordinal: u64) -> Result<()> {
        msg!("User {} cancelled ask: {}", ctx.accounts.user.key, &ctx.accounts.ask.content);

        // Todo: Unstake on cancellation
        // Check if ask token account has a balance > 0
        // If so, send tokens to associated user
        // Then updated ctx.accounts.user_account.total_staked and ctx.accounts.ask.stake

        msg!("Returned {} $ASK to user {}", 0, ctx.accounts.user.key);

        Ok(())
    }

    pub fn prioritize_ask(ctx: Context<PrioritizeAsk>, _ordinal: u64, _addedStake: u64) -> Result<()> {
        msg!("User {} commits {} $ASK to ask {}: {}",
            ctx.accounts.user.key,
            _addedStake,
            _ordinal,
            ctx.accounts.ask.content);
        
        // Todo: Transfer $ASK from user account to ask account

        ctx.accounts.user_account.total_staked += _addedStake;
        ctx.accounts.ask.stake += _addedStake;

        Ok(())
    }
}

/// ######################
/// ### Initialization ###
/// ######################

#[derive(Accounts)]
pub struct InitializeToken<'info> {
    #[account(
        init,
        seeds = [b"mint"],
        bump,
        payer = user,
        mint::decimals = 6,
        mint::authority = authority)]
    pub mint: Account<'info, Mint>,

    // The user who is paying for the creation of the mint account.
    #[account(mut)]
    pub user: Signer<'info>,

    #[account(
        init,
        seeds = [b"authority"],
        bump,
        payer = user,
        space = 8)]
    pub authority: Account<'info, Authority>,

    pub token_program: Program<'info, Token>,
    pub system_program: Program<'info, System>,
    pub rent: Sysvar<'info, Rent>,
}

#[derive(Accounts)]
pub struct InitializeUser<'info> {
    #[account(
        init,
        seeds= [user.key().as_ref()],
        bump,
        space = 8 + 8 + 8,
        payer = user)]
    pub user_account: Account<'info, User>,

    #[account(mut)]
    pub user: Signer<'info>,

    #[account(
        mut,
        seeds = ["mint".as_bytes()],
        bump)]
    pub mint: Account<'info, Mint>,

    #[account(
        init_if_needed,
        payer = user,
        associated_token::mint = mint,
        associated_token::authority = user)]
    pub user_token_account: Account<'info, TokenAccount>,

    pub token_program: Program<'info, Token>,
    pub associated_token_program: Program<'info, AssociatedToken>,
    pub system_program: Program<'info, System>,
    pub rent: Sysvar<'info, Rent>,
}

/// ##################
/// ####   Asks   ####
/// ##################

#[derive(Accounts)]
#[instruction(content: String)]
pub struct PlaceAsk<'info> {
    #[account(
        init, // this 'ask' account will be initialized
        seeds = [user.key().as_ref(), &user_account.running_ask_ordinal.to_le_bytes()], // unique ask address from user key and ordinal
        bump,
        payer = user, // 'user' account pays fees
        space = 8 + 4 + content.len() + 8 + 8)]
    pub ask: Account<'info, Ask>,

    // $ASK global mint
    #[account(mut, seeds=[b"mint"], bump)]
    pub mint: Account<'info, Mint>,

    // Asks's staking ATA
    #[account(
        init_if_needed,
        payer = user,
        associated_token::mint = mint,
        associated_token::authority = ask)]
    pub ask_token_account: Account<'info, TokenAccount>,

    #[account(
        mut, // users running ask ordinal is incremented after ask placement
        seeds = [user.key().as_ref()], // 'user' account is derived from the users public key
        bump)]
    pub user_account: Account<'info, User>,
    
    #[account(mut)]
    pub user: Signer<'info>, // signer of the transaction, implying the 'user' account

    // Solana's built-in system program. Required for operations like account initialization.
    pub token_program: Program<'info, Token>,
    pub associated_token_program: Program<'info, AssociatedToken>,
    pub system_program: Program<'info, System>,
}

#[derive(Accounts)]
#[instruction(content: String, ordinal: u64)]
pub struct UpdateAsk<'info> {
    #[account(
        mut, // the content of the existing 'ask' account will be mutated
        seeds = [user.key().as_ref(), &ordinal.to_le_bytes()], // 'ask' account is identified by instruction parameters
        bump,
        realloc = 8 + 4 + content.len() + 8 + 8,
        realloc::zero = true,
        realloc::payer = user)] // 'user' account pays fees
    pub ask: Account<'info, Ask>,
    
    #[account(
        seeds = [user.key().as_ref()],
        bump)]
    pub user_account: Account<'info, User>,
    
    #[account(mut)]
    pub user: Signer<'info>, // signer of the transaction, implying the 'user' account

    pub system_program: Program<'info, System>,
}

#[derive(Accounts)]
#[instruction(ordinal: u64)]
pub struct PrioritizeAsk<'info> {
    // Storage account
    #[account(mut, seeds= [user.key().as_ref(), &ordinal.to_le_bytes()], bump)]
    pub ask: Account<'info, Ask>,

    #[account(mut)]
    pub user: Signer<'info>,

    #[account(
        seeds = [user.key().as_ref()],
        bump)]
    pub user_account: Account<'info, User>,
    
    #[account(mut, seeds=[b"authority"], bump)]
    pub authority: Account<'info, Authority>,

    // $ASK global mint
    #[account(mut, seeds=[b"mint"], bump)]
    pub mint: Account<'info, Mint>,

    // User's ATA
    #[account(mut, associated_token::mint = mint, associated_token::authority = user)]
    pub user_token_account: Account<'info, TokenAccount>,

    // Asks's ATA
    #[account(mut, associated_token::mint = mint, associated_token::authority = ask)]
    pub ask_token_account: Account<'info, TokenAccount>,

    // Token program stuff
    pub token_program: Program<'info, Token>,
    pub associated_token_program: Program<'info, AssociatedToken>,
    pub system_program: Program<'info, System>,
    pub rent: Sysvar<'info, Rent>,
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

    #[account(
        seeds = [user.key().as_ref()],
        bump)]
    pub user_account: Account<'info, User>,
    
    // $ASK global mint
    // Required to reference this Asks token account to be deconstructed.
    // Also for referencing the Users token account to receive staked tokens, if any.
    #[account(mut, seeds=[b"mint"], bump)]
    pub mint: Account<'info, Mint>,

    // User's ATA
    #[account(mut, associated_token::mint = mint, associated_token::authority = user)]
    pub user_token_account: Account<'info, TokenAccount>,

    // Asks's ATA
    #[account(
        mut,
        close = user,
        associated_token::mint = mint,
        associated_token::authority = ask)]
    pub ask_token_account: Account<'info, TokenAccount>,

    pub system_program: Program<'info, System>,
}

/// #################
/// ####  State  ####
/// #################

#[account]
pub struct User {
    /// Total amount of asks the user has placed until now.
    /// Used as an ever increasing identifier for asks.
    pub running_ask_ordinal: u64,

    /// The sum of all $ASK this user has staked on his asks.
    /// Used in the UI to quickly calculate the users total balance,
    /// including all locked tokens.
    pub total_staked: u64,
}

#[account]
pub struct Ask {
    /// Plain-text payload of this Ask, freely definable by the user.
    /// This is to be translated into a causal query for matching with offers.
    pub content: String,  // 4 + len()

    /// A numeric index of this Ask local to the user. The tuple (user.key, ordinal)
    /// uniquely addresses an Ask. Keep in mind that Asks are mutable.
    pub ordinal: u64,     // 8

    pub stake: u64,       // 8
}

#[account]
pub struct Authority {}
