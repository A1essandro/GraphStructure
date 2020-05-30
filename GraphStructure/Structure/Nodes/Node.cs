using System.Diagnostics;

namespace GraphStructure.Structure.Nodes
{

    [DebuggerDisplay("Data: {Data}; Hashcode: {HashCode}")]
    public class Node<T>
    {

        public T Data { get; set; }

        private int HashCode => GetHashCode();

        public Node(T data)
        {
            Data = data;
        }

        public Node() : this(default(T))
        {
        }

    }

    public class Node : Node<object>
    {

    }

}