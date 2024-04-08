import React from 'react';
import { Box, Flex, Heading, Textarea, FormControl, FormLabel } from '@chakra-ui/react';
import { AppBar } from '../components/AppBar';

const Page = () => {
  const organizationAlias = "RABOT CRYPTO"; // Replace with dynamic content as needed
  const offerText = `Profit Share of an automated trading bot based on the +EV-Crawler.
The profit share is paid out quarterly in EUROe to all Stakeholders proportionally.`; // Prefilled text

  return (
    <div>
      <Box w='full'>
        <AppBar />

        {/* Organization Alias as a fixed header just below the AppBar */}
        <Heading as="h1" size="2xl" my="4" textAlign="center">
          {organizationAlias}
        </Heading>

        {/* Main content area */}
        <Flex direction="column" alignItems="center" pt="4" pb="8">
          {/* Labeled offers text area */}
          <FormControl id="organization-offers" w={['90%', '70%', '50%', '40%']}>
            {/* Dynamic form label using organizationAlias */}
            <FormLabel>{`${organizationAlias}'s Offers`}</FormLabel>
            <Textarea
              placeholder="Enter your offers..."
              size="md"
              rows={10}
              defaultValue={offerText} // Set the default value for the textarea
            />
          </FormControl>
        </Flex>
      </Box>
    </div>
  );
};

export default Page;
