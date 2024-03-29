use anchor_lang::prelude::*;

#[account]
pub struct Membership {}

impl Membership {
    const DISCRIMINATOR_SIZE: usize = 8; // 8 bytes for the discriminator

    pub const SIZE: usize = Self::DISCRIMINATOR_SIZE;

    pub fn invariant(&self) -> Result<()> {
        // Todo: Implement validation code depending only on the account type data
        Ok(())
    }

    pub fn is_in_state(&self) -> bool {
        // Todo: Probe account data for specific conditions in a reusable fashion
        false
    }
}
