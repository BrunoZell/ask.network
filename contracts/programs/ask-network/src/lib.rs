use anchor_lang::prelude::*;

declare_id!("4ktm3bQPuEfsyGRR95QrkRdcrfb268hGzgjDr9Y17FGE");

#[program]
pub mod ask_network {
    use super::*;

    pub fn initialize(ctx: Context<Initialize>) -> Result<()> {
        Ok(())
    }
}

#[derive(Accounts)]
pub struct Initialize {}
