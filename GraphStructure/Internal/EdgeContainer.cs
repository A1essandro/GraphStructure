using System;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure.Configuration;
using GraphStructure.Structure.Edges;

namespace GraphStructure.Internal
{
    internal sealed class EdgeContainer<T> : ItemContainer<IEdge<T>>
    {

        private readonly GraphConfiguration _graphConfiguration;

        public EdgeContainer(GraphConfiguration graphConfiguration)
        {
            _graphConfiguration = graphConfiguration;
        }

        public override void Add(IEdge<T> item)
        {
            if (_isAvailableToAdd(item))
            {
                base.Add(item);
            }
        }

        public override Task AddAsync(IEdge<T> item)
        {
            if (_isAvailableToAdd(item))
            {
                return base.AddAsync(item);
            }

            return Task.CompletedTask;
        }

        private bool _isAvailableToAdd(IEdge<T> edge)
        {
            using (RwLock.WriterLock())
            {
                if (!Items.Any(x => x.IsHasSameDirectionWith(edge))
                    || _graphConfiguration.ExistingEdgeAddingPolicy == ExistingEdgeAddingPolicy.Add) return true;

                switch (_graphConfiguration.ExistingEdgeAddingPolicy)
                {
                    case ExistingEdgeAddingPolicy.Exception:
                        throw new ArgumentException();
                    case ExistingEdgeAddingPolicy.Ignore:
                        return false;
                }
            }

            return true;
        }

    }
}