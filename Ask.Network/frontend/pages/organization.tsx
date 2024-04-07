import React, { useEffect, useRef } from 'react';
import { Box, Flex } from '@chakra-ui/react';
import { AppBar } from '../components/AppBar';

const Organization = () => {
  return (
    <div>
      <Box w='full'>
        <AppBar />

        <Flex direction="column" justifyContent="center" alignItems="center" h="100vh">

        </Flex>
      </Box>
    </div>
  );
};

export default Organization;
