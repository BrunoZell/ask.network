import { Box, Button, flexbox, Input, Stack, VStack } from '@chakra-ui/react';
import React, { useEffect, useState } from 'react';
import * as anchor from '@project-serum/anchor';
import {
  useAnchorWallet,
  useConnection,
  useWallet,
} from '@solana/wallet-adapter-react';
import idl from '../../target/idl/ask_network.json';
import { PublicKey } from '@solana/web3.js';
import { AskNetwork as AskIdl } from '../../target/types/ask_network';
import { IdlAccounts, Program } from '@project-serum/anchor';
import { AppBar } from '../components/AppBar';
import { getAccount, getAssociatedTokenAddress } from '@solana/spl-token';
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
  const [userPda, setUserPda] = useState<PublicKey>();

  const placeAsk = async () => {
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
        .accounts({ userAccount: userPda, ask: askPda })
        .rpc();

      scheduleAskRefetch(r => !r);
      console.log(
        `https://explorer.solana.com/tx/${tx}?cluster=devnet&customUrl=http://localhost:8899`
      );
    }
  };

  /**
   * Set User PDA
   * 
   * @dependency wallet - The effect re-runs whenever the wallet changes.
   * 
   * If the user's Program Derived Address (PDA) is not set:
   * 1. It attempts to find the PDA using the user's wallet public key and the program's ID.
   * 2. If successful, it sets the user's PDA in the component's state.
   * 3. Any errors encountered during this process are logged to the console.
   */
  useEffect(() => {
    if (userPda) return;
    try {
      const [pda] = anchor.web3.PublicKey.findProgramAddressSync(
        [wallet?.publicKey.toBuffer()],
        program.programId
      );
      setUserPda(pda);
    } catch (error) {
      console.log('🚀 ~ file: index.tsx:68 ~ useEffect ~ error', error);
    }
  }, [wallet]);

  const initializeUser = async () => {
    const [mint] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from('mint')],
      program.programId
    );
    const [authority] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from('authority')],
      program.programId
    );
    const ATA = await getAssociatedTokenAddress(mint, wallet.publicKey);
    if (!userPda) return;
    const tx = await program.methods
      .initializeUser()
      .accounts({
        userAccount: userPda,
        user: wallet.publicKey,
        mint,
        tokenAccount: ATA,
      })
      .rpc();

    console.log(
      `https://explorer.solana.com/tx/${tx}?cluster=devnet&customUrl=http://localhost:8899`
    );

    setIsInitialized(true);
  };

  const getAsks = async (counter: number) => {
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

  const stakeAsk = async (index: anchor.BN) => {
    const [askPda] = anchor.web3.PublicKey.findProgramAddressSync(
      [wallet.publicKey.toBuffer(), index.toArrayLike(Buffer, 'le', 8)],
      program.programId
    );
    const [mint] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from('mint')],
      program.programId
    );
    const [authority] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from('authority')],
      program.programId
    );

    const ata = await getAssociatedTokenAddress(mint, wallet.publicKey);

    const tx = await program.methods
      .stakeAsk(index)
      .accounts({ ask: askPda, mint, tokenAccount: ata, authority })
      .rpc();

    console.log(
      `https://explorer.solana.com/tx/${tx}?cluster=localnet&customUrl=http://localhost:8899`
    );

    scheduleAskRefetch(r => !r);
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
    const [askPda] = anchor.web3.PublicKey.findProgramAddressSync(
      [wallet.publicKey.toBuffer(), ordinal.toArrayLike(Buffer, 'le', 8)],
      program.programId
    );

    const tx = await program.methods
      .cancelAsk(ordinal)
      .accounts({ ask: askPda })
      .rpc();

    console.log(
      `https://explorer.solana.com/tx/${tx}?cluster=localnet&customUrl=http://localhost:8899`
    );

    scheduleAskRefetch(r => !r);
  };

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
    let provider: anchor.Provider;

    try {
      provider = anchor.getProvider();
    } catch (error) {
      if (wallet) {
        provider = new anchor.AnchorProvider(connection, wallet, {});
      }
    }

    if (provider) {
      const program = new anchor.Program(
        idl as anchor.Idl,
        new PublicKey('4ktm3bQPuEfsyGRR95QrkRdcrfb268hGzgjDr9Y17FGE'),
        provider
      );
      setProgram(program as Program<AskIdl>);
    }
  }, [wallet]);

  /**
   * Initialize user data
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
        const userAccount = await program.account.user.fetchNullable(userPda);
        if (userAccount) {
          await getAsks(userAccount.runningAskOrdinal.toNumber());

          setIsInitialized(true);
        }
      }
    })();
  }, [wallet, program, refetchAsks]);

  return (
    <div className=''>
      <Box justifyContent='center' alignContent='center' w='full'>
        <AppBar />

        {!wallet?.publicKey ? (
          <div>Connect your wallet 🧸</div>
        ) : !isInitialized ? (
          <Button onClick={initializeUser}>Init user</Button>
        ) : (
          <Stack
            w='700px'
            h='calc(100vh)'
            margin='auto'
            justify='center'
            align='center'>
            <div>
              <h2>Points</h2>
              {userBalance ?? ''}
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
                    <>
                      {Object.keys(ask.status).includes('completed') ? (
                        <span style={{ textDecoration: 'line-through' }}>
                          {ask?.content}
                        </span>
                      ) : (
                        ask.content
                      )}
                    </>
                    <div>
                      <Button onClick={() => cancelAsk(ask.ordinal)}>
                        Cancel
                      </Button>

                      {!Object.keys(ask.status).includes('completed') && (
                        <Button onClick={() => stakeAsk(ask.ordinal)}>
                          Stake
                        </Button>
                      )}
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
