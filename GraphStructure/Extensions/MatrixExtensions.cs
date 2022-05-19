using GraphStructure.Paths;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphStructure.Extensions
{
    public static class MatrixExtensions
    {

        public static Matrix<double> Power(this Matrix<double> matrix, uint power)
        {
            var result = new Matrix<double>(matrix.Vertices);

            for (var step = 1; step < power; step++)
            {
                foreach (var i in matrix.Vertices)
                    foreach (var k in matrix.Vertices)
                        foreach (var j in matrix.Vertices)
                        {
                            result[i, k] += matrix[j, k] * matrix[i, j];
                        }
            }

            return result;
        }

        public static Matrix<int> Power(this Matrix<int> matrix, uint power)
        {
            var result = new Matrix<int>(matrix.Vertices);

            for (var step = 1; step < power; step++)
            {
                foreach (var i in matrix.Vertices)
                    foreach (var k in matrix.Vertices)
                        foreach (var j in matrix.Vertices)
                        {
                            result[i, k] += matrix[j, k] * matrix[i, j];
                        }
            }

            return result;
        }

        public static Matrix<bool> Or<T>(this Matrix<T> matrix1, Matrix<T> matrix2) where T : IComparable<int>
        {
            if (!IsComparableTo(matrix1, matrix2)) throw new ArgumentException();

            var result = new Matrix<bool>(matrix1.Vertices);
            foreach (var item in matrix1)
            {
                var x = item.Key.Item1;
                var y = item.Key.Item2;
                result[x, y] = matrix1[x, y].CompareTo(0) > 0 || matrix2[x, y].CompareTo(0) > 0;
            }

            return result;
        }

        public static bool IsComparableTo<T>(this Matrix<T> matrix1, Matrix<T> matrix2)
        {
            return matrix1.Size == matrix2.Size && !matrix1.Vertices.Except(matrix2.Vertices).Any();
        }

        public static Matrix<T> Copy<T>(this Matrix<T> source)
        {
            var result = new Matrix<T>(source.Vertices);

            foreach (var cell in source)
            {
                result[cell.Key.Item1, cell.Key.Item2] = cell.Value;
            }

            return result;
        }

        public static Matrix<T2> Convert<T1, T2>(this Matrix<T1> source, Func<KeyValuePair<(IVertex, IVertex), T1>, T2> converter)
        {
            var result = new Matrix<T2>(source.Vertices);

            foreach (var cell in source)
            {
                result[cell.Key.Item1, cell.Key.Item2] = converter(cell);
            }

            return result;
        }

        public static Matrix<T2> Convert<T1, T2>(this Matrix<T1> source, Func<T1, T2> converter)
        {
            var result = new Matrix<T2>(source.Vertices);

            foreach (var cell in source)
            {
                result[cell.Key.Item1, cell.Key.Item2] = converter(cell.Value);
            }

            return result;
        }

    }
}