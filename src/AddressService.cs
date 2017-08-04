using Fidget.Commander;
using Fidget.Validation.Addresses.Metadata;
using Fidget.Validation.Addresses.Metadata.Commands;
using Fidget.Validation.Addresses.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses
{
    /// <summary>
    /// Service for address validation and metadata exploration.
    /// </summary>

    public class AddressService : IAddressService
    {
        /// <summary>
        /// Factory method for creating a new instance of the service.
        /// </summary>
        
        public static IAddressService FactoryMethod() => DependencyInjection.Container.GetInstance<IAddressService>();

        /// <summary>
        /// Gets the default instance of the service.
        /// </summary>

        public static IAddressService Default { get; } = FactoryMethod();
        
        /// <summary>
        /// Command dispatcher.
        /// </summary>
        
        readonly ICommandDispatcher Dispatcher;

        /// <summary>
        /// Constructs a service for address validation and metadata exploration.
        /// </summary>
        /// <param name="dispatcher">Command dispatcher.</param>
        
        public AddressService( ICommandDispatcher dispatcher )
        {
            Dispatcher = dispatcher ?? throw new ArgumentNullException( nameof(dispatcher) );
        }

        /// <summary>
        /// Returns gobal metadata information.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        
        public async Task<GlobalMetadata> GetGlobalAsync( CancellationToken cancellationToken ) => 
            await Dispatcher.Execute( GlobalMetadataQuery.Default, cancellationToken );

        /// <summary>
        /// Returns metadata for the specified country.
        /// </summary>
        /// <param name="country">Key of the country.</param>
        /// <param name="language">Requested language of the metadata.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Metadata for the specified country if found, otherwise null.</returns>
        
        public async Task<CountryMetadata> GetCountryAsync( string country, string language, CancellationToken cancellationToken )
        {
            var query = new CountryMetadataQuery
            {
                Country = country,
                Language = language,
            };

            return await Dispatcher.Execute( query, cancellationToken );
        }

        /// <summary>
        /// Returns metadata for the specified province.
        /// </summary>
        /// <param name="country">Key of the containing country.</param>
        /// <param name="province">Key or name of the province.</param>
        /// <param name="language">Requested language of the metadata.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Metadata for the specified province if found, otherwise null.</returns>
        
        public async Task<ProvinceMetadata> GetProvinceAsync( string country, string province, string language, CancellationToken cancellationToken )
        {
            var query = new ProvinceMetadataQuery
            {
                Country = country,
                Province = province,
                Language = language,
            };

            return await Dispatcher.Execute( query, cancellationToken );
        }

        /// <summary>
        /// Returns metadata for the specified locality.
        /// </summary>
        /// <param name="country">Key of the containing country.</param>
        /// <param name="province">Key or name of the containing province.</param>
        /// <param name="locality">Key or name of the locality.</param>
        /// <param name="language">Requested language of the metadata.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Metadata for the specified locality if found, otherwise null.</returns>

        public async Task<LocalityMetadata> GetLocalityAsync( string country, string province, string locality, string language, CancellationToken cancellationToken )
        {
            var query = new LocalityMetadataQuery
            {
                Country = country,
                Province = province,
                Locality = locality,
                Language = language,
            };

            return await Dispatcher.Execute( query, cancellationToken );
        }

        /// <summary>
        /// Returns metadata for the specified sublocality.
        /// </summary>
        /// <param name="country">Key of the containing country.</param>
        /// <param name="province">Key or name of the containing province.</param>
        /// <param name="locality">Key or name of the containing locality.</param>
        /// <param name="sublocality">Key or name of the sublocality.</param>
        /// <param name="language">Requested language of the metadata.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Metadata for the specified sublocality if found, otherwise null.</returns>

        public async Task<SublocalityMetadata> GetSublocalityAsync( string country, string province, string locality, string sublocality, string language, CancellationToken cancellationToken )
        {
            var query = new SublocalityMetadataQuery
            {
                Country = country,
                Province = province,
                Locality = locality,
                Sublocality = sublocality,
                Language = language,
            };

            return await Dispatcher.Execute( query, cancellationToken );
        }

        /// <summary>
        /// Validates the given address and returns the collection of any validation failures.
        /// </summary>
        /// <param name="address">Address to validate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// A collection containing any validation failures. 
        /// When the address is valid, the collection will be empty.
        /// </returns>
        
        public async Task<IEnumerable<AddressValidationFailure>> ValidateAsync( AddressData address, CancellationToken cancellationToken )
        {
            if ( address == null ) throw new ArgumentNullException( nameof( address ) );

            var command = new Validation.Commands.ValidateAddressCommand
            {
                Address = address,
            };

            return await Dispatcher.Execute( command, cancellationToken );
        }
    }
}