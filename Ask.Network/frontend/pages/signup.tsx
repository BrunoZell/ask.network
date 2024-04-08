import React, { useState } from 'react';
import { Box, Button, Flex, Input, Text } from '@chakra-ui/react';
import { AppBar } from '../components/AppBar';

const Page = () => {
  const [alias, setAlias] = useState('');

  return (
    <div>
      <Box w='full'>
        <AppBar />

        <Flex direction="column" justifyContent="center" alignItems="center" h="100vh">
          <Box mb="4">
            <Text mb="2">Alias</Text>
            <Input
              placeholder="Enter organization alias"
              value={alias}
              onChange={(e) => setAlias(e.target.value)}
            />
          </Box>

          <Button
            size="lg"
            colorScheme="teal"
            px="8"
            py="6"
            fontSize="xl"
            shadow="md"
            _hover={{ bg: "teal.500" }}
          >
            {alias.trim() ? `Create ${alias}` : "Create Organization"}
          </Button>
        </Flex>
      </Box>
    </div>
  );
};

export default Page;
