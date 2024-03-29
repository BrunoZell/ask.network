use anchor_lang::prelude::*;

#[account]
pub struct User {
    /// Total amount of asks the user has placed until now.
    /// Used as an ever increasing identifier for users asks.
    pub running_ask_ordinal: u64, // 8
}

impl User {
    pub const DISCRIMINATOR_SIZE: usize = 8; // 8 bytes for the discriminator
    pub const SIZE: usize = Self::DISCRIMINATOR_SIZE + 8;

    pub fn invariant() -> Result<()> {
        Ok(())
    }
}
