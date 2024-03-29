use anchor_lang::prelude::*;

#[account]
pub struct Ask {
    /// A numeric index of this Ask local to the user. The tuple (user.key, ordinal)
    /// uniquely addresses an Ask. Keep in mind that Asks are mutable.
    pub ordinal: u64, // 8

    /// Plain-text payload of this Ask, freely definable by the user.
    /// This is to be translated into a causal query for matching with offers.
    pub content: String, // 4 + len()
}

impl Ask {
    // pub const SIZE: u64 = 8 + 8 + (4 + len());

    pub fn invariant() -> Result<()> {
        Ok(())
    }
}
