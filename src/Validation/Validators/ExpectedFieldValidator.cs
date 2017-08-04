using System.Collections.Generic;
using System.Linq;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    /// <summary>
    /// Validates that all fields of an address are expected.
    /// </summary>

    public class ExpectedFieldValidator : IAddressValidator
    {
        /// <summary>
        /// Error for validation failures of this type.
        /// </summary>

        readonly AddressFieldError Error = AddressFieldError.UnexpectedField;

        /// <summary>
        /// Validates the address.
        /// </summary>
        /// <param name="context">Address validation context.</param>
        /// <returns>The collection of any validation errors.</returns>

        public IEnumerable<AddressValidationFailure> Validate( ValidationContext context )
        {
            var format = context.CountryMetadata?.Format ?? string.Empty;
            var fields = new Dictionary<AddressField, string>
            {
                // note: country is always allowed (since it is always required)
                { AddressField.Locality, context.Address.Locality },
                { AddressField.Name, context.Address.Name },
                { AddressField.Organization, context.Address.Organization },
                { AddressField.PostalCode, context.Address.PostalCode },
                { AddressField.Province, context.Address.Province },
                { AddressField.SortingCode, context.Address.SortingCode },
                { AddressField.StreetAddress, context.Address.StreetAddress },
                { AddressField.Sublocality, context.Address.Sublocality },
            };

            return fields
                .Where( _=> !format.Contains( $"%{(char)_.Key}" ) )
                .Where( _=> !string.IsNullOrWhiteSpace( _.Value ) )
                .Select( _=> new AddressValidationFailure { Field = _.Key, Error = Error } )
                .OrderBy( _=> _.Field )
                .ThenBy( _=> _.Error )
                .ToArray();
        }
    }
}