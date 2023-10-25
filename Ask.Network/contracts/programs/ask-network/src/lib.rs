use anchor_lang::prelude::*;
use anchor_lang::solana_program::entrypoint::*;

mod asks;
mod users;

declare_id!("4ktm3bQPuEfsyGRR95QrkRdcrfb268hGzgjDr9Y17FGE");

#[program]
pub mod ask_network {
    use super::*;
    // Users

    pub fn initialize_user(ctx: Context<crate::users::InitializeUser>) -> ProgramResult {
        users::initialize_user(ctx)
    }

    // Asks

    pub fn place_ask(ctx: Context<crate::asks::PlaceAsk>, content: String) -> ProgramResult {
        asks::place_ask(ctx, content)
    }

    pub fn update_ask(
        ctx: Context<crate::asks::UpdateAsk>,
        content: String,
        ordinal: u64,
    ) -> ProgramResult {
        asks::update_ask(ctx, content, ordinal)
    }

    pub fn cancel_ask(ctx: Context<crate::asks::CancelAsk>, ordinal: u64) -> ProgramResult {
        asks::cancel_ask(ctx, ordinal)
    }
}
