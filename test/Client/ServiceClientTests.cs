using Fidget.Validation.Addresses.Client.Decorators;
using Xunit;

namespace Fidget.Validation.Addresses.Client
{
    public class ServiceClientTests
    {
        public class Registration
        {
            [Fact]
            public void IsRegistered()
            {
                // ensure service client is decorated with the correct layering
                var actual = DependencyInjection.Container.GetInstance<IServiceClient>();
                
                var copying = Assert.IsType<CopyingDecorator>( actual );
                var nullifying = Assert.IsType<NullifyingDecorator>( copying.Client );
                var caching = Assert.IsType<CachingDecorator>( nullifying.Client );
                var client = Assert.IsType<ServiceClient>( caching.Client );
            }
        }
    }
}