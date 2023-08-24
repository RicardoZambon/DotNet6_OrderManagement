using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Zambon.OrderManagement.Core;

namespace UnitTests
{
    public class SharedDatabaseFixture
    {
        private readonly SqliteConnection connection;

        public SharedDatabaseFixture()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
        }


        public void Dispose() => connection.Dispose();

        public AppDbContext CreateContext()
        {
            var result = new AppDbContext(
                new DbContextOptionsBuilder<AppDbContext>()
                .UseLazyLoadingProxies()
                .UseSqlite(connection)
                .Options
            );

            result.Database.EnsureCreated();
            return result;
        }
    }
}