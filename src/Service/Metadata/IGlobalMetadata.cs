using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Service.Metadata
{
    /// <summary>
    /// Defines the elements of global-level address metadata. 
    /// </summary>

    public interface IGlobalMetadata : ICommonMetadata
    {
        /// <summary>
        /// Gets the keys of countries for which data is available.
        /// </summary>

        IEnumerable<string> Countries { get; }
    }
}