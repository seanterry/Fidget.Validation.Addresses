using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fidget.Validation.Addresses.Validation
{
    public class AllowedElementsValidatorTests
    {
        IAddressValidator instance => new AllowedElementsValidator();

        public class Validate : AllowedElementsValidatorTests
        {
            static string random() => Guid.NewGuid().ToString();

            AddressData address = new AddressData
            {
                Country = random(),
                Province = random(),
                Locality = random(),
                Sublocality = random(),
                Name = random(),
                Organization = random(),
                PostalCode = random(),
                SortingCode = random(),
                StreetAddress = random(),
            };

            Mock<IValidationContext> MockContext = new Mock<IValidationContext>();
            IValidationContext context => MockContext?.Object;

            IEnumerable<ValidationFailure> invoke() => instance.Validate( address, context );

            [Fact]
            public void Requires_address()
            {
                address = null;
                Assert.Throws<ArgumentNullException>( nameof( address ), () => invoke() );
            }

            [Fact]
            public void Requires_context()
            {
                MockContext = null;
                Assert.Throws<ArgumentNullException>( nameof( context ), () => invoke() );
            }

            [Fact]
            public void WhenAllFieldsEmpty_returns_empty()
            {
                address = new AddressData();
                var fields = Enum.GetValues( typeof( AddressField ) ).OfType<AddressField>();
                MockContext.Setup( _ => _.GetRequiredFields() ).Returns( fields );

                var expected = Enumerable.Empty<ValidationFailure>();
                var actual = invoke();
                Assert.Equal( expected, actual );
            }

            delegate void MutatorDelegate( AddressData address, string value );

            /// <summary>
            /// Mutators for setting address property values.
            /// </summary>

            static readonly IReadOnlyDictionary<AddressField, MutatorDelegate> Mutators = new Dictionary<AddressField, MutatorDelegate>
            {
                { AddressField.Country, ( a, v ) => a.Country = v },
                { AddressField.Locality, ( a, v ) => a.Locality = v },
                { AddressField.Name, ( a, v ) => a.Name = v },
                { AddressField.Organization, ( a, v ) => a.Organization = v },
                { AddressField.PostalCode, ( a, v ) => a.PostalCode = v },
                { AddressField.Province, ( a, v ) => a.Province = v },
                { AddressField.SortingCode, ( a, v ) => a.SortingCode = v },
                { AddressField.StreetAddress, ( a, v ) => a.StreetAddress = v },
                { AddressField.Sublocality, ( a, v ) => a.Sublocality = v },
            };

            public static IEnumerable<object[]> EmptyFieldsCases()
            {
                var fields = Mutators.Keys;
                var values = new string[] { null, string.Empty, "\t" };

                return
                    from field in fields
                    from value in values
                    select new object[] { field, value };
            }

            [Theory]
            [MemberData( nameof( EmptyFieldsCases ) )]
            public void WhenAllowedFieldEmpty_returns_pass( AddressField field, string value )
            {
                Mutators[field].Invoke( address, value );
                MockContext.Setup( _ => _.GetAllowedFields() ).Returns( new AddressField[] { field } );

                var expected = new ValidationFailure( field, AddressFieldError.UnexpectedField );
                var actual = invoke();
                Assert.DoesNotContain( expected, actual );
            }

            [Theory]
            [MemberData( nameof( EmptyFieldsCases ) )]
            public void WhenFieldNotAllowedEmpty_returns_pass( AddressField field, string value )
            {
                var allowed = Enum.GetValues( typeof( AddressField ) ).OfType<AddressField>().Where( _ => _ != field );
                Mutators[field].Invoke( address, value );
                MockContext.Setup( _ => _.GetAllowedFields() ).Returns( allowed );

                var expected = new ValidationFailure( field, AddressFieldError.UnexpectedField );
                var actual = invoke();

                Assert.DoesNotContain( expected, actual );
            }

            public static IEnumerable<object[]> FilledFieldsCases() => Mutators.Keys
                .Select( _ => new object[] { _ } );

            [Theory]
            [MemberData( nameof( FilledFieldsCases ) )]
            public void WhenAllowedFieldFilled_returns_pass( AddressField field )
            {
                address = new AddressData();
                Mutators[field].Invoke( address, random() );
                MockContext.Setup( _ => _.GetAllowedFields() ).Returns( new AddressField[] { field } );

                var expected = new ValidationFailure( field, AddressFieldError.UnexpectedField );
                var actual = invoke();

                Assert.DoesNotContain( expected, actual );
            }

            [Theory]
            [MemberData(nameof(FilledFieldsCases))]
            public void WhenNotAllowedFieldFilled_returns_failure( AddressField field )
            {
                var allowed = Enum.GetValues( typeof( AddressField ) ).OfType<AddressField>().Where( _ => _ != field );
                address = new AddressData();
                Mutators[field].Invoke( address, random() );
                MockContext.Setup( _ => _.GetAllowedFields() ).Returns( allowed );

                var expected = new ValidationFailure( field, AddressFieldError.UnexpectedField );
                var actual = invoke();

                Assert.Contains( expected, actual );
            }
        }
    }
}
