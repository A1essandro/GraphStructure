using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure;
using GraphStructure.Nodes;
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
                graph.AddAsync(new Node<int>()),
                graph.AddAsync(new Node<int>()),
                graph.AddAsync(new Node<int>()),
                graph.AddAsync(new Node<int>()),
                graph.AddAsync(new Node<int>())
            );

            await graph.AddAsync(new Node<int>());
            graph.Add(new Node<int>());

            Assert.Equal(7, graph.Nodes.Count);

            var node = new Node<int>();
            await graph.AddAsync(node);
            Assert.Equal(7, graph.Nodes.Select(x => x).ToList().IndexOf(node));
            Assert.Throws<ArgumentException>(() => graph.Add(node));
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
                graph.AddAsync(node0),
                graph.AddAsync(node1),
                graph.AddAsync(node2),
                graph.AddAsync(node3)
            );

            graph.Remove(new Node<int>()); //no action
            Assert.Equal(4, graph.Nodes.Count);

            await graph.RemoveAsync(node0);
            Assert.Throws<ArgumentOutOfRangeException>(() => graph.RemoveNode(3));
            Assert.Equal(3, graph.Nodes.Count);

            await graph.RemoveNodeAsync(2);
            Assert.Equal(2, graph.Nodes.Count);

            await graph.RemoveAsync(node3); //no action
            graph.Remove(node1);
            Assert.Equal(1, graph.Nodes.Count);

            graph.RemoveNode(0);
            Assert.Equal(0, graph.Nodes.Count);
        }

    }
}
