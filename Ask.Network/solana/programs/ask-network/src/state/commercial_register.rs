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

    // France: Registre du Commerce et des Sociétés (RCS)
    // Companies are given a SIREN number, a 9-digit numerical code that is unique nationwide.
    France {
        rcs_number: String,
    },

    // Spain: Registro Mercantil
    // The commercial register assigns a unique alphanumeric code that varies by the provincial registry.
    Spain {
        registro_mercantil_number: String,
    },

    // United States: Secretary of State
    // Each state has its own Secretary of State or equivalent registry, issuing unique identification numbers for registered entities.
    UnitedStates {
        state: String,
        ein: String, // Employer Identification Number
    },

    // United Kingdom: Companies House
    // Each company is assigned a unique Company Number when registered with Companies House.
    UnitedKingdom {
        company_number: String,
    },

    // Singapore: Accounting and Corporate Regulatory Authority (ACRA)
    // Companies are identified by a Unique Entity Number (UEN).
    Singapore {
        uen: String,
    },

    // Switzerland: Unternehmens-Identifikationsnummer (UID)
    // Companies receive a unique UID for identification.
    Switzerland {
        uid: String,
    },

    // Estonia: e-Business Register
    // Known for its e-Residency program, companies are assigned a registry code.
    Estonia {
        registry_code: String,
    },

    // South Korea: Supreme Court Registry
    // Companies are given a unique Business Registration Number.
    SouthKorea {
        registration_number: String,
    },

    // Japan: National Tax Agency
    // Companies are provided with a Corporate Number (法人番号, Hōjin Bangō).
    Japan {
        corporate_number: String,
    },

    // Australia: Australian Securities and Investments Commission (ASIC)
    // Companies are identified by an Australian Business Number (ABN).
    Australia {
        abn: String,
    },

    // Canada: Corporations Canada
    // Federal corporations are assigned a unique Corporation Number, while businesses may also have a Business Number (BN) for tax purposes.
    Canada {
        business_number: String,
    },

    // Italy: Registro Imprese
    // Companies are registered with a unique Codice Fiscale (Tax Code).
    Italy {
        codice_fiscale: String,
    },

    // India: Ministry of Corporate Affairs (MCA)
    // Companies are identified with a unique Corporate Identity Number (CIN).
    India {
        cin: String,
    },

    // Brazil: National Registry of Legal Entities (CNPJ)
    // Companies are identified by a CNPJ number.
    Brazil {
        cnpj: String,
    },

    // Russia: Unified State Register of Legal Entities (USRLE)
    // Companies receive an ОГРН (OGRN), a primary state registration number.
    Russia {
        ogrn: String,
    },

    // Netherlands: Kamer van Koophandel (KvK)
    // Companies are identified by a KvK number.
    Netherlands {
        kvk_number: String,
    },

    // Mexico: Sistema de Información Empresarial Mexicano (SIEM)
    // Companies are registered with a unique registry number.
    Mexico {
        registry_number: String,
    },

    // United Arab Emirates: Department of Economic Development (DED)
    // Companies are issued a unique commercial license number.
    UnitedArabEmirates {
        commercial_license_number: String,
    },

    // South Africa: Companies and Intellectual Property Commission (CIPC)
    // Companies are identified by a unique registration number.
    SouthAfrica {
        registration_number: String,
    },

    // New Zealand: Companies Office
    // Companies are assigned a unique New Zealand Business Number (NZBN).
    NewZealand {
        nzbn: String,
    },
}
