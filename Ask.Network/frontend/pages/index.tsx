import React, { useEffect, useRef } from 'react';
import { Box, Button, Flex, flexbox, Input, Stack, VStack } from '@chakra-ui/react';
import { AppBar } from '../components/AppBar';

const HomePage = () => {
  return (
    <div>
      <Box w='full'>
        <AppBar />

        {/* Use Flex instead of div for centering */}
        <Flex justifyContent="center" alignItems="center" h="100vh">
          {/* h="100vh" ensures the container takes full viewport height */}

          <Button
            size="lg" // Makes the button larger
            colorScheme="teal" // A predefined color scheme for a visually appealing look
            px="8" // Padding left and right for a wider button
            py="6" // Padding top and bottom for a taller button
            fontSize="xl" // Larger font size for the button text
            shadow="md" // Adds a slight shadow for depth
            _hover={{ bg: "teal.500" }} // Changes background color on hover for interactivity
          >
            Create an Organization
          </Button>
        </Flex>
      </Box>
    </div>
  )
};

export default HomePage;
