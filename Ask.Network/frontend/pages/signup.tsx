import React, { useState, useEffect } from 'react';
import { Box, Button, Flex, Input, Text } from '@chakra-ui/react';
import { useAnchorWallet } from '@solana/wallet-adapter-react';
import { Connection, PublicKey } from '@solana/web3.js';
import * as anchor from '@project-serum/anchor';
import idl from '../../solana/target/idl/ask_network.json';
import { AskNetwork } from '../../solana/target/types/ask_network';
import { AppBar } from '../components/AppBar';

const programID = new PublicKey('8WfQ3nACPcoBKxFnN4ekiHp8bRTd35R4L8Pu3Ak15is3'); // Replace with your program's public key

const Page = () => {
  const [alias, setAlias] = useState('');
  const wallet = useAnchorWallet();
  const [program, setProgram] = useState<anchor.Program<AskNetwork>>();

  useEffect(() => {
    if (wallet) {
      const network = "https://api.devnet.solana.com"; // or your Solana cluster of choice
      const provider = new anchor.AnchorProvider(
        new Connection(network),
        wallet,
        anchor.AnchorProvider.defaultOptions(),
      );
      const program = new anchor.Program<AskNetwork>(idl as any, programID, provider);
      setProgram(program);
    }
  }, [wallet]);

  const signUpOrganization = async () => {
    if (!wallet || !program || !alias.trim()) return;

    try {
      // Assume `global` account has already been initialized and is a singleton
      const [globalPda] = anchor.web3.PublicKey.findProgramAddressSync(
        [Buffer.from('global')],
        program.programId
      );

      // Fetch the global account to get the running_organization_ordinal
      const globalAccount = await program.account.global.fetch(globalPda);

      console.log(globalAccount);

      const ordinalBytes = new anchor.BN(globalAccount.runningOrganizationOrdinal).toArrayLike(Buffer, 'le', 8);

      // Deriving PDAs based on the provided seeds
      const [organizationPda] = anchor.web3.PublicKey.findProgramAddressSync(
        [Buffer.from('organization'), ordinalBytes],
        program.programId
      );

      const [initialMembershipPda] = anchor.web3.PublicKey.findProgramAddressSync(
        [Buffer.from('member'), ordinalBytes, wallet.publicKey.toBuffer()],
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
    } catch (error) {
      console.error('Error signing up organization:', error);
    }
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
