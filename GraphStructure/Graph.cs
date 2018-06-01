using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace GraphStructure
{
    public class Graph<T>
    {

        public IReadOnlyCollection<Node<T>> Nodes => _nodes.AsReadOnly();

        private AsyncReaderWriterLock _rwLock = new AsyncReaderWriterLock();
        private List<Node<T>> _nodes = new List<Node<T>>();

        public Graph()
        {

        }

        #region Add Nodes

        public Graph<T> AddNode(Node<T> node)
        {
            _checkSameReference(node);

            using (_rwLock.WriterLock())
            {
                _nodes.Add(node);
            }

            return this;
        }

        public Graph<T> AddNodes(IEnumerable<Node<T>> nodes)
        {
            _checkDuplicates(nodes);
            nodes.AsParallel().ForAll(async (node) => await _checkSameReferenceAsync(node));

            using (_rwLock.WriterLock())
            {
                _nodes.AddRange(nodes);
            }

            return this;
        }

        public async Task<Graph<T>> AddNodeAsync(Node<T> node)
        {
            await _checkSameReferenceAsync(node);

            using (await _rwLock.WriterLockAsync())
            {
                _nodes.Add(node);
            }

            return this;
        }

        public async Task<Graph<T>> AddNodesAsync(IEnumerable<Node<T>> nodes)
        {
            _checkDuplicates(nodes);
            nodes.AsParallel().ForAll(async (node) => await _checkSameReferenceAsync(node));

            using (await _rwLock.WriterLockAsync())
            {
                _nodes.AddRange(nodes);
            }

            return this;
        }

        private void _checkSameReference(Node<T> node)
        {
            using (_rwLock.ReaderLock())
            {
                if (_nodes.Contains(node))
                {
                    throw new ArgumentException();
                }
            }
        }

        private async Task _checkSameReferenceAsync(Node<T> node)
        {
            using (await _rwLock.ReaderLockAsync())
            {
                if (_nodes.Contains(node))
                {
                    throw new ArgumentException();
                }
            }
        }

        private void _checkDuplicates(IEnumerable<Node<T>> nodes)
        {
            if (nodes.Count() != nodes.Distinct().Count())
            {
                throw new ArgumentException();
            }
        }

        #endregion

        #region Remove Nodes

        public Graph<T> RemoveNode(int index)
        {
            using (_rwLock.WriterLock())
            {
                _nodes.Remove(_nodes[index]);
            }

            return this;
        }

        public async Task<Graph<T>> RemoveNodeAsync(int index)
        {
            using (await _rwLock.WriterLockAsync())
            {
                _nodes.Remove(_nodes[index]);
            }

            return this;
        }

        public Graph<T> RemoveNode(Node<T> node)
        {
            using (_rwLock.WriterLock())
            {
                _nodes.Remove(node);
            }

            return this;
        }

        public async Task<Graph<T>> RemoveNodeAsync(Node<T> node)
        {
            using (await _rwLock.WriterLockAsync())
            {
                _nodes.Remove(node);
            }

            return this;
        }

        #endregion

    }
}
