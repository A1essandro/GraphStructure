using GraphStructure;
using GraphStructure.Extensions;
using GraphStructure.Paths;
using Moq;
using Xunit;

namespace Test
{

    public class MatrixExtensionsTest
    {

        [Fact]
        public void PowerTest()
        {
            var v1 = new Mock<IVertex>().Object;
            var v2 = new Mock<IVertex>().Object;
            var v3 = new Mock<IVertex>().Object;
            var matrix = new Matrix<int>(v1, v2, v3);

            matrix[v1, v1] = -1; matrix[v1, v2] = 2; matrix[v1, v3] = -5;
            matrix[v2, v1] = 3; matrix[v2, v2] = 4; matrix[v2, v3] = 1;
            matrix[v3, v1] = 0; matrix[v3, v2] = 1; matrix[v3, v3] = 2;

            var result = matrix.Power(2);

            Assert.Equal(7, result[v1, v1]); Assert.Equal(1, result[v1, v2]); Assert.Equal(-3, result[v1, v3]);
            Assert.Equal(9, result[v2, v1]); Assert.Equal(23, result[v2, v2]); Assert.Equal(-9, result[v2, v3]);
            Assert.Equal(3, result[v3, v1]); Assert.Equal(6, result[v3, v2]); Assert.Equal(5, result[v3, v3]);
        }

        [Fact]
        public void OrTest()
        {
            var v1 = new Mock<IVertex>().Object;
            var v2 = new Mock<IVertex>().Object;
            var matrix1 = new Matrix<int>(v1, v2);
            var matrix2 = new Matrix<int>(v1, v2);

            matrix1[v1, v1] = 0; matrix1[v1, v2] = 1;
            matrix1[v2, v1] = 1; matrix1[v2, v2] = 0;

            matrix2[v1, v1] = 0; matrix2[v1, v2] = 1;
            matrix2[v2, v1] = 0; matrix2[v2, v2] = 1;

            var result = matrix1.Or(matrix2);

            Assert.False(result[v1, v1]); Assert.True(result[v1, v2]);
            Assert.True(result[v2, v1]); Assert.True(result[v2, v2]);
        }

    }
}