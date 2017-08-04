using Fidget.Validation.Addresses.Metadata;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fidget.Validation.Addresses.Validation.Validators
{
    /// <summary>
    /// Base type for regional metadata test cases.
    /// </summary>
    /// <typeparam name="TParent">Parent metadata type.</typeparam>
    /// <typeparam name="TChild">Child metadata type.</typeparam>

    public abstract class RegionalValidatorTests<TValidator,TParent,TChild> 
        where TValidator : IAddressValidator
        where TParent : RegionalMetadata, new()
        where TChild : RegionalMetadata, new()
    {
        protected abstract TValidator instance { get; }
        protected abstract AddressField field { get; }
        protected ValidationContext context = new ValidationContext();
        protected IEnumerable<AddressValidationFailure> invoke() => instance.Validate( context );

        [Fact]
        public void IsRegistered()
        {
            var actual = DependencyInjection.Container.GetAllInstances<IAddressValidator>();
            Assert.Contains( actual, _ => _ is TValidator );
        }

        protected abstract void Configure( string value, TParent parent, TChild child );

        /// <summary>
        /// Cases where the child should be considered valid.
        /// </summary>

        public static IEnumerable<object[]> ValidCases()
        {
            // cases where the value is not specified in the address
            var unspecified =
                from value in new string[] { null, string.Empty, "\t" }
                from parent in new TParent[] { null, new TParent(), new TParent { ChildRegionKeys = new string[] { "XX" } } }
                from child in new TChild[] { null, new TChild { Key = "XX" } }
                select new object[] { value, parent, child };

            // cases where the parent does not specify any children
            var freeForm =
                from parent in new TParent[] { null, new TParent(), new TParent { ChildRegionKeys = new string[0] } }
                from child in new TChild[] { null, new TChild { Key = "XX" } }
                select new object[] { "XX", parent, child };

            // cases where the child is specified and the metadata key is in the parent
            var valid = new object[][]
            {
                new object[] { "XXChild", new TParent { ChildRegionKeys = new string[] { "XX" } }, new TChild { Key = "XX" } },
            };

            return valid.Union( unspecified ).Union( freeForm );
        }

        [Theory]
        [MemberData( nameof( ValidCases ) )]
        public void Validate_whenValid_returnsSuccess( string value, TParent parent, TChild child )
        {
            Configure( value, parent, child );

            var actual = invoke();
            Assert.Empty( actual );
        }

        public static IEnumerable<object[]> InvalidCases()
        {
            var keys = new string[] { "XA", "XX", "XB" };

            return
                from child in new TChild[] { null, new TChild { Key = "XXChild" } }
                select new object[] { "XXChild", new TParent { ChildRegionKeys = keys }, child };
        }

        [Theory]
        [MemberData( nameof( InvalidCases ) )]
        public void Validate_whenNotValid_returnsFailure( string value, TParent parent, TChild child )
        {
            Configure( value, parent, child );
            
            // when the child region is specified but its metadata does not match a key in the parent
            var expected = new AddressValidationFailure { Field = field, Error = AddressFieldError.UnkownValue };
            var actual = invoke();

            Assert.Equal( expected, actual.Single() );
        }
    }
}