using Fidget.Validation.Addresses.Metadata;

namespace Fidget.Validation.Addresses.Adapters
{
    /// <summary>
    /// Defines a service for locating metadata keys.
    /// </summary>

    public interface IKeyService
    {
        /// <summary>
        /// Returns whether the given country is in the global metadata collection.
        /// </summary>
        /// <param name="globalMeta">Global metadata.</param>
        /// <param name="country">Key of the country (case insensitive).</param>
        /// <param name="key">The located key of the country if found, otherwise null.</param>

        bool TryGetCountryKey( GlobalMetadata globalMeta, string country, out string key );

        /// <summary>
        /// Returns whether the specified child region is found in the parent region metadata.
        /// </summary>
        /// <param name="meta">Parent region metadata.</param>
        /// <param name="value">Key, name, or latin name of the child region (case insensitive).</param>
        /// <param name="key">The located key of the child region if found, otherwise null.</param>

        bool TryGetChildKey( RegionalMetadata meta, string value, out string key );

        /// <summary>
        /// Builds and returns the data service identifier for the specified entry.
        /// </summary>
        /// <param name="parent">Parent region metadata.</param>
        /// <param name="key">Key of the entry for which to build an identifier.</param>
        /// <param name="language">Language of the metadata (optional).</param>

        string BuildIdentifier( CommonMetadata parent, string key, string language = null );
    }
}