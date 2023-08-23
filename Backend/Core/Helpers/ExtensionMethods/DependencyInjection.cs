using Microsoft.Extensions.DependencyInjection;
using Zambon.OrderManagement.Core.Repositories.General;
using Zambon.OrderManagement.Core.Repositories.General.Interfaces;
using Zambon.OrderManagement.Core.Repositories.Security;
using Zambon.OrderManagement.Core.Repositories.Security.Interfaces;
using Zambon.OrderManagement.Core.Repositories.Stock;
using Zambon.OrderManagement.Core.Repositories.Stock.Interfaces;

namespace Zambon.OrderManagement.Core.Helpers.ExtensionMethods
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                // General
                .AddScoped<ICustomersRepository, CustomersRepository>()

                // Security
                .AddScoped<IRefreshTokensRepository, RefreshTokensRepository>()
                .AddScoped<IUsersRepository, UsersRepository>()

                // Stock
                .AddScoped<IOrdersProductsRepository, OrdersProductsRepository>()
                .AddScoped<IOrdersRepository, OrdersRepository>()
                .AddScoped<IProductsRepository, ProductsRepository>()
                ;
        }
    }
}