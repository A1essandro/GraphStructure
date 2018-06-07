using System;
using System.Collections.Generic;
using System.Linq;
using GraphStructure.Structure.Nodes;
using Nito.AsyncEx;

namespace GraphStructure.Paths
{
    public class Matrix<T, TData>
    {

        private readonly IList<Node<T>> _indexToNodeMap = new List<Node<T>>();
        private readonly AsyncReaderWriterLock _rwMapLock = new AsyncReaderWriterLock();
        private readonly AsyncReaderWriterLock _rwDataLock = new AsyncReaderWriterLock();
        private readonly TData[,] _rawArray;

        public Matrix(int size)
        {
            _rawArray = new TData[size, size];
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

        public TData this[Node<T> a, Node<T> b]
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