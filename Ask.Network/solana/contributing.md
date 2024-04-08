# Solana Development

## Add an Instruction

To implement an instruciton, you need to:

- Create a new file `src/instructions/[instruction].rs` containing:
  - A `struct` declaring the instructions arguments
  - A `struct` declaring all accounts read or written during the instructions execution
  - An `impl` of that instruction struct containing:
    - A `handle`-function, implementing the imperative code that reads instruction arguments and passed accounts to determine whether to write data to accounts, or to create or delete accounts.
    - A `validate`-function specifiying imperative account and argument validation in addition to declarative anchor constraints. This is called by the Anchor hook `#[access_control(ctx.accounts.validate(&args))]` on every call to `handle`
- Create a new file `src/state/[account].rs` for every new data type to be stored in a typed Solana account. This file also contain:
  - A `struct` declaring the accounts root data type
  - Possibly inner types like enums or other structs referenced by the root account struct.
  - An `impl` of that account struct containing:
    - A `const SIZE: usize` if the account struct has a constant size to be used in Anchor constraints. A dynamic function otherwise.
    - A function `invariant` defining all imperative validaton code depending on solely an instance of the account type. This must be called manually in each instruction that interacts with accounts of that type
- Add the instructions entry point to `lib.rs`
- Add the new instruction and state files to `src/instructions/mod.rs` and `src/state/mod.rs` respectively

Here are code sample for each of these steps:

### `src/instructions/[instruction].rs`

```rust
use crate::state::*;
use anchor_lang::prelude::*;

#[derive(AnchorSerialize, AnchorDeserialize)]
pub struct IxArgs {}

#[derive(Accounts)]
#[instruction(args: IxArgs)]
pub struct Ix<'info> {
    pub system_program: Program<'info, System>,
    pub rent: Sysvar<'info, Rent>,
}

impl Ix<'_> {
    fn validate(&self, _args: &IxArgs) -> Result<()> {
        // Todo: Implement account and argument validation
        Ok(())
    }

    #[access_control(ctx.accounts.validate(&args))]
    pub fn handle(ctx: Context<Self>, args: IxArgs) -> Result<()> {
        // Todo: Implement instruction
    }
}
```

Add these two lines to `src/instructions/mod.rs` (where `ix` is the files name of `src/instructions/[instruction].rs`):

```rust
pub use ix::*;

mod ix;
```

### `src/state/[account].rs`

```rust
use anchor_lang::prelude::*;

#[account]
pub struct AccountType {
    /// A number stored in the account.
    pub data_field: u64, // 8
}

impl AccountType {
    // pub const SIZE: usize = 8 + ...; // Sum of all data field sizes + 8b Anchor discriminator

    pub fn invariant(&self) -> Result<()> {
        // Todo: Implement validation code depending only on the account type data
        Ok(())
    }
    
    pub fn is_in_state(&self) -> bool {
        // Todo: Probe account data for specific conditions in a reusable fashion
        false
    }
}
```

Add these two lines to `src/state/mod.rs` (where `ix` is the files name of `src/state/[account].rs`):

```rust
pub use account::*;

mod account;
```

### `src/lib.rs`

```rust
use anchor_lang::prelude::*;

pub use instructions::*;
pub use state::*;

pub mod instructions;
pub mod state;

declare_id!("8WfQ3nACPcoBKxFnN4ekiHp8bRTd35R4L8Pu3Ak15is3");

#[program]
pub mod solana_program {
    use super::*;

    pub fn instuction(ctx: Context<Ix>, args: IxArgs) -> Result<()> {
        Ix::handle(ctx, args)
    }
}
```
