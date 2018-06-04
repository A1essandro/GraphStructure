using System;
using System.Collections.Generic;

namespace GraphStructure.Common
{
    internal static class MatrixExtensions
    {

        public static int[,] Power(this int[,] matrix, int power)
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
            for (var x = 0; x < len; x++)
            {
                for (var y = 0; y < len; y++)
                {
                    matrix1[x, y] = Convert.ToInt32(matrix1[x, y] > 0 || matrix2[x, y] > 0);
                }
            }

            return matrix1;
        }

        public static int[] GetRow(this int[,] matrix, int index)
        {
            var len = matrix.GetLength(1);
            var result = new int[len];

            for (var i = 0; i < len; i++)
            {
                result[i] = matrix[index, i];
            }

            return result;
        }

        public static IEnumerable<int> GetPositiveIndexes(this int[] arr)
        {
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] > 0) yield return i;
            }
        }

    }
}