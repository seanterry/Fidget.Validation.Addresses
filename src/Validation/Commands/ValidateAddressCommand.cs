using Fidget.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Validation.Commands
{
    /// <summary>
    /// Command for validating an address.
    /// </summary>

    public struct ValidateAddressCommand : ICommand<AddressValidationFailure[]>
    {
        /// <summary>
        /// Gets or sets the address to validate.
        /// </summary>
        
        public AddressData Address { get; set; }

        /// <summary>
        /// Handler for the command.
        /// </summary>
        
        public class Handler : ICommandHandler<ValidateAddressCommand, AddressValidationFailure[]>
        {
            /// <summary>
            /// Validation context factory.
            /// </summary>
            
            IValidationContextFactory Factory;

            /// <summary>
            /// Collection of address validators.
            /// </summary>
            
            IEnumerable<IAddressValidator> Validators;

            /// <summary>
            /// Constructs a handler for the command.
            /// </summary>
            /// <param name="factory">Validation context factory.</param>
            /// <param name="validators">Collection of address validators.</param>
            
            public Handler( IValidationContextFactory factory, IEnumerable<IAddressValidator> validators )
            {
                Factory = factory ?? throw new ArgumentNullException( nameof(factory) );
                Validators = validators ?? throw new ArgumentNullException( nameof(validators) );
            }

            /// <summary>
            /// Executes the command.
            /// </summary>
            
            public async Task<AddressValidationFailure[]> Handle( ValidateAddressCommand command, CancellationToken cancellationToken )
            {
                var context = await Factory.Create( command.Address, cancellationToken );
                var failures = Validators.SelectMany( _=> _.Validate( context ) );
                    
                return failures
                    .OrderBy( _=> _.Field )
                    .ThenBy( _=> _.Error )
                    .ToArray();
            }
        }
    }
}