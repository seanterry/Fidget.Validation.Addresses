using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fidget.Validation.Addresses.Service.Metadata
{
    /// <summary>
    /// Extension methods related to metadata.
    /// </summary>
    
    public static class MetadataExtensions
    {
        /// <summary>
        /// Converts the given collection to an array if it is not already one.
        /// </summary>
        /// <typeparam name="T">Type of the elements in the collection.</typeparam>
        /// <param name="source">Source collection.</param>

        static T[] AsArray<T>( this IEnumerable<T> source ) => source is T[] array
            ? array
            : source?.ToArray();

        /// <summary>
        /// Attempts to locate the key of the child region by its candidate text.
        /// </summary>
        /// <param name="parent">Metadata whose child regions to search.</param>
        /// <param name="candidate">Candidate value to locate.</param>
        /// <param name="key">Key of the child entry, if found.</param>
        /// <returns>True if the candidate value could be resolved to a value, otherwise false.</returns>
        
        public static bool TryGetChildKey( this IHierarchicalMetadata parent, string candidate, out string key )
        {
            int? getKeyIndex( IEnumerable<string> collection ) => collection != null
                ? Array.FindIndex( collection.AsArray(), _=> _.Equals( candidate, StringComparison.OrdinalIgnoreCase ) )
                : (int?)null;

            var index =
                getKeyIndex( parent?.ChildRegionKeys ) ??
                getKeyIndex( parent?.ChildRegionNames ) ??
                getKeyIndex( parent?.ChildRegionLatinNames );
            
            key = index.HasValue
                ? parent.ChildRegionKeys.ElementAtOrDefault( index.Value )
                : null;
            
            return key != null;
        }
    }
}