use anchor_lang::prelude::*;
use anchor_lang::solana_program::entrypoint::*;

pub mod cancel_ask;
pub mod initialize_user;
pub mod place_ask;
pub mod state;
pub mod update_ask;

declare_id!("4ktm3bQPuEfsyGRR95QrkRdcrfb268hGzgjDr9Y17FGE");

#[program]
pub mod ask_network {
    use super::*;
    // Users

    pub fn initialize_user(ctx: Context<initialize_user::InitializeUser>) -> ProgramResult {
        initialize_user::initialize_user(ctx)
    }

    // Asks

    pub fn place_ask(ctx: Context<place_ask::PlaceAsk>, content: String) -> ProgramResult {
        place_ask::place_ask(ctx, content)
    }

    pub fn update_ask(
        ctx: Context<update_ask::UpdateAsk>,
        content: String,
        ordinal: u64,
    ) -> ProgramResult {
        update_ask::update_ask(ctx, content, ordinal)
    }

    pub fn cancel_ask(ctx: Context<cancel_ask::CancelAsk>, ordinal: u64) -> ProgramResult {
        cancel_ask::cancel_ask(ctx, ordinal)
    }
}
