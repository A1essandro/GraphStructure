using GraphStructure;
using Moq;
using System;
using Xunit;

namespace Test
{
    public class VertexTest
    {

        [Fact]
        public void NotNullEdgesByDefaulyTest()
        {
            var vertex = new Vertex();

            Assert.NotNull(vertex.Edges);
            Assert.Equal(0, vertex.Edges.Count);
        }

        [Fact]
        public void AddNullEdgeTest()
        {
            var vertex = new Vertex();

            Assert.Throws<ArgumentNullException>(() => vertex.AddEdge(null));
        }

        [Fact]
        public void AddIncorrectEdgeTest()
        {
            var vertex = new Vertex();
            var v1 = new Mock<IVertex>();
            var v2 = new Mock<IVertex>();
            var edge = new Mock<IEdge>();
            edge.SetupGet(x => x.Vertices).Returns((v1.Object, v2.Object));

            Assert.Throws<ArgumentException>(() => vertex.AddEdge(edge.Object));
        }

        [Fact]
        public void AddCorrectEdgeTest()
        {
            var vertex = new Vertex();
            var v1 = new Mock<IVertex>();
            var edge = new Mock<IEdge>();
            edge.SetupGet(x => x.Vertices).Returns((v1.Object, vertex));

            vertex.AddEdge(edge.Object);

            Assert.Equal(1, vertex.Edges.Count);
        }

        [Fact]
        public void AddEdgeToSelfTest()
        {
            var vertex = new Vertex();
            var edge = new Mock<IEdge>();
            edge.SetupGet(x => x.Vertices).Returns((vertex, vertex));

            vertex.AddEdge(edge.Object);

            Assert.Equal(1, vertex.Edges.Count);
        }

    }
}