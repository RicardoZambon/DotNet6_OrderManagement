using Microsoft.AspNetCore.Mvc;
using Moq;
using Zambon.OrderManagement.Core.BusinessEntities.General;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Helpers.Validations;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.WebApi.Controllers.General;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.General;
using Zambon.OrderManagement.WebApi.Services.General.Interfaces;

namespace UnitTests.ControllerTests
{
    public class CustomerControllerTest
    {
        [Fact]
        public async Task Add_Failure_GeneralException()
        {
            // Arrange
            var customerModel = new CustomerInsertModel() { Name = "Customer 1" };

            var customersServiceMock = new Mock<ICustomersService>();

            customersServiceMock.Setup(x => x.InsertNewCustomerAsync(It.IsAny<CustomerInsertModel>()))
                .ThrowsAsync(new Exception());


            // Act
            var customersController = new CustomersController(customersServiceMock.Object);

            var response = await customersController.Add(customerModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Add_Failure_ValidationFailureException()
        {
            // Arrange
            var customerModel = new CustomerInsertModel() { Name = string.Empty };

            var customersServiceMock = new Mock<ICustomersService>();

            customersServiceMock.Setup(x => x.InsertNewCustomerAsync(It.IsAny<CustomerInsertModel>()))
                .ThrowsAsync(new EntityValidationFailureException(nameof(Customers), 1L, new ValidationResult()));


            // Act
            var customersController = new CustomersController(customersServiceMock.Object);

            var response = await customersController.Add(customerModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<BadRequestObjectResult>(response);
            Assert.Equal(400, ((BadRequestObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Add_Success()
        {
            // Arrange
            var customerModel = new CustomerInsertModel() { Name = "Customer 1" };

            var customersServiceMock = new Mock<ICustomersService>();

            customersServiceMock.Setup(x => x.InsertNewCustomerAsync(It.IsAny<CustomerInsertModel>()))
                .ReturnsAsync(new CustomerUpdateModel { ID = 1, Name = "Customer 1" });


            // Act
            var customersController = new CustomersController(customersServiceMock.Object);

            var response = await customersController.Add(customerModel);


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

            var customersServiceMock = new Mock<ICustomersService>();

            customersServiceMock.Setup(x => x.RemoveCustomersAsync(It.IsAny<long[]>()))
                .ThrowsAsync(new Exception());


            // Act
            var customersController = new CustomersController(customersServiceMock.Object);

            var response = await customersController.Delete(entityIds);


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

            var customersServiceMock = new Mock<ICustomersService>();

            customersServiceMock.Setup(x => x.RemoveCustomersAsync(It.IsAny<long[]>()))
                .ThrowsAsync(new EntityNotFoundException(nameof(Customers), entityId));


            // Act
            var customersController = new CustomersController(customersServiceMock.Object);

            var response = await customersController.Delete(new long[] { entityId });


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

            var customersServiceMock = new Mock<ICustomersService>();

            customersServiceMock.Setup(x => x.RemoveCustomersAsync(It.IsAny<long[]>()))
                .Returns(Task.CompletedTask);


            // Act
            var customersController = new CustomersController(customersServiceMock.Object);

            var response = await customersController.Delete(entityIds);


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

            var customersServiceMock = new Mock<ICustomersService>();

            customersServiceMock.Setup(x => x.FindCustomerByIdAsync(It.IsAny<long>()))
                .Throws(new EntityNotFoundException(nameof(Customers), entityId));


            // Act
            var customersController = new CustomersController(customersServiceMock.Object);

            var response = await customersController.Get(entityId);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<NotFoundResult>(response);
            Assert.Equal(404, ((NotFoundResult)response).StatusCode);
        }

        [Fact]
        public async Task Get_Failure_GeneralException()
        {
            // Arrange
            var customersServiceMock = new Mock<ICustomersService>();

            customersServiceMock.Setup(x => x.FindCustomerByIdAsync(It.IsAny<long>()))
                .Throws(new Exception());


            // Act
            var customersController = new CustomersController(customersServiceMock.Object);

            var response = await customersController.Get(1L);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Get_Success()
        {
            // Arrange
            var customersServiceMock = new Mock<ICustomersService>();

            customersServiceMock.Setup(x => x.FindCustomerByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new CustomerUpdateModel { ID = 1, Name = "Customer 1" });


            // Act
            var customersController = new CustomersController(customersServiceMock.Object);

            var response = await customersController.Get(1L);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }

        [Fact]
        public void List_Failure_GeneralException()
        {
            // Arrange
            var customersServiceMock = new Mock<ICustomersService>();

            customersServiceMock.Setup(x => x.ListCustomers(It.IsAny<IListParameters>()))
                .Throws(new Exception());


            // Act
            var customersController = new CustomersController(customersServiceMock.Object);

            var response = customersController.List(new ListParametersModel());


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public void List_Success()
        {
            // Arrange
            var customersServiceMock = new Mock<ICustomersService>();

            customersServiceMock.Setup(x => x.ListCustomers(It.IsAny<IListParameters>()))
                .Returns(new List<CustomersListModel>().AsQueryable());


            // Act
            var customersController = new CustomersController(customersServiceMock.Object);

            var response = customersController.List(new ListParametersModel());


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Update_Failure_GeneralException()
        {
            // Arrange
            var customerModel = new CustomerUpdateModel() { ID = 1, Name = "Customer 1" };

            var customersServiceMock = new Mock<ICustomersService>();

            customersServiceMock.Setup(x => x.UpdateExistingCustomerAsync(It.IsAny<CustomerUpdateModel>()))
                .ThrowsAsync(new Exception());


            // Act
            var customersController = new CustomersController(customersServiceMock.Object);

            var response = await customersController.Update(customerModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Update_Failure_ValidationFailureException()
        {
            // Arrange
            var customerModel = new CustomerUpdateModel() { ID = 1, Name = string.Empty };

            var customersServiceMock = new Mock<ICustomersService>();

            customersServiceMock.Setup(x => x.UpdateExistingCustomerAsync(It.IsAny<CustomerUpdateModel>()))
                .ThrowsAsync(new EntityValidationFailureException(nameof(Customers), 1L, new ValidationResult()));


            // Act
            var customersController = new CustomersController(customersServiceMock.Object);

            var response = await customersController.Update(customerModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<BadRequestObjectResult>(response);
            Assert.Equal(400, ((BadRequestObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Update_Success()
        {
            // Arrange
            var customerModel = new CustomerUpdateModel() { ID = 1, Name = "Customer 1" };

            var customersServiceMock = new Mock<ICustomersService>();

            customersServiceMock.Setup(x => x.UpdateExistingCustomerAsync(It.IsAny<CustomerUpdateModel>()))
                .ReturnsAsync(new CustomerUpdateModel { ID = 1, Name = "Customer 1" });


            // Act
            var customersController = new CustomersController(customersServiceMock.Object);

            var response = await customersController.Update(customerModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }
    }
}
