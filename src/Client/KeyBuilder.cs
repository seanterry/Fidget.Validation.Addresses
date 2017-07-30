using Fidget.Validation.Addresses.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fidget.Validation.Addresses.Client
{
    /// <summary>
    /// Builder for creating metadata identifiers.
    /// </summary>

    class KeyBuilder : IKeyBuilder
    {
        /// <summary>
        /// Returns the key of the child region if it is found in the parent metadata, otherwise null.
        /// </summary>
        /// <param name="parent">Parent region metadata.</param>
        /// <param name="value">
        /// Key or name of the child region whose key to return (case-insensitive).
        /// For countries, this only operates against the key, since parent metadata does not include names.
        /// </param>
        
        public string GetChildKey( CommonMetadata parent, string value )
        {
            // returns the index of the first occurrence of the value within any of the given collections
            // names and latin names are in the same ordinal position as their keys
            int getKeyIndex( params IEnumerable<string>[] collections )
            {
                var index = -1;

                foreach ( var collection in collections )
                {
                    if ( collection != null )
                    {
                        index = Array.FindIndex( collection.ToArray(), _ => _.Equals( value, StringComparison.OrdinalIgnoreCase ) );
                        if ( index >= 0 ) break;
                    }
                }

                return index;
            }

            switch ( parent )
            {
                // when searching global metadata, only the country keys are available
                case GlobalMetadata global:
                    var country = getKeyIndex( global.Countries );
                    return global.Countries?.ElementAtOrDefault( country );
                    
                // when searching regional metadata, use the keys, names, and latin names
                case RegionalMetadata region:
                    var child = getKeyIndex( region.ChildRegionKeys, region.ChildRegionNames, region.ChildRegionLatinNames );
                    return region.ChildRegionKeys?.ElementAtOrDefault( child );

                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns the identifier of the specified region if it can be found, otherwise null.
        /// </summary>
        /// <param name="parent">Parent region metadata.</param>
        /// <param name="key">Key of the child region.</param>
        /// <param name="language">Language to encode in the identifier.</param>
        
        public string BuildIdentifier( CommonMetadata parent, string key, string language )
        {
            if ( parent == null ) throw new ArgumentNullException( nameof( parent ) );
            if ( key == null ) throw new ArgumentNullException( nameof( key ) );

            if ( parent.Id is string root )
            {
                switch ( parent )
                {
                    // when the parent is global, we're requesting country data - this is the only case where we encode the language
                    case GlobalMetadata global:
                        return language != null
                            ? $"{root}/{key}--{language}"
                            : $"{root}/{key}";

                    // all other requests use the language of the parent entity
                    // this is because the default language for a country does not accept the language argument
                    default:
                        return root.Contains( "--" )
                            ? root.Insert( root.IndexOf( "--" ), $"/{key}" )
                            : $"{root}/{key}";
                }
            }

            return null;
        }
    }
}