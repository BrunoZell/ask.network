import React, { useEffect, useRef } from 'react';
import * as d3 from 'd3';

const CompanyDetails = () => {
  // Simulated invoice data
  const invoices = [
    { supplier: 'Google', billTo: 'RABOT CRYPTO GmbH', productDescription: 'Cloud Services', quantity: 3, totalCost: '€1200', linkToPdf: 'https://example.com/invoice1.pdf' },
    { supplier: 'Ionos', billTo: 'RABOT CRYPTO GmbH', productDescription: 'Web Hosting', quantity: 12, totalCost: '€600', linkToPdf: 'https://example.com/invoice2.pdf' },
    { supplier: 'OpenAI', billTo: 'RABOT CRYPTO GmbH', productDescription: 'API Usage', quantity: 1, totalCost: '€300', linkToPdf: 'https://example.com/invoice3.pdf' }
  ];

  return (
    <div>
      <h2>Organization Details</h2>
      <p>Organization ID: 1</p>
      <p>Registration Number: Germany, Hamburg, HRB 17920</p>
      <p>Registered Organization Name: RABOT CRYPTO GmbH ✅</p>
      <p>Registered Organization Address: Reimersbrucke 5, PLZ, Hamburg ✅</p>
      <CompanyStructureVisualization />
      <h2>Uploaded Invoices</h2>
      <table>
        <thead>
          <tr>
            <th>Supplier</th>
            <th>Product Description</th>
            <th>Quantity</th>
            <th>Total Cost</th>
            <th>Link to PDF</th>
          </tr>
        </thead>
        <tbody>
          {invoices.map((invoice, index) => (
            <tr key={index}>
              <td>{invoice.supplier}</td>
              <td>{invoice.productDescription}</td>
              <td>{invoice.quantity}</td>
              <td>{invoice.totalCost}</td>
              <td><a href={invoice.linkToPdf} target="_blank" rel="noopener noreferrer">View Invoice</a></td>
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
