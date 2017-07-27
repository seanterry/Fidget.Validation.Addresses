using Fidget.Validation.Addresses.Metadata;

namespace Fidget.Validation.Addresses.Client
{
    /// <summary>
    /// Defines a builder for creating metadata identifiers.
    /// </summary>

    public interface IKeyBuilder
    {
        /// <summary>
        /// Returns the key of the child region if it is found in the parent metadata, otherwise null.
        /// </summary>
        /// <param name="parent">Parent region metadata.</param>
        /// <param name="value">
        /// Key or name of the child region whose key to return (case-insensitive).
        /// For countries, this only operates against the key, since parent metadata does not include names.
        /// </param>

        string GetChildKey( CommonMetadata parent, string value );

        /// <summary>
        /// Returns the identifier of the specified region if it can be found, otherwise null.
        /// </summary>
        /// <param name="parent">Parent region metadata.</param>
        /// <param name="key">Key of the child region.</param>

        string BuildIdentifier( CommonMetadata parent, string key, string language );
    }
}