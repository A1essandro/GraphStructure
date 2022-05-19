using GraphStructure;
using Moq;
using Xunit;

namespace Test
{
    public class GraphBuilderTest
    {

        [Fact]
        public void AddingVertexTest()
        {
            var graph = new Graph();
            var vertex = new Mock<IVertex>();
            vertex.SetupGet(x => x.Edges).Returns(new IEdge[] { });

            graph.Builder.AddVertex(vertex.Object);

            Assert.Equal(1, graph.Order);
        }

        [Fact]
        public void AddingConnectionTest()
        {
            var graph = new Graph();
            var v1 = new Vertex();
            var v2 = new Vertex();

            graph.Builder.AddVertex(v1).ConnectWith(v2);

            Assert.Equal(2, graph.Order);
            Assert.Equal(1, graph.Size);
        }

    }
}