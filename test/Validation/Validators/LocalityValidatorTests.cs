using Fidget.Validation.Addresses.Metadata;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    public class LocalityValidatorTests : RegionalValidatorTests<LocalityValidator,ProvinceMetadata,LocalityMetadata>
    {
        protected override LocalityValidator instance => new LocalityValidator();
        protected override AddressField field => AddressField.Locality;

        protected override void Configure( string value, ProvinceMetadata parent, LocalityMetadata child )
        {
            context.Address = new AddressData { Locality = value };
            context.ProvinceMetadata = parent;
            context.LocalityMetadata = child;
        }
    }
}