using GraphStructure;
using GraphStructure.Builder;
using GraphStructure.Paths;
using Moq;
using Xunit;

namespace Test
{


    public class PathFinderTest
    {

        [Fact]
        public void DefaultPropertiesTest()
        {
            var graph = new Graph();
            var firstVertex = new Vertex("Start (A)");
            var mergeVertex = new Vertex("D");
            var finishVertex = new Vertex("Finish (E)");

            var firstVertexContext = graph.Builder.AddVertex(firstVertex);
            firstVertexContext.ConnectWith(new Vertex("B")).ConnectWith(mergeVertex);
            var mergeVertexContext = firstVertexContext.ConnectWith(new Vertex("C")).ConnectWith(mergeVertex);
            mergeVertexContext.ConnectWith(finishVertex);

            var pathFinder = new Pathfinder(graph);
            var paths = pathFinder.GetAllBetween(firstVertex, finishVertex);
        }

    }

    public class GraphTest
    {

        [Fact]
        public void DefaultPropertiesTest()
        {
            var graph = new Graph();

            Assert.NotNull(graph.Edges);
            Assert.NotNull(graph.Vertices);
            Assert.NotNull(graph.Builder);
            Assert.Equal(0, graph.Order);
            Assert.Equal(0, graph.Size);
        }

        [Fact]
        public void GraphStructureTest()
        {
            var graph = new Graph();

            var v1 = new Mock<IVertex>();
            var v2 = new Mock<IVertex>();
            var v3 = new Mock<IVertex>();
            var e1 = new Mock<IEdge>();
            var e2 = new Mock<IEdge>();
            v1.SetupGet(x => x.Edges).Returns(new[] { e1.Object });
            v2.SetupGet(x => x.Edges).Returns(new[] { e2.Object, e2.Object });
            v3.SetupGet(x => x.Edges).Returns(new[] { e2.Object });

            graph.vertices.Add(v1.Object);
            graph.vertices.Add(v2.Object);
            graph.vertices.Add(v3.Object);

            Assert.Equal(2, graph.Size);
            Assert.Equal(2, graph.Edges.Count);
            Assert.Equal(3, graph.Order);
            Assert.Equal(3, graph.Vertices.Count);
        }

    }
}