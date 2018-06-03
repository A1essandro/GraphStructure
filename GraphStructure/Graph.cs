using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure.Common;
using GraphStructure.Edges;
using GraphStructure.Nodes;
using Nito.AsyncEx;

namespace GraphStructure
{
    public class Graph<T>
    {

        public IReadOnlyCollection<Node<T>> Nodes => _nodes.AsReadOnly();
        public IReadOnlyCollection<IEdge<T>> Edges => _edges.AsReadOnly();

        private List<Node<T>> _nodes = new List<Node<T>>();
        private List<IEdge<T>> _edges = new List<IEdge<T>>();
        private AsyncReaderWriterLock _rwNodesLock = new AsyncReaderWriterLock();
        private AsyncReaderWriterLock _rwEdgesLock = new AsyncReaderWriterLock();

        public Graph()
        {

        }

        public Graph(IEnumerable<IEdge<T>> edges)
        {
            var tasks = edges.Select(edge => AddAsync(edge)).ToArray();
            Task.WaitAll(tasks);
        }

        public Graph(params IEdge<T>[] edges) : this(edges.AsEnumerable())
        {
        }

        #region Add Nodes

        public Graph<T> Add(Node<T> node)
        {
            _nodes.ThrowIfContainsWithLock(node, _rwNodesLock);
            _nodes.AddWithLock(node, _rwNodesLock);

            return this;
        }

        public async Task<Graph<T>> AddAsync(Node<T> node)
        {
            await _nodes.ThrowIfContainsWithLockAsync(node, _rwNodesLock);
            await _nodes.AddWithLockAsync(node, _rwNodesLock);

            return this;
        }

        #endregion

        #region Remove Nodes

        public Graph<T> RemoveNode(int index)
        {
            using (_rwNodesLock.WriterLock())
            {
                _nodes.Remove(_nodes[index]);
            }

            return this;
        }

        public async Task<Graph<T>> RemoveNodeAsync(int index)
        {
            using (await _rwNodesLock.WriterLockAsync())
            {
                _nodes.Remove(_nodes[index]);
            }

            return this;
        }

        public Graph<T> RemoveNode(Node<T> node)
        {
            using (_rwNodesLock.WriterLock())
            {
                _nodes.Remove(node);
            }

            return this;
        }

        public async Task<Graph<T>> RemoveNodeAsync(Node<T> node)
        {
            using (await _rwNodesLock.WriterLockAsync())
            {
                _nodes.Remove(node);
            }

            return this;
        }

        #endregion

        #region Add edges

        public Graph<T> Add(IEdge<T> edge)
        {
            _edges.ThrowIfContainsWithLock(edge, _rwEdgesLock);
            _edges.AddWithLock(edge, _rwEdgesLock);

            if (!_nodes.ContainsWithLock(edge.Nodes.Item1, _rwNodesLock))
            {
                _nodes.AddWithLock(edge.Nodes.Item1, _rwNodesLock);
            }
            if (!_nodes.ContainsWithLock(edge.Nodes.Item2, _rwNodesLock))
            {
                _nodes.AddWithLock(edge.Nodes.Item2, _rwNodesLock);
            }

            return this;
        }

        public async Task<Graph<T>> AddAsync(IEdge<T> edge)
        {
            _edges.ThrowIfContainsWithLock(edge, _rwEdgesLock);
            _edges.AddWithLock(edge, _rwEdgesLock);

            if (!await _nodes.ContainsWithLockAsync(edge.Nodes.Item1, _rwNodesLock))
            {
                await _nodes.AddWithLockAsync(edge.Nodes.Item1, _rwNodesLock);
            }
            if (!await _nodes.ContainsWithLockAsync(edge.Nodes.Item2, _rwNodesLock))
            {
                await _nodes.AddWithLockAsync(edge.Nodes.Item2, _rwNodesLock);
            }

            return this;
        }

        #endregion

    }
}
