import React from 'react';
import { Box, Flex, Heading, Textarea, FormControl, FormLabel } from '@chakra-ui/react';
import { AppBar } from '../components/AppBar';

const Organization = () => {
  const organizationAlias = "RABOT CRYPTO"; // Replace with dynamic content as needed

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
            <FormLabel>The Organization's Offers</FormLabel>
            <Textarea
              placeholder="Enter your offers..."
              size="md"
              rows={10}
            />
          </FormControl>
        </Flex>
      </Box>
    </div>
  );
};

export default Organization;
