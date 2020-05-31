using System.Collections.Generic;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace GraphStructure.Internal
{
    internal abstract class ItemContainer<T>
    {

        public IReadOnlyCollection<T> Collection
        {
            get
            {
                using (RwLock.ReaderLock())
                {
                    return Items.AsReadOnly();
                }
            }
        }

        public int Count => Items.Count;

        protected readonly List<T> Items = new List<T>();


        protected readonly AsyncReaderWriterLock RwLock = new AsyncReaderWriterLock();

        public virtual void Add(T item)
        {
            using (RwLock.WriterLock())
            {
                Items.Add(item);
            }
        }

        public virtual async Task AddAsync(T item)
        {
            using (await RwLock.WriterLockAsync())
            {
                Items.Add(item);
            }
        }

        public virtual void Remove(T item)
        {
            using (RwLock.WriterLock())
            {
                Items.Remove(item);
            }
        }

        public virtual async Task RemoveAsync(T item)
        {
            using (await RwLock.WriterLockAsync())
            {
                Items.Remove(item);
            }
        }

        public virtual bool Contains(T item)
        {
            using (RwLock.ReaderLock())
            {
                return Items.Contains(item);
            }
        }

        public virtual async Task<bool> ContainsAsync(T item)
        {
            using (await RwLock.ReaderLockAsync())
            {
                return Items.Contains(item);
            }
        }

        public async Task<IReadOnlyCollection<T>> GetCollectionAsync()
        {
            using (await RwLock.ReaderLockAsync())
            {
                return Items.AsReadOnly();
            }
        }

        public void RemoveByIndex(int index)
        {
            using (RwLock.WriterLock())
            {
                Items.Remove(Items[index]);
            }
        }

        public async Task RemoveByIndexAsync(int index)
        {
            using (await RwLock.WriterLockAsync())
            {
                Items.Remove(Items[index]);
            }
        }
    }
}