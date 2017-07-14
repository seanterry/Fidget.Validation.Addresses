using System;
using System.Collections.Generic;
using System.Text;
using Fidget.Validation.Addresses.Service.Metadata;
using System.Linq;

namespace Fidget.Validation.Addresses.Validation
{
    /// <summary>
    /// Validates elements that are allowed in an address.
    /// </summary>
    
    class AllowedElementsValidator : IAddressValidator
    {
        /// <summary>
        /// Validates the given address.
        /// </summary>

        public IEnumerable<ValidationFailure> Validate( AddressData address, IValidationContext context )
        {
            if ( address == null ) throw new ArgumentNullException( nameof( address ) );
            if ( context == null ) throw new ArgumentNullException( nameof( context ) );

            var failures = new List<ValidationFailure>();
            var allowed = context.GetAllowedFields();

            void validate( AddressField field, string value )
            {
                if ( !string.IsNullOrWhiteSpace( value ) && !allowed.Contains( field ) )
                    failures.Add( new ValidationFailure( field, AddressFieldError.UnexpectedField ) );
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
