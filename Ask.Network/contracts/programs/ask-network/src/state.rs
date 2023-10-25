use anchor_lang::prelude::*;

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
