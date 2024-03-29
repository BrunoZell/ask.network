use anchor_lang::prelude::*;

// Find a full list here: https://db.offeneregister.de/openregister?sql=SELECT+DISTINCT+courtName%2C+courtCode%0D%0AFROM+ReferenceNumbers%3B%0D%0A
#[derive(AnchorSerialize, AnchorDeserialize, Clone, Debug, PartialEq)]
pub enum GermanCourt {
    BerlinCharlottenburg,
    Lemgo,
    FrankfurtAmMain,
    Muenchen,
    Gelsenkirchen,
    BadOeynhausen,
    Duesseldorf,
    Wiesbaden,
    Kiel,
    Mannheim,
    Siegburg,
    Koblenz,
    Iserlohn,
    Bonn,
    Duisburg,
    Dresden,
    LudwigshafenAmRhein,
    Arnsberg,
    Koeln,
    Wuerzburg,
    Hannover,
    RegistergerichtFreiburgImBreisgau,
    Cottbus,
    Regensburg,
    Saarbruecken,
    Ulm,
    OffenbachAmMain,
    BadHomburgVorDerHoehe,
    Stuttgart,
    Deggendorf,
    Flensburg,
    Montabaur,
    Hamburg,
    Aachen,
    Lueneburg,
    Schweinfurt,
    Augsburg,
    Nuernberg,
    Chemnitz,
    Osnabrueck,
    Darmstadt,
    KemptenAllgaeu,
    Memmingen,
    Kassel,
    Zweibruecken,
    Amberg,
    FrankfurtOder,
    Potsdam,
    Bochum,
    Wuppertal,
    Kaiserslautern,
    Fritzlar,
    Pinneberg,
    Koenigstein,
    LimburgAnDerLahn,
    Oldenburg,
    Fulda,
    Stendal,
    Luebeck,
    Mainz,
    Dortmund,
    Walsrode,
    Friedberg,
    Aschaffenburg,
    Essen,
    Recklinghausen,
    Siegen,
    Jena,
    RegistergerichtLandauInDerPfalz,
    Wittlich,
    Leipzig,
    Hildesheim,
    Hanau,
    BadKreuznach,
    Neuruppin,
    Bremen,
    Giessen,
    Coesfeld,
    Krefeld,
    Goettingen,
    BadHersfeld,
    Bamberg,
    Traunstein,
    Passau,
    WeidenInDerOberpfalz,
    Coburg,
    Moenchengladbach,
    Wetzlar,
    Ansbach,
    Eschwege,
    Kleve,
    Landshut,
    Steinfurt,
    Rostock,
    Stralsund,
    Hamm,
    Paderborn,
    Fuerth,
    Schwerin,
    Muenster,
    Guetersloh,
    Neuss,
    Bayreuth,
    Tostedt,
    Braunschweig,
    Hof,
    Dueren,
    Bielefeld,
    Neubrandenburg,
    Aurich,
    Hagen,
    Ingolstadt,
    Stadthagen,
    Marburg,
    Straubing,
    Korbach,
}

#[derive(AnchorSerialize, AnchorDeserialize, Clone, Debug)]
pub enum RegistrationNumber {
    Germany {
        court: GermanCourt,
        court_hrb_id: u64,
    },
    France {
        siren_number: String,
    },
    Spain {
        registro_mercantil_number: String,
    },
    UnitedStates {
        ein: String,
    }, // Employer Identification Number
    UnitedKingdom {
        company_number: String,
    }, // Registered with Companies House
    Singapore {
        uen: String,
    }, // Unique Entity Number
    Switzerland {
        uid: String,
    }, // Unternehmens-Identifikationsnummer (Company Identification Number)
    Estonia {
        registry_code: String,
    }, // Known for its e-Residency program
    SouthKorea {
        registration_number: String,
    }, // Business Registration Number
    Japan {
        corporate_number: String,
    }, // Corporate Number (法人番号, Hōjin Bangō)
    Australia {
        abn: String,
    }, // Australian Business Number
    Canada {
        business_number: String,
    }, // Business Number
       // Extend with other countries...
}
