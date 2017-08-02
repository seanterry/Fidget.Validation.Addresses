using Fidget.Validation.Addresses.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    /// <summary>
    /// Validates that all required fields of an address are filled.
    /// </summary>

    public class RequiredFieldValidator : IAddressValidator
    {
        /// <summary>
        /// Error for validation failures of this type.
        /// </summary>
        
        readonly AddressFieldError Error = AddressFieldError.MissingRequiredField;

        /// <summary>
        /// Validates the address.
        /// </summary>
        /// <param name="context">Address validation context.</param>
        /// <returns>The collection of any validation errors.</returns>
        
        public IEnumerable<AddressValidationFailure> Validate( ValidationContext context )
        {
            // gets the collection of required fields based on the metadata
            IEnumerable<AddressField> getRequired( params RegionalMetadata[] regions ) => regions
                .Where( _=> _?.Required != null )
                .SelectMany( _=> _.Required )
                .Distinct()
                .ToArray();
                
            // determine the required fields, this is additive over the subregions
            // country is always required
            var required = getRequired( context.CountryMetadata, context.ProvinceMetadata, context.LocalityMetadata, context.SublocalityMetadata )
                .Union( new AddressField[] { AddressField.Country } );

            var fields = new Dictionary<AddressField,string>
            {
                { AddressField.Country, context.Address.Country },
                { AddressField.Locality, context.Address.Locality },
                { AddressField.Name, context.Address.Name },
                { AddressField.Organization, context.Address.Organization },
                { AddressField.PostalCode, context.Address.PostalCode },
                { AddressField.Province, context.Address.Province },
                { AddressField.SortingCode, context.Address.SortingCode },
                { AddressField.StreetAddress, context.Address.StreetAddress },
                { AddressField.Sublocality, context.Address.Sublocality },
            };

            return required
                .Where( _=> string.IsNullOrWhiteSpace( fields[_] ) )
                .Select( _=> new AddressValidationFailure { Field = _, Error = Error } )
                .OrderBy( _=> _.Field )
                .ThenBy( _=> _.Error )
                .ToArray();
        }
    }
}