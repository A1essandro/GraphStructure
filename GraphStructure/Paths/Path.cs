using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure.Structure.Nodes;
using Nito.AsyncEx;

namespace GraphStructure.Paths
{
    public class Path<T> : IEnumerable<Node<T>>
    {

        private readonly AsyncReaderWriterLock _rwLock = new AsyncReaderWriterLock();
        private readonly IList<Node<T>> _steps = new List<Node<T>>();

        /// <summary>
        /// Gets the number of steps
        /// </summary>
        /// <returns></returns>
        public int Length
        {
            get
            {
                using (_rwLock.ReaderLock())
                {
                    return _steps.Count;
                }
            }
        }

        public Path()
        {

        }

        internal Path(Node<T> firstStep)
        {
            _steps.Add(firstStep);
        }

        internal Path(IEnumerable<Node<T>> steps)
        {
            _steps = steps.ToList();
        }

        public Node<T> this[int index]
        {
            get
            {
                using (_rwLock.ReaderLock())
                {
                    return _steps[index];
                }
            }
            set
            {
                using (_rwLock.WriterLock())
                {
                    _steps.Insert(index, value);
                }
            }
        }

        /// <summary>
        /// Adding the step to the path
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task Add(Node<T> node)
        {
            using (await _rwLock.WriterLockAsync())
            {
                _steps.Add(node);
            }
        }

        /// <summary>
        /// Copy path to new reference
        /// </summary>
        /// <returns></returns>
        public async Task<Path<T>> Clone()
        {
            using (await _rwLock.ReaderLockAsync())
            {
                return new Path<T>(_steps.Select(x => x));
            }
        }

        /// <summary>
        /// Is path contains node
        /// </summary>
        /// <returns></returns>
        public bool Contains(Node<T> node)
        {
            using (_rwLock.ReaderLock())
            {
                return _steps.Contains(node);
            }
        }

        #region IEnumerator

        public IEnumerator<Node<T>> GetEnumerator()
        {
            using (_rwLock.ReaderLock())
            {
                return _steps.ToList().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}