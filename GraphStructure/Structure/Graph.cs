using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure.Common;
using GraphStructure.Configuration;
using GraphStructure.Internal;
using GraphStructure.Structure.Edges;
using GraphStructure.Structure.Nodes;
using Nito.AsyncEx;

namespace GraphStructure.Structure
{
    public class Graph<T>
    {

        public IReadOnlyCollection<Node<T>> Nodes => _nodeContainer.Collection;
        public IReadOnlyCollection<IEdge<T>> Edges => _edgeContainer.Collection;
        private GraphConfiguration Configuration { get; }
        public int Size => _edgeContainer.Count;
        public int Order => _nodeContainer.Count;

        #region Private fields

        private readonly NodeContainer<T> _nodeContainer;
        private readonly EdgeContainer<T> _edgeContainer;

        #endregion

        #region ctors

        public Graph(GraphConfiguration config = default)
        {
            Configuration = config ?? GraphConfiguration.Default;
            _nodeContainer = new NodeContainer<T>();
            _edgeContainer = new EdgeContainer<T>(Configuration);
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
                var nodesHash = 23 * _nodeContainer.Collection.Sum(x => x.GetHashCode());
                var edgesHash = 23 * _edgeContainer.Collection.Sum(x => x.GetHashCode());
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
            if (_nodeContainer.Contains(node)) throw new ArgumentException();
            _nodeContainer.Add(node);

            return this;
        }

        /// <summary>
        /// Asynchronous addition of the node (asynchronous lock of nodes list)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task<Graph<T>> AddAsync(Node<T> node)
        {
            if (await _nodeContainer.ContainsAsync(node)) throw new ArgumentException();
            await _nodeContainer.AddAsync(node);

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
            _nodeContainer.RemoveByIndex(index);

            return this;
        }

        /// <summary>
        /// Asynchronous remove of node by the index (asynchronous lock of nodes list)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task<Graph<T>> RemoveNodeAsync(int index)
        {
            await _nodeContainer.RemoveByIndexAsync(index);

            return this;
        }

        /// <summary>
        /// Synchronous remove of node by the referense (synchronous lock of nodes list)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Graph<T> Remove(Node<T> node)
        {
            _nodeContainer.Remove(node);

            return this;
        }

        /// <summary>
        /// Asynchronous remove of node by the referense (asynchronous lock of nodes list)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task<Graph<T>> RemoveAsync(Node<T> node)
        {
            await _nodeContainer.RemoveAsync(node);

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
            _edgeContainer.Add(edge);

            if (!_nodeContainer.Contains(edge.Nodes.Item1))
            {
                _nodeContainer.Add(edge.Nodes.Item1);
            }
            if (!_nodeContainer.Contains(edge.Nodes.Item2))
            {
                _nodeContainer.Add(edge.Nodes.Item2);
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
            await _edgeContainer.AddAsync(edge);

            if (!await _nodeContainer.ContainsAsync(edge.Nodes.Item1))
            {
                await _nodeContainer.AddAsync(edge.Nodes.Item1);
            }
            if (!await _nodeContainer.ContainsAsync(edge.Nodes.Item2))
            {
                await _nodeContainer.AddAsync(edge.Nodes.Item2);
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
            _edgeContainer.RemoveByIndex(index);

            return this;
        }

        /// <summary>
        /// Asynchronous remove of edge (or arc) by the index (asynchronous lock of edges list)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task<Graph<T>> RemoveEdgeAsync(int index)
        {
            await _edgeContainer.RemoveByIndexAsync(index);

            return this;
        }

        /// <summary>
        /// Remove edge (or arc) by the referense
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public Graph<T> Remove(IEdge<T> edge)
        {
            _edgeContainer.Remove(edge);

            return this;
        }

        /// <summary>
        /// Asynchronous remove of edge (or arc) by the referense
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public async Task<Graph<T>> RemoveAsync(IEdge<T> edge)
        {
            await _edgeContainer.RemoveAsync(edge);

            return this;
        }

        #endregion

        public async Task<IReadOnlyCollection<Node<T>>> GetSlaveNodesFor(Node<T> node)
        {
            var edges = await _edgeContainer.GetCollectionAsync();
            var fromFirstNodeOfEdge = edges.Where(x => x.Nodes.Item1 == node).Select(x => x.Nodes.Item2);
            var fromSecondNodeOfArc = edges.Where(x => !x.HasDirection && x.Nodes.Item2 == node).Select(x => x.Nodes.Item1);

            return fromFirstNodeOfEdge.Union(fromSecondNodeOfArc).ToArray();
        }

        public async Task<IEdge<T>> GetConnectionBetween(Node<T> from, Node<T> to)
        {
            var edges = await _edgeContainer.GetCollectionAsync();
            var res = edges
                .OrderBy(x => x.Weight)
                .FirstOrDefault(x => x.Nodes.Item1 == from && x.Nodes.Item2 == to);

            if (res != null)
            {
                return res;
            }

            return edges
                .OrderBy(x => x.Weight)
                .FirstOrDefault(x => !x.HasDirection && x.Nodes.Item2 == from && x.Nodes.Item1 == to);
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
