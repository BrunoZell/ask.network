import React, { useEffect, useRef } from 'react';
import * as d3 from 'd3';
// Data Structure as per the new format
const organizationData = {
  Organization: {
    Identity: 1,
    Alias: "RABOT CRYPTO",
  },
  Registration: {
    RegistrationNumber: {
      Germany: {
        court: "Hamburg",
        hrb_registration_number: 17920,
      },
    },
    Address: "Reimersbrucke 5, PLZ, Hamburg",
    Name: "RABOT CRYPTO GmbH"
  },
  Counterparties: ['Google', 'Ionos', 'OpenAI'], // Slugs for companies
  Invoices: [
    {
      supplier: 'Google', // slug from Counterparties
      product: 'Cloud Services',
      quantity: 3.0,
      cost: 1200.0,
      pdfLink: 'https://example.com/invoice1.pdf'
    },
    {
      supplier: 'Ionos', // slug from Counterparties
      product: 'Web Hosting',
      quantity: 12.0,
      cost: 600.0,
      pdfLink: 'https://example.com/invoice2.pdf'
    },
    {
      supplier: 'OpenAI', // slug from Counterparties
      product: 'API Usage',
      quantity: 1.0,
      cost: 300.0,
      pdfLink: 'https://example.com/invoice3.pdf'
    }
  ]
};

// CompanyDetails component
const CompanyDetails = () => {
  const { Germany } = organizationData.Registration.RegistrationNumber;
  return (
    <div>
      <div>
        <h3>Organization</h3>
        <p><span className="variable-name">ID:</span> <span className="variable-value">{organizationData.Organization.Identity}</span></p>
        <p><span className="variable-name">Alias:</span> <span className="variable-value">{organizationData.Organization.Alias}</span></p>
      </div>
      <div>
        <h3>Registration</h3>
        <p><span className="variable-name">Number:</span> <span className="variable-value">{`Germany, ${Germany.court}, HRB ${Germany.hrb_registration_number}`}</span></p>
        <p><span className="variable-name">Name:</span> <span className="variable-value">{organizationData.Registration.Name} ✅</span></p>
        <p><span className="variable-name">Address:</span> <span className="variable-value">{organizationData.Registration.Address} ✅</span></p>
      </div><div>
        <h3>Counterparties</h3>
        <CompanyStructureVisualization />
      </div>
      <h3>Uploaded Invoices</h3>
      <table>
        <thead>
          <tr>
            <th>Supplier</th>
            <th>Product</th>
            <th>Quantity</th>
            <th>Cost</th>
            <th>PDF Link</th>
          </tr>
        </thead>
        <tbody>
          {organizationData.Invoices.map((invoice, index) => (
            <tr key={index}>
              <td>{invoice.supplier}</td>
              <td>{invoice.product}</td>
              <td>{invoice.quantity}</td>
              <td>€{invoice.cost}</td>
              <td><a href={invoice.pdfLink} target="_blank" rel="noopener noreferrer">View Invoice</a></td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

const CompanyStructureVisualization = () => {
  const d3Container = useRef(null);

  useEffect(() => {
    if (d3Container.current) {
      const svg = d3.select(d3Container.current).append('svg')
        .attr('width', 800)
        .attr('height', 400);

      const nodes = [
        { id: 'RABOT CRYPTO GmbH', group: 1 },
        { id: 'Google', group: 2 },
        { id: 'Ionos', group: 2 },
        { id: 'OpenAI', group: 2 },
      ];

      const simulation = d3.forceSimulation(nodes)
        .force('charge', d3.forceManyBody().strength(5))
        .force('center', d3.forceCenter(800 / 2, 400 / 2))
        .force('collision', d3.forceCollide().radius(() => 50));

      const padding = 10; // Margin around the text

      // Create labels to measure width
      const labels = svg.selectAll(null)
        .data(nodes)
        .enter()
        .append('text')
        .attr('text-anchor', 'middle')
        .attr('fill', 'white')
        .attr('alignment-baseline', 'central')
        .text(d => d.id);

      // Get the width of each label and create rectangles based on that width
      labels.each(function (d, i) {
        const bbox = this.getBBox();
        const rectWidth = bbox.width + 2 * padding;
        const rectHeight = bbox.height + 2 * padding;

        svg.append('rect')
          .attr('x', -rectWidth / 2)
          .attr('y', -rectHeight / 2)
          .attr('width', rectWidth)
          .attr('height', rectHeight)
          .attr('rx', 20) // Rounded corners
          .attr('ry', 20)
          .attr('fill', d.group === 1 ? 'blue' : 'green')
          .attr('class', `rect-${i}`);
      });

      // Ensure the labels are on top of the rectangles
      labels.raise();

      // Update positions each tick
      simulation.on('tick', () => {
        labels.attr('x', d => d.x)
          .attr('y', d => d.y);

        labels.each(function (d, i) {
          const bbox = this.getBBox();
          d3.select(`.rect-${i}`)
            .attr('x', d.x - bbox.width / 2 - padding)
            .attr('y', d.y - bbox.height / 2 - padding);
        });
      });
    }
  }, []);

  return <div ref={d3Container} />;
};

export default CompanyDetails;
