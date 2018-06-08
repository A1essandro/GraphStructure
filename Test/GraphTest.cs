using GraphStructure.Structure;
using GraphStructure.Structure.Edges;
using GraphStructure.Structure.Nodes;
using Xunit;

namespace Test
{
    public class GraphTest
    {

        [Fact]
        public void HashCodeTest()
        {
            var graph = new Graph<int>();
            var code0 = graph.GetHashCode();
            var node0 = new Node<int>();
            var node1 = new Node<int>();

            graph.Add(node0);
            graph.Add(node1);
            var code1 = graph.GetHashCode();

            graph.Add(new Edge<int>(node0, node1));
            var code2 = graph.GetHashCode();

            Assert.NotEqual(node0.GetHashCode(), code1.GetHashCode());
            Assert.NotEqual(code0, code1);
            Assert.NotEqual(code1, code2);
        }

    }
}