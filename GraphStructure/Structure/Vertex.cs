using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace GraphStructure
{

    [DebuggerDisplay("Vertex {Name ?? \"NO NAME\"}")]
    public class Vertex : IVertex
    {

        public string Name { get; set; }

        public Vertex() { }

        public Vertex(string name) => Name = name;

        private ConcurrentBag<IEdge> _edges = new ConcurrentBag<IEdge>();
        public IReadOnlyCollection<IEdge> Edges
        {
            get { return _edges.ToArray(); }
            set { _edges = new ConcurrentBag<IEdge>(value); }
        }

        public void AddEdge(IEdge edge)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            if (edge.Vertices.Item1 != this && edge.Vertices.Item2 != this)
                throw new ArgumentException($"One of {nameof(edge.Vertices)} of {nameof(edge)} should be this vertex");

            _edges.Add(edge);
        }

    }

}
