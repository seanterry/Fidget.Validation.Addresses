using Fidget.Validation.Addresses.Metadata;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    public class ProvinceValidatorTests : RegionalValidatorTests<ProvinceValidator, CountryMetadata, ProvinceMetadata>
    {
        protected override ProvinceValidator instance => new ProvinceValidator();
        protected override AddressField field => AddressField.Province;

        protected override void Configure( string value, CountryMetadata parent, ProvinceMetadata child )
        {
            context.Address = new AddressData { Province = value };
            context.CountryMetadata = parent;
            context.ProvinceMetadata = child;
        }
    }
}