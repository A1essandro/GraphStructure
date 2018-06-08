using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure.Common;
using GraphStructure.Structure.Edges;
using GraphStructure.Structure.Nodes;
using Nito.AsyncEx;

namespace GraphStructure.Structure
{
    public class Graph<T>
    {

        public IReadOnlyCollection<Node<T>> Nodes => _nodes.AsReadOnly();
        public IReadOnlyCollection<IEdge<T>> Edges => _edges.AsReadOnly();
        public int Size => _edges.Count;
        public int Order => _nodes.Count;

        #region Private fields

        private readonly List<Node<T>> _nodes = new List<Node<T>>();
        private readonly List<IEdge<T>> _edges = new List<IEdge<T>>();
        private readonly AsyncReaderWriterLock _rwNodesLock = new AsyncReaderWriterLock();
        private readonly AsyncReaderWriterLock _rwEdgesLock = new AsyncReaderWriterLock();

        #endregion

        #region ctors

        public Graph()
        {

        }

        public Graph(IEnumerable<IEdge<T>> edges)
        {
            var tasks = edges.Select(edge => AddAsync(edge)).ToArray();
            Task.WaitAll(tasks);
        }

        public Graph(params IEdge<T>[] edges)
            : this(edges.AsEnumerable())
        {
        }

        public override int GetHashCode()
        {
            var nodesHash = 23 * _nodes.Sum(x => x.GetHashCode());
            var edgesHash = 23 * _edges.Sum(x => x.GetHashCode());
            return 17 * (nodesHash + edgesHash);
        }

        #endregion

        #region Add Nodes

        /// <summary>
        /// Synchronous addition of the node (synchronous lock of nodes list)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Graph<T> Add(Node<T> node)
        {
            _nodes.ThrowIfContainsWithLock(node, _rwNodesLock);
            _nodes.AddWithLock(node, _rwNodesLock);

            return this;
        }

        /// <summary>
        /// Asynchronous addition of the node (asynchronous lock of nodes list)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task<Graph<T>> AddAsync(Node<T> node)
        {
            await _nodes.ThrowIfContainsWithLockAsync(node, _rwNodesLock);
            await _nodes.AddWithLockAsync(node, _rwNodesLock);

            return this;
        }

        #endregion

        #region Remove Nodes

        /// <summary>
        /// Synchronous remove of node by the index (synchronous lock of nodes list)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Graph<T> RemoveNode(int index)
        {
            _nodes.RemoveWithLock(_nodes[index], _rwNodesLock);

            return this;
        }

        /// <summary>
        /// Asynchronous remove of node by the index (asynchronous lock of nodes list)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task<Graph<T>> RemoveNodeAsync(int index)
        {
            await _nodes.RemoveWithLockAsync(_nodes[index], _rwNodesLock);

            return this;
        }

        /// <summary>
        /// Synchronous remove of node by the referense (synchronous lock of nodes list)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Graph<T> Remove(Node<T> node)
        {
            _nodes.RemoveWithLock(node, _rwNodesLock);

            return this;
        }

        /// <summary>
        /// Asynchronous remove of node by the referense (asynchronous lock of nodes list)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task<Graph<T>> RemoveAsync(Node<T> node)
        {
            await _nodes.RemoveWithLockAsync(node, _rwNodesLock);

            return this;
        }

        #endregion

        #region Add Edges

        /// <summary>
        /// Synchronous addition of the edge (synchronous lock of edges list)
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
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

            edge.Connect();

            return this;
        }

        /// <summary>
        /// Asynchronous addition of the edge (asynchronous lock of edges list)
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
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

            edge.Connect();

            return this;
        }

        #endregion

        #region Remove Edges

        /// <summary>
        /// Remove edge (or arc) by the index (synchronous lock of edges list)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Graph<T> RemoveEdge(int index)
        {
            _edges[index].Disconnect();
            _edges.RemoveWithLock(_edges[index], _rwEdgesLock);

            return this;
        }

        /// <summary>
        /// Asynchronous remove of edge (or arc) by the index (asynchronous lock of edges list)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task<Graph<T>> RemoveEdgeAsync(int index)
        {
            _edges[index].Disconnect();
            await _edges.RemoveWithLockAsync(_edges[index], _rwEdgesLock);

            return this;
        }

        /// <summary>
        /// Remove edge (or arc) by the referense
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public Graph<T> Remove(IEdge<T> edge)
        {
            edge.Disconnect();
            _edges.RemoveWithLock(edge, _rwEdgesLock);

            return this;
        }

        /// <summary>
        /// Asynchronous remove of edge (or arc) by the referense
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public async Task<Graph<T>> RemoveAsync(IEdge<T> edge)
        {
            edge.Disconnect();
            await _edges.RemoveWithLockAsync(edge, _rwEdgesLock);

            return this;
        }

        #endregion

    }
}
