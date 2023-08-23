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
    /// Métodos auxiliares para injeção de dependência dos serviços de WebAPI.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adiciona serviços necessários ao WebAPI.
        /// </summary>
        /// <param name="services">O <see cref="IServiceCollection" /> para adicionar os serviços.</param>
        /// <returns>O <see cref="IServiceCollection"/>, de forma que permite que sejam feitas chamadas em cadeia.</returns>
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