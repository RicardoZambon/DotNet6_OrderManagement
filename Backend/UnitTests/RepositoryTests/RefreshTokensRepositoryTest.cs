using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Repositories.Security;

namespace UnitTests.RepositoryTests
{
    public class RefreshTokensRepositoryTest : IDisposable
    {
        private readonly SharedDatabaseFixture databaseFixture;

        public RefreshTokensRepositoryTest()
        {
            databaseFixture = new SharedDatabaseFixture();
        }

        public void Dispose()
        {
            databaseFixture.Dispose();
            GC.SuppressFinalize(this);
        }


        [Fact]
        public async Task FindByIdAsync_Success_InvalidToken()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var username = "username1";

            await context.Set<Users>().AddAsync(new Users { ID = 1, Username = username });
            await context.SaveChangesAsync();

            var refreshToken = new RefreshTokens { UserID = 1, Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1aWQiOiI2ODciLCJuYW1laWQiOiJaYW1ib24tSlVOIiwibmJmIjoxNjkyODAxMjU5LCJleHAiOjE2OTI4MzcyNTksImlhdCI6MTY5MjgwMTI1OSwiaXNzIjoiSW5zdGl0dXRvTml0ZW4tU3lzdGVtLVdlYkFwaSIsImF1ZCI6Ikluc3RpdHV0b05pdGVuLVN5c3RlbS1XZWJBcGktVXNlcnMifQ.Fa0vWgUDsCDrhmCykO7-9dKr21BQfNSkg0jIn6y2SZA" };

            context.Set<RefreshTokens>().Add(refreshToken);
            await context.SaveChangesAsync();


            // Act
            var refreshTokensRepository = new RefreshTokensRepository(context);

            var dbRefreshToken = await refreshTokensRepository.FindByUsernameAndTokenAsync(username, "invalid-token");


            // Assert
            Assert.Null(dbRefreshToken);
        }

        [Fact]
        public async Task FindByIdAsync_Success_InvalidUsername()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1aWQiOiI2ODciLCJuYW1laWQiOiJaYW1ib24tSlVOIiwibmJmIjoxNjkyODAxMjU5LCJleHAiOjE2OTI4MzcyNTksImlhdCI6MTY5MjgwMTI1OSwiaXNzIjoiSW5zdGl0dXRvTml0ZW4tU3lzdGVtLVdlYkFwaSIsImF1ZCI6Ikluc3RpdHV0b05pdGVuLVN5c3RlbS1XZWJBcGktVXNlcnMifQ.Fa0vWgUDsCDrhmCykO7-9dKr21BQfNSkg0jIn6y2SZA";

            await context.Set<Users>().AddAsync(new Users { ID = 1, Username = "username1" });
            await context.SaveChangesAsync();

            var refreshToken = new RefreshTokens { UserID = 1, Token = token };

            context.Set<RefreshTokens>().Add(refreshToken);
            await context.SaveChangesAsync();


            // Act
            var refreshTokensRepository = new RefreshTokensRepository(context);

            var dbRefreshToken = await refreshTokensRepository.FindByUsernameAndTokenAsync("username", token);


            // Assert
            Assert.Null(dbRefreshToken);
        }

        [Fact]
        public async Task FindByIdAsync_Success_ValidUsernameAndToken()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var username = "username1";
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1aWQiOiI2ODciLCJuYW1laWQiOiJaYW1ib24tSlVOIiwibmJmIjoxNjkyODAxMjU5LCJleHAiOjE2OTI4MzcyNTksImlhdCI6MTY5MjgwMTI1OSwiaXNzIjoiSW5zdGl0dXRvTml0ZW4tU3lzdGVtLVdlYkFwaSIsImF1ZCI6Ikluc3RpdHV0b05pdGVuLVN5c3RlbS1XZWJBcGktVXNlcnMifQ.Fa0vWgUDsCDrhmCykO7-9dKr21BQfNSkg0jIn6y2SZA";

            await context.Set<Users>().AddAsync(new Users { ID = 1, Username = username });
            await context.SaveChangesAsync();

            var refreshToken = new RefreshTokens { UserID = 1, Token = token };

            context.Set<RefreshTokens>().Add(refreshToken);
            await context.SaveChangesAsync();


            // Act
            var refreshTokensRepository = new RefreshTokensRepository(context);

            var dbRefreshToken = await refreshTokensRepository.FindByUsernameAndTokenAsync(username, token);


            // Assert
            Assert.NotNull(dbRefreshToken);
            Assert.Equal(dbRefreshToken.UserID, refreshToken.UserID);
            Assert.Equal(dbRefreshToken.Token, refreshToken.Token);
        }

        [Fact]
        public async Task InsertAsync_Fail_InvalidRefreshToken()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Users>().AddAsync(new Users { ID = 1, Username = "username1" });
            await context.SaveChangesAsync();


            // Act
            var refreshTokensRepository = new RefreshTokensRepository(context);

            var method = async () =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                await refreshTokensRepository.InsertAsync(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            };


            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(method);
        }

        [Fact]
        public async Task InsertAsync_Success_ValidRefreshToken()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var username = "username1";
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1aWQiOiI2ODciLCJuYW1laWQiOiJaYW1ib24tSlVOIiwibmJmIjoxNjkyODAxMjU5LCJleHAiOjE2OTI4MzcyNTksImlhdCI6MTY5MjgwMTI1OSwiaXNzIjoiSW5zdGl0dXRvTml0ZW4tU3lzdGVtLVdlYkFwaSIsImF1ZCI6Ikluc3RpdHV0b05pdGVuLVN5c3RlbS1XZWJBcGktVXNlcnMifQ.Fa0vWgUDsCDrhmCykO7-9dKr21BQfNSkg0jIn6y2SZA";

            await context.Set<Users>().AddAsync(new Users { ID = 1, Username = username });
            await context.SaveChangesAsync();

            var refreshToken = new RefreshTokens { UserID = 1, Token = token };


            // Act
            var refreshTokensRepository = new RefreshTokensRepository(context);

            await refreshTokensRepository.InsertAsync(refreshToken);
            await context.SaveChangesAsync();


            // Assert
            var dbRefreshToken = await refreshTokensRepository.FindByUsernameAndTokenAsync(username, token);

            Assert.NotNull(dbRefreshToken);
            Assert.Equal(refreshToken.UserID, dbRefreshToken.UserID);
            Assert.Equal(refreshToken.Token, dbRefreshToken.Token);
        }

        [Fact]
        public async Task RevokeAsync_Fail_InvalidRefreshToken()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Users>().AddAsync(new Users { ID = 1, Username = "username1" });
            await context.SaveChangesAsync();

            var refreshToken = new RefreshTokens { UserID = 1, Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1aWQiOiI2ODciLCJuYW1laWQiOiJaYW1ib24tSlVOIiwibmJmIjoxNjkyODAxMjU5LCJleHAiOjE2OTI4MzcyNTksImlhdCI6MTY5MjgwMTI1OSwiaXNzIjoiSW5zdGl0dXRvTml0ZW4tU3lzdGVtLVdlYkFwaSIsImF1ZCI6Ikluc3RpdHV0b05pdGVuLVN5c3RlbS1XZWJBcGktVXNlcnMifQ.Fa0vWgUDsCDrhmCykO7-9dKr21BQfNSkg0jIn6y2SZA" };

            context.Set<RefreshTokens>().Add(refreshToken);
            await context.SaveChangesAsync();


            // Act
            var refreshTokensRepository = new RefreshTokensRepository(context);

            var method = async () =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                await refreshTokensRepository.RevokeAsync(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            };


            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(method);
        }

        [Fact]
        public async Task RevokeAsync_Success_ValidRefreshToken()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var username = "username1";
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1aWQiOiI2ODciLCJuYW1laWQiOiJaYW1ib24tSlVOIiwibmJmIjoxNjkyODAxMjU5LCJleHAiOjE2OTI4MzcyNTksImlhdCI6MTY5MjgwMTI1OSwiaXNzIjoiSW5zdGl0dXRvTml0ZW4tU3lzdGVtLVdlYkFwaSIsImF1ZCI6Ikluc3RpdHV0b05pdGVuLVN5c3RlbS1XZWJBcGktVXNlcnMifQ.Fa0vWgUDsCDrhmCykO7-9dKr21BQfNSkg0jIn6y2SZA";

            await context.Set<Users>().AddAsync(new Users { ID = 1, Username = username });
            await context.SaveChangesAsync();

            var refreshToken = new RefreshTokens { UserID = 1, Token = token };

            context.Set<RefreshTokens>().Add(refreshToken);
            await context.SaveChangesAsync();


            // Act
            var refreshTokensRepository = new RefreshTokensRepository(context);

            await refreshTokensRepository.RevokeAsync(refreshToken);
            await context.SaveChangesAsync();


            // Assert
            var dbRefreshToken = await refreshTokensRepository.FindByUsernameAndTokenAsync(username, token);

            Assert.NotNull(dbRefreshToken);
            Assert.NotNull(dbRefreshToken.RevokedOn);
        }
    }
}