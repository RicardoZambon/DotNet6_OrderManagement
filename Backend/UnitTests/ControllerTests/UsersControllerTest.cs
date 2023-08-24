using Microsoft.AspNetCore.Mvc;
using Moq;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Helpers.Validations;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.WebApi.Controllers.Security;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.Security;
using Zambon.OrderManagement.WebApi.Services.Security.Interfaces;

namespace UnitTests.ControllerTests
{
    public class UserControllerTest
    {
        [Fact]
        public async Task Add_Failure_GeneralException()
        {
            // Arrange
            var userModel = new UserInsertModel() { Username = "username1" };

            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.InsertNewUserAsync(It.IsAny<UserInsertModel>()))
                .ThrowsAsync(new Exception());


            // Act
            var usersController = new UsersController(usersServiceMock.Object);

            var response = await usersController.Add(userModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Add_Failure_ValidationFailureException()
        {
            // Arrange
            var userModel = new UserInsertModel() { Username = string.Empty };

            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.InsertNewUserAsync(It.IsAny<UserInsertModel>()))
                .ThrowsAsync(new EntityValidationFailureException(nameof(Users), 1L, new ValidationResult()));


            // Act
            var usersController = new UsersController(usersServiceMock.Object);

            var response = await usersController.Add(userModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<BadRequestObjectResult>(response);
            Assert.Equal(400, ((BadRequestObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Add_Success()
        {
            // Arrange
            var userModel = new UserInsertModel() { Username = "username1" };

            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.InsertNewUserAsync(It.IsAny<UserInsertModel>()))
                .ReturnsAsync(new UserUpdateModel { ID = 1, Username = "username1" });


            // Act
            var usersController = new UsersController(usersServiceMock.Object);

            var response = await usersController.Add(userModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Delete_Failure_GeneralException()
        {
            // Arrange
            var entityIds = new long[] { 1L, 2L };

            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.RemoveUsersAsync(It.IsAny<long[]>()))
                .ThrowsAsync(new Exception());


            // Act
            var usersController = new UsersController(usersServiceMock.Object);

            var response = await usersController.Delete(entityIds);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Delete_Failure_EntityNotFoundException()
        {
            // Arrange
            var entityId = 1L;

            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.RemoveUsersAsync(It.IsAny<long[]>()))
                .ThrowsAsync(new EntityNotFoundException(nameof(Users), entityId));


            // Act
            var usersController = new UsersController(usersServiceMock.Object);

            var response = await usersController.Delete(new long[] { entityId });


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<NotFoundResult>(response);
            Assert.Equal(404, ((NotFoundResult)response).StatusCode);
        }

        [Fact]
        public async Task Delete_Success()
        {
            // Arrange
            var entityIds = new long[] { 1L, 2L };

            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.RemoveUsersAsync(It.IsAny<long[]>()))
                .Returns(Task.CompletedTask);


            // Act
            var usersController = new UsersController(usersServiceMock.Object);

            var response = await usersController.Delete(entityIds);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkResult>(response);
            Assert.Equal(200, ((OkResult)response).StatusCode);
        }

        [Fact]
        public async Task Get_Failure_EntityNotFoundException()
        {
            // Arrange
            var entityId = 1L;

            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.FindUserByIdAsync(It.IsAny<long>()))
                .Throws(new EntityNotFoundException(nameof(Users), entityId));


            // Act
            var usersController = new UsersController(usersServiceMock.Object);

            var response = await usersController.Get(entityId);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<NotFoundResult>(response);
            Assert.Equal(404, ((NotFoundResult)response).StatusCode);
        }

        [Fact]
        public async Task Get_Failure_GeneralException()
        {
            // Arrange
            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.FindUserByIdAsync(It.IsAny<long>()))
                .Throws(new Exception());


            // Act
            var usersController = new UsersController(usersServiceMock.Object);

            var response = await usersController.Get(1L);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Get_Success()
        {
            // Arrange
            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.FindUserByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new UserUpdateModel { ID = 1, Username = "User 1" });


            // Act
            var usersController = new UsersController(usersServiceMock.Object);

            var response = await usersController.Get(1L);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }

        [Fact]
        public void List_Failure_GeneralException()
        {
            // Arrange
            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.ListUsers(It.IsAny<IListParameters>()))
                .Throws(new Exception());


            // Act
            var usersController = new UsersController(usersServiceMock.Object);

            var response = usersController.List(new ListParametersModel());


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public void List_Success()
        {
            // Arrange
            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.ListUsers(It.IsAny<IListParameters>()))
                .Returns(new List<UsersListModel>().AsQueryable());


            // Act
            var usersController = new UsersController(usersServiceMock.Object);

            var response = usersController.List(new ListParametersModel());


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Update_Failure_GeneralException()
        {
            // Arrange
            var userModel = new UserUpdateModel() { ID = 1, Username = "username1" };

            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.UpdateExistingUserAsync(It.IsAny<UserUpdateModel>()))
                .ThrowsAsync(new Exception());


            // Act
            var usersController = new UsersController(usersServiceMock.Object);

            var response = await usersController.Update(userModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Update_Failure_ValidationFailureException()
        {
            // Arrange
            var userModel = new UserUpdateModel() { ID = 1, Username = string.Empty };

            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.UpdateExistingUserAsync(It.IsAny<UserUpdateModel>()))
                .ThrowsAsync(new EntityValidationFailureException(nameof(Users), 1L, new ValidationResult()));


            // Act
            var usersController = new UsersController(usersServiceMock.Object);

            var response = await usersController.Update(userModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<BadRequestObjectResult>(response);
            Assert.Equal(400, ((BadRequestObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Update_Success()
        {
            // Arrange
            var userModel = new UserUpdateModel() { ID = 1, Username = "username1" };

            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.UpdateExistingUserAsync(It.IsAny<UserUpdateModel>()))
                .ReturnsAsync(new UserUpdateModel { ID = 1, Username = "username1" });


            // Act
            var usersController = new UsersController(usersServiceMock.Object);

            var response = await usersController.Update(userModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }
    }
}
