using Fidget.Validation.Addresses.Adapters;
using Fidget.Validation.Addresses.Metadata;
using System;

namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Extension methods related to adapters.
    /// </summary>

    public static class AdapterExtensions
    {
        /// <summary>
        /// Returns the global metadata entry.
        /// </summary>
        /// <param name="adapter">Global service adapter.</param>
        
        public static GlobalMetadata Query( this IGlobalAdapter adapter )
        {
            if ( adapter == null ) throw new ArgumentNullException( nameof( adapter ) );

            return adapter.QueryAsync().Result;
        }
    }
}