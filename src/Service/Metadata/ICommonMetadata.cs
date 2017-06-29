namespace Fidget.Validation.Addresses.Service.Metadata
{
    /// <summary>
    /// Defines the common elements of all address metadata.
    /// </summary>

    public interface ICommonMetadata
    {
        /// <summary>
        /// Gets the key of the region or data record, in the form of a path from parent identifiers to the record key.
        /// </summary>

        string Id { get; set; }
    }
}