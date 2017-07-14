using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Defines a factory for creating validation contexts.
    /// </summary>

    public interface IValidationContextFactory
    {
        /// <summary>
        /// Gets the collection of address validators.
        /// </summary>

        IEnumerable<IAddressValidator> Validators { get; }

        /// <summary>
        /// Creates and returns a validation context.
        /// </summary>
        /// <param name="address">Address for which to create a validation context.</param>
        /// <param name="service">Service from which to acquire metadata.</param>
        /// <param name="language">Metadata language.</param>

        Task<IValidationContext> Create( AddressData address, IAddressService service, string language = null );
    }
}