﻿using Fidget.Validation.Addresses.Metadata;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Adapters
{
    /// <summary>
    /// Defines an adapter for querying locality metadata.
    /// </summary>

    public interface ILocalityAdapter
    {
        /// <summary>
        /// Returns the metadata for the specified province if found, otherwise null.
        /// </summary>
        /// <param name="country">Key of the country containing the province.</param>
        /// <param name="province">Key or name of the province containing the locality.</param>
        /// <param name="locality">Key or name of the locality.</param>
        /// <param name="language">Metadata language (optional).</param>

        Task<LocalityMetadata> QueryAsync( string country, string province, string locality, string language = null );
    }
}