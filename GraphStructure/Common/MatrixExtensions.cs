using System;

namespace GraphStructure.Common
{
    internal static class MatrixExtensions
    {

        public static int[,] Power(this int[,] matrix, uint power)
        {
            var len = matrix.GetLength(0);
            var result = (int[,])matrix.Clone();

            for (var step = 0; step < power; step++)
            {
                for (var i = 0; i < len; i++)
                {
                    for (var k = 0; k < len; k++)
                    {
                        for (var j = 0; j < len; j++)
                        {
                            result[i, k] += result[j, k] * matrix[i, j];
                        }
                    }
                }
            }

            return result;
        }

        public static int[,] Or(this int[,] matrix1, int[,] matrix2)
        {
            var len = matrix1.GetLength(0);
            var result = new int[len, len];

            for (var x = 0; x < len; x++)
            {
                for (var y = 0; y < len; y++)
                {
                    result[x, y] = Convert.ToInt32(matrix1[x, y] > 0 || matrix2[x, y] > 0);
                }
            }

            return result;
        }

    }
}