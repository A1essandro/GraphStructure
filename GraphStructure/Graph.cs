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
            using (_rwLock.WriterLock())
            {
                _nodes.Add(node);
            }

            return this;
        }

        public Graph<T> AddNodes(IEnumerable<Node<T>> nodes)
        {
            using (_rwLock.WriterLock())
            {
                _nodes.AddRange(nodes);
            }

            return this;
        }

        public async Task<Graph<T>> AddNodeAsync(Node<T> node)
        {
            using (await _rwLock.WriterLockAsync())
            {
                _nodes.Add(node);
            }

            return this;
        }

        public async Task<Graph<T>> AddNodesAsync(IEnumerable<Node<T>> nodes)
        {
            using (await _rwLock.WriterLockAsync())
            {
                _nodes.AddRange(nodes);
            }

            return this;
        }

        #endregion

    }
}
