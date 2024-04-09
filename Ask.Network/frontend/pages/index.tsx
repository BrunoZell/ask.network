import React from 'react';
import { Box, Flex, Button, Link, Text, Icon } from '@chakra-ui/react';
import { AppBar } from '../components/AppBar';
import { ArrowForwardIcon } from '@chakra-ui/icons';
import NextLink from 'next/link'; // Make sure to import NextLink for client-side transitions

const Page = () => {
  return (
    <div>
      <Box w='full'>
        <AppBar />

        <Flex direction="column" justifyContent="center" alignItems="center" h="100vh">
          <NextLink href="/signup" passHref>
            <Button
              as="a" // Make the Button work as an anchor tag
              size="lg"
              colorScheme="teal"
              px="8"
              py="6"
              fontSize="xl"
              shadow="md"
              _hover={{ bg: "teal.500" }}
              mb="6" // Add some margin at the bottom
            >
              Create an Organization
            </Button>
          </NextLink>
          <NextLink href="/list" passHref>
            <Link
              color="teal.600" // Or any color that fits your design
              _hover={{ textDecoration: 'none', color: "teal.700" }} // Change color on hover
              fontSize="md" // Adjust the font size as needed
              display="flex"
              alignItems="center" // Aligns the text and icon
            >
              <Text>See all Organizations on Ask Network</Text>
              <Icon as={ArrowForwardIcon} ml="2" /> {/* Add some left margin to the icon */}
            </Link>
          </NextLink>
        </Flex>
      </Box>
    </div>
  );
};

export default Page;
