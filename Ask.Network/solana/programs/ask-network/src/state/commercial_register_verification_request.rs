use anchor_lang::prelude::*;

#[account]
pub struct CommercialRegisterVerificationRequest {
    /// Official company registration number requested to be verified.
    pub registration_number: RegistrationNumber,
}

impl CommercialRegisterVerificationRequest {
    // pub const SIZE: usize = 8 + ...; // Sum of all data field sizes + 8b Anchor discriminator
}
