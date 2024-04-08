import React from 'react';
import {
    Box,
    Heading,
    List,
    ListItem,
    Container,
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
    const borderColor = useColorModeValue('gray.200', 'gray.700');

    return (
        <Box>
            <AppBar />
            <Container maxW="container.xl">
                <Heading as="h1" size="xl" textAlign="center" my="40px">
                    Organizations on Ask Network
                </Heading>
                <List spacing={3}>
                    {organizations.map((org) => (
                        <Link key={org.id} href={`/${org.id}`} passHref>
                            <ListItem
                                key={org.id}
                                as="a"
                                padding="20px"
                                shadow="md"
                                borderWidth="1px"
                                borderRadius="md"
                                display="flex"
                                justifyContent="space-between"
                                alignItems="center"
                                borderColor={borderColor}
                                _hover={{ bg: useColorModeValue('gray.100', 'gray.700'), textDecoration: 'none' }}
                            >
                                <Box flex="1">
                                    <Heading as="h3" size="lg">{org.name}</Heading>
                                    <Box>{org.description}</Box>
                                </Box>
                                <Box as={ArrowForwardIcon} color={borderColor} />
                            </ListItem>
                        </Link>
                    ))}
                </List>
            </Container>
        </Box>
    );
};

export default Page;
