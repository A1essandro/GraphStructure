using System.Collections.Generic;

namespace GraphStructure.Paths
{
    public interface IMatrix<T> : IEnumerable<KeyValuePair<(IVertex, IVertex), T>>
    {

        T this[(IVertex, IVertex) vector] { get; set; }

        T this[IVertex a, IVertex b] { get; set; }

        int Size { get; }

        ICollection<IVertex> Vertices { get; }

        IDictionary<IVertex, T> GetRow(IVertex index);

    }

}