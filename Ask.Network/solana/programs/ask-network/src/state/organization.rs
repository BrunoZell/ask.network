use anchor_lang::prelude::*;

#[account]
pub struct Organization {
    /// Display name of the organization. Not unique.
    pub alias: String, // 4 + len()
}

impl Organization {
    const DISCRIMINATOR_SIZE: usize = 8; // 8 bytes for the discriminator
    const ALIAS_LEN_PREFIX: usize = 4; // 4 bytes to store the length of `alias`

    pub fn size(alias_len: usize) -> usize {
        Self::DISCRIMINATOR_SIZE + Self::ALIAS_LEN_PREFIX + alias_len
    }

    pub fn invariant() -> Result<()> {
        Ok(())
    }
}
