using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Validation;

namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Service for address validation and metadata exploration.
    /// </summary>

    public static partial class AddressService
    {
        /// <summary>
        /// Gets the default instance of the address service.
        /// </summary>
        
        public static IAddressService Instance { get; } = DependencyInjection.Container.GetInstance<IAddressService>();
    }
}