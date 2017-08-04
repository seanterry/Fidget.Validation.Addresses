using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Defines a factory for creating address validation contexts.
    /// </summary>

    public interface IValidationContextFactory
    {
        /// <summary>
        /// Creates and returns a context for validating the given address.
        /// </summary>
        /// <param name="address">Address to validate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>

        Task<ValidationContext> Create( AddressData address, CancellationToken cancellationToken );
    }
}