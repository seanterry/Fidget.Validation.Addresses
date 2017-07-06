using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Service.Metadata
{
    /// <summary>
    /// Defines metadata available at any regional level.
    /// </summary>

    public interface IRegionalMetadata : ICommonMetadata
    {
        /// <summary>
        /// Gets the key of the region, which uniquely identifies it within its parent data element.
        /// </summary>

        string Key { get; }

        /// <summary>
        /// Gets the language for the regional data (if known).
        /// </summary>

        string Language { get; }

        /// <summary>
        /// Gets the collection of address elements that are required for addresses in the region.
        /// If this value is null, the value from the parent region should be used.
        /// </summary>
        
        IEnumerable<char> Required { get; }

        /// <summary>
        /// Gets a regular expression pattern to use for validating the postal code.
        /// At the country level, this pattern should match the entire pattern.
        /// For all other levels, this is a prefix.
        /// </summary>
        
        string PostalCodePattern { get; }
    }
}