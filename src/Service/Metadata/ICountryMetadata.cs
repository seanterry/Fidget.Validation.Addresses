using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Service.Metadata
{
    /// <summary>
    /// Defines the elements of country-level address metadata. 
    /// </summary>

    public interface ICountryMetadata : IHierarchicalMetadata
    {
        /// <summary>
        /// Gets the collection of languages for which regional data is available (if known).
        /// </summary>
        /// <remarks>
        
        IEnumerable<string> Languages { get; }

        /// <summary>
        /// Gets a string that represents the address format in the country.
        /// The sequence %n indicates a line feed. 
        /// All other characters preceded by a percent (%) character are address elements that are allowed in the address (in order).
        /// Characters that are not preceded by a percent (%) character are assumed to be literals.
        /// </summary>
        
        string Format { get; }

        /// <summary>
        /// Get an alternate value of <see cref="Format"/> to use when formatting an address in a latin script.
        /// </summary>
        /// <remarks>
        
        string LatinFormat { get; }

        /// <summary>
        /// Gets the name used to describe the state/province level region in the country.
        /// </summary>

        string StateType { get; }

        /// <summary>
        /// Gets the name used to describe the city/locality level region in the country.
        /// </summary>

        string LocalityType { get; }

        /// <summary>
        /// Gets the name used to describe the sub-locality level region in the country.
        /// </summary>
        
        string SublocalityType { get; }

        /// <summary>
        /// Gets the name used to describe the postal code in the country.
        /// </summary>

        string PostalCodeType { get; }
    }
}