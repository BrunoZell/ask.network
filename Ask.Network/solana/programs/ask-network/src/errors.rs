use anchor_lang::prelude::*;

#[error_code]
pub enum ErrorCode {
    #[msg("Insufficient funds to purchase token.")]
    InsufficientFunds,

    #[msg("The provided community treasury address is invalid.")]
    InvalidCommunityTreasuryAddress,
}
