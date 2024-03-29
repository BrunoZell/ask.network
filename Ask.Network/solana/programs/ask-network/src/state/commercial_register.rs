use anchor_lang::prelude::*;

use crate::GermanCourt;

#[derive(AnchorSerialize, AnchorDeserialize, Clone, Debug)]
pub enum RegistrationNumber {
    // Germany: Handelsregister
    // The commercial register in Germany is known as "Handelsregister".
    // Companies are registered with a unique number usually starting with the court's abbreviation.
    Germany {
        court: GermanCourt,
        hrb_registration_number: u64,
    },
    // Todo: Add other jurisdictions
}
