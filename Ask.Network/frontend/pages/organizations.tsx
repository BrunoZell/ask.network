import React from 'react';
import { Box, Heading, List, ListItem, Container } from '@chakra-ui/react';
import { AppBar } from '../components/AppBar';

const organizations = [
    { name: 'Ask Network', description: 'This is Org One.' },
    { name: 'RABOT CRYPTO GmbH', description: 'This is Org Two.' },
    { name: 'Superteam Germany', description: 'This is Org Three.' },
];

const Page = () => {
    return (
        <Box>
            <AppBar />
            <Container maxW="container.xl">
                <Heading as="h1" size="xl" textAlign="center" my="40px">
                    Organizations on Ask Network
                </Heading>
                <List spacing={3}>
                    {organizations.map((org, index) => (
                        <ListItem key={index} padding="20px" shadow="md" borderWidth="1px" borderRadius="md">
                            <Heading as="h3" size="lg">{org.name}</Heading>
                            <Box>{org.description}</Box>
                        </ListItem>
                    ))}
                </List>
            </Container>
        </Box>
    );
};

export default Page;