using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace GraphStructure.Common
{
    internal static class CollectionExtensions
    {

        internal static bool ContainsWithLock<T>(this ICollection<T> collection, T item, AsyncReaderWriterLock rwLock)
        {
            using (rwLock.ReaderLock())
            {
                return collection.Contains(item);
            }
        }

        internal static void ThrowIfContainsWithLock<T>(this ICollection<T> collection, T item, AsyncReaderWriterLock rwLock)
        {
            if (ContainsWithLock(collection, item, rwLock))
            {
                throw new ArgumentException();
            }
        }

        internal static async Task<bool> ContainsWithLockAsync<T>(this ICollection<T> collection, T item, AsyncReaderWriterLock rwLock)
        {
            using (await rwLock.ReaderLockAsync())
            {
                return collection.Contains(item);
            }
        }

        internal static async Task ThrowIfContainsWithLockAsync<T>(this ICollection<T> collection, T item, AsyncReaderWriterLock rwLock)
        {
            if (await ContainsWithLockAsync(collection, item, rwLock))
            {
                throw new ArgumentException();
            }
        }

        internal static void AddWithLock<T>(this ICollection<T> collection, T item, AsyncReaderWriterLock rwLock)
        {
            using (rwLock.WriterLock())
            {
                collection.Add(item);
            }
        }

        internal static async Task AddWithLockAsync<T>(this ICollection<T> collection, T item, AsyncReaderWriterLock rwLock)
        {
            using (await rwLock.WriterLockAsync())
            {
                collection.Add(item);
            }
        }

        internal static void RemoveWithLock<T>(this ICollection<T> collection, T item, AsyncReaderWriterLock rwLock)
        {
            using (rwLock.WriterLock())
            {
                collection.Remove(item);
            }
        }

        internal static async Task RemoveWithLockAsync<T>(this ICollection<T> collection, T item, AsyncReaderWriterLock rwLock)
        {
            using (await rwLock.WriterLockAsync())
            {
                collection.Remove(item);
            }
        }

    }
}