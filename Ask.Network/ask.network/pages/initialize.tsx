import { Box, Button } from '@chakra-ui/react';
import React, { useEffect, useState } from 'react';
import * as anchor from '@project-serum/anchor';
import {
  useAnchorWallet,
  useConnection,
} from '@solana/wallet-adapter-react';
import { ASK_NETWORK_PROGRAM_ID, METAPLEX_METADATA_PROGRAM_ID } from '../program-ids';
import idl from '../../solana/target/idl/ask_network.json';
import { PublicKey } from '@solana/web3.js';
import { AskNetwork as AskNetworkIdl } from '../../solana/target/types/ask_network';
import { AppBar } from '../components/AppBar';

const Page = () => {
  const [isInitialized, setIsInitialized] = useState(false);
  const [program, setProgram] = useState<anchor.Program<AskNetworkIdl>>();
  const { connection } = useConnection();
  const wallet = useAnchorWallet();

  /**
   * Initialize wallet provider and onchain program
   *
   * @dependency wallet - The effect re-runs whenever the user's wallet changes.
   *
   * This effect sets up the Anchor program provider for interacting with the Solana blockchain.
   * It first attempts to get the default provider. If that fails (e.g., when not in a browser environment),
   * it creates a new provider using the current wallet and connection. Once the provider is obtained,
   * it initializes the Anchor program with the Ask Network IDL and sets it in the local state.
   */
  useEffect(() => {
    console.log("Updating provider, then program...");

    let provider: anchor.Provider | undefined;

    try {
      provider = anchor.getProvider();
    } catch (error) {
      if (wallet) {
        provider = new anchor.AnchorProvider(connection, wallet, {});
      }
    }

    if (provider) {
      try {
        const program = new anchor.Program<AskNetworkIdl>(
          idl as unknown as AskNetworkIdl,
          ASK_NETWORK_PROGRAM_ID,
          provider
        );
        setProgram(program);
      } catch (error) {
        console.log("error updating program");
      }
    }
  }, [wallet]);

  /**
   * Fetch mint account data from the chain to enable or disable the init button accordingly.
   *
   * @dependency wallet - The effect re-runs whenever the user's wallet changes
   * @dependency program - The effect re-runs whenever the program changes.
   *
   * If the user's wallet is connected (i.e., wallet?.publicKey exists) and the program is defined:
   * 1. It calculates the PDA for the mint account using the program's ID and a static seed.
   * 2. It attempts to fetch the mint account data from the Solana program.
   * 3. If the mint account exists:
   *    a. Logs the existing mint account details to the console.
   *    b. Sets the component's state to indicate that the mint account has been initialized.
   * 4. If the mint account does not exist:
   *    a. Logs a message indicating the mint account does not exist.
   *    b. Sets the component's state to indicate that the mint account has not been initialized.
   */
  useEffect(() => {
    (async () => {
      if (wallet?.publicKey && program) {
        const [mintPda] = anchor.web3.PublicKey.findProgramAddressSync(
          [Buffer.from("mint")],
          program.programId
        );

        const mintAccount = await program.provider.connection.getAccountInfo(mintPda);

        if (mintAccount) {
          console.log('Mint account already exists:');
          console.log(mintAccount);

          setIsInitialized(true);
        } else {
          console.log('Mint account does not exist.');
          setIsInitialized(false);
        }
      }
    })();
  }, [wallet, program]);

  const initializeToken = async () => {
    console.log("initialize token");

    if (wallet?.publicKey && program) {
      const [mintPda] = anchor.web3.PublicKey.findProgramAddressSync(
        [Buffer.from("mint")],
        program.programId
      );

      const [authorityPda] = anchor.web3.PublicKey.findProgramAddressSync(
        [Buffer.from("authority")],
        program.programId
      );

      const tx = await program.methods
        .initializeToken()
        .accounts({
          signer: wallet.publicKey,
          mint: mintPda,
          authority: authorityPda
        })
        .rpc();

      console.log(`https://explorer.solana.com/tx/${tx}?cluster=devnet`);

      setIsInitialized(true);
    }
  };

  const initializeTreasuryClaims = async () => {
    console.log("Initializing Treasury Claims");

    if (wallet?.publicKey && program) {
      // Construct the accounts needed for the InitializeTreasuryClaims instruction
      const [treasuryClaimsOrdinalPda] = anchor.web3.PublicKey.findProgramAddressSync(
        [Buffer.from("treasury_claims_ordinal")],
        program.programId
      );

      const [treasuryClaimsCollectionAuthorityPda] = anchor.web3.PublicKey.findProgramAddressSync(
        [Buffer.from("treasury_claims_collection_authority")],
        program.programId
      );

      const [treasuryClaimsCollectionMintPda] = anchor.web3.PublicKey.findProgramAddressSync(
        [Buffer.from("treasury_claims_collection_mint")],
        program.programId
      );

      const [treasuryClaimsCollectionAtaPda] = anchor.web3.PublicKey.findProgramAddressSync(
        [Buffer.from("treasury_claims_collection_ata")],
        program.programId
      );

      const [metadataPda] = await anchor.web3.PublicKey.findProgramAddressSync(
        [
          Buffer.from("metadata"),
          METAPLEX_METADATA_PROGRAM_ID.toBuffer(),
          treasuryClaimsCollectionMintPda.toBuffer()
        ],
        METAPLEX_METADATA_PROGRAM_ID
      );

      // Call the initializeTreasuryClaims method
      const tx = await program.methods
        .initializeTreasuryClaims()
        .accounts({
          accounts: {
            signer: wallet.publicKey,
            treasuryClaimsOrdinal: treasuryClaimsOrdinalPda,
            treasuryClaimsCollectionAuthority: treasuryClaimsCollectionAuthorityPda,
            treasuryClaimsCollectionMint: treasuryClaimsCollectionMintPda,
            treasuryClaimsCollectionAta: treasuryClaimsCollectionAtaPda,
            metadataProgram: METAPLEX_METADATA_PROGRAM_ID,
            metadata: metadataPda,
          }
        })
        .rpc();

      console.log(`https://explorer.solana.com/tx/${tx}?cluster=devnet`);
    }
  }

  return (
    <div className=''>
      <Box justifyContent='center' alignContent='center' w='full'>
        <AppBar />

        {!wallet?.publicKey ? (
          <div>Connect your wallet 🧸</div>
        ) : isInitialized ? (
          <div>Token mint already initialized 🌻</div>
        ) : (
          <div>
            <Button onClick={initializeToken}>Initialize Token</Button>
          </div>
        )}

        <Button onClick={initializeTreasuryClaims}>Initialize Treasury Claims</Button>
      </Box>
    </div>
  );
};

export default Page;
