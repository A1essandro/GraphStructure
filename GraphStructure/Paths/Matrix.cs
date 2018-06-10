using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure.Common;
using GraphStructure.Structure.Nodes;
using Nito.AsyncEx;

namespace GraphStructure.Paths
{

    public class Matrix<T, TCell>
    {
        protected readonly IList<Node<T>> _indexToNodeMap = new List<Node<T>>();
        protected readonly AsyncReaderWriterLock _rwMapLock = new AsyncReaderWriterLock();
        protected readonly AsyncReaderWriterLock _rwDataLock = new AsyncReaderWriterLock();
        protected readonly TCell[,] _rawArray;

        public Matrix(IEnumerable<Node<T>> nodes)
        {
            var size = nodes.Count();
            _rawArray = new TCell[size, size];
            _indexToNodeMap = nodes.ToList();
        }

        protected Matrix(IEnumerable<Node<T>> nodes, TCell[,] rawArray)
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

        public TCell this[Node<T> a, Node<T> b]
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

        public TCell this[int a, int b]
        {
            get
            {
                using (_rwDataLock.ReaderLock())
                {
                    return _rawArray[a, b];
                }
            }
            internal set
            {
                using (_rwDataLock.WriterLock())
                {
                    _rawArray[a, b] = value;
                }
            }
        }

        public async Task<IDictionary<Node<T>, TCell>> GetRow(Node<T> index)
        {
            var len = Size;
            var result = new Dictionary<Node<T>, TCell>();

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

    public class Matrix<T> : Matrix<T, int>
    {

        public Matrix(IEnumerable<Node<T>> nodes)
            : base(nodes)
        {
        }

        private Matrix(IEnumerable<Node<T>> nodes, int[,] rawArray)
            : base(nodes, rawArray)
        {
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

    }
}