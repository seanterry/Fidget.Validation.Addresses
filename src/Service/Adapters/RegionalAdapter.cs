using Fidget.Validation.Addresses.Service.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fidget.Validation.Addresses.Service.Adapters
{
    /// <summary>
    /// Defines an adapter for working with regional metadata.
    /// </summary>

    abstract class RegionalAdapter
    {
        /// <summary>
        /// Gets the service client that backs the current instance.
        /// </summary>
        
        protected IServiceClient Client { get; }

        /// <summary>
        /// Initializes an instance for working with regional metadata.
        /// </summary>
        /// <param name="client">Service client that will back the current instance.</param>
        
        protected RegionalAdapter( IServiceClient client )
        {
            Client = client ?? throw new ArgumentNullException( nameof(client) );
        }

        /// <summary>
        /// Returns the identifier for the record.
        /// </summary>
        /// <param name="parent">Parent region key.</param>
        /// <param name="key">Key of the region to identify.</param>
        /// <param name="language">Language for the metadata.</param>

        protected string BuildIdentifier( string parent, string key, string language )
        {
            // remove language component of parent if present
            parent = parent.Contains( "--" )
                ? parent.Remove( parent.IndexOf( "--" ) )
                : parent;

            return $"{parent}/{key}{(language != null ? $"--{language}" : string.Empty)}";
        }

        /// <summary>
        /// Returns whether the given region contains the specified child region.
        /// </summary>
        /// <param name="parent">Region whose children to search.</param>
        /// <param name="child">Key or name of the child region (case insinsitive).</param>
        /// <param name="key">Key value of the child region, if found.</param>

        protected bool TryGetChildKey( IHierarchicalMetadata parent, string child, out string key )
        {
            var keys = parent?.ChildRegionKeys;

            int? getKeyIndex( params IEnumerable<string>[] collections )
            {
                foreach ( var collection in collections )
                {
                    if ( collection.IndexOf( child, out int index, StringComparer.OrdinalIgnoreCase ) )
                    {
                        return index;
                    }
                }

                return null;
            }

            var keyIndex = getKeyIndex( parent?.ChildRegionKeys, parent?.ChildRegionNames, parent?.ChildRegionLatinNames );

            if ( keys != null && keyIndex.HasValue )
            {
                key = keys.ElementAtOrDefault( keyIndex.Value );
                return true;
            }

            else
            {
                key = null;
                return false;
            }
        }
    }
}