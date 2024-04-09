use anchor_lang::prelude::*;

#[account]
pub struct CommercialRegisterVerificationResponse {
    // Registration request this is a response to
    pub verification_request: CommercialRegisterVerificationRequest,

    // The official address the company is registed under, as extracted from the jurisdictions commercial register
    pub official_address: String,

    // The official name of the company, as extracted from the jurisdictions commercial register
    pub official_name: String,

    // The SHA256 hash of the secret key sent as a letter to the companies official address
    pub verification_secret_hash: bytes,
    // Todo: Add a Reclaim Protocol proof for the name here for onchain verification
    // Todo: Add a Reclaim Protocol proof for the address here for onchain verification
    // Todo: Add a Reclaim Protocol proof for the ePOST letter sent for onchain verification
}

impl CommercialRegisterVerificationResponse {
    // pub const SIZE: usize = 8 + ...; // Sum of all data field sizes + 8b Anchor discriminator
}
