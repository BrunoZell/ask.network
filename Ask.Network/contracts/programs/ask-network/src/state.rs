use anchor_lang::prelude::*;
use borsh::{BorshDeserialize, BorshSerialize};

#[account]
pub struct Ask {
    /// Plain-text payload of this Ask, freely definable by the user.
    /// This is to be translated into a causal query for matching with offers.
    pub content: String, // 4 + len()

    /// A numeric index of this Ask local to the user. The tuple (user.key, ordinal)
    /// uniquely addresses an Ask. Keep in mind that Asks are mutable.
    pub ordinal: u64, // 8
}

#[account]
pub struct User {
    /// Total amount of asks the user has placed until now.
    /// Used as an ever increasing identifier for asks.
    pub running_ask_ordinal: u64,
}

#[account]
pub struct TokenAuthority {}

#[account]
pub struct TreasuryClaimsAuthority {}

/// Data payload of each minted treasury claim NFT
#[account]
pub struct TreasuryClaim {
    pub unit_of_value: TreasuryCurrency,

    pub deposit_amount: u64,

    pub deposit_timestamp: i64, // Unix timestamp of mint
}

#[derive(BorshSerialize, BorshDeserialize, Clone)]
pub enum TreasuryCurrency {
    SOL, // Solana, in Lamports
    USDC,
    ETH,
}

#[account]
pub struct TreasuryClaimsOrdinal {
    /// The amount of treasury claim NFTs minted.
    /// Each treasury claim NFT gets a unique ordinal assigned, sequencing all claims.
    /// This is used to derive the token mints PDA and for the NFT to have an additional trait of deposit precedence.
    /// On each deposit, this counter is incremented by 1.
    pub claims_issued: u64,
}

impl TreasuryClaimsOrdinal {
    pub const SIZE: usize = 8;
}
