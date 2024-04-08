import React, { useState, useEffect } from 'react';
import {
  Box, VStack, Heading, Text, Divider, Grid, Icon, Textarea, Button, useToast, useBreakpointValue
} from '@chakra-ui/react';
import { AppBar } from '../components/AppBar';
import { ArrowRightIcon, ArrowDownIcon } from '@chakra-ui/icons';
import { useRouter } from 'next/router';
import { useWallet } from '@solana/wallet-adapter-react';

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
  const { organizationId } = router.query;
  const orgId = Array.isArray(organizationId) ? organizationId[0] : organizationId;
  const org = organizations.find(o => o.id === parseInt(orgId, 10));
  const { publicKey } = useWallet();
  const [isMember, setIsMember] = useState(false);
  const toast = useToast();

  useEffect(() => {
    if (org && publicKey) {
      setIsMember(org.members.includes(publicKey.toString()));
    }
  }, [org, publicKey]);

  // Using useBreakpointValue hook to dynamically switch between column and row layouts
  const arrowIcon = useBreakpointValue({ base: ArrowDownIcon, md: ArrowRightIcon });
  const gridTemplateColumns = useBreakpointValue({
    base: "1fr", // On small devices, have a single column layout
    md: "1fr auto 1fr" // On larger devices, split into 3 columns with the icon in the center
  });

  const handleChange = (e, index, field, subfield = null) => {
    const newValue = e.target.value;
    if (subfield) {
      // For strategy field changes
      org[field][index][subfield] = newValue;
    } else {
      // For offer field changes
      org[field][index] = newValue;
    }
    // This is a simplified example; for real applications, consider using state management libraries or context
  };

  const saveChanges = () => {
    toast({
      title: 'Changes saved.',
      description: "Your changes have been submitted to the blockchain.",
      status: 'success',
      duration: 9000,
      isClosable: true,
    });
    // Placeholder for on-chain submission logic
  };

  if (!org) {
    return <p>Organization not found</p>;
  }

  const isReadOnly = !isMember;

  return (
    <Box w='full'>
      <AppBar />
      <VStack spacing={8} align="stretch" my={8} mx="auto" maxW="container.md" px={4}>
        <Heading as="h1" size="2xl" textAlign="center">{org.alias}</Heading>
        <Text textAlign="center">{org.description}</Text>

        <Box as="section" borderWidth="1px" p={4} borderRadius="md">
          <Heading as="h2" size="lg" mb={4}>Offers</Heading>
          {org.offers.map((offer, index) => (
            <VStack key={index} divider={<Divider />} spacing={4}>
              {isMember ? (
                <Textarea
                  defaultValue={offer}
                  isReadOnly={isReadOnly}
                  resize="none"
                  size="lg"
                  w="100%"
                />
              ) : (
                <Text p={2}>{offer}</Text>
              )}
            </VStack>
          ))}
        </Box>

        <Box as="section" borderWidth="1px" p={4} borderRadius="md">
          <Heading as="h2" size="lg" mb={4}>Strategy</Heading>
          <VStack divider={<Divider />} spacing={4}>
            {org.strategy.map((strat, index) => (
              <Grid templateColumns={gridTemplateColumns} gap={4} alignItems="center" key={index}>
                {isMember ? (
                  <>
                    <Textarea
                      defaultValue={strat.condition}
                      isReadOnly={isReadOnly}
                      resize="none"
                      size="lg"
                      w="100%"
                    />
                    <Icon as={arrowIcon} color="gray.500" display="flex" justifyContent="center" alignItems="center" w="100%" />
                    <Textarea
                      defaultValue={strat.action}
                      isReadOnly={isReadOnly}
                      resize="none"
                      size="lg"
                      w="100%"
                    />
                  </>
                ) : (
                  <>
                    <Text p={2} textAlign="left">{strat.condition}</Text>
                    <Icon as={arrowIcon} color="gray.500" display="flex" justifyContent="center" alignItems="center" w="100%" />
                    <Text p={2} textAlign="right">{strat.action}</Text>
                  </>
                )}
              </Grid>
            ))}
          </VStack>
        </Box>

        {isMember && (
          <Button
            colorScheme="blue"
            onClick={saveChanges}
            mt={6}
          >
            Save Changes
          </Button>
        )}
      </VStack>
    </Box>
  );
};

export default OrganizationPage;
