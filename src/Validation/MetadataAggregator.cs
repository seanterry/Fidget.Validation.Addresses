using Fidget.Validation.Addresses.Service.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Aggregator for combining and overriding validation values.
    /// </summary>

    class MetadataAggregator : IMetadataAggregator
    {
        /// <summary>
        /// The default collection of required fields.
        /// Country is always required.
        /// </summary>
        
        readonly IEnumerable<AddressField> DefaultRequiredFields = new AddressField[] { AddressField.Country };

        /// <summary>
        /// Returns the collection of fields required for the given regions.
        /// </summary>
        /// <param name="regions">Regional metadata.</param>
        
        public IEnumerable<AddressField> GetRequiredFields( params IRegionalMetadata[] regions ) => regions
            .SelectMany( _=> _?.Required ?? DefaultRequiredFields )
            .Union( DefaultRequiredFields )
            .Distinct();
    }
}