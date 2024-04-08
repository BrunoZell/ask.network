import React from 'react';
import { Box, Flex, Heading, Textarea, FormControl, FormLabel } from '@chakra-ui/react';
import { AppBar } from '../components/AppBar';
import { useRouter } from 'next/router';

const organizations = [
  {
    id: 0,
    alias: 'Ask Network',
    description: 'Process mining',
    offers: [
      ''
    ],
    strategy: [

    ],
    members: [
      'ApVKLr6pY3QpwYVRyg2kj4i5FyGMrgZz5CBCXLbvWuvm'
    ]
  },
  {
    id: 1,
    alias: 'RABOT CRYPTO GmbH',
    description: 'Operates the +EV-Crawler as a trading bot, sharing its profits with stakeholders',
    offers: [
      'Profit Share of an automated trading bot based on the +EV-Crawler. The profit share is paid out quarterly in EUROe to all Stakeholders proportionally.'
    ],
    strategy: [
      {
        condition: 'At the end of every quarter',
        action: 'Sum up all profits that quarter and pay them out as a GmbH profit distribution (Gewinnausschüttung)'
      },
      {
        condition: 'On every GmbH profit distribution',
        action: 'Deposit the share of all $RABOT token holders into the smart contract 0xd586D47f880AC1a18337932010005B175ACea41E in EUROe'
      }
    ],
    members: [
      'ApVKLr6pY3QpwYVRyg2kj4i5FyGMrgZz5CBCXLbvWuvm'
    ]
  },
  {
    id: 2,
    alias: 'Superteam Germany',
    description: 'Supporting builders throughout the German Solana Community',
    offers: [
      'Hackathon Pitch Reviews'
    ],
    strategy: [
      {
        condition: 'On every global Solana Hackathon',
        action: 'Organizing a Build Station in Berlin'
      },
      {
        condition: 'On every global Solana Hackathon',
        action: 'Organizing a Build Station in Munich'
      }
    ],
    members: [
      'ApVKLr6pY3QpwYVRyg2kj4i5FyGMrgZz5CBCXLbvWuvm'
    ]
  },
];

const OrganizationPage = () => {
  const router = useRouter();
  const { organizationId } = router.query; // You get the ID from the URL
  const orgId = Array.isArray(organizationId) ? organizationId[0] : organizationId;
  const org = organizations.find(o => o.id === parseInt(orgId, 10));

  // If the organization doesn't exist, you can return null or show a not found message
  if (!org) {
    return <p>Organization not found</p>;
  }

  return (
    <div>
      <Box w='full'>
        <AppBar />

        <Heading as="h1" size="2xl" my="4" textAlign="center">
          {org.alias}
        </Heading>

        <Flex direction="column" alignItems="center" pt="4" pb="8">
          <FormControl id="organization-offers" w={['90%', '70%', '50%', '40%']}>
            <FormLabel>{`${org.alias}'s Offers`}</FormLabel>
            <Textarea
              placeholder="Enter your offers..."
              size="md"
              rows={10}
              defaultValue={org.description}
            />
          </FormControl>
        </Flex>
      </Box>
    </div>
  );
};

export default OrganizationPage;
