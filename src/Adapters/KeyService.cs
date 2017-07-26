using Fidget.Validation.Addresses.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fidget.Validation.Addresses.Adapters
{
    /// <summary>
    /// Service for locating regional key values.
    /// </summary>

    class KeyService : IKeyService
    {
        /// <summary>
        /// Returns whether the given country is in the global metadata collection.
        /// </summary>
        /// <param name="globalMeta">Global metadata.</param>
        /// <param name="country">Key of the country (case insensitive).</param>
        /// <param name="key">The located key of the country if found, otherwise null.</param>
        
        public bool TryGetCountryKey( GlobalMetadata globalMeta, string country, out string key )
        {
            key = globalMeta != null && globalMeta.Countries.IndexOf( country, out int index, StringComparer.OrdinalIgnoreCase )
                ? globalMeta.Countries.ElementAtOrDefault( index )
                : null;
            
            return key != null;
        }

        /// <summary>
        /// Returns whether the specified child region is found in the parent region metadata.
        /// </summary>
        /// <param name="meta">Parent region metadata.</param>
        /// <param name="value">Key, name, or latin name of the child region (case insensitive).</param>
        /// <param name="key">The located key of the child region if found, otherwise null.</param>
        
        public bool TryGetChildKey( RegionalMetadata meta, string value, out string key )
        {
            string getKey( IEnumerable<string> collection ) => collection.IndexOf( value, out int index, StringComparer.OrdinalIgnoreCase )
                ? meta?.ChildRegionKeys?.ElementAtOrDefault( index )
                : null;
                        
            key = 
                getKey( meta?.ChildRegionKeys ) ??
                getKey( meta?.ChildRegionNames ) ??
                getKey( meta?.ChildRegionLatinNames );
            
            return key != null;
        }

        /// <summary>
        /// Builds and returns the data service identifier for the specified entry.
        /// </summary>
        /// <param name="parent">Parent region metadata.</param>
        /// <param name="key">Key of the entry for which to build an identifier.</param>
        /// <param name="language">Language of the metadata.</param>
        
        public string BuildIdentifier( CommonMetadata parent, string key, string language )
        {
            if ( parent == null ) throw new ArgumentNullException( nameof( parent ) );
            if ( key == null ) throw new ArgumentNullException( nameof( key ) );
            
            if ( parent?.Id is string root )
            {
                // remove language component of parent if present
                root = root.Contains( "--" )
                    ? root.Remove( root.IndexOf( "--" ) )
                    : root;

                return $"{root}/{key}{( language != null ? $"--{language}" : string.Empty )}";
            }

            else return null;            
        }
    }
}