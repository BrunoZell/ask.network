use anchor_lang::{prelude::*, system_program};
use anchor_spl::{
    associated_token::AssociatedToken,
    token::{Mint, Token, TokenAccount},
};

use crate::state::*;

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

    pub system_program: Program<'info, System>,
    pub rent: Sysvar<'info, Rent>,
}

#[derive(Accounts)]
pub struct InitializeToken<'info> {
    #[account(mut)]
    pub signer: Signer<'info>,

    #[account(
        init,
        seeds = [b"mint"],
        bump,
        payer = signer,
        mint::decimals = 6,
        mint::authority = authority
    )]
    pub mint: Account<'info, Mint>,

    #[account(
        init,
        seeds = [b"authority"],
        bump,
        payer = signer,
        space = 8)]
    pub authority: Account<'info, TokenAuthority>,
    
    pub token_program: Program<'info, Token>,
    pub rent: Sysvar<'info, Rent>,
    pub system_program: Program<'info, System>,
}

#[derive(Accounts)]
pub struct InitializeTreasuryClaims<'info> {
    #[account(mut)]
    pub signer: Signer<'info>,

    #[account(
        init,
        payer = signer,
        space = 8 + TreasuryClaimsOrdinal::SIZE,
        seeds = [b"treasury_claims_ordinal"],
        bump)]
    pub treasury_claims_ordinal: Account<'info, TreasuryClaimsOrdinal>,

    pub token_program: Program<'info, Token>,
    pub rent: Sysvar<'info, Rent>,
    pub system_program: Program<'info, System>,
}

#[derive(Accounts)]
#[instruction(lamport_amount: u64)]
pub struct DepositSol<'info> {
    #[account(mut)]
    pub depositor: Signer<'info>,

    #[account()] /// CHECK: Address is checked within instruction. I don't know how to encode a const PubKey.
    pub community_treasury: AccountInfo<'info>,
    
    #[account(mut, seeds = [b"treasury_claims_ordinal"], bump)]
    pub treasury_claims_ordinal: Account<'info, TreasuryClaimsOrdinal>,

    /// SPL token mint account of the new treasury claim NFT
    #[account(
        init,
        payer = depositor,
        space = Mint::LEN,
        seeds = [b"treasury_claim_" as &[u8], &(treasury_claims_ordinal.claims_issued + 1).to_le_bytes()],
        bump)]
    pub treasury_claim_mint: Account<'info, Mint>,

    pub token_program: Program<'info, Token>,
    pub rent: Sysvar<'info, Rent>,
    pub system_program: Program<'info, System>,
}

#[derive(Accounts)]
#[instruction(ask_amount: u64)]
pub struct AcquireToken<'info> {
    #[account(mut)]
    pub user: Signer<'info>,

    // User's ATA, the benefitiary
    #[account(
        mut,
        associated_token::mint = token_mint,
        associated_token::authority = user)]
    pub user_token_account: Account<'info, TokenAccount>,

    // Token authority as PDA, meaning the instruction code
    // decides what mint is allowed.
    #[account(mut, seeds=[b"authority"], bump)]
    pub token_authority: Account<'info, TokenAuthority>,

    // Program's global mint
    #[account(mut, seeds=[b"mint"], bump)]
    pub token_mint: Account<'info, Mint>,

    #[account()] /// CHECK: Address is checked within instruction. I don't know how to encode a const PubKey.
    pub community_treasury: AccountInfo<'info>,

    pub token_program: Program<'info, Token>,
    pub associated_token_program: Program<'info, AssociatedToken>,
    pub rent: Sysvar<'info, Rent>,
    pub system_program: Program<'info, System>,
}

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
    pub user: Signer<'info>, // signer of the transaction, implying the 'user' account

    // Solana's built-in system program. Required for operations like account initialization.
    pub system_program: Program<'info, System>,
}

#[derive(Accounts)]
#[instruction(content: String, ordinal: u64)]
pub struct UpdateAsk<'info> {
    #[account(
        mut, // the content of the existing 'ask' account will be mutated
        seeds = [user.key().as_ref(), &ordinal.to_le_bytes()], // 'ask' account is identified by instruction parameters
        bump,
        realloc = 8 + 4 + content.len() + 8 ,
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

    pub system_program: Program<'info, System>,
}
