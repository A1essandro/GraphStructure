using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure.Common;
using GraphStructure.Configuration;
using GraphStructure.Structure.Edges;
using GraphStructure.Structure.Nodes;
using Nito.AsyncEx;

namespace GraphStructure.Structure
{
    public class Graph<T>
    {

        public IReadOnlyCollection<Node<T>> Nodes => _nodes.AsReadOnly();
        public IReadOnlyCollection<IEdge<T>> Edges => _edges.AsReadOnly();
        private GraphConfiguration Configuration { get; }
        public int Size => _edges.Count;
        public int Order => _nodes.Count;

        #region Private fields

        private readonly List<Node<T>> _nodes = new List<Node<T>>();
        private readonly List<IEdge<T>> _edges = new List<IEdge<T>>();
        private readonly AsyncReaderWriterLock _rwNodesLock = new AsyncReaderWriterLock();
        private readonly AsyncReaderWriterLock _rwEdgesLock = new AsyncReaderWriterLock();

        #endregion

        #region ctors

        public Graph(GraphConfiguration config = default)
        {
            Configuration = config ?? GraphConfiguration.Default;
        }

        public Graph(IEnumerable<IEdge<T>> edges)
            : this(GraphConfiguration.Default)
        {
            var tasks = edges.Select(edge => AddAsync(edge)).ToArray();
            Task.WaitAll(tasks);
        }

        public Graph(params IEdge<T>[] edges)
            : this(edges.AsEnumerable())
        {
        }

        #endregion

        #region Object override

        public override int GetHashCode()
        {
            unchecked
            {
                var nodesHash = 23 * _nodes.Sum(x => x.GetHashCode());
                var edgesHash = 23 * _edges.Sum(x => x.GetHashCode());
                return 17 * (nodesHash + edgesHash);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return GetHashCode() == obj.GetHashCode();
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
            using (_rwEdgesLock.ReaderLock())
            {
                if (!_isAvailableToAdd(edge))
                    return this;
            }

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

        /// <summary>
        /// Asynchronous addition of the edge (asynchronous lock of edges list)
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public async Task<Graph<T>> AddAsync(IEdge<T> edge)
        {
            using (await _rwEdgesLock.ReaderLockAsync())
            {
                if (!_isAvailableToAdd(edge))
                    return this;
            }

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

        #region Remove Edges

        /// <summary>
        /// Remove edge (or arc) by the index (synchronous lock of edges list)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Graph<T> RemoveEdge(int index)
        {
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
            await _edges.RemoveWithLockAsync(edge, _rwEdgesLock);

            return this;
        }

        #endregion

        public async Task<IReadOnlyCollection<Node<T>>> GetSlaveNodesFor(Node<T> node)
        {
            using (await _rwEdgesLock.ReaderLockAsync())
            {
                var fromFirstNodeOfEdge = _edges.Where(x => x.Nodes.Item1 == node).Select(x => x.Nodes.Item2);
                var fromSecondNodeOfArc = _edges.Where(x => !x.HasDirection && x.Nodes.Item2 == node).Select(x => x.Nodes.Item1);

                return fromFirstNodeOfEdge.Union(fromSecondNodeOfArc).ToArray();
            }
        }

        public async Task<IEdge<T>> GetConnectionBetween(Node<T> from, Node<T> to)
        {
            using (await _rwEdgesLock.ReaderLockAsync())
            {
                var res = _edges.FirstOrDefault(x => x.Nodes.Item1 == from && x.Nodes.Item2 == to);
                if (res != null)
                {
                    return res;
                }

                return _edges.FirstOrDefault(x => x is Edge<T> && x.Nodes.Item2 == from && x.Nodes.Item1 == to);
            }
        }

        private bool _isAvailableToAdd(IEdge<T> edge)
        {
            if (!_edges.Any(x => x.IsHasSameDirectionWith(edge))
                || Configuration.ExistingEdgeAddingPolicy == ExistingEdgeAddingPolicy.Add) return true;

            switch (Configuration.ExistingEdgeAddingPolicy)
            {
                case ExistingEdgeAddingPolicy.Exception:
                    throw new ArgumentException();
                case ExistingEdgeAddingPolicy.Ignore:
                    return false;
            }

            return true;
        }

    }

    public class Graph : Graph<object>
    {

        public Graph(GraphConfiguration config = default)
            : base(config)
        {
        }

        public Graph(IEnumerable<IEdge<object>> edges)
            : base(edges)
        {
        }

        public Graph(params IEdge<object>[] edges)
            : base(edges)
        {
        }

    }

}
