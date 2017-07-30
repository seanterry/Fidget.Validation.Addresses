using Fidget.Commander.Dispatch;
using Fidget.Validation.Addresses.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fidget.Validation.Addresses.Metadata.Commands.Decorators
{
    /// <summary>
    /// Decorator that handles applying default values to a country.
    /// </summary>

    public class DefaultCountryValuesDecorator : ICommandDecorator<CountryMetadataQuery, CountryMetadata>
    {
        /// <summary>
        /// Remote address service client.
        /// </summary>
        
        readonly IServiceClient Client;

        /// <summary>
        /// Constructs a decorator that handles applying default values to a country.
        /// </summary>
        /// <param name="client">Remote address service client.</param>
        
        public DefaultCountryValuesDecorator( IServiceClient client )
        {
            Client = client ?? throw new ArgumentNullException( nameof(client) );
        }

        /// <summary>
        /// Executes the decorator.
        /// </summary>
        /// <param name="command">Command being decorated.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="continuation">Delegate for execution continuation.</param>
        
        public async Task<CountryMetadata> Execute( CountryMetadataQuery command, CancellationToken cancellationToken, CommandDelegate<CountryMetadataQuery, CountryMetadata> continuation )
        {
            var result = await continuation( command, cancellationToken );

            if ( result is CountryMetadata country )    
            if ( await Client.Query<CountryMetadata>( "data/ZZ" ) is CountryMetadata defaults )
            {
                result.Format = result.Format ?? defaults.Format;
                result.Required = result.Required ?? defaults.Required;
                result.Uppercase = result.Uppercase ?? defaults.Uppercase;
                result.StateType = result.StateType ?? defaults.StateType;
                result.LocalityType = result.LocalityType ?? defaults.LocalityType;
                result.SublocalityType = result.SublocalityType ?? defaults.SublocalityType;
                result.PostalCodeType = result.PostalCodeType ?? defaults.PostalCodeType;
            }
            
            return result;
        }
    }
}