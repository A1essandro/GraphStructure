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

            Assert.Equal(7, graph.Nodes.Count);
        }
    }
}
