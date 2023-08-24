using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.Security.Interfaces;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.Security;
using Zambon.OrderManagement.WebApi.Services.Security;

namespace UnitTests.ServiceTests
{
    public class UsersServiceDefaultTest
    {
        private readonly IMapper mapper;

        public UsersServiceDefaultTest()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Zambon.OrderManagement.WebApi.Helpers.ValidationProblemEntityDetails).Assembly));
            configuration.CompileMappings();
            mapper = new Mapper(configuration);
        }


        [Fact]
        public async Task FindUserByIdAsync_Success_InvalidUserId()
        {
            // Arrange
            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            usersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(await Task.FromResult<Users>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.


            // Act
            var userService = new UsersServiceDefault(dbContextMock.Object, mapper, usersRepositoryMock.Object);

            var userResult = await userService.FindUserByIdAsync(1);


            // Assert
            Assert.Null(userResult);
        }

        [Fact]
        public async Task FindUserByIdAsync_Success_ValidUserId()
        {
            // Arrange
            var userId = 1L;
            var user = new Users { ID = userId, Username = "username1" };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();

            usersRepositoryMock.Setup(x => x.FindByIdAsync(It.Is<long>(x => x == userId)))
                .ReturnsAsync(user);


            // Act
            var userService = new UsersServiceDefault(dbContextMock.Object, mapper, usersRepositoryMock.Object);

            var userResult = await userService.FindUserByIdAsync(userId);


            // Assert
            usersRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<long>()), Times.Once);

            Assert.NotNull(userResult);
            Assert.Equal(user.ID, userResult.ID);
            Assert.Equal(user.Username, userResult.Username);
        }

        [Fact]
        public async Task InsertNewUserAsync_Success()
        {
            // Arrange
            var userModel = new UserInsertModel { Username = "username1" };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();


            // Act
            var userService = new UsersServiceDefault(dbContextMock.Object, mapper, usersRepositoryMock.Object);

            var userResult = await userService.InsertNewUserAsync(userModel);


            // Assert
            usersRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Users>()), Times.Once);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(userModel.Username, userResult.Username);
        }

        [Fact]
        public void ListUsers_Success()
        {
            // Arrange
            var users = new Users[]
            {
                new Users { Username = "username1" },
                new Users { Username = "username2" },
                new Users { Username = "username3" },
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();

            usersRepositoryMock.Setup(x => x.List(It.IsAny<IListParameters>(), It.IsAny<ICollection<Users>>()))
                .Returns(users.AsQueryable());


            // Act
            var userService = new UsersServiceDefault(dbContextMock.Object, mapper, usersRepositoryMock.Object);

            var usersList = userService.ListUsers(new ListParametersModel());


            // Assert
            usersRepositoryMock.Verify(x => x.List(It.IsAny<IListParameters>(), It.IsAny<ICollection<Users>>()), Times.Once);

            Assert.NotNull(usersList);
            Assert.Equal(users.Length, usersList.Count());
        }

        [Fact]
        public async Task RemoveUsersAsync_Success()
        {
            // Arrange
            var userIds = new long[] { 1, 2 };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var databaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();
            var usersRepositoryMock = new Mock<IUsersRepository>();

            dbContextMock.Setup(x => x.Database)
                .Returns(databaseMock.Object);

            databaseMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            usersRepositoryMock.Setup(x => x.RemoveAsync(It.IsAny<long>()))
                .Returns(Task.CompletedTask);


            // Act
            var userService = new UsersServiceDefault(dbContextMock.Object, mapper, usersRepositoryMock.Object);

            await userService.RemoveUsersAsync(userIds);


            // Assert
            databaseMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            usersRepositoryMock.Verify(x => x.RemoveAsync(It.IsAny<long>()), Times.Exactly(userIds.Length));
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateExistingUserAsync_Failure_EntityNotFound()
        {
            // Arrange
            var userModel = new UserUpdateModel { ID = 1, Username = "username1" };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            usersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(await Task.FromResult<Users>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.


            // Act
            var userService = new UsersServiceDefault(dbContextMock.Object, mapper, usersRepositoryMock.Object);

            var method = async () =>
            {
                _ = await userService.UpdateExistingUserAsync(userModel);
            };


            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(method);
        }

        [Fact]
        public async Task UpdateExistingUserAsync_Success_ValidUserModel()
        {
            // Arrange
            var user = new Users { ID = 1, Username = "username1" };
            var userModel = new UserUpdateModel { ID = 1, Username = "username2" };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();

            usersRepositoryMock.Setup(x => x.FindByIdAsync(It.Is<long>(x => x == user.ID)))
                .ReturnsAsync(user);


            // Act
            var userService = new UsersServiceDefault(dbContextMock.Object, mapper, usersRepositoryMock.Object);

            var userResult = await userService.UpdateExistingUserAsync(userModel);


            // Assert
            usersRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Users>()), Times.Once);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(userModel.Username, userResult.Username);
        }
    }
}