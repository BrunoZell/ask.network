use anchor_lang::prelude::*;

#[account]
pub struct Organization {
    /// Display name of the organization. Not unique.
    pub alias: String, // 4 + len()
}

impl Organization {
    // pub const SIZE: usize = 8 + 8;

    pub fn invariant() -> Result<()> {
        Ok(())
    }
}
