import React, { useState, useEffect } from 'react';
import {
  Box, VStack, Heading, Text, Divider, Grid, Icon, Textarea, Button, useToast, useBreakpointValue
} from '@chakra-ui/react';
import { AppBar } from '../components/AppBar';
import { ArrowRightIcon, ArrowDownIcon } from '@chakra-ui/icons';
import { useRouter } from 'next/router';
import * as anchor from '@project-serum/anchor';
import {
  useAnchorWallet,
  useConnection,
} from '@solana/wallet-adapter-react';
import { IdlAccounts, Program } from '@project-serum/anchor';
import { PublicKey } from '@solana/web3.js';
import idl from '../../solana/target/idl/ask_network.json';
import { AskNetwork } from '../../solana/target/types/ask_network';

type Organization = IdlAccounts<AskNetwork>['organization'];

const Page = () => {
  const router = useRouter();
  const [organization, setOrganization] = useState<Organization>(null);
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
          new PublicKey('JDX5MmkTDxAwuQomKL3xT9ycETa6T7NNPWFFCAKX1gc9'),
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
   * Fetch organization account from chain
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
      if (wallet?.publicKey && program && router && router.query) {
        const organizationId = router.query.organizationId;

        console.log(organizationId);

        if (!organizationId) {
          console.log("organization id undefined");
          return;
        }

        const orgIdInt = parseInt(organizationId as string);

        console.log("Trying to fetch organization " + orgIdInt + " from chian");

        const [organizationPda] = anchor.web3.PublicKey.findProgramAddressSync(
          [
            Buffer.from('organization'),
            new anchor.BN(orgIdInt).toArrayLike(Buffer, 'le', 8),
          ],
          program.programId
        );
        const organizationAccount = await program.account.organization.fetchNullable(organizationPda);

        if (organizationAccount) {
          console.log('Fetched existing organization account ' + organizationPda + ":");
          console.log(organizationAccount);

          setOrganization(organizationAccount);

          // await getMembership(globalAccount.runningOrganizationOrdinal.toNumber());

          setIsInitialized(true);
        } else {
          console.log('Organization account ' + organizationId + ' does not exist.');
          setOrganization(null);
          setIsInitialized(true);
        }
      }
    })();
  }, [wallet, program, router]);

  return (
    <Box w='full'>
      <AppBar />

      {!isInitialized ? (
        <div>Loading 🧸</div>
      ) : !organization ? (
        <div>Organization does not exist</div>
      ) : (
        <VStack spacing={8} align="stretch" my={8} mx="auto" maxW="container.md" px={4}>
          <Heading as="h1" size="2xl" textAlign="center">{organization.alias}</Heading>
          {/* <Text textAlign="center">{org.description}</Text> */}
          {/* 
          {isMember && (
            <Button
              colorScheme="blue"
              onClick={saveChanges}
              mt={6}
            >
              Save Changes
            </Button>
          )} */}
        </VStack>
      )}
    </Box>
  );
}

// const OrganizationPage = () => {
//   const wallet = useAnchorWallet(); // This hooks into the wallet adapter
//   const [program, setProgram] = useState<anchor.Program<AskNetwork>>();

//   useEffect(() => {
//     if (wallet) {
//       // Setup the connection and program when the wallet changes
//       const network = "http://127.0.0.1:8899"; // Adjust according to your network; could be a Devnet, Testnet, or Mainnet URL
//       const provider = new anchor.AnchorProvider(
//         new Connection(network),
//         wallet,
//         anchor.AnchorProvider.defaultOptions()
//       );
//       const program = new anchor.Program<AskNetwork>(idl as any, programID, provider);
//       setProgram(program);
//     }
//   }, [wallet]); // This effect depends on the wallet

//   const router = useRouter();
//   const { organizationId } = router.query;
//   const orgId = Array.isArray(organizationId) ? organizationId[0] : organizationId;
//   const org = { alias: "test", description: "test" };
//   const { publicKey } = useWallet();
//   const [isMember, setIsMember] = useState(false);
//   const toast = useToast();

//   useEffect(() => {
//     if (org && publicKey) {
//       setIsMember(false);
//       // Assume you have a way to obtain `runningOrganizationOrdinal` for the organization
//       const runningOrganizationOrdinal = orgId; // This needs to be correctly obtained

//       // Convert the ordinal to bytes in little-endian format, assuming it's a BigInt or BN.js instance
//       const ordinalBytes = new anchor.BN(runningOrganizationOrdinal).toArrayLike(Buffer, 'le', 8);

//       // Deriving the PDA for the initial membership based on the seeds
//       // Note: This is an async operation, and we're using `findProgramAddress` which returns a Promise
//       const [memberPda] = anchor.web3.PublicKey.findProgramAddressSync(
//         [Buffer.from('member'), ordinalBytes, publicKey.toBuffer()],
//         programID
//       );

//       // Once you have the PDA, you need to check if an account exists at this address
//       // This is also an async operation
//       program.account.membership.fetchNullable(memberPda).then(membershipAccount => {
//         // If an account is returned, the wallet is a member
//         setIsMember(!!membershipAccount);
//       }).catch(err => {
//         console.error("Error fetching membership account:", err);
//         setIsMember(false);
//       });
//     }
//   }, [org, publicKey]);

//   // Using useBreakpointValue hook to dynamically switch between column and row layouts
//   const arrowIcon = useBreakpointValue({ base: ArrowDownIcon, md: ArrowRightIcon });
//   const gridTemplateColumns = useBreakpointValue({
//     base: "1fr", // On small devices, have a single column layout
//     md: "1fr auto 1fr" // On larger devices, split into 3 columns with the icon in the center
//   });

//   const handleChange = (e, index, field, subfield = null) => {
//     const newValue = e.target.value;
//     if (subfield) {
//       // For strategy field changes
//       org[field][index][subfield] = newValue;
//     } else {
//       // For offer field changes
//       org[field][index] = newValue;
//     }
//     // This is a simplified example; for real applications, consider using state management libraries or context
//   };

//   const saveChanges = () => {
//     toast({
//       title: 'Changes saved.',
//       description: "Your changes have been submitted to the blockchain.",
//       status: 'success',
//       duration: 9000,
//       isClosable: true,
//     });
//     // Placeholder for on-chain submission logic
//   };

//   if (!org) {
//     return <p>Organization not found</p>;
//   }

//   const isReadOnly = !isMember;

//   return (
//     <Box w='full'>
//       <AppBar />
//       <VStack spacing={8} align="stretch" my={8} mx="auto" maxW="container.md" px={4}>
//         <Heading as="h1" size="2xl" textAlign="center">{org.alias}</Heading>
//         <Text textAlign="center">{org.description}</Text>


//         {isMember && (
//           <Button
//             colorScheme="blue"
//             onClick={saveChanges}
//             mt={6}
//           >
//             Save Changes
//           </Button>
//         )}
//       </VStack>
//     </Box>
//   );
// };

export default Page;
