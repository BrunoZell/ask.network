import React, { useEffect, useState } from 'react';
import {
    Box,
    Heading,
    List,
    ListItem,
    Container,
    useColorModeValue,
    Icon
} from '@chakra-ui/react';
import * as anchor from '@project-serum/anchor';
import {
    useAnchorWallet,
    useConnection,
} from '@solana/wallet-adapter-react';
import { AppBar } from '../components/AppBar';
import { IdlAccounts, Program } from '@project-serum/anchor';
import { PublicKey } from '@solana/web3.js';
import { ArrowForwardIcon } from '@chakra-ui/icons';
import Link from 'next/link';
import idl from '../../solana/target/idl/ask_network.json';
import { AskNetwork } from '../../solana/target/types/ask_network';

const organizations = [
    { id: 0, name: 'Ask Network', description: 'This is Org One.' },
    { id: 1, name: 'RABOT CRYPTO GmbH', description: 'This is Org Two.' },
    { id: 2, name: 'Superteam Germany', description: 'This is Org Three.' },
];

type Organization = IdlAccounts<AskNetwork>['organization'];

const Page = () => {
    const borderColor = useColorModeValue('gray.200', 'gray.500');
    const arrowColor = useColorModeValue('blue.200', 'blue.500');
    const [organizations, setOrganizations] = useState<Organization[]>([]);
    const [isInitialized, setIsInitialized] = useState(false);
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


    /**
     * Fetch global account from chain
     * 
     * @dependency wallet - The effect re-runs whenever the user's wallet changes
     * @dependency program - The effect re-runs whenever the program changes.
     * @dependency refetchAsks - The effect re-runs whenever the user modifies his asks.
     * 
     * If the user's wallet is connected (i.e., wallet?.publicKey exists) and the program is defined:
     * 1. It fetches the user's account data from the Solana program.
     * 2. If the user account exists:
     *    a. It retrieves the user's asks up to the current runningAskOrdinal.
     *    b. It sets the component's state to indicate that the user's data has been initialized.
     */
    useEffect(() => {
        (async () => {
            if (wallet?.publicKey && program) {
                const [globalPda] = anchor.web3.PublicKey.findProgramAddressSync(
                    [Buffer.from('global')],
                    program.programId
                );
                const globalAccount = await program.account.global.fetchNullable(globalPda);

                if (globalAccount) {
                    console.log('Fetched existing global account:');
                    console.log(globalAccount);

                    await getOrganizations(globalAccount.runningOrganizationOrdinal.toNumber());

                    setIsInitialized(true);
                } else {
                    console.log('Global account does not exist.');
                }
            }
        })();
    }, [wallet, program]);


    const getOrganizations = async (counter: number) => {
        console.log("Fetch all " + counter + " organizations...");

        const organizationAccountKeys = [];
        try {
            for (let i = 0; i < counter; i++) {
                const [organizationPda] = anchor.web3.PublicKey.findProgramAddressSync(
                    [
                        [Buffer.from('organization')],
                        new anchor.BN(i).toArrayLike(Buffer, 'le', 8),
                    ],
                    program.programId
                );
                organizationAccountKeys.push(new PublicKey(organizationPda));
            }

            const organizations = await program.account.organization.fetchMultiple(organizationAccountKeys);

            console.log(organizations);
            setOrganizations(organizations as Organization[]);
        } catch (error) {
            console.log('failed to fetch all Organizations from chain', error);
            setOrganizations([]);
        }
    };

    return (
        <Box>
            <AppBar />
            <Container maxW="container.xl">
                <Heading as="h1" size="xl" textAlign="center" my="40px">
                    Organizations on Ask Network
                </Heading>
                <List spacing={3}>
                    {organizations.map((org, i) => (
                        <Link key={i} href={`/${i}`} passHref>
                            <ListItem
                                as="a"
                                padding="20px"
                                shadow="md"
                                borderWidth="1px"
                                borderRadius="md"
                                display="flex"
                                justifyContent="space-between"
                                alignItems="center"
                                borderColor={borderColor}
                                _hover={{ bg: useColorModeValue('gray.100', 'gray.700'), textDecoration: 'none' }}
                            >
                                <Box flex="1">
                                    <Heading as="h3" size="lg">{org?.alias ?? ""}</Heading>
                                    {/* <Box>{org.description}</Box> */}
                                </Box>
                                <Icon as={ArrowForwardIcon} w={14} h={14} color={arrowColor} />
                            </ListItem>
                        </Link>
                    ))}
                </List>
            </Container>
        </Box>
    );
}

export default Page;
