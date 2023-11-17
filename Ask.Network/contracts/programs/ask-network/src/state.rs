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
    pub ordinal: u64,

    pub unit_of_value: TreasuryCurrency,

    pub deposit_amount: u64,

    pub deposit_timestamp: i64, // Unix timestamp of mint
}

impl TreasuryClaim {
    // ordinal: u64: As per the chart, u64 occupies 8 bytes.
    // unit_of_value: TreasuryCurrency: This is an enum. The space it occupies will be 1 byte for the enum discriminator plus the size of its largest variant. Without knowing the exact definition of TreasuryCurrency, I'll assume it's the size of a u8, which is 1 byte, making the total size 2 bytes (1 for the enum discriminator + 1 for the largest variant). If TreasuryCurrency has larger variants, this needs to be adjusted accordingly.
    // deposit_amount: u64: Occupies 8 bytes.
    // deposit_timestamp: i64: Occupies 8 bytes.
    pub const SIZE: usize = 8 + 2 + 8 + 8; // Adjust if the largest variant of TreasuryCurrency is larger than u8
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
