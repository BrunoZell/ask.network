import { FC } from 'react';
import styles from '../styles/Home.module.css';
import dynamic from 'next/dynamic'
import Link from 'next/link';

const WalletMultiButtonDynamic = dynamic(
  async () =>
    (await import('@solana/wallet-adapter-react-ui')).WalletMultiButton,
  { ssr: false }
)

export const AppBar: FC = () => {
  return (
    <div className={styles.AppHeader}>
      {/* These pseudo-elements "::before" and "::after" are used to balance the space */}
      <Link href="/" passHref>
        <a className={styles.AppHeaderBrand}>Ask Network</a>
      </Link>
      <div style={{ flex: 1 }}>
      </div>
      <WalletMultiButtonDynamic />
    </div>
  );
};
