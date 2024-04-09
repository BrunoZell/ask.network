import React, { useEffect, useState, useRef } from 'react';
import { Box, Heading, Container } from '@chakra-ui/react';
import { AppBar } from '../components/AppBar';
import * as anchor from '@project-serum/anchor';
import { useConnection } from '@solana/wallet-adapter-react';
import idl from '../../solana/target/idl/ask_network.json';
import * as d3 from 'd3';
import { AskNetwork } from '../../solana/target/types/ask_network';
import { IdlAccounts } from '@project-serum/anchor';
import { Connection, PublicKey } from '@solana/web3.js';

type Organization = IdlAccounts<AskNetwork>['organization'];

const Page = () => {
    const [organizations, setOrganizations] = useState<Organization[]>([]);
    const [program, setProgram] = useState<anchor.Program<AskNetwork>>();
    const svgRef = useRef();

    useEffect(() => {
        console.log("Updating provider, then program...");

        const RPC_URL = "https://api.devnet.solana.com";
        const connection = new Connection(RPC_URL, "processed");
        const provider = new anchor.AnchorProvider(
            connection,
            new anchor.Wallet(anchor.web3.Keypair.generate()), // This is a dummy wallet, used for read-only operations.
            anchor.AnchorProvider.defaultOptions()
        );

        try {
            // Create a new Program instance with the IDL, program ID, and provider
            const program = new anchor.Program(
                idl as anchor.Idl,
                new PublicKey('8WfQ3nACPcoBKxFnN4ekiHp8bRTd35R4L8Pu3Ak15is3'),
                provider
            );
            setProgram(program as any);
        } catch (error) {
            console.error("Error updating program:", error);
        }
    }, []);

    useEffect(() => {
        // Fetch organizations here
    }, [program]);

    useEffect(() => {
        if (organizations.length > 0) {
            drawChart(organizations);
        }
    }, [organizations]);

    const drawChart = (orgs: Organization[]) => {
        const svg = d3.select(svgRef.current);
        const width = +svg.attr("width");
        const height = +svg.attr("height");
        const margin = { top: 20, right: 20, bottom: 20, left: 20 };
        const innerWidth = width - margin.left - margin.right;
        const innerHeight = height - margin.top - margin.bottom;

        const chart = svg.append("g")
            .attr("transform", `translate(${margin.left},${margin.top})`);

        const xScale = d3.scaleLinear()
            .domain([0, orgs.length])
            .range([0, innerWidth]);

        chart.selectAll(".organization")
            .data(orgs)
            .enter()
            .append("rect")
            .attr("class", "organization")
            .attr("x", (d, i) => xScale(i))
            .attr("y", innerHeight / 2)
            .attr("width", 100) // Set your width here
            .attr("height", 60) // Set your height here
            .attr("rx", 15) // Rounded corners
            .style("fill", "skyblue")
            .attr("transform", (d, i) => `translate(${(innerWidth - orgs.length * 110) / 2},0)`);

        chart.selectAll(".org-text")
            .data(orgs)
            .enter()
            .append("text")
            .attr("class", "org-text")
            .attr("x", (d, i) => xScale(i) + 50) // Adjust text position
            .attr("y", innerHeight / 2 + 30) // Adjust text position
            .attr("text-anchor", "middle")
            .text(d => d.alias ?? "")
            .style("fill", "black")
            .attr("font-size", "14px")
            .attr("transform", (d, i) => `translate(${(innerWidth - orgs.length * 110) / 2},0)`);
    };

    return (
        <Box>
            <AppBar />
            <Container maxW="container.xl">
                <Heading as="h1" size="xl" textAlign="center" my="40px">
                    Organizations on Ask Network
                </Heading>
                <svg ref={svgRef as any} width="800" height="600" style={{ border: "1px solid black" }}></svg>
            </Container>
        </Box>
    );
}

export default Page;
