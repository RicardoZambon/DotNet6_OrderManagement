using Zambon.OrderManagement.Core.Services.Interfaces;
using Zambon.OrderManagement.WebApi.Services.General;
using Zambon.OrderManagement.WebApi.Services.General.Interfaces;
using Zambon.OrderManagement.WebApi.Services.Security;
using Zambon.OrderManagement.WebApi.Services.Security.Interfaces;
using Zambon.OrderManagement.WebApi.Services.Stock;
using Zambon.OrderManagement.WebApi.Services.Stock.Interfaces;

namespace Zambon.OrderManagement.WebApi.Helpers.ExtensionMethods
{
    /// <summary>
    /// Auxiliary methods for the dependency injection of the WebAPI services.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Add required services for the WebAPI.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <returns>A reference to the <see cref="IServiceCollection"/> instance after the operation has completed.</returns>
        public static IServiceCollection AddWebAPIServices(this IServiceCollection services)
        {
            return services
                // General
                .AddScoped<ICustomersService, CustomersServiceDefault>()

                // Security
                .AddScoped<ICurrentUserProvider, CurrentUserProviderDefault>()
                .AddScoped<IAuthenticationService, AuthenticationServiceDefault>()
                .AddScoped<IUsersService, UsersServiceDefault>()

                // Stock
                .AddScoped<IOrdersProductsService, OrdersProductsServiceDefault>()
                .AddScoped<IOrdersService, OrdersServiceDefault>()
                .AddScoped<IProductsService, ProductsServiceDefault>()
                ;
        }
    }
}