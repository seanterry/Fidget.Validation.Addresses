using Fidget.Validation.Addresses.Service.Metadata;
using Fidget.Validation.Addresses.Service.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Fidget.Validation.Addresses.Validation
{
    public class ValidationContextTests
    {
        IGlobalMetadata global = new GlobalMetadata();
        CountryMetadata country = new CountryMetadata();
        IProvinceMetadata province = new ProvinceMetadata();
        ILocalityMetadata locality = new LocalityMetadata();
        ISublocalityMetadata sublocality = new SublocalityMetadata();

        IValidationContext instance => new ValidationContext( global, country, province, locality, sublocality );

        public class Constructor : ValidationContextTests
        {
            [Fact]
            public void Requires_global()
            {
                global = null;
                Assert.Throws<ArgumentNullException>( nameof(global), ()=>instance );
            }

            [Fact]
            public void DoesNotRequire_optionalParameters()
            {
                country = null;
                province = null;
                locality = null;
                sublocality = null;
                var actual = instance;
                Assert.IsType<ValidationContext>( actual );
            }
        }

        public class GetRequiredFields : ValidationContextTests
        {
            readonly ITestOutputHelper Output;
            public GetRequiredFields( ITestOutputHelper output ) => Output = output;

            /// <summary>
            /// Collection of all requirable fields (excluded country - it is a special case).
            /// </summary>
            
            static readonly AddressField[] RequireableFields = Enum.GetValues( typeof( AddressField ) ).OfType<AddressField>().Where( _ => _ != AddressField.Country ).ToArray();

            /// <summary>
            /// Returns a robust set of instances for test cases.
            /// </summary>
            /// <param name="field">Field for which to create test cases.</param>
            /// <param name="criteria">Test case matching criteria.</param>
            
            static IEnumerable<ValidationContext> getTestCases( AddressField field, Func<ValidationContext,bool> criteria )
            {
                var global = new GlobalMetadata();
                
                IEnumerable<T> metadata<T>() where T : RegionalMetadata, new() => new T[]
                {
                    null,                                                                       // metadata not supplied
                    new T { Required = null },                                                  // required fields are null
                    new T { Required = Enumerable.Empty<AddressField>() },                      // no fields required
                    new T { Required = RequireableFields.Where( _=> _ != field ).ToArray() },   // all fields required
                    new T { Required = RequireableFields },                                     // specific field not required
                    new T { Required = new AddressField[] { field } },                          // specific field is required
                };

                return (
                    from country in metadata<CountryMetadata>()
                    from province in metadata<ProvinceMetadata>()
                    from locality in metadata<LocalityMetadata>()
                    from sublocality in metadata<SublocalityMetadata>()
                    select new ValidationContext( global, country, province, locality, sublocality ) )
                    .Where( _=> criteria( _ ) )
                    .ToArray();
            }

            /// <summary>
            /// Country should always be returned as a required field.
            /// </summary>

            [Fact]
            public void InAllCases_returns_Country()
            {
                var cases = 0;

                foreach ( var field in RequireableFields )
                {
                    var contexts = getTestCases( field, _=> true );

                    foreach ( var context in contexts )
                    {
                        cases++;
                        var actual = context.GetRequiredFields();
                        Assert.Contains( AddressField.Country, actual );
                    }
                }

                Output.WriteLine( $"{cases} test cases" );
            }

            /// <summary>
            /// If a field is required by any region, it should be returned in the collection.
            /// </summary>

            [Fact]
            public void WhenFieldRequiredByAnyRegion_returns_field()
            {
                var cases = 0;

                foreach ( var field in RequireableFields )
                {
                    // returns whether the field is required by any of the given regions
                    bool isRequired( params IRegionalMetadata[] regions ) => regions.Any( _ => _?.Required?.Contains( field ) == true );
                    bool criteria( ValidationContext context ) => isRequired( context.Country, context.Province, context.Locality, context.Sublocality );

                    var contexts = getTestCases( field, criteria );
                        
                    foreach ( var context in contexts )
                    {
                        cases++;
                        var actual = context.GetRequiredFields();
                        Assert.Contains( field, actual );
                    }
                }

                Output.WriteLine( $"{cases} test cases" );
            }

            /// <summary>
            /// When a field is not required by any region, it should not be in the result.
            /// </summary>
            
            [Fact]
            public void WhenFieldNotRequiredByAnyRegion_doesNotReturnField()
            {
                var cases = 0;

                foreach ( var field in RequireableFields )
                {
                    // returns whether the field is required by any of the given regions
                    bool isRequired( params IRegionalMetadata[] regions ) => regions.Any( _ => _?.Required?.Contains( field ) == true );
                    bool criteria( ValidationContext context ) => !isRequired( context.Country, context.Province, context.Locality, context.Sublocality );

                    var contexts = getTestCases( field, criteria );

                    foreach ( var context in contexts )
                    {
                        cases++;
                        var actual = context.GetRequiredFields();
                        Assert.DoesNotContain( field, actual );
                    }
                }

                Output.WriteLine( $"{cases} test cases" );
            }
        }

        public class GetAllowedFields : ValidationContextTests
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("\t")]
            public void Country_alwaysAllowed( string format )
            {
                country.Format = format;
                var actual = instance.GetAllowedFields();
                Assert.Contains( AddressField.Country, actual );
            }

            static readonly IEnumerable<AddressField> all = Enum.GetValues( typeof( AddressField ) )
                .OfType<AddressField>()
                .ToArray();

            // excludes country since it is a special case
            public static IEnumerable<object[]> FieldCases() => all
                .Where( _=> _ != AddressField.Country )
                .Select( field => new object[] { field, $"%{(char)field}" } );
            
            [Theory]
            [MemberData(nameof(FieldCases))]
            public void WhenFieldInFormat_returnsAllowed( AddressField field, string format )
            {
                country.Format = format;
                var actual = instance.GetAllowedFields();
                Assert.Contains( field, actual );
            }

            [Theory]
            [MemberData(nameof(FieldCases))]
            public void WhenFieldNotInFormat_returnsNotAllowed( AddressField field, string format )
            {
                var others = all.Where( _=> _ != field ).ToArray();
                country.Format = string.Concat( others.Select( _=> $"%{(char)_}" ) );
                var actual = instance.GetAllowedFields();
                Assert.DoesNotContain( field, actual );
                Assert.Equal( others, actual );
            }
        }
    }
}