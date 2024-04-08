import React from 'react';
import {
    Box,
    Heading,
    List,
    ListItem,
    Container,
    Button,
    useColorModeValue,
} from '@chakra-ui/react';
import { AppBar } from '../components/AppBar';
import { ArrowForwardIcon } from '@chakra-ui/icons';
import Link from 'next/link';

const organizations = [
    { id: 0, name: 'Ask Network', description: 'This is Org One.' },
    { id: 1, name: 'RABOT CRYPTO GmbH', description: 'This is Org Two.' },
    { id: 2, name: 'Superteam Germany', description: 'This is Org Three.' },
];

const Page = () => {
    // Get the border color that works with the current color mode
    const borderColor = useColorModeValue('gray.200', 'gray.700');

    return (
        <Box>
            <AppBar />
            <Container maxW="container.xl">
                <Heading as="h1" size="xl" textAlign="center" my="40px">
                    Organizations on Ask Network
                </Heading>
                <List spacing={3}>
                    {organizations.map((org, index) => (
                        <ListItem
                            key={index}
                            padding="20px"
                            shadow="md"
                            borderWidth="1px"
                            borderRadius="md"
                            display="flex"
                            justifyContent="space-between"
                            alignItems="center"
                            borderColor={borderColor}
                        >
                            <Box>
                                <Heading as="h3" size="lg">{org.name}</Heading>
                                <Box>{org.description}</Box>
                            </Box>
                            <Link href={`/${org.id}`} passHref>
                                <Button as="a" variant="outline" borderColor={borderColor} colorScheme="teal" rightIcon={<ArrowForwardIcon />}>
                                    Details
                                </Button>
                            </Link>
                        </ListItem>
                    ))}
                </List>
            </Container>
        </Box>
    );
};

export default Page;
