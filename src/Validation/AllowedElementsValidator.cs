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
    
    class AllowedElementsValidator : AddressValidatorDecorator
    {
        /// <summary>
        /// Constructs an instance to validate elements that are allowed in an address.
        /// </summary>
        /// <param name="next">Validator decorated by the current instance.</param>
        
        public AllowedElementsValidator( IAddressValidator next ) : base( next ) {}

        /// <summary>
        /// Collection of failures indexed by field type.
        /// </summary>

        internal static readonly IReadOnlyDictionary<AddressField, ValidationFailure> Failures = Enum.GetValues( typeof( AddressField ) )
            .OfType<AddressField>()
            .Where( _=> _ != AddressField.Country )
            .Select( _ => new ValidationFailure( _, AddressFieldError.UnexpectedField ) )
            .ToDictionary( _ => _.Field );

        /// <summary>
        /// Validates the given address.
        /// </summary>

        public override IEnumerable<ValidationFailure> Validate( AddressData address, IGlobalMetadata global, ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality )
        {
            if ( address == null ) throw new ArgumentNullException( nameof( address ) );

            var failures = Next
                .Validate( address, global, country, province, locality, sublocality )
                .ToList();

            if ( country?.Format is string format )
            {
                void validate( AddressField field, string value )
                {
                    if ( !string.IsNullOrWhiteSpace( value ) && !format.Contains( $"%{(char)field}" ) )
                        failures.Add( Failures[field] );
                }

                validate( AddressField.Province, address.Province );
                validate( AddressField.Locality, address.Locality );
                validate( AddressField.Sublocality, address.Sublocality );
                validate( AddressField.PostalCode, address.PostalCode );
                validate( AddressField.SortingCode, address.SortingCode );
                validate( AddressField.StreetAddress, address.StreetAddress );
                validate( AddressField.Organization, address.Organization );
                validate( AddressField.Name, address.Name );
            }

            return failures;
        }
    }
}
