import { Box, Button, flexbox, Input, Stack, VStack } from '@chakra-ui/react';
import React, { useEffect, useState } from 'react';
import * as anchor from '@project-serum/anchor';
import {
  useAnchorWallet,
  useConnection,
  useWallet,
} from '@solana/wallet-adapter-react';
import idl from '../../solana/target/idl/ask_network.json';
import { PublicKey } from '@solana/web3.js';
import { AskNetwork as AskIdl } from '../../solana/target/types/ask_network';
import { IdlAccounts, Program } from '@project-serum/anchor';
import { AppBar } from '../components/AppBar';
import { createCloseAccountInstruction, getAccount, getAssociatedTokenAddress } from '@solana/spl-token';
import {
  IdlAccount,
  IdlEnumFields,
  IdlEnumVariant,
} from '@project-serum/anchor/dist/cjs/idl';

type Ask = IdlAccounts<AskIdl>['ask'];

const Page = () => {
  const [content, setContent] = useState('');
  const [userBalance, setUserBalance] = useState<number>();
  const [asks, setAsks] = useState<Ask[]>([]);
  const [isInitialized, setIsInitialized] = useState(false);
  const [refetchAsks, scheduleAskRefetch] = useState(false);
  const [program, setProgram] = useState<anchor.Program<AskIdl>>();
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
          new PublicKey('4ktm3bQPuEfsyGRR95QrkRdcrfb268hGzgjDr9Y17FGE'),
          provider
        );
        setProgram(program as Program<AskIdl>);
      } catch (error) {
        console.log("error updating program");
      }
    }
  }, [wallet]);

  /**
   * Fetch user account from chain
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
        const [userPda] = anchor.web3.PublicKey.findProgramAddressSync(
          [wallet?.publicKey.toBuffer()],
          program.programId
        );
        const userAccount = await program.account.user.fetchNullable(userPda);

        if (userAccount) {
          console.log('Fetched existing user account:');
          console.log(userAccount);

          await getAsks(userAccount.runningAskOrdinal.toNumber());

          setIsInitialized(true);
        } else {
          console.log('User account does not exist.');
        }
      }
    })();
  }, [wallet, program, refetchAsks]);

  const initializeUser = async () => {
    console.log("initialize user");

    const [userPda] = anchor.web3.PublicKey.findProgramAddressSync(
      [wallet?.publicKey.toBuffer()],
      program.programId
    );

    const tx = await program.methods
      .initializeUser()
      .accounts({
        userAccount: userPda,
        user: wallet.publicKey
      })
      .rpc();

    console.log(
      `https://explorer.solana.com/tx/${tx}?cluster=localnet&customUrl=http://localhost:8899`
    );

    setIsInitialized(true);
  };

  const getAsks = async (counter: number) => {
    console.log("Fetch users asks...");

    const askAccountKeys = [];
    try {
      for (let i = 0; i < counter; i++) {
        const [askPda] = anchor.web3.PublicKey.findProgramAddressSync(
          [
            wallet.publicKey.toBuffer(),
            new anchor.BN(i).toArrayLike(Buffer, 'le', 8),
          ],
          program.programId
        );
        askAccountKeys.push(new PublicKey(askPda));
      }

      const asks = await program.account.ask.fetchMultiple(askAccountKeys);
      setAsks(asks as Ask[]);
    } catch (error) {
      console.log('🚀 ~ file: index.tsx:118 ~ getAsks ~ error', error);
      setAsks([]);
    }
  };

  const placeAsk = async () => {
    console.log("place ask");

    const [userPda] = anchor.web3.PublicKey.findProgramAddressSync(
      [wallet?.publicKey.toBuffer()],
      program.programId
    );

    // Fetch users current ask ordinal, which is the index used for his next placed ask.
    const { runningAskOrdinal } = await program.account.user.fetch(userPda);

    // If it (and the user) exists, compute the new asks PDA and submit the transaction.
    if (runningAskOrdinal) {
      const nextRunningOrdinal: number = runningAskOrdinal.toNumber();

      const [askPda] = anchor.web3.PublicKey.findProgramAddressSync(
        [
          wallet.publicKey.toBuffer(),
          new anchor.BN(nextRunningOrdinal).toArrayLike(Buffer, 'le', 8),
        ],
        program.programId
      );

      const tx = await program.methods
        .placeAsk(content)
        .accounts({
          ask: askPda,
          userAccount: userPda,
          user: wallet.publicKey
        })
        .rpc();

      scheduleAskRefetch(r => !r);
      console.log(
        `https://explorer.solana.com/tx/${tx}?cluster=devnet&customUrl=http://localhost:8899`
      );
    }
  };

  /**
   * Send Cancel Ask Transaction
   * 
   * This function cancels an ask based on its ordinal. It first determines the associated
   * program address for the ask, then sends a cancellation request to the Solana network.
   * After the transaction is processed, it logs the transaction URL and schedules a refetch
   * to update the local state.
   */
  const cancelAsk = async (ordinal: anchor.BN) => {
    const [userPda] = anchor.web3.PublicKey.findProgramAddressSync(
      [wallet?.publicKey.toBuffer()],
      program.programId
    );
    const [askPda] = anchor.web3.PublicKey.findProgramAddressSync(
      [wallet.publicKey.toBuffer(), ordinal.toArrayLike(Buffer, 'le', 8)],
      program.programId
    );

    const tx = await program.methods
      .cancelAsk(ordinal)
      .accounts({
        ask: askPda,
        user: wallet.publicKey,
        userAccount: userPda
      })
      .rpc();

    console.log(
      `https://explorer.solana.com/tx/${tx}?cluster=localnet&customUrl=http://localhost:8899`
    );

    scheduleAskRefetch(r => !r);
  };

  return (
    <div className=''>
      <Box justifyContent='center' alignContent='center' w='full'>
        <AppBar />

        {!wallet?.publicKey ? (
          <div>Connect your wallet 🧸</div>
        ) : !isInitialized ? (
          <div>
            <Button onClick={initializeUser}>Sign up</Button>
          </div>
        ) : (
          <Stack
            w='700px'
            h='70%'
            margin='auto'
            justify='center'
            align='center'
            padding-top='80px'>

            <div>
              <h2>What do you want?</h2>
            </div>
            <Input
              onChange={e => setContent(e.currentTarget.value)}
              value={content}
              type='text'
            />
            <Button onClick={placeAsk}>Place Ask</Button>

            {asks &&
              asks
                .filter(ask => ask)
                .map((ask, i) => (
                  <div
                    key={i}
                    style={{
                      display: 'flex',
                      justifyContent: 'space-between',
                      alignItems: 'center',
                      width: '100%',
                    }}>

                    <span>
                      {ask?.content}
                    </span>

                    <div>
                      <Button onClick={() => cancelAsk(ask.ordinal)}>
                        Cancel
                      </Button>
                    </div>
                  </div>
                ))}
          </Stack>
        )}
      </Box>
    </div>
  );
};

export default Page;
