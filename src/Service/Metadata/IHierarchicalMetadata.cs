using System.Collections.Generic;

namespace Fidget.Validation.Addresses.Service.Metadata
{
    /// <summary>
    /// Defines metadata available for regions that can contain child regions.
    /// </summary>

    public interface IHierarchicalMetadata : IRegionalMetadata
    {
        /// <summary>
        /// Gets the <see cref="Key"/> values of this region's sub-regions (if any).
        /// </summary>

        IEnumerable<string> ChildRegionKeys { get; }

        /// <summary>
        /// Gets the <see cref="Name"/> values of this region's sub-regions (if any).
        /// </summary>

        IEnumerable<string> ChildRegionNames { get; }

        /// <summary>
        /// Gets the <see cref="LatinName"/> values of this region's sub-regions (if any).
        /// </summary>

        IEnumerable<string> ChildRegionLatinNames { get; }

        /// <summary>
        /// Gets whether child regions have their own children.
        /// </summary>

        IEnumerable<bool> ChildRegionExpands { get; }
    }
}