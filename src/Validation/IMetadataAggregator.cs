using Fidget.Validation.Addresses.Service.Metadata;
using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Defines an aggregator for combining and overriding validation values.
    /// </summary>

    public interface IMetadataAggregator
    {
        /// <summary>
        /// Returns the collection of fields required for the given regions.
        /// </summary>
        /// <param name="regions">Regional metadata.</param>

        IEnumerable<AddressField> GetRequiredFields( params IRegionalMetadata[] regions );
    }
}