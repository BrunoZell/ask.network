import React from 'react';
import { Button, Box, Flex, Text } from '@chakra-ui/react';
import { useAnchorWallet } from '@solana/wallet-adapter-react';
import { Connection } from '@solana/web3.js';
import * as anchor from '@project-serum/anchor';
import idl from '../../solana/target/idl/ask_network.json';
import { AskNetwork } from '../../solana/target/types/ask_network';

const programID = new anchor.web3.PublicKey('FEZKARPjNEcugQZPddiCnAS6Quw9hryKkfNjtX8FLmCy'); // Replace with your program's public key

const InitializeGlobalPage = () => {
    const wallet = useAnchorWallet();

    const initializeGlobal = async () => {
        if (!wallet) {
            console.log("Please connect your wallet.");
            return;
        }

        const network = "http://127.0.0.1:8899"; // Adjust for your Solana cluster
        const provider = new anchor.AnchorProvider(
            new Connection(network),
            wallet,
            anchor.AnchorProvider.defaultOptions(),
        );
        const program = new anchor.Program<AskNetwork>(idl as any, programID, provider);

        try {
            const [globalPda] = anchor.web3.PublicKey.findProgramAddressSync(
                [Buffer.from('global')],
                program.programId
            );

            await program.methods
                .initializeGlobal({})
                .accounts({
                    global: globalPda,
                    signer: wallet.publicKey,
                    systemProgram: anchor.web3.SystemProgram.programId,
                    rent: anchor.web3.SYSVAR_RENT_PUBKEY,
                })
                .rpc();

            console.log('Global state initialized.');
        } catch (error) {
            console.error('Error initializing global state:', error);
        }
    };

    return (
        <Flex direction="column" justifyContent="center" alignItems="center" height="100vh">
            <Box>
                <Text mb="4" fontSize="xl">Initialize Global State</Text>
                <Button onClick={initializeGlobal} colorScheme="teal" size="md">
                    Initialize Global Account
                </Button>
            </Box>
        </Flex>
    );
};

export default InitializeGlobalPage;
