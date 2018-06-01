using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphStructure;
using Xunit;

namespace Test
{
    public class GraphNodesTest
    {

        [Fact]
        public void AccessTest()
        {
            var graph = new Graph<object>();
            Assert.Throws<InvalidCastException>(() => (List<Node<object>>)(graph.Nodes));
        }

        [Fact]
        public async Task ParallelAdditionTest()
        {
            var graph = new Graph<int>();

            await Task.WhenAll(
                graph.AddNodeAsync(new Node<int>()),
                graph.AddNodeAsync(new Node<int>()),
                graph.AddNodeAsync(new Node<int>()),
                graph.AddNodeAsync(new Node<int>()),
                graph.AddNodeAsync(new Node<int>()),
                graph.AddNodesAsync(new Node<int>[] { new Node<int>(), new Node<int>() })
            );

            graph.AddNodes(new Node<int>[] { new Node<int>(), new Node<int>() });
            graph.AddNode(new Node<int>());

            Assert.Equal(10, graph.Nodes.Count);

            var node = new Node<int>();
            Assert.Throws<ArgumentException>(() => graph.AddNodes(new Node<int>[] { node, node }));
            await graph.AddNodeAsync(node);
            Assert.Throws<ArgumentException>(() => graph.AddNode(node));
        }

        [Fact]
        public async Task RemoveTest()
        {
            var graph = new Graph<int>();
            var node0 = new Node<int>();
            var node1 = new Node<int>();
            var node2 = new Node<int>();
            var node3 = new Node<int>();

            await Task.WhenAll(
                graph.AddNodeAsync(node0),
                graph.AddNodeAsync(node1),
                graph.AddNodeAsync(node2),
                graph.AddNodeAsync(node3)
            );

            graph.RemoveNode(new Node<int>()); //no action
            Assert.Equal(4, graph.Nodes.Count);

            await graph.RemoveNodeAsync(node0);
            Assert.Throws<ArgumentOutOfRangeException>(() => graph.RemoveNode(3));
            Assert.Equal(3, graph.Nodes.Count);

            await graph.RemoveNodeAsync(2);
            Assert.Equal(2, graph.Nodes.Count);

            await graph.RemoveNodeAsync(node3); //no action
            graph.RemoveNode(node1);
            Assert.Equal(1, graph.Nodes.Count);

            graph.RemoveNode(0);
            Assert.Equal(0, graph.Nodes.Count);
        }

    }
}
