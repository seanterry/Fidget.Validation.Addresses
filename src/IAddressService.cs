using Fidget.Validation.Addresses.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Defines a service for address validation and metadata exploration.
    /// </summary>
    
    public interface IAddressService
    {
        /// <summary>
        /// Returns gobal metadata information.
        /// </summary>
        /// <param name="cancellationToken">Not used.</param>

        Task<GlobalMetadata> GetGlobalAsync( CancellationToken cancellationToken = default(CancellationToken) );
    }
}