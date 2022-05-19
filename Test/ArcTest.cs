using GraphStructure;
using Moq;
using Xunit;

namespace Test
{

    public class ArcTest
    {

        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        [InlineData(0)]
        public void EqualsTest(double weight)
        {
            var a = new Mock<IVertex>();
            var b = new Mock<IVertex>();
            var edge1 = new Arc(a.Object, b.Object, weight);
            var edge2 = new Arc(a.Object, b.Object, weight);

            Assert.Equal(edge1, edge2);
            Assert.True(edge1 == edge2);
            Assert.False(edge1 != edge2);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(int.MinValue)]
        [InlineData(0)]
        public void NotEqualsSameDirectionTest(double weight)
        {
            var a = new Mock<IVertex>();
            var b = new Mock<IVertex>();
            var edge1 = new Arc(a.Object, b.Object, weight);
            var edge2 = new Arc(a.Object, b.Object, weight + 0.1);

            Assert.NotEqual(edge1, edge2);
            Assert.False(edge1 == edge2);
            Assert.True(edge1 != edge2);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        [InlineData(0)]
        public void NotEqualsOppositeDirectionTest(double weight)
        {
            var a = new Mock<IVertex>();
            var b = new Mock<IVertex>();
            var edge1 = new Arc(a.Object, b.Object, weight);
            var edge2 = new Arc(b.Object, a.Object, weight);

            Assert.NotEqual(edge1, edge2);
            Assert.False(edge1 == edge2);
            Assert.True(edge1 != edge2);
        }
    }
}