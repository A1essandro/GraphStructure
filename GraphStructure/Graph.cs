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
        public int Size => _edges.Count;
        public int Order => _nodes.Count;

        #region Private fields

        private List<Node<T>> _nodes = new List<Node<T>>();
        private List<IEdge<T>> _edges = new List<IEdge<T>>();
        private AsyncReaderWriterLock _rwNodesLock = new AsyncReaderWriterLock();
        private AsyncReaderWriterLock _rwEdgesLock = new AsyncReaderWriterLock();

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

        public Graph(params IEdge<T>[] edges) : this(edges.AsEnumerable())
        {
        }

        #endregion

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
            _nodes.RemoveWithLock(_nodes[index], _rwNodesLock);

            return this;
        }

        public async Task<Graph<T>> RemoveNodeAsync(int index)
        {
            await _nodes.RemoveWithLockAsync(_nodes[index], _rwNodesLock);

            return this;
        }

        public Graph<T> Remove(Node<T> node)
        {
            _nodes.RemoveWithLock(node, _rwNodesLock);

            return this;
        }

        public async Task<Graph<T>> RemoveAsync(Node<T> node)
        {
            await _nodes.RemoveWithLockAsync(node, _rwNodesLock);

            return this;
        }

        #endregion

        #region Add Edges

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

        public Graph<T> RemoveEdge(int index)
        {
            _edges[index].Disconnect();
            _edges.RemoveWithLock(_edges[index], _rwEdgesLock);

            return this;
        }

        public async Task<Graph<T>> RemoveEdgeAsync(int index)
        {
            _edges[index].Disconnect();
            await _edges.RemoveWithLockAsync(_edges[index], _rwEdgesLock);

            return this;
        }

        public Graph<T> Remove(IEdge<T> edge)
        {
            edge.Disconnect();
            _edges.RemoveWithLock(edge, _rwEdgesLock);

            return this;
        }

        public async Task<Graph<T>> RemoveAsync(IEdge<T> edge)
        {
            edge.Disconnect();
            await _edges.RemoveWithLockAsync(edge, _rwEdgesLock);

            return this;
        }

        #endregion

        public async Task<int[,]> GetAdjacencyMatrix()
        {
            var matrix = new int[Order, Order];

            using (await _rwNodesLock.ReaderLockAsync())
            {
                for (var x = 0; x < Order; x++)
                {
                    for (var y = 0; y < Order; y++)
                    {
                        var hasPath = _nodes[x].SlaveNodes.Contains(_nodes[y]);
                        matrix[x, y] = hasPath ? 1 : 0;
                    }
                }
            }

            return matrix;
        }

        public async Task<int[,]> GetReachibilityMatrix()
        {
            var adjacencyMatrix = await GetAdjacencyMatrix();
            return await Task.Run(() =>
            {
                var len = adjacencyMatrix.GetLength(0);
                var result = (int[,])adjacencyMatrix.Clone();
                var power = (int[,])adjacencyMatrix.Clone();

                for (var pow = 0; pow < len; pow++)
                {
                    result = result.Or(power.Power(pow));
                }

                return result;
            });
        }

    }
}
