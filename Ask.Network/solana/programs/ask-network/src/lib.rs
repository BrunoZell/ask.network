use anchor_lang::prelude::*;

pub use instructions::*;
pub use state::*;
pub mod instructions;
pub mod state;

declare_id!("EaBpabfkGswhPnC14mnwz3XxHHyJuqZjPZAMU9mR7KR3");

#[program]
pub mod ask_network {
    use super::*;

    pub fn initialize_global(
        ctx: Context<InitializeGlobal>,
        args: InitializeGlobalArgs,
    ) -> Result<()> {
        InitializeGlobal::handle(ctx, args)
    }

    pub fn sign_up_user(ctx: Context<SignUpUser>, args: SignUpUserArgs) -> Result<()> {
        SignUpUser::handle(ctx, args)
    }

    pub fn sign_up_organization(
        ctx: Context<SignUpOrganization>,
        args: SignUpOrganizationArgs,
    ) -> Result<()> {
        SignUpOrganization::handle(ctx, args)
    }

    pub fn place_ask(ctx: Context<PlaceAsk>, args: PlaceAskArgs) -> Result<()> {
        PlaceAsk::handle(ctx, args)
    }

    pub fn cancecl_ask(ctx: Context<CancelAsk>, args: CancelAskArgs) -> Result<()> {
        CancelAsk::handle(ctx, args)
    }
}
