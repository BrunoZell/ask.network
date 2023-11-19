export type AskNetwork = {
  "version": "0.1.0",
  "name": "ask_network",
  "instructions": [
    {
      "name": "initializeUser",
      "accounts": [
        {
          "name": "userAccount",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "user",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "rent",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": []
    },
    {
      "name": "initializeToken",
      "accounts": [
        {
          "name": "signer",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "mint",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "authority",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "tokenProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "rent",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": []
    },
    {
      "name": "initializeTreasuryClaims",
      "accounts": [
        {
          "name": "signer",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "treasuryClaimsOrdinal",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "treasuryClaimsCollectionAuthority",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "SPL token authority account of the singleton treasury claim collection NFT."
          ]
        },
        {
          "name": "treasuryClaimsCollectionMint",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "SPL token mint account of the singleton treasury claim collection NFT."
          ]
        },
        {
          "name": "treasuryClaimsCollectionAta",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "Associated token account owned by the PDA treasury claims authority holding the newly minted treasury claim NFT."
          ]
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "Metaplex Metadata (PDA derived from ['metadata', program ID, mint ID])"
          ]
        },
        {
          "name": "splTokenProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "metadataProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "metadata",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "tokenProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "rent",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "associatedTokenProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": []
    },
    {
      "name": "placeAsk",
      "accounts": [
        {
          "name": "ask",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "userAccount",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "user",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": [
        {
          "name": "content",
          "type": "string"
        }
      ]
    },
    {
      "name": "updateAsk",
      "accounts": [
        {
          "name": "ask",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "userAccount",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "user",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": [
        {
          "name": "content",
          "type": "string"
        },
        {
          "name": "ordinal",
          "type": "u64"
        }
      ]
    },
    {
      "name": "cancelAsk",
      "accounts": [
        {
          "name": "ask",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "user",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "userAccount",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": [
        {
          "name": "ordinal",
          "type": "u64"
        }
      ]
    },
    {
      "name": "depositSol",
      "accounts": [
        {
          "name": "thisTreasuryClaim",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "depositor",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "communityTreasury",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "treasuryClaimsOrdinal",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "treasuryClaimsAuthority",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "Global singleton treasury claims authority.",
            "Each SPL treasury claim NFT has this PDA as authority."
          ]
        },
        {
          "name": "treasuryClaimMint",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "SPL token mint account of the new treasury claim NFT, uniquely addressed by the claim ordinal."
          ]
        },
        {
          "name": "treasuryClaimAta",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "Associated token account for the depositor holding the newly minted treasury claim NFT."
          ]
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "Metaplex Metadata (PDA derived from ['metadata', program ID, mint ID])"
          ]
        },
        {
          "name": "splTokenProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "metadataProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "metadata",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "rent",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "tokenProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "associatedTokenProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": [
        {
          "name": "lamportAmount",
          "type": "u64"
        }
      ]
    }
  ],
  "accounts": [
    {
      "name": "ask",
      "type": {
        "kind": "struct",
        "fields": [
          {
            "name": "content",
            "docs": [
              "Plain-text payload of this Ask, freely definable by the user.",
              "This is to be translated into a causal query for matching with offers."
            ],
            "type": "string"
          },
          {
            "name": "ordinal",
            "docs": [
              "A numeric index of this Ask local to the user. The tuple (user.key, ordinal)",
              "uniquely addresses an Ask. Keep in mind that Asks are mutable."
            ],
            "type": "u64"
          }
        ]
      }
    },
    {
      "name": "user",
      "type": {
        "kind": "struct",
        "fields": [
          {
            "name": "runningAskOrdinal",
            "docs": [
              "Total amount of asks the user has placed until now.",
              "Used as an ever increasing identifier for asks."
            ],
            "type": "u64"
          }
        ]
      }
    },
    {
      "name": "tokenAuthority",
      "type": {
        "kind": "struct",
        "fields": []
      }
    },
    {
      "name": "treasuryClaimsAuthority",
      "type": {
        "kind": "struct",
        "fields": []
      }
    },
    {
      "name": "treasuryClaim",
      "docs": [
        "Data payload of each minted treasury claim NFT"
      ],
      "type": {
        "kind": "struct",
        "fields": [
          {
            "name": "ordinal",
            "type": "u64"
          },
          {
            "name": "unitOfValue",
            "type": {
              "defined": "TreasuryCurrency"
            }
          },
          {
            "name": "depositAmount",
            "type": "u64"
          },
          {
            "name": "depositTimestamp",
            "type": "i64"
          }
        ]
      }
    },
    {
      "name": "treasuryClaimsOrdinal",
      "type": {
        "kind": "struct",
        "fields": [
          {
            "name": "claimsIssued",
            "docs": [
              "The amount of treasury claim NFTs minted.",
              "Each treasury claim NFT gets a unique ordinal assigned, sequencing all claims.",
              "This is used to derive the token mints PDA and for the NFT to have an additional trait of deposit precedence.",
              "On each deposit, this counter is incremented by 1."
            ],
            "type": "u64"
          }
        ]
      }
    }
  ],
  "types": [
    {
      "name": "TreasuryCurrency",
      "type": {
        "kind": "enum",
        "variants": [
          {
            "name": "SOL"
          },
          {
            "name": "USDC"
          },
          {
            "name": "ETH"
          }
        ]
      }
    }
  ],
  "errors": [
    {
      "code": 6000,
      "name": "InsufficientFunds",
      "msg": "Insufficient funds to purchase token."
    },
    {
      "code": 6001,
      "name": "InvalidCommunityTreasuryAddress",
      "msg": "The provided community treasury address is invalid."
    }
  ]
};

export const IDL: AskNetwork = {
  "version": "0.1.0",
  "name": "ask_network",
  "instructions": [
    {
      "name": "initializeUser",
      "accounts": [
        {
          "name": "userAccount",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "user",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "rent",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": []
    },
    {
      "name": "initializeToken",
      "accounts": [
        {
          "name": "signer",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "mint",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "authority",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "tokenProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "rent",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": []
    },
    {
      "name": "initializeTreasuryClaims",
      "accounts": [
        {
          "name": "signer",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "treasuryClaimsOrdinal",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "treasuryClaimsCollectionAuthority",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "SPL token authority account of the singleton treasury claim collection NFT."
          ]
        },
        {
          "name": "treasuryClaimsCollectionMint",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "SPL token mint account of the singleton treasury claim collection NFT."
          ]
        },
        {
          "name": "treasuryClaimsCollectionAta",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "Associated token account owned by the PDA treasury claims authority holding the newly minted treasury claim NFT."
          ]
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "Metaplex Metadata (PDA derived from ['metadata', program ID, mint ID])"
          ]
        },
        {
          "name": "splTokenProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "metadataProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "metadata",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "tokenProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "rent",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "associatedTokenProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": []
    },
    {
      "name": "placeAsk",
      "accounts": [
        {
          "name": "ask",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "userAccount",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "user",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": [
        {
          "name": "content",
          "type": "string"
        }
      ]
    },
    {
      "name": "updateAsk",
      "accounts": [
        {
          "name": "ask",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "userAccount",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "user",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": [
        {
          "name": "content",
          "type": "string"
        },
        {
          "name": "ordinal",
          "type": "u64"
        }
      ]
    },
    {
      "name": "cancelAsk",
      "accounts": [
        {
          "name": "ask",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "user",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "userAccount",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": [
        {
          "name": "ordinal",
          "type": "u64"
        }
      ]
    },
    {
      "name": "depositSol",
      "accounts": [
        {
          "name": "thisTreasuryClaim",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "depositor",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "communityTreasury",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "treasuryClaimsOrdinal",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "treasuryClaimsAuthority",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "Global singleton treasury claims authority.",
            "Each SPL treasury claim NFT has this PDA as authority."
          ]
        },
        {
          "name": "treasuryClaimMint",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "SPL token mint account of the new treasury claim NFT, uniquely addressed by the claim ordinal."
          ]
        },
        {
          "name": "treasuryClaimAta",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "Associated token account for the depositor holding the newly minted treasury claim NFT."
          ]
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "Metaplex Metadata (PDA derived from ['metadata', program ID, mint ID])"
          ]
        },
        {
          "name": "splTokenProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "metadataProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "metadata",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "rent",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "tokenProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "associatedTokenProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": [
        {
          "name": "lamportAmount",
          "type": "u64"
        }
      ]
    }
  ],
  "accounts": [
    {
      "name": "ask",
      "type": {
        "kind": "struct",
        "fields": [
          {
            "name": "content",
            "docs": [
              "Plain-text payload of this Ask, freely definable by the user.",
              "This is to be translated into a causal query for matching with offers."
            ],
            "type": "string"
          },
          {
            "name": "ordinal",
            "docs": [
              "A numeric index of this Ask local to the user. The tuple (user.key, ordinal)",
              "uniquely addresses an Ask. Keep in mind that Asks are mutable."
            ],
            "type": "u64"
          }
        ]
      }
    },
    {
      "name": "user",
      "type": {
        "kind": "struct",
        "fields": [
          {
            "name": "runningAskOrdinal",
            "docs": [
              "Total amount of asks the user has placed until now.",
              "Used as an ever increasing identifier for asks."
            ],
            "type": "u64"
          }
        ]
      }
    },
    {
      "name": "tokenAuthority",
      "type": {
        "kind": "struct",
        "fields": []
      }
    },
    {
      "name": "treasuryClaimsAuthority",
      "type": {
        "kind": "struct",
        "fields": []
      }
    },
    {
      "name": "treasuryClaim",
      "docs": [
        "Data payload of each minted treasury claim NFT"
      ],
      "type": {
        "kind": "struct",
        "fields": [
          {
            "name": "ordinal",
            "type": "u64"
          },
          {
            "name": "unitOfValue",
            "type": {
              "defined": "TreasuryCurrency"
            }
          },
          {
            "name": "depositAmount",
            "type": "u64"
          },
          {
            "name": "depositTimestamp",
            "type": "i64"
          }
        ]
      }
    },
    {
      "name": "treasuryClaimsOrdinal",
      "type": {
        "kind": "struct",
        "fields": [
          {
            "name": "claimsIssued",
            "docs": [
              "The amount of treasury claim NFTs minted.",
              "Each treasury claim NFT gets a unique ordinal assigned, sequencing all claims.",
              "This is used to derive the token mints PDA and for the NFT to have an additional trait of deposit precedence.",
              "On each deposit, this counter is incremented by 1."
            ],
            "type": "u64"
          }
        ]
      }
    }
  ],
  "types": [
    {
      "name": "TreasuryCurrency",
      "type": {
        "kind": "enum",
        "variants": [
          {
            "name": "SOL"
          },
          {
            "name": "USDC"
          },
          {
            "name": "ETH"
          }
        ]
      }
    }
  ],
  "errors": [
    {
      "code": 6000,
      "name": "InsufficientFunds",
      "msg": "Insufficient funds to purchase token."
    },
    {
      "code": 6001,
      "name": "InvalidCommunityTreasuryAddress",
      "msg": "The provided community treasury address is invalid."
    }
  ]
};
