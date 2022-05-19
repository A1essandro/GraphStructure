using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GraphStructure.Paths
{
    public class Path : IGraph, IEnumerable<IDirection>
    {

        private readonly List<IDirection> _steps = new List<IDirection>();

        /// <summary>
        /// Gets the number of steps
        /// </summary>
        /// <returns></returns>
        public int Length => _steps.Count;

        public IReadOnlyCollection<IVertex> Vertices => throw new System.NotImplementedException();

        public IReadOnlyCollection<IEdge> Edges => throw new System.NotImplementedException();

        public int Size => throw new System.NotImplementedException();

        public int Order => throw new System.NotImplementedException();

        #region Ctors

        public Path()
        {

        }

        internal Path(IDirection firstStep)
        {
            _steps.Add(firstStep);
        }

        internal Path(IEnumerable<IDirection> steps)
        {
            _steps = steps.ToList();
        }

        #endregion

        public IDirection this[int index]
        {
            get
            {
                return _steps[index];
            }
            internal set
            {
                _steps.Insert(index, value);
            }
        }

        /// <summary>
        /// Adding the step to the path
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public void Add(IDirection vertex)
        {
            _steps.Add(vertex);
        }

        /// <summary>
        /// Copy path to new reference
        /// </summary>
        /// <returns></returns>
        public Path Clone() => new Path(_steps.Select(x => x));

        /// <summary>
        /// Is path contains vertex
        /// </summary>
        /// <returns></returns>
        public bool Contains(IVertex vertex) => _steps.Any(x => ReferenceEquals(x.Start, vertex) || ReferenceEquals(x.End, vertex));

        /// <summary>
        /// Is path contains edge
        /// </summary>
        /// <returns></returns>
        public bool Contains(IEdge edge) => _steps.Select(x => x.Edge).Contains(edge);

        #region IEnumerator

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IDirection> GetEnumerator()
        {
            return _steps.ToList().GetEnumerator();
        }

        #endregion
    }
}