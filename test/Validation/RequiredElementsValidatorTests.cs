using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Fidget.Validation.Addresses.Validation
{
    public class RequiredElementsValidatorTests
    {
        IAddressValidator create() => new RequiredElementsValidator();

        public class Validate : RequiredElementsValidatorTests
        {
            static string random() => Guid.NewGuid().ToString();
            AddressData address = new AddressData();

            IGlobalMetadata global = new GlobalMetadata { Id = "data", Countries = new string[] { "XW", "XA" } };
            IEnumerable<ValidationFailure> invoke( ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality ) => create().Validate( address, global, country, province, locality, sublocality );
            
            [Fact]
            public void Requires_address()
            {
                address = null;
                Assert.Throws<ArgumentNullException>( nameof( address ), () => invoke( null, null, null, null ) );
            }

            [Fact]
            public void Requires_global()
            {
                global = null;
                Assert.Throws<ArgumentNullException>( nameof( global ), () => invoke( null, null, null, null ) );
            }
            
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("\t")]
            public void WhenCountryEmpty_returns_failure( string value )
            {
                address.Country = value;
                var expected = RequiredElementsValidator.Failures[AddressField.Country];
                var actual = invoke( null, null, null, null );

                Assert.Contains( expected, actual );
            }

            [Fact]
            public void WhenCountryPresent_returns_valid()
            {
                address.Country = random();
                var expected = RequiredElementsValidator.Failures[AddressField.Country];
                var actual = invoke( null, null, null, null );

                Assert.DoesNotContain( expected, actual );
            }

            static readonly AddressField[] AllRequiredFields = Enum.GetValues( typeof( AddressField ) ).OfType<AddressField>().ToArray();

            /// <summary>
            /// Matrix of conditions where a given field should be required, and the value to supply for testing.
            /// </summary>
            
            public class RequiredFieldTheoryData : TheoryData<string,ICountryMetadata,IProvinceMetadata,ILocalityMetadata,ISublocalityMetadata>
            {
                public RequiredFieldTheoryData( AddressField field, params string[] values )
                {
                    var target = new AddressField[] { field };
                    var others = AllRequiredFields.Except( target );
                    var empty = Enumerable.Empty<AddressField>();
                    CountryMetadata country( IEnumerable<AddressField> required = null ) => new CountryMetadata { Required = required };
                    ProvinceMetadata province( IEnumerable<AddressField> required = null ) => new ProvinceMetadata { Required = required };
                    LocalityMetadata locality( IEnumerable<AddressField> required = null ) => new LocalityMetadata { Required = required };
                    SublocalityMetadata sublocality( IEnumerable<AddressField> required = null ) => new SublocalityMetadata { Required = required };
                    
                    foreach ( var value in values )
                    {
                        // null metadata should still union the required values
                        Add( value, country( target ), null, null, null );
                        Add( value, null, province( target ), null, null );
                        Add( value, null, null, locality( target ), null );
                        Add( value, null, null, null, sublocality( target ) );

                        // null required collections should still union the required values
                        Add( value, country( target ), province(), locality(), sublocality() );
                        Add( value, country(), province( target ), locality(), sublocality() );
                        Add( value, country(), province(), locality( target ), sublocality() );
                        Add( value, country(), province(), locality(), sublocality( target ) );

                        // empty collections should still union the required values
                        Add( value, country( target ), province( empty ), locality( empty ), sublocality( empty ) );
                        Add( value, country( empty ), province( target ), locality( empty ), sublocality( empty ) );
                        Add( value, country( empty ), province( empty ), locality( target ), sublocality( empty ) );
                        Add( value, country( empty ), province( empty ), locality( empty ), sublocality( target ) );

                        // presence of other required fields should not affect target value
                        Add( value, country( target ), province( others ), locality( others ), sublocality( others ) );
                        Add( value, country( others ), province( target ), locality( others ), sublocality( others ) );
                        Add( value, country( others ), province( others ), locality( target ), sublocality( others ) );
                        Add( value, country( others ), province( others ), locality( others ), sublocality( target ) );
                    }
                }
            }
            
            public static RequiredFieldTheoryData ProvinceRequired_valueEmpty_Data = new RequiredFieldTheoryData( AddressField.Province, null, "", "\t" );

            [Theory]
            [MemberData(nameof(ProvinceRequired_valueEmpty_Data))]
            public void WhenProvinceRequired_valueEmpty_returns_failure( string value, ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality )
            {
                address.Province = value;
                var expected = RequiredElementsValidator.Failures[AddressField.Province];
                var actual = invoke( country, province, locality, sublocality );
                Assert.Contains( expected, actual );
            }

            public static RequiredFieldTheoryData ProvinceRequired_valuePresent_Data = new RequiredFieldTheoryData( AddressField.Province, random() );

            [Theory]
            [MemberData( nameof( ProvinceRequired_valuePresent_Data ) )]
            public void WhenProvinceRequired_valuePresent_returns_success( string value, ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality )
            {
                address.Province = value;
                var expected = RequiredElementsValidator.Failures[AddressField.Province];
                var actual = invoke( country, province, locality, sublocality );
                Assert.DoesNotContain( expected, actual );
            }

            public static RequiredFieldTheoryData LocalityRequired_valueEmpty_Data = new RequiredFieldTheoryData( AddressField.Locality, null, "", "\t" );

            [Theory]
            [MemberData( nameof( LocalityRequired_valueEmpty_Data ) )]
            public void WhenLocalityRequired_valueEmpty_returns_failure( string value, ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality )
            {
                address.Locality = value;
                var expected = RequiredElementsValidator.Failures[AddressField.Locality];
                var actual = invoke( country, province, locality, sublocality );
                Assert.Contains( expected, actual );
            }

            public static RequiredFieldTheoryData LocalityRequired_valuePresent_Data = new RequiredFieldTheoryData( AddressField.Locality, random() );

            [Theory]
            [MemberData( nameof( LocalityRequired_valuePresent_Data ) )]
            public void WhenLocalityRequired_valuePresent_returns_success( string value, ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality )
            {
                address.Locality = value;
                var expected = RequiredElementsValidator.Failures[AddressField.Locality];
                var actual = invoke( country, province, locality, sublocality );
                Assert.DoesNotContain( expected, actual );
            }

            public static RequiredFieldTheoryData SublocalityRequired_valueEmpty_Data = new RequiredFieldTheoryData( AddressField.Sublocality, null, "", "\t" );

            [Theory]
            [MemberData( nameof( SublocalityRequired_valueEmpty_Data ) )]
            public void WhenSublocalityRequired_valueEmpty_returns_failure( string value, ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality )
            {
                address.Sublocality = value;
                var expected = RequiredElementsValidator.Failures[AddressField.Sublocality];
                var actual = invoke( country, province, locality, sublocality );
                Assert.Contains( expected, actual );
            }

            public static RequiredFieldTheoryData SublocalityRequired_valuePresent_Data = new RequiredFieldTheoryData( AddressField.Locality, random() );

            [Theory]
            [MemberData( nameof( SublocalityRequired_valuePresent_Data ) )]
            public void WhenSubocalityRequired_valuePresent_returns_success( string value, ICountryMetadata country, IProvinceMetadata province, ILocalityMetadata locality, ISublocalityMetadata sublocality )
            {
                address.Sublocality = value;
                var expected = RequiredElementsValidator.Failures[AddressField.Sublocality];
                var actual = invoke( country, province, locality, sublocality );
                Assert.DoesNotContain( expected, actual );
            }
        }
    }
}