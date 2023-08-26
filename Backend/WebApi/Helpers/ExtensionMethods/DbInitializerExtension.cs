using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.DataInitializers;

namespace Zambon.OrderManagement.WebApi.Helpers.ExtensionMethods
{
    /// <summary>
    /// Auxiliary methods for the database initialization.
    /// </summary>
    public static class DbInitializerExtension
    {
        /// <summary>
        /// Seeds
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <param name="pendingMigrations">The list of the pending migrations to be applied in the database.</param>
        /// <returns>A reference to the <see cref="IApplicationBuilder"/> instance after the operation has completed.</returns>
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