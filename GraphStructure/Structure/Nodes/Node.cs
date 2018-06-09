using System.Collections.Generic;
using GraphStructure.Common;
using Nito.AsyncEx;

namespace GraphStructure.Structure.Nodes
{

    public class Node<T>
    {

        public T Data { get; set; }

        /// <summary>
        /// Collection of nodes where you can go directly from the current node
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<Node<T>> SlaveNodes => _slaveNodes.AsReadOnly();

        /// <summary>
        /// Сollection of nodes from which you can go directly to the current node
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<Node<T>> MasterNodes => _masterNodes.AsReadOnly();

        private List<Node<T>> _slaveNodes = new List<Node<T>>();
        private List<Node<T>> _masterNodes = new List<Node<T>>();
        private AsyncReaderWriterLock _rwSlaveLock = new AsyncReaderWriterLock();
        private AsyncReaderWriterLock _rwMasterLock = new AsyncReaderWriterLock();

        public Node(T data)
        {
            Data = data;
        }

        public Node() : this(default(T))
        {
        }

        internal Node<T> AddSlave(Node<T> node)
        {
            _slaveNodes.ThrowIfContainsWithLock(node, _rwSlaveLock);
            _slaveNodes.AddWithLock(node, _rwSlaveLock);

            return this;
        }

        internal Node<T> RemoveSlave(Node<T> node)
        {
            _slaveNodes.RemoveWithLock(node, _rwSlaveLock);

            return this;
        }

        internal Node<T> AddMaster(Node<T> node)
        {
            _masterNodes.ThrowIfContainsWithLock(node, _rwMasterLock);
            _masterNodes.AddWithLock(node, _rwMasterLock);

            return this;
        }

        internal Node<T> RemoveMaster(Node<T> node)
        {
            _masterNodes.RemoveWithLock(node, _rwMasterLock);

            return this;
        }

    }

    public class Node : Node<object>
    {

    }

}