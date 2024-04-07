import { FC } from 'react';
import styles from '../styles/Home.module.css';
import dynamic from 'next/dynamic'
import Image from 'next/image';

const WalletMultiButtonDynamic = dynamic(
  async () =>
    (await import('@solana/wallet-adapter-react-ui')).WalletMultiButton,
  { ssr: false }
)

export const AppBar: FC = () => {
  return (
    <div className={styles.AppHeader}>
      <span>Ask Network</span>

      <WalletMultiButtonDynamic />
    </div>
  );
};
