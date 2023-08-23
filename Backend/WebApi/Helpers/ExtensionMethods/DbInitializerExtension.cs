using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.DataInitializers;

namespace Zambon.OrderManagement.WebApi.Helpers.ExtensionMethods
{
    public static class DbInitializerExtension
    {
        public static IApplicationBuilder SeedSqlServer(this IApplicationBuilder app, IEnumerable<string> pendingMigrations)
        {
            ArgumentNullException.ThrowIfNull(app, nameof(app));

            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();

            context.Database.EnsureCreated();

            try
            {
                foreach (var migration in pendingMigrations)
                {
                    BaseInitializer? initializer = null;

                    if (migration.EndsWith("_Initial"))
                    {
                        initializer = new InitialInitializer(context);
                    }

                    initializer?.Initialize();
                }
            }
            catch (Exception)
            {
                if (services.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
                {
                    throw;
                }
            }

            return app;
        }
    }
}