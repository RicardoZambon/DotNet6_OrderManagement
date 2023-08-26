using Microsoft.EntityFrameworkCore;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Repositories.Security;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;

namespace UnitTests.RepositoryTests
{
    public class UsersRepositoryTest : IDisposable
    {
        private readonly SharedDatabaseFixture databaseFixture;

        public UsersRepositoryTest()
        {
            databaseFixture = new SharedDatabaseFixture();
        }

        public void Dispose()
        {
            databaseFixture.Dispose();
            GC.SuppressFinalize(this);
        }


        [Fact]
        public async Task AddAsync_Success_NewUser()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var user = new Users { Username = "username1" };


            // Act
            var usersRepository = new UsersRepository(context);

            await usersRepository.AddAsync(user);
            await context.SaveChangesAsync();


            // Assert
            var dbUser = await context.Set<Users>().FirstOrDefaultAsync();

            Assert.Equal(1, await context.Set<Users>().CountAsync());

            Assert.NotNull(dbUser);
            Assert.Equal(dbUser.Username, user.Username);
        }

        [Fact]
        public async Task FindByIdAsync_Success_InvalidUserId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var user = new Users { Username = "username1" };

            context.Set<Users>().Add(user);
            await context.SaveChangesAsync();


            // Act
            var usersRepository = new UsersRepository(context);

            var dbUser = await usersRepository.FindByIdAsync(2);
            await context.SaveChangesAsync();


            // Assert
            Assert.Null(dbUser);
        }

        [Fact]
        public async Task FindByIdAsync_Success_ValidUserId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var user = new Users { ID = 1, Username = "username1" };

            context.Set<Users>().Add(user);
            await context.SaveChangesAsync();


            // Act
            var usersRepository = new UsersRepository(context);

            var dbUser = await usersRepository.FindByIdAsync(1);
            await context.SaveChangesAsync();


            // Assert
            Assert.NotNull(dbUser);
            Assert.Equal(dbUser.Username, user.Username);
        }

        [Fact]
        public async Task FindByIds_Success_InvalidUserIds()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var users = new Users[]
            {
                new Users { ID = 1, Username = "username1" },
                new Users { ID = 2, Username = "username2" },
                new Users { ID = 3, Username = "username3" },
            };

            context.Set<Users>().AddRange(users);
            await context.SaveChangesAsync();

            var userIds = new long[] { 4 };


            // Act
            var usersRepository = new UsersRepository(context);

            var dbUsers = usersRepository.FindByIds(userIds);
            await context.SaveChangesAsync();


            // Assert
            Assert.NotNull(dbUsers);
            Assert.False(await dbUsers.AnyAsync());
            Assert.Equal(0, await dbUsers.CountAsync());
        }

        [Fact]
        public async Task FindByIds_Success_ValidUserIds()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var users = new Users[]
            {
                new Users { ID = 1, Username = "username1" },
                new Users { ID = 2, Username = "username2" },
                new Users { ID = 3, Username = "username3" },
            };

            context.Set<Users>().AddRange(users);
            await context.SaveChangesAsync();

            var userIds = new long[] { 1, 2 };


            // Act
            var usersRepository = new UsersRepository(context);

            var dbUsers = usersRepository.FindByIds(userIds);
            await context.SaveChangesAsync();


            // Assert
            Assert.NotNull(dbUsers);
            Assert.True(await dbUsers.AnyAsync());
            Assert.Equal(users.Count(x => userIds.Contains(x.ID)), await dbUsers.CountAsync());
        }

        [Fact]
        public async Task FindByUsernameAsync_Success_InvalidUsername()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var username = "username0";

            var users = new Users[]
            {
                new Users { ID = 1, Username = "username1" },
                new Users { ID = 2, Username = "username2" },
                new Users { ID = 3, Username = "username3" },
            };

            context.Set<Users>().AddRange(users);
            await context.SaveChangesAsync();


            // Act
            var usersRepository = new UsersRepository(context);

            var dbUser = await usersRepository.FindByUsernameAsync(username);
            await context.SaveChangesAsync();


            // Assert
            Assert.Null(dbUser);
        }

        [Fact]
        public async Task FindByUsernameAsync_Success_ValidUsername()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var username = "username1";

            var users = new Users[]
            {
                new Users { ID = 1, Username = username },
                new Users { ID = 2, Username = "username2" },
                new Users { ID = 3, Username = "username3" },
            };

            context.Set<Users>().AddRange(users);
            await context.SaveChangesAsync();


            // Act
            var usersRepository = new UsersRepository(context);

            var dbUser = await usersRepository.FindByUsernameAsync(username);
            await context.SaveChangesAsync();


            // Assert
            Assert.NotNull(dbUser);
            Assert.Equal(username, dbUser.Username);
        }

        [Fact]
        public async Task List_Fail_MissingParameters()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var users = new Users[]
            {
                new Users { Username = "username1" },
                new Users { Username = "username2" },
                new Users { Username = "username3" },
            };

            context.Set<Users>().AddRange(users);
            await context.SaveChangesAsync();


            // Act
            var usersRepository = new UsersRepository(context);

            var method = () =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _ = usersRepository.List(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            };


            // Assert
            Assert.Throws<ArgumentNullException>(method);
        }

        [Fact]
        public async Task List_Success_WithEndRow()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var users = new List<Users>();
            for (var i = 0; i < 10; i++)
            {
                users.Add(new Users { Username = $"User {i}" });
            }

            context.Set<Users>().AddRange(users);
            await context.SaveChangesAsync();


            // Act
            var usersRepository = new UsersRepository(context);

            var usersList = usersRepository.List(new ListParametersModel { EndRow = 5, StartRow = 0 });


            // Assert
            Assert.NotNull(usersList);

            Assert.Equal(users.Skip(0).Take(5).Count(), await usersList.CountAsync());
        }

        [Fact]
        public async Task List_Success_WithStartAndEndRow()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var users = new List<Users>();
            for (var i = 0; i < 10; i++)
            {
                users.Add(new Users { Username = $"User {i}" });
            }

            context.Set<Users>().AddRange(users);
            await context.SaveChangesAsync();


            // Act
            var usersRepository = new UsersRepository(context);

            var usersList = usersRepository.List(new ListParametersModel { EndRow = 5, StartRow = 2 });


            // Assert
            Assert.NotNull(usersList);

            Assert.Equal(users.Skip(2).Take(5).Count(), await usersList.CountAsync());
        }

        [Fact]
        public async Task List_Success_WithFilterByUsername()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var users = new Users[]
            {
                new Users { Username = "username1" },
                new Users { Username = "John.Doe2" },
                new Users { Username = "Company3" },
            };

            context.Set<Users>().AddRange(users);
            await context.SaveChangesAsync();


            // Act
            var usersRepository = new UsersRepository(context);

            var parameters = new ListParametersModel();
            parameters.Filters.Add(nameof(Users.Username), "username");

            var usersList = usersRepository.List(parameters);


            // Assert
            Assert.NotNull(usersList);

            Assert.Equal(users.Count(x => x.Username != null && x.Username.Contains("username")), await usersList.CountAsync());
        }

        [Fact]
        public async Task List_Success_WithoutFilters()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var users = new Users[]
            {
                new Users { Username = "username1" },
                new Users { Username = "username2" },
                new Users { Username = "username3" },
            };

            context.Set<Users>().AddRange(users);
            await context.SaveChangesAsync();


            // Act
            var usersRepository = new UsersRepository(context);

            var usersList = usersRepository.List(new ListParametersModel());


            // Assert
            Assert.NotNull(usersList);

            Assert.Equal(users.Length, await usersList.CountAsync());
        }

        [Fact]
        public async Task RemoveAsync_Fail_InvalidUserId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var userId = 2;
            var user = new Users { ID = 1, Username = "username1" };

            context.Set<Users>().Add(user);
            await context.SaveChangesAsync();


            // Act
            var usersRepository = new UsersRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await usersRepository.RemoveAsync(userId);
            });


            // Assert
            Assert.NotNull(ex);

            Assert.Equal(new EntityNotFoundException(nameof(Users), userId).Message, ex.Message);
        }

        [Fact]
        public async Task RemoveAsync_Success_ValidUserId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var userId = 1;
            var user = new Users { ID = userId, Username = "username1" };

            context.Set<Users>().Add(user);
            await context.SaveChangesAsync();


            // Act
            var usersRepository = new UsersRepository(context);

            await usersRepository.RemoveAsync(userId);
            await context.SaveChangesAsync();


            // Assert
            var dbUser = await usersRepository.FindByIdAsync(userId);

            Assert.Equal(0, await context.Set<Users>().CountAsync());
            Assert.NotNull(dbUser);
            Assert.True(dbUser.IsDeleted);
        }

        [Fact]
        public async Task UpdateAsync_Fail_NewUser()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange


            // Act
            var usersRepository = new UsersRepository(context);

            await usersRepository.UpdateAsync(new Users { ID = 1, Username = "username1" });

            var method = async () =>
            {
                await context.SaveChangesAsync();
            };


            // Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(method);
        }

        [Fact]
        public async Task UpdateAsync_Success_ExistingUser()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var userId = 1;

            context.Set<Users>().Add(new Users { ID = userId, Username = "username1" });
            await context.SaveChangesAsync();


            // Act
            var usersRepository = new UsersRepository(context);

            var user = await usersRepository.FindByIdAsync(userId);
            user!.Username = "new.username1";

            await usersRepository.UpdateAsync(user);
            await context.SaveChangesAsync();


            // Assert
            var dbUser = await usersRepository.FindByIdAsync(userId);

            Assert.NotNull(dbUser);
            Assert.Equal(dbUser.Username, user.Username);
        }

        [Fact]
        public async Task ValidateAsync_Fail_InvalidUser_UsernameDuplicated()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            context.Set<Users>().Add(new Users { ID = 1, Username = "username1" });
            await context.SaveChangesAsync();


            // Act
            var usersRepository = new UsersRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await usersRepository.ValidateAsync(new Users { Username = "username1" });
            });


            // Assert
            Assert.NotNull(ex);

            Assert.IsType<EntityValidationFailureException>(ex);

            Assert.Equal($"Entity '{nameof(Users)} ({0})' has validation problems.", ex.Message);

            Assert.True(((EntityValidationFailureException)ex).ValidationResult.Errors.ContainsKey(nameof(Users.Username)));

            Assert.Contains("exists", ((EntityValidationFailureException)ex).ValidationResult.Errors[nameof(Users.Username)]);
        }

        [Fact]
        public async Task ValidateAsync_Fail_InvalidUser_UsernameInvalid()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange


            // Act
            var usersRepository = new UsersRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await usersRepository.ValidateAsync(new Users { Username = string.Empty });
            });


            // Assert
            Assert.NotNull(ex);

            Assert.IsType<EntityValidationFailureException>(ex);

            Assert.Equal($"Entity '{nameof(Users)} ({0})' has validation problems.", ex.Message);

            Assert.True(((EntityValidationFailureException)ex).ValidationResult.Errors.ContainsKey(nameof(Users.Username)));

            Assert.Contains("required", ((EntityValidationFailureException)ex).ValidationResult.Errors[nameof(Users.Username)]);
        }

        [Fact]
        public async Task ValidateAsync_Success_ValidUser()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange


            // Act
            var usersRepository = new UsersRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await usersRepository.ValidateAsync(new Users { Username = "username1" });
            });


            // Assert
            Assert.Null(ex);
        }
    }
}