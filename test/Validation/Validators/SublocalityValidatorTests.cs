using Fidget.Validation.Addresses.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    public class SublocalityValidatorTests : RegionalValidatorTests<SublocalityValidator,LocalityMetadata, SublocalityMetadata>
    {
        protected override SublocalityValidator instance => new SublocalityValidator();
        protected override AddressField field => AddressField.Sublocality;

        protected override void Configure( string value, LocalityMetadata parent, SublocalityMetadata child )
        {
            context.Address = new AddressData { Sublocality = value };
            context.LocalityMetadata = parent;
            context.SublocalityMetadata = child;
        }
    }
}