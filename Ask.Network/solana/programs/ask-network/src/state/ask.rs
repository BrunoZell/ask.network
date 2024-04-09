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
    pub const BASE_SIZE: usize = 8; // 8 bytes for the discriminator
    pub const ORDINAL_SIZE: usize = 8; // 8 bytes for the `ordinal` field
    pub const CONTENT_LEN_PREFIX: usize = 4; // 4 bytes to store the length of `content`

    pub fn size(content_len: usize) -> usize {
        Self::BASE_SIZE + Self::ORDINAL_SIZE + Self::CONTENT_LEN_PREFIX + content_len
    }

    pub fn invariant() -> Result<()> {
        Ok(())
    }
}
