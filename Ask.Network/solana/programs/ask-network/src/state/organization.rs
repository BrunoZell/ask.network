use anchor_lang::prelude::*;

#[account]
pub struct Organization {
    /// Total amount of asks the organization has placed until now.
    /// Used as an ever increasing identifier for the organizations asks.
    pub running_ask_ordinal: u64, // 8
}

impl Organization {
    pub const SIZE: usize = 8 + 8;

    pub fn invariant() -> Result<()> {
        Ok(())
    }
}
