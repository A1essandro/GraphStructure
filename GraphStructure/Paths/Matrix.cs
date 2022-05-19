using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GraphStructure.Paths
{

    public class Matrix<T> : IMatrix<T>
    {

        private readonly ConcurrentDictionary<IVertex, ConcurrentDictionary<IVertex, T>> _hidenStructure = new ConcurrentDictionary<IVertex, ConcurrentDictionary<IVertex, T>>();

        public int Size => _hidenStructure.Count;

        public ICollection<IVertex> Vertices { get; }

        public T this[IVertex a, IVertex b]
        {
            get
            {
                return _hidenStructure[a][b];
            }
            set
            {
                _hidenStructure[a][b] = value;
            }
        }

        public T this[(IVertex, IVertex) vector]
        {
            get
            {
                return _hidenStructure[vector.Item1][vector.Item2];
            }
            set
            {
                _hidenStructure[vector.Item1][vector.Item2] = value;
            }
        }

        public Matrix(params IVertex[] vertices) : this(vertices as IEnumerable<IVertex>)
        {

        }

        public Matrix(IEnumerable<IVertex> vertices)
        {
            Vertices = vertices.ToArray();
            var matrix = new ConcurrentDictionary<IVertex, ConcurrentDictionary<IVertex, T>>();
            foreach (var x in vertices)
            {
                matrix[x] = new ConcurrentDictionary<IVertex, T>();
                foreach (var y in vertices)
                {
                    matrix[x][y] = default;
                }
            }

            _hidenStructure = matrix;
        }

        public IEnumerator<KeyValuePair<(IVertex, IVertex), T>> GetEnumerator()
        {
            foreach (var pair in _hidenStructure)
            {
                foreach (var v in pair.Value)
                {
                    yield return new KeyValuePair<(IVertex, IVertex), T>((pair.Key, v.Key), v.Value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IDictionary<IVertex, T> GetRow(IVertex index)
        {
            return this.Where(x => x.Key.Item1.Equals(index)).ToDictionary(x => x.Key.Item2, x => x.Value);
        }

    }

}