using System;
using GraphStructure;
using GraphStructure.Edges;
using GraphStructure.Nodes;
using Xunit;

namespace Test
{
    public class GraphEdgesTest
    {

        [Fact]
        public void ConstructorTest()
        {
            var node = new Node<object>();
            var arc = new Arc<object>(node, new Node<object>());
            var edge = new Edge<object>(new Node<object>(), node);
            var graph = new Graph<object>(arc, edge);

            Assert.Equal(2, graph.Edges.Count);
            Assert.Equal(3, graph.Nodes.Count);
        }

        [Fact]
        public void AdditionTest()
        {
            var node0 = new Node<object>();
            var node1 = new Node<object>();
            var node2 = new Node<object>();
            var arc = new Arc<object>(node0, node1);
            var edge = new Edge<object>(node1, node2);
            var graph = new Graph<object>(arc, edge);

            Assert.Equal(2, graph.Edges.Count);
            graph.Add(new Arc<object>(node1, node0));
            Assert.Equal(3, graph.Edges.Count);
            Assert.ThrowsAsync<ArgumentException>(async () => await graph.AddAsync(new Edge<object>(node0, node1)));
            Assert.Throws<ArgumentException>(() => graph.Add(new Edge<object>(node1, node0)));
        }

    }
}