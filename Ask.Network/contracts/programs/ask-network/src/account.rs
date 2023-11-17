use anchor_lang::{prelude::*, system_program};
use anchor_spl::{
    associated_token::AssociatedToken,
    token::{Mint, Token, TokenAccount},
};
use mpl_token_metadata::state::Metadata;

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
        seeds = [b"token_mint"],
        bump,
        payer = signer,
        mint::decimals = 6,
        mint::authority = authority
    )]
    pub mint: Account<'info, Mint>,

    #[account(
        init,
        seeds = [b"token_authority"],
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

    #[account(
        init,
        payer = signer,
        space = 8,
        seeds = [b"treasury_claims_authority"],
        bump)]
    pub treasury_claims_authority: Account<'info, TreasuryClaimsAuthority>,
    
    pub token_program: Program<'info, Token>,
    pub rent: Sysvar<'info, Rent>,
    pub system_program: Program<'info, System>,
}

#[derive(Accounts)]
#[instruction(lamport_amount: u64)]
pub struct DepositSol<'info> {
    #[account(
        init,
        payer = depositor,
        space = 8 + TreasuryClaim::SIZE,
        seeds = [b"treasury_claims_ordinal"],
        bump)]
    pub this_treasury_claim: Account<'info, TreasuryClaim>,

    #[account(mut)]
    pub depositor: Signer<'info>,

    #[account()] /// CHECK: Address is checked within instruction. I don't know how to encode a const PubKey.
    pub community_treasury: AccountInfo<'info>,
    
    #[account(
        mut,
        seeds = [b"treasury_claims_ordinal"],
        bump)]
    pub treasury_claims_ordinal: Account<'info, TreasuryClaimsOrdinal>,

    /// Global singleton treasury claims authority.
    /// Each SPL treasury claim NFT has this PDA as authority.
    #[account(
        seeds = [b"treasury_claims_authority"],
        bump)]
    pub treasury_claims_authority: Account<'info, TreasuryClaimsAuthority>,

    /// SPL token mint account of the new treasury claim NFT, uniquely addressed by the claim ordinal.
    #[account(
        init,
        payer = depositor,
        seeds = [b"treasury_claim_" as &[u8], &(treasury_claims_ordinal.claims_issued + 1).to_le_bytes()],
        mint::decimals = 0,
        mint::authority = treasury_claims_authority,
        bump)]
    pub treasury_claim_mint: Account<'info, Mint>,
    
    /// Associated token account for the depositor holding the newly minted treasury claim NFT.
    #[account(
        init,
        payer = depositor,
        associated_token::mint = treasury_claim_mint,
        associated_token::authority = depositor)]
    pub treasury_claim_ata: Account<'info, TokenAccount>,

    /// Metaplex Metadata (PDA derived from ['metadata', program ID, mint ID])
    pub system_program: Program<'info, System>,
    pub sysvar_instructions: AccountInfo<'info>,
    pub spl_token_program: Program<'info, Token>,
    pub metadata_program: Program<'info, System>,
    #[account(
        seeds = [b"metadata", mpl_token_metadata::id().as_ref(), treasury_claim_mint.key().as_ref()],
        bump)]
    pub metadata: AccountInfo<'info>,

    // For SPL token mint
    pub rent: Sysvar<'info, Rent>,
    pub token_program: Program<'info, Token>,
    pub associated_token_program: Program<'info, AssociatedToken>,
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
