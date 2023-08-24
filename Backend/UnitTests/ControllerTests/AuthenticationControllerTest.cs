using Microsoft.AspNetCore.Mvc;
using Moq;
using Zambon.OrderManagement.WebApi.Controllers.Security;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Services.Security.Interfaces;

namespace UnitTests.ControllerTests
{
    public class AuthenticationControllerTest
    {
        [Fact]
        public async Task RefreshToken_Failure_GeneralException()
        {
            // Arrange
            var refreshTokenModel = new RefreshTokenModel() { Username = "username1", RefreshToken = "refresh-token" };

            var authenticationServiceMock = new Mock<IAuthenticationService>();

            authenticationServiceMock.Setup(x => x.RefreshTokenAsync(It.IsAny<RefreshTokenModel>()))
                .ThrowsAsync(new Exception());


            // Act
            var authenticationController = new AuthenticationController(authenticationServiceMock.Object);

            var response = await authenticationController.RefreshToken(refreshTokenModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task RefreshToken_Failure_InvalidRefreshTokenException()
        {
            // Arrange
            var refreshTokenModel = new RefreshTokenModel() { Username = "username1", RefreshToken = "refresh-token" };

            var authenticationServiceMock = new Mock<IAuthenticationService>();

            authenticationServiceMock.Setup(x => x.RefreshTokenAsync(It.IsAny<RefreshTokenModel>()))
                .ThrowsAsync(new InvalidRefreshTokenException());


            // Act
            var authenticationController = new AuthenticationController(authenticationServiceMock.Object);

            var response = await authenticationController.RefreshToken(refreshTokenModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<UnauthorizedResult>(response);
            Assert.Equal(401, ((UnauthorizedResult)response).StatusCode);
        }

        [Fact]
        public async Task RefreshToken_Failure_RefreshTokenNotFoundException()
        {
            // Arrange
            var refreshTokenModel = new RefreshTokenModel() { Username = "username1", RefreshToken = "refresh-token" };

            var authenticationServiceMock = new Mock<IAuthenticationService>();

            authenticationServiceMock.Setup(x => x.RefreshTokenAsync(It.IsAny<RefreshTokenModel>()))
                .ThrowsAsync(new RefreshTokenNotFoundException());


            // Act
            var authenticationController = new AuthenticationController(authenticationServiceMock.Object);

            var response = await authenticationController.RefreshToken(refreshTokenModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<UnauthorizedResult>(response);
            Assert.Equal(401, ((UnauthorizedResult)response).StatusCode);
        }

        [Fact]
        public async Task RefreshToken_Success()
        {
            // Arrange
            var refreshTokenModel = new RefreshTokenModel() { Username = "username1", RefreshToken = "refresh-token" };

            var authenticationResponse = new AuthenticationResponseModel
            {
                Email = string.Empty,
                RefreshToken = "new-refresh-token",
                RefreshTokenExpiration = DateTime.UtcNow.AddMinutes(30),
                Token = "token",
                Username = refreshTokenModel.Username,
            };

            var authenticationServiceMock = new Mock<IAuthenticationService>();

            authenticationServiceMock.Setup(x => x.RefreshTokenAsync(It.IsAny<RefreshTokenModel>()))
                .ReturnsAsync(authenticationResponse);


            // Act
            var authenticationController = new AuthenticationController(authenticationServiceMock.Object);

            var response = await authenticationController.RefreshToken(refreshTokenModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task SignIn_Failure_GeneralException()
        {
            // Arrange
            var signInModel = new SignInModel() { Username = "username1", Password = "password" };

            var authenticationServiceMock = new Mock<IAuthenticationService>();

            authenticationServiceMock.Setup(x => x.SignInAsync(It.IsAny<SignInModel>()))
                .ThrowsAsync(new Exception());


            // Act
            var authenticationController = new AuthenticationController(authenticationServiceMock.Object);

            var response = await authenticationController.SignIn(signInModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task SignIn_Failure_InvalidAuthenticationException()
        {
            // Arrange
            var signInModel = new SignInModel() { Username = "username1", Password = "password" };

            var authenticationServiceMock = new Mock<IAuthenticationService>();

            authenticationServiceMock.Setup(x => x.SignInAsync(It.IsAny<SignInModel>()))
                .ThrowsAsync(new InvalidAuthenticationException());


            // Act
            var authenticationController = new AuthenticationController(authenticationServiceMock.Object);

            var response = await authenticationController.SignIn(signInModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<UnauthorizedObjectResult>(response);
            Assert.Equal(401, ((UnauthorizedObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task SignIn_Success()
        {
            // Arrange
            var signInModel = new SignInModel() { Username = "username1", Password = "password" };

            var authenticationResponse = new AuthenticationResponseModel
            {
                Email = string.Empty,
                RefreshToken = "refresh-token",
                RefreshTokenExpiration = DateTime.UtcNow.AddMinutes(30),
                Token = "token",
                Username = signInModel.Username,
            };

            var authenticationServiceMock = new Mock<IAuthenticationService>();

            authenticationServiceMock.Setup(x => x.SignInAsync(It.IsAny<SignInModel>()))
                .ReturnsAsync(authenticationResponse);


            // Act
            var authenticationController = new AuthenticationController(authenticationServiceMock.Object);

            var response = await authenticationController.SignIn(signInModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }
    }
}
