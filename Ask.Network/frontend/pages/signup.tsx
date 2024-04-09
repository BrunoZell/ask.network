import React, { useState, useEffect } from 'react';
import { Box, Button, Flex, Input, Text } from '@chakra-ui/react';
import { AppBar } from '../components/AppBar';
import * as anchor from '@project-serum/anchor';
import {
  useAnchorWallet,
  useConnection,
} from '@solana/wallet-adapter-react';
import { IdlAccounts, Program } from '@project-serum/anchor';
import { PublicKey } from '@solana/web3.js';
import idl from '../../solana/target/idl/ask_network.json';
import { AskNetwork } from '../../solana/target/types/ask_network';

const Page = () => {
  const [alias, setAlias] = useState('');
  const [program, setProgram] = useState<anchor.Program<AskNetwork>>();
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

    let provider: anchor.Provider;

    try {
      provider = anchor.getProvider();
    } catch (error) {
      if (wallet) {
        provider = new anchor.AnchorProvider(connection, wallet, {});
      }
    }

    if (provider) {
      try {
        const program = new anchor.Program(
          idl as anchor.Idl,
          new PublicKey('8WfQ3nACPcoBKxFnN4ekiHp8bRTd35R4L8Pu3Ak15is3'),
          provider
        );
        setProgram(program as Program<AskNetwork>);
      } catch (error) {
        console.log("error updating program");
      }
    } else {
      console.log("error")
    }
  }, [wallet]);

  const signUpOrganization = async () => {
    if (!wallet || !program || !alias.trim()) return;

    console.log("Create organization...");

    // Assume `global` account has already been initialized and is a singleton
    const [globalPda] = anchor.web3.PublicKey.findProgramAddressSync(
      [
        Buffer.from('global')
      ],
      program.programId
    );

    // Fetch the global account to get the running_organization_ordinal
    const globalAccount = await program.account.global.fetch(globalPda);

    console.log("Fetched global account:");
    console.log(globalAccount);

    // Deriving PDAs based on the provided seeds
    const [organizationPda] = anchor.web3.PublicKey.findProgramAddressSync(
      [
        Buffer.from('organization'),
        new anchor.BN(globalAccount.runningOrganizationOrdinal).toArrayLike(Buffer, 'le', 8),
      ],
      program.programId
    );

    const [initialMembershipPda] = anchor.web3.PublicKey.findProgramAddressSync(
      [
        Buffer.from('member'),
        new anchor.BN(globalAccount.runningOrganizationOrdinal).toArrayLike(Buffer, 'le', 8),
        wallet.publicKey.toBuffer()
      ],
      program.programId
    );

    console.log('Deploying organization ' + globalAccount.runningOrganizationOrdinal);

    // Building the instruction with derived accounts
    const tx = await program.methods
      .signUpOrganization({
        alias: alias.trim(),
      })
      .accounts({
        organizationAccount: organizationPda,
        initialMembership: initialMembershipPda,
        initialMemberLogin: wallet.publicKey,
        global: globalPda,
        systemProgram: anchor.web3.SystemProgram.programId,
        rent: anchor.web3.SYSVAR_RENT_PUBKEY,
      })
      .rpc();

    console.log('Transaction signature', tx);
  };

  return (
    <div>
      <Box w='full'>
        <AppBar />

        <Flex direction="column" justifyContent="center" alignItems="center" h="100vh">
          <Box mb="4">
            <Text mb="2">Alias</Text>
            <Input
              placeholder="Enter organization alias"
              value={alias}
              onChange={(e) => setAlias(e.target.value)}
            />
          </Box>

          <Button
            onClick={signUpOrganization}
            size="lg"
            colorScheme="teal"
            px="8"
            py="6"
            fontSize="xl"
            shadow="md"
            _hover={{ bg: "teal.500" }}
          >
            {alias.trim() ? `Create ${alias}` : "Create Organization"}
          </Button>
        </Flex>
      </Box>
    </div>
  );
};

export default Page;
