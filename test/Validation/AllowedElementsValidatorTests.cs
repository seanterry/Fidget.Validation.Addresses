using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fidget.Validation.Addresses.Validation
{
    public class AllowedElementsValidatorTests
    {
        Mock<IAddressValidatorEx> MockNext = new Mock<IAddressValidatorEx>();
        IAddressValidatorEx next => MockNext?.Object;
        IAddressValidatorEx instance => new AllowedElementsValidator( next );

        public class Constructor : AllowedElementsValidatorTests
        {
            [Fact]
            public void Requires_next()
            {
                MockNext = null;
                Assert.Throws<ArgumentNullException>( nameof(next), ()=>instance );
            }
        }

        public class Validate : AllowedElementsValidatorTests
        {
            IGlobalMetadata global = new GlobalMetadata();
            IProvinceMetadata province = new ProvinceMetadata();
            ILocalityMetadata locality = new LocalityMetadata();
            ISublocalityMetadata sublocality = new SublocalityMetadata();

            IEnumerable<ValidationFailure> invoke( AddressData address, ICountryMetadata country ) => instance.Validate( address, global, country, province, locality, sublocality );

            public class AllowedElementsData : TheoryData<AddressField,AddressData>
            {
                public AllowedElementsData( params string[] values )
                {
                    foreach ( var value in values )
                    {
                        Add( AddressField.Locality, new AddressData { Locality = value } );
                        Add( AddressField.Name, new AddressData { Name = value } );
                        Add( AddressField.Organization, new AddressData { Organization = value } );
                        Add( AddressField.PostalCode, new AddressData { PostalCode = value } );
                        Add( AddressField.Province, new AddressData { Province = value } );
                        Add( AddressField.SortingCode, new AddressData { SortingCode = value } );
                        Add( AddressField.StreetAddress, new AddressData { StreetAddress = value } );
                        Add( AddressField.Sublocality, new AddressData { Sublocality = value } );
                    }
                }
            }

            public static AllowedElementsData ElementsWithWhitespace = new AllowedElementsData( null, "", "\t" );
            public static AllowedElementsData ElementsWithValues = new AllowedElementsData( Guid.NewGuid().ToString() );
            public static AllowedElementsData ElementsWithAny = new AllowedElementsData( null, "", "\t", Guid.NewGuid().ToString() );
            static readonly IEnumerable<AddressField> allFields = Enum.GetValues( typeof( AddressField ) ).OfType<AddressField>().ToArray();

            [Theory]
            [MemberData(nameof(ElementsWithWhitespace))]
            public void WhenFieldNotAllowed_valueIsWhitespace_Pass( AddressField field, AddressData address )
            {
                var others = allFields.Where( _=> _ != field ).Select( _=> $"%{(char)_}" ).ToArray();
                var country = new CountryMetadata { Format = string.Join( string.Empty, others ) };
                MockNext.Setup( _ => _.Validate( address, global, country, province, locality, sublocality ) ).Returns( Enumerable.Empty<ValidationFailure>() ).Verifiable() ;

                var expected = new ValidationFailure( field, AddressFieldError.UnexpectedField );
                var actual = invoke( address, country );

                Assert.DoesNotContain( expected, actual );
                MockNext.VerifyAll();
            }

            [Theory]
            [MemberData( nameof( ElementsWithValues ) )]
            public void WhenFieldNotAllowed_valueIsNotWhitespace_Fail( AddressField field, AddressData address )
            {
                var others = allFields.Where( _ => _ != field ).Select( _ => $"%{(char)_}" ).ToArray();
                var country = new CountryMetadata { Format = string.Join( string.Empty, others ) };
                MockNext.Setup( _ => _.Validate( address, global, country, province, locality, sublocality ) ).Returns( Enumerable.Empty<ValidationFailure>() ).Verifiable();

                var expected = new ValidationFailure( field, AddressFieldError.UnexpectedField );
                var actual = invoke( address, country );

                Assert.Contains( expected, actual );
                Assert.Equal( AddressFieldError.UnexpectedField, expected.Error );
                MockNext.VerifyAll();
            }

            [Theory]
            [MemberData( nameof( ElementsWithAny ) )]
            public void WhenFieldAllowed_Pass( AddressField field, AddressData address )
            {
                var format = allFields.Select( _ => $"%{(char)_}" ).ToArray();
                var country = new CountryMetadata { Format = string.Join( string.Empty, format ) };
                MockNext.Setup( _ => _.Validate( address, global, country, province, locality, sublocality ) ).Returns( Enumerable.Empty<ValidationFailure>() ).Verifiable();

                var expected = new ValidationFailure( field, AddressFieldError.UnexpectedField );
                var actual = invoke( address, country );

                Assert.DoesNotContain( expected, actual );
                MockNext.VerifyAll();
            }

            /// <summary>
            /// Country is always allowed (since it is always required).
            /// </summary>
            
            [Fact]
            public void WhenCountryHasValue_Pass()
            {
                var country = new CountryMetadata { Format = string.Empty };
                var address = new AddressData { Country = Guid.NewGuid().ToString() };
                MockNext.Setup( _ => _.Validate( address, global, country, province, locality, sublocality ) ).Returns( Enumerable.Empty<ValidationFailure>() ).Verifiable();

                var actual = invoke( address, country );

                Assert.DoesNotContain( actual, _=> _.Field == AddressField.Country );
                MockNext.VerifyAll();
            }

            [Theory]
            [MemberData( nameof( ElementsWithAny ) )]
            public void WhenCountryNull_Pass( AddressField field, AddressData address )
            {
                ICountryMetadata country = null;
                MockNext.Setup( _ => _.Validate( address, global, country, province, locality, sublocality ) ).Returns( Enumerable.Empty<ValidationFailure>() ).Verifiable();

                var expected = new ValidationFailure( field, AddressFieldError.UnexpectedField );
                var actual = invoke( address, country );

                Assert.DoesNotContain( expected, actual );
                MockNext.VerifyAll();
            }

            [Theory]
            [MemberData( nameof( ElementsWithAny ) )]
            public void WhenFormatNull_Pass( AddressField field, AddressData address )
            {
                var country = new CountryMetadata { Format = null };
                MockNext.Setup( _ => _.Validate( address, global, country, province, locality, sublocality ) ).Returns( Enumerable.Empty<ValidationFailure>() ).Verifiable();

                var expected = new ValidationFailure( field, AddressFieldError.UnexpectedField );
                var actual = invoke( address, country );

                Assert.DoesNotContain( expected, actual );
                MockNext.VerifyAll();
            }

            [Fact]
            public void Returns_PastFailures()
            {
                var address = new AddressData { Country = "ABC" };
                var country = new CountryMetadata { Format = null };
                var expected = allFields.Select( _=> new ValidationFailure( _, AddressFieldError.InvalidFormat ) );
                MockNext.Setup( _ => _.Validate( address, global, country, province, locality, sublocality ) ).Returns( expected ).Verifiable();

                var actual = invoke( address, country );
                Assert.Equal( expected, actual );
                MockNext.VerifyAll();
            }
        }
    }
}