using Fidget.Commander;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fidget.Validation.Addresses.Validation.Commands
{
    public class ValidateAddressCommandTests
    {
        Mock<IValidationContextFactory> MockFactory = new Mock<IValidationContextFactory>();
        
        IValidationContextFactory factory => MockFactory?.Object;
        List<IAddressValidator> validators = new List<IAddressValidator>();

        ICommandHandler<ValidateAddressCommand, AddressValidationFailure[]> handler => new ValidateAddressCommand.Handler( factory, validators );

        public class Constructor : ValidateAddressCommandTests
        {
            [Fact]
            public void Requires_factory()
            {
                MockFactory = null;
                Assert.Throws<ArgumentNullException>( nameof(factory), ()=>handler );
            }

            [Fact]
            public void Requires_validators()
            {
                validators = null;
                Assert.Throws<ArgumentNullException>( nameof(validators), ()=>handler );
            }

            [Fact]
            public void IsRegistered()
            {
                var actual = DependencyInjection.Container.GetInstance<ICommandHandler<ValidateAddressCommand, AddressValidationFailure[]>>();
                Assert.IsType<ValidateAddressCommand.Handler>( actual );
            }
        }

        public class Handle : ValidateAddressCommandTests
        {
            CancellationToken cancellationToken = CancellationToken.None;
            ValidateAddressCommand command = new ValidateAddressCommand
            {
                Address = new AddressData { StreetAddress = "123 Anywhere st" },
            };

            Mock<IAddressValidator> MockValidator1 = new Mock<IAddressValidator>();
            Mock<IAddressValidator> MockValidator2 = new Mock<IAddressValidator>();

            async Task<AddressValidationFailure[]> invoke()
            {
                validators.Add( MockValidator1.Object );
                validators.Add( MockValidator2.Object );
                return await handler.Handle( command, cancellationToken );
            }

            public static IEnumerable<object[]> ConsolidationCases()
            {
                var empty = new AddressValidationFailure[0];
                var failures = new AddressValidationFailure[]
                {
                    new AddressValidationFailure { Field = AddressField.Country, Error = AddressFieldError.MissingRequiredField },
                    new AddressValidationFailure { Field = AddressField.Province, Error = AddressFieldError.UnkownValue },
                    new AddressValidationFailure { Field = AddressField.SortingCode, Error = AddressFieldError.UnexpectedField },
                    new AddressValidationFailure { Field = AddressField.PostalCode, Error = AddressFieldError.InvalidFormat },
                };

                // no failures
                yield return new object[] { empty, empty };

                // failures in one validator
                yield return new object[] { empty, failures };
                yield return new object[] { failures, empty };

                // split failures
                yield return new object[] { failures.Take(2).ToArray(), failures.Skip(2).ToArray() };
            }

            [Theory]
            [MemberData(nameof(ConsolidationCases))]
            public async Task Returns_consolidation_of_validators( AddressValidationFailure[] first, AddressValidationFailure[] second )
            {
                var expected = first
                    .Union( second )
                    .OrderBy( _=> _.Field )
                    .ThenBy( _=> _.Error )
                    .ToArray();

                var context = new ValidationContext { Address = command.Address };
                MockFactory.Setup( _ => _.Create( command.Address, cancellationToken ) ).ReturnsAsync( context );
                MockValidator1.Setup( _ => _.Validate( context ) ).Returns( first );
                MockValidator2.Setup( _ => _.Validate( context ) ).Returns( second );

                var actual = await invoke();
                Assert.Equal( expected, actual );
            }
        }
    }
}