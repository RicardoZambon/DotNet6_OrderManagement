using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Moq;
using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Repositories.Security.Interfaces;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Services.Security;

namespace UnitTests.ServiceTests
{
    public class AuthenticationServiceDefaultTest
    {
        private readonly SharedDatabaseFixture databaseFixture;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        public AuthenticationServiceDefaultTest()
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Zambon.OrderManagement.WebApi.Helpers.ValidationProblemEntityDetails).Assembly));
            mapperConfig.CompileMappings();
            mapper = new Mapper(mapperConfig);

            databaseFixture = new SharedDatabaseFixture();

            configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    // Some random key here
                    { "JWT:Key", "2HERTGd+fgU*Lc3jW#5Bw7hp$hAF1" },
                    { "JWT:Issuer", "Zambon-OrderManagement-WebApi" },
                    { "JWT:Audience", "Zambon-OrderManagement-WebApi-Users" },
                    { "JWT:DurationInMinutes", "600" },
                    { "JWT:RefreshTokenExpiration", "10" },
               }).Build();
        }


        [Fact]
        public async Task RefreshTokenAsync_Failure_DeactivatedRefreshToken()
        {
            // Arrange
            var refreshTokenModel = new RefreshTokenModel
            {
                Username = "username1",
                RefreshToken = "Sgy2x3/5EPrL7QIWYO2twMEHV9MQV46YzBmt7g3tGxk=",
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();

            refreshTokensRepositoryMock.Setup(x => x.FindByUsernameAndTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new RefreshTokens
                {
                    Expiration = DateTime.UtcNow.AddMinutes(-10),
                    Token = refreshTokenModel.RefreshToken,
                    UserID = 1,
                    User = new Users { ID = 1, Username = refreshTokenModel.Username }
                });


            // Act
            var authenticationService = new AuthenticationServiceDefault(configuration, dbContextMock.Object, mapper, usersRepositoryMock.Object, refreshTokensRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                _ = await authenticationService.RefreshTokenAsync(refreshTokenModel);
            });


            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidRefreshTokenException>(exception);
        }

        [Fact]
        public async Task RefreshTokenAsync_Failure_InvalidModel()
        {
            // Arrange
            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();


            // Act
            var authenticationService = new AuthenticationServiceDefault(configuration, dbContextMock.Object, mapper, usersRepositoryMock.Object, refreshTokensRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _ = await authenticationService.RefreshTokenAsync(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            });


            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task RefreshTokenAsync_Failure_InvalidRefreshToken()
        {
            // Arrange
            var refreshTokenModel = new RefreshTokenModel
            {
                Username = "username1",
                RefreshToken = string.Empty,
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();


            // Act
            var authenticationService = new AuthenticationServiceDefault(configuration, dbContextMock.Object, mapper, usersRepositoryMock.Object, refreshTokensRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                _ = await authenticationService.RefreshTokenAsync(refreshTokenModel);
            });


            // Assert
            Assert.NotNull(exception);
            Assert.IsType<RefreshTokenNotFoundException>(exception);
        }

        [Fact]
        public async Task RefreshTokenAsync_Failure_InvalidUsername()
        {
            // Arrange
            var refreshTokenModel = new RefreshTokenModel
            {
                Username = string.Empty,
                RefreshToken = "Sgy2x3/5EPrL7QIWYO2twMEHV9MQV46YzBmt7g3tGxk=",
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();


            // Act
            var authenticationService = new AuthenticationServiceDefault(configuration, dbContextMock.Object, mapper, usersRepositoryMock.Object, refreshTokensRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                _ = await authenticationService.RefreshTokenAsync(refreshTokenModel);
            });


            // Assert
            Assert.NotNull(exception);
            Assert.IsType<RefreshTokenNotFoundException>(exception);
        }

        [Fact]
        public async Task RefreshTokenAsync_Failure_Rollback()
        {
            // Arrange
            var refreshTokenModel = new RefreshTokenModel
            {
                Username = "username1",
                RefreshToken = "Sgy2x3/5EPrL7QIWYO2twMEHV9MQV46YzBmt7g3tGxk=",
            };

            var refreshToken = new RefreshTokens
            {
                Expiration = DateTime.UtcNow.AddMinutes(30),
                Token = refreshTokenModel.RefreshToken,
                UserID = 1,
                User = new Users { ID = 1, Username = refreshTokenModel.Username }
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var databaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();

            dbContextMock.Setup(x => x.Database)
                .Returns(databaseMock.Object);

            databaseMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            transactionMock.Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            refreshTokensRepositoryMock.Setup(x => x.FindByUsernameAndTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(refreshToken);

            refreshTokensRepositoryMock.Setup(x => x.RevokeAsync(It.Is<RefreshTokens>(token => token == refreshToken)))
                .ThrowsAsync(new Exception());


            // Act
            var authenticationService = new AuthenticationServiceDefault(configuration, dbContextMock.Object, mapper, usersRepositoryMock.Object, refreshTokensRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                var _ = await authenticationService.RefreshTokenAsync(refreshTokenModel);
            });


            // Assert
            Assert.NotNull(exception);

            databaseMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            refreshTokensRepositoryMock.Verify(x => x.RevokeAsync(It.IsAny<RefreshTokens>()), Times.Once);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_Failure_UserNotFound()
        {
            // Arrange
            var refreshTokenModel = new RefreshTokenModel
            {
                Username = "username1",
                RefreshToken = "Sgy2x3/5EPrL7QIWYO2twMEHV9MQV46YzBmt7g3tGxk=",
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();

            refreshTokensRepositoryMock.Setup(x => x.FindByUsernameAndTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new RefreshTokens
                {
                    Expiration = DateTime.Now.AddMinutes(30),
                    Token = refreshTokenModel.RefreshToken,
                    UserID = 1,
                    User = null
                });


            // Act
            var authenticationService = new AuthenticationServiceDefault(configuration, dbContextMock.Object, mapper, usersRepositoryMock.Object, refreshTokensRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                _ = await authenticationService.RefreshTokenAsync(refreshTokenModel);
            });


            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidRefreshTokenException>(exception);
        }

        [Fact]
        public async Task RefreshTokenAsync_Success_ValidAuthenticationResponse()
        {
            // Arrange
            var refreshTokenModel = new RefreshTokenModel
            {
                Username = "username1",
                RefreshToken = "Sgy2x3/5EPrL7QIWYO2twMEHV9MQV46YzBmt7g3tGxk=",
            };

            var refreshToken = new RefreshTokens
            {
                Expiration = DateTime.UtcNow.AddMinutes(30),
                Token = refreshTokenModel.RefreshToken,
                UserID = 1,
                User = new Users { ID = 1, Username = refreshTokenModel.Username }
            };

            using var context = databaseFixture.CreateContext();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();

            refreshTokensRepositoryMock.Setup(x => x.FindByUsernameAndTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(refreshToken);

            refreshTokensRepositoryMock.Setup(x => x.RevokeAsync(It.Is<RefreshTokens>(token => token == refreshToken)))
                .Returns(Task.CompletedTask);

            refreshTokensRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<RefreshTokens>()))
                .Returns(Task.CompletedTask);


            // Act
            var authenticationService = new AuthenticationServiceDefault(configuration, context, mapper, usersRepositoryMock.Object, refreshTokensRepositoryMock.Object);

            var authenticationResponse = await authenticationService.RefreshTokenAsync(refreshTokenModel);


            // Assert
            Assert.NotNull(authenticationResponse);
            Assert.Equal(refreshTokenModel.Username, authenticationResponse.Username);
            Assert.NotEqual(refreshTokenModel.RefreshToken, authenticationResponse.RefreshToken);

            refreshTokensRepositoryMock.Verify(x => x.RevokeAsync(It.IsAny<RefreshTokens>()), Times.Once);
        }

        [Fact]
        public async Task SignInAsync_Failure_InvalidModel()
        {
            // Arrange
            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();


            // Act
            var authenticationService = new AuthenticationServiceDefault(configuration, dbContextMock.Object, mapper, usersRepositoryMock.Object, refreshTokensRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _ = await authenticationService.SignInAsync(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            });


            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task SignInAsync_Failure_InvalidPassword()
        {
            // Arrange
            var signInModel = new SignInModel
            {
                Password = string.Empty,
                Username = "username1",
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();

            usersRepositoryMock.Setup(x => x.FindByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((Users?)null);


            // Act
            var authenticationService = new AuthenticationServiceDefault(configuration, dbContextMock.Object, mapper, usersRepositoryMock.Object, refreshTokensRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                _ = await authenticationService.SignInAsync(signInModel);
            });


            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidAuthenticationException>(exception);
        }

        [Fact]
        public async Task SignInAsync_Failure_InvalidUsername()
        {
            // Arrange
            var signInModel = new SignInModel
            {
                Password = "password",
                Username = string.Empty,
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();

            usersRepositoryMock.Setup(x => x.FindByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((Users?)null);


            // Act
            var authenticationService = new AuthenticationServiceDefault(configuration, dbContextMock.Object, mapper, usersRepositoryMock.Object, refreshTokensRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                _ = await authenticationService.SignInAsync(signInModel);
            });


            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidAuthenticationException>(exception);
        }

        [Fact]
        public async Task SignInAsync_Failure_UserNotFound()
        {
            // Arrange
            var signInModel = new SignInModel
            {
                Password = "password",
                Username = "username1",
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();

            usersRepositoryMock.Setup(x => x.FindByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((Users?)null);


            // Act
            var authenticationService = new AuthenticationServiceDefault(configuration, dbContextMock.Object, mapper, usersRepositoryMock.Object, refreshTokensRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                _ = await authenticationService.SignInAsync(signInModel);
            });


            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidAuthenticationException>(exception);
        }

        [Fact]
        public async Task SignInAsync_Failure_UserWithoutPassword()
        {
            // Arrange
            var signInModel = new SignInModel
            {
                Password = "password",
                Username = "username1",
            };

            var user = new Users
            {
                ID = 1,
                Username = signInModel.Username,
                Password = string.Empty,
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();

            usersRepositoryMock.Setup(x => x.FindByUsernameAsync(It.Is<string>(username => username == user.Username)))
                .ReturnsAsync(user);


            // Act
            var authenticationService = new AuthenticationServiceDefault(configuration, dbContextMock.Object, mapper, usersRepositoryMock.Object, refreshTokensRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                _ = await authenticationService.SignInAsync(signInModel);
            });


            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidAuthenticationException>(exception);
        }

        [Fact]
        public async Task SignInAsync_Failure_UserWithWrongPassword()
        {
            // Arrange
            var signInModel = new SignInModel
            {
                Password = "password",
                Username = "username1",
            };

            var user = new Users
            {
                ID = 1,
                Username = signInModel.Username,
                Password = "other-password",
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();

            usersRepositoryMock.Setup(x => x.FindByUsernameAsync(It.Is<string>(username => username == user.Username)))
                .ReturnsAsync(user);


            // Act
            var authenticationService = new AuthenticationServiceDefault(configuration, dbContextMock.Object, mapper, usersRepositoryMock.Object, refreshTokensRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                _ = await authenticationService.SignInAsync(signInModel);
            });


            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidAuthenticationException>(exception);
        }

        [Fact]
        public async Task SignInAsync_Success_UserWithValidPassword()
        {
            // Arrange
            var signInModel = new SignInModel
            {
                Password = "password",
                Username = "username1",
            };

            var user = new Users
            {
                ID = 1,
                Username = signInModel.Username,
                Password = signInModel.Password,
                Email = "any@email.com",
            };

            using var context = databaseFixture.CreateContext();
            var usersRepositoryMock = new Mock<IUsersRepository>();
            var refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();

            usersRepositoryMock.Setup(x => x.FindByUsernameAsync(It.Is<string>(username => username == user.Username)))
                .ReturnsAsync(user);

            refreshTokensRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<RefreshTokens>()))
                .Returns(Task.CompletedTask);


            // Act
            var authenticationService = new AuthenticationServiceDefault(configuration, context, mapper, usersRepositoryMock.Object, refreshTokensRepositoryMock.Object);

            var authenticationRespondeModel = await authenticationService.SignInAsync(signInModel);
            


            // Assert
            Assert.NotNull(authenticationRespondeModel);
            Assert.Equal(signInModel.Username, authenticationRespondeModel.Username);
            Assert.Equal(user.Email, authenticationRespondeModel.Email);

            refreshTokensRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<RefreshTokens>()), Times.Once);
        }
    }
}