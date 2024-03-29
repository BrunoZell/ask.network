use anchor_lang::prelude::*;

#[account]
pub struct Global {
    /// Each account, both organizational and personal, are identified by a unique account number.
    /// These account numbers are issued sequentially starting from 0, incrementing by one with each account creation.
    pub running_account_ordinal: u64, // 8
}

impl Global {
    pub const SIZE: usize = 8 + 8;

    pub fn invariant(&self) -> Result<()> {
        // Todo: Implement validation depending only on the account type data
        Ok(())
    }

    pub fn next_account_number(&self) -> u64 {
        self.running_account_ordinal + 1
    }
}
