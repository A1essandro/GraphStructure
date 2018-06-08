using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure.Common;
using GraphStructure.Structure.Nodes;
using Nito.AsyncEx;

namespace GraphStructure.Paths
{
    public class Matrix<T>
    {

        private readonly IList<Node<T>> _indexToNodeMap = new List<Node<T>>();
        private readonly AsyncReaderWriterLock _rwMapLock = new AsyncReaderWriterLock();
        private readonly AsyncReaderWriterLock _rwDataLock = new AsyncReaderWriterLock();
        private readonly int[,] _rawArray;

        public Matrix(IEnumerable<Node<T>> nodes)
        {
            var size = nodes.Count();
            _rawArray = new int[size, size];
            _indexToNodeMap = nodes.ToList();
        }

        private Matrix(IEnumerable<Node<T>> nodes, int[,] rawArray)
            : this(nodes)
        {
            _rawArray = rawArray;
        }

        public int Size
        {
            get
            {
                using (_rwMapLock.ReaderLock())
                {
                    return _indexToNodeMap.Count();
                }
            }
        }

        public int this[Node<T> a, Node<T> b]
        {
            get
            {
                using (_rwDataLock.ReaderLock())
                {
                    return _rawArray[_indexToNodeMap.IndexOf(a), _indexToNodeMap.IndexOf(b)];
                }
            }
            set
            {
                _checkMap(a, b);
                using (_rwDataLock.WriterLock())
                {
                    _rawArray[_indexToNodeMap.IndexOf(a), _indexToNodeMap.IndexOf(b)] = value;
                }
            }
        }

        public static async Task<Matrix<T>> Power(Matrix<T> matrix, uint power)
        {
            using (await matrix._rwMapLock.ReaderLockAsync())
            using (await matrix._rwDataLock.ReaderLockAsync())
            {
                return new Matrix<T>(matrix._indexToNodeMap, matrix._rawArray.Power(power));
            }
        }

        public static async Task<Matrix<T>> Or(Matrix<T> matrix1, Matrix<T> matrix2)
        {
            using (await matrix1._rwMapLock.ReaderLockAsync())
            using (await matrix1._rwDataLock.ReaderLockAsync())
            using (await matrix2._rwDataLock.ReaderLockAsync())
            {
                return new Matrix<T>(matrix1._indexToNodeMap, matrix1._rawArray.Or(matrix2._rawArray));
            }
        }

        public async Task<IDictionary<Node<T>, int>> GetRow(Node<T> index)
        {
            var len = Size;
            var result = new Dictionary<Node<T>, int>();

            using (await _rwMapLock.ReaderLockAsync())
            {
                foreach (var node in _indexToNodeMap)
                {
                    result[node] = this[index, node];
                }
            }

            return result;
        }

        private void _checkMap(params Node<T>[] nodes)
        {
            using (_rwMapLock.ReaderLock())
            {
                foreach (var node in nodes)
                {
                    if (!_indexToNodeMap.Contains(node))
                    {
                        _indexToNodeMap.Add(node);
                    }
                }
            }
        }

    }
}