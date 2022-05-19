using GraphStructure;
using GraphStructure.Paths;
using Moq;
using System.Linq;
using Xunit;

namespace Test
{
    public class MatrixTest
    {

        [Fact]
        public void IndexerTest()
        {
            var v1 = new Mock<IVertex>().Object;
            var v2 = new Mock<IVertex>().Object;
            var v3 = new Mock<IVertex>().Object;
            var matrix = new Matrix<int>(v1, v2, v3);

            matrix[v1, v1] = -1; matrix[v1, v2] = 2;
            matrix[(v2, v1)] = -3; matrix[(v2, v2)] = 4;

            Assert.Equal(2, matrix.Sum(x => x.Value));
            Assert.Equal(-1, matrix[(v1, v1)]);
            Assert.Equal(2, matrix[(v1, v2)]);
            Assert.Equal(-3, matrix[v2, v1]);
            Assert.Equal(4, matrix[v2, v2]);
        }

    }
}