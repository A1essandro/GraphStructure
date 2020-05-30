using GraphStructure.Structure.Edges;
using GraphStructure.Structure.Nodes;
using Xunit;

namespace Test
{
    public class EdgeTest
    {
        [Fact]
        public void ConstructorTest()
        {
            var arc = new Arc(new Node(), new Node());
            var edge = new Edge(new Node(), new Node(), 3);

            Assert.Equal(1, arc.Weight);
            Assert.Equal(3, edge.Weight);
            Assert.True(edge.Weight == edge.Weight);
        }
    }
}