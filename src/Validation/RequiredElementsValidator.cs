using Fidget.Validation.Addresses.Service.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Validator that ensures required elements are present in the address.
    /// </summary>

    partial class RequiredElementsValidator : IAddressValidatorEx
    {
        internal static readonly IAddressValidatorEx Default = DependencyInjection.Container.GetInstance<IAddressValidatorEx>();

        /// <summary>
        /// Validates the given address.
        /// </summary>
        
        public IEnumerable<ValidationFailure> Validate( AddressData address, IGlobalMetadata global, ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality )
        {
            if ( address == null ) throw new ArgumentNullException( nameof( address ) );
            if ( global == null ) throw new ArgumentNullException( nameof( global ) );

            var failures = new List<ValidationFailure>();
            var defaults = new AddressField[] { AddressField.Country };
            var required = Enumerable.Empty<AddressField>();
            
            void validate( AddressField field, string value ) 
            { 
                if ( string.IsNullOrWhiteSpace( value ) && required.Contains( field ) ) 
                    failures.Add( new ValidationFailure( field, AddressFieldError.MissingRequiredField ) ); 
            }

            validate( AddressField.Country, address.Country );
            validate( AddressField.Province, address.Province );
            validate( AddressField.Locality, address.Locality );
            validate( AddressField.Sublocality, address.Sublocality );
            validate( AddressField.PostalCode, address.PostalCode );
            validate( AddressField.SortingCode, address.SortingCode );
            validate( AddressField.StreetAddress, address.StreetAddress );
            validate( AddressField.Organization, address.Organization );
            validate( AddressField.Name, address.Name );
            
            return failures;
        }
    }
}