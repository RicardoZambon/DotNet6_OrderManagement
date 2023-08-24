using Microsoft.AspNetCore.Mvc;
using Moq;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Helpers.Validations;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.WebApi.Controllers.Stock;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.Stock;
using Zambon.OrderManagement.WebApi.Services.Stock.Interfaces;

namespace UnitTests.ControllerTests
{
    public class OrderControllerTest
    {
        [Fact]
        public async Task Add_Failure_GeneralException()
        {
            // Arrange
            var orderModel = new OrderInsertModel() { CustomerID = 1 };

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersServiceMock.Setup(x => x.InsertNewOrderAsync(It.IsAny<OrderInsertModel>()))
                .ThrowsAsync(new Exception());


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Add(orderModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Add_Failure_ValidationFailureException()
        {
            // Arrange
            var orderModel = new OrderInsertModel() { CustomerID = 0 };

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersServiceMock.Setup(x => x.InsertNewOrderAsync(It.IsAny<OrderInsertModel>()))
                .ThrowsAsync(new EntityValidationFailureException(nameof(Orders), 1L, new ValidationResult()));


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Add(orderModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<BadRequestObjectResult>(response);
            Assert.Equal(400, ((BadRequestObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Add_Success()
        {
            // Arrange
            var orderModel = new OrderInsertModel() { CustomerID = 1 };

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersServiceMock.Setup(x => x.InsertNewOrderAsync(It.IsAny<OrderInsertModel>()))
                .ReturnsAsync(new OrderDisplayModel { ID = 1, CustomerID = 1, CreatedOn = DateTime.Now });


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Add(orderModel);


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

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersServiceMock.Setup(x => x.RemoveOrdersAsync(It.IsAny<long[]>()))
                .ThrowsAsync(new Exception());


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Delete(entityIds);


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

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersServiceMock.Setup(x => x.RemoveOrdersAsync(It.IsAny<long[]>()))
                .ThrowsAsync(new EntityNotFoundException(nameof(Orders), entityId));


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Delete(new long[] { entityId });


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

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersServiceMock.Setup(x => x.RemoveOrdersAsync(It.IsAny<long[]>()))
                .Returns(Task.CompletedTask);


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Delete(entityIds);


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

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersServiceMock.Setup(x => x.FindOrderByIdAsync(It.IsAny<long>()))
                .Throws(new EntityNotFoundException(nameof(Orders), entityId));


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Get(entityId);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<NotFoundResult>(response);
            Assert.Equal(404, ((NotFoundResult)response).StatusCode);
        }

        [Fact]
        public async Task Get_Failure_GeneralException()
        {
            // Arrange
            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersServiceMock.Setup(x => x.FindOrderByIdAsync(It.IsAny<long>()))
                .Throws(new Exception());


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Get(1L);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Get_Success()
        {
            // Arrange
            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersServiceMock.Setup(x => x.FindOrderByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new OrderDisplayModel { ID = 1, CustomerID = 1, CreatedOn = DateTime.Now });


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Get(1L);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }

        [Fact]
        public void List_Failure_GeneralException()
        {
            // Arrange
            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersServiceMock.Setup(x => x.ListOrders(It.IsAny<IListParameters>()))
                .Throws(new Exception());


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = ordersController.List(new ListParametersModel());


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public void List_Success()
        {
            // Arrange
            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersServiceMock.Setup(x => x.ListOrders(It.IsAny<IListParameters>()))
                .Returns(new List<OrdersListModel>().AsQueryable());


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = ordersController.List(new ListParametersModel());


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task ProductsList_Failure_EntityNotFoundException()
        {
            // Arrange
            var orderId = 1L;

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersProductsServiceMock.Setup(x => x.ListOrdersProductsAsync(It.IsAny<long>(), It.IsAny<IListParameters>()))
                .ThrowsAsync(new EntityNotFoundException(nameof(Orders), orderId));


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Products(orderId, new ListParametersModel());


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<NotFoundResult>(response);
            Assert.Equal(404, ((NotFoundResult)response).StatusCode);
        }

        [Fact]
        public async Task ProductsList_Failure_GeneralException()
        {
            // Arrange
            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersProductsServiceMock.Setup(x => x.ListOrdersProductsAsync(It.IsAny<long>(), It.IsAny<IListParameters>()))
                .ThrowsAsync(new Exception());


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Products(1L, new ListParametersModel());


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task ProductsList_Success()
        {
            // Arrange
            var orderId = 1L;

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersProductsServiceMock.Setup(x => x.ListOrdersProductsAsync(It.Is<long>(id => id == orderId), It.IsAny<IListParameters>()))
                .ReturnsAsync(new List<OrdersProductsListModel>().AsQueryable());


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Products(orderId, new ListParametersModel());


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task ProductsUpdate_Failure_EntityNotFoundException()
        {
            // Arrange
            var orderId = 1L;

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersProductsServiceMock.Setup(x => x.BatchUpdateOrdersProductsAsync(It.Is<long>(id => id == orderId), It.IsAny<BatchUpdateModel<OrdersProductUpdateModel, long>>()))
                .ThrowsAsync(new EntityNotFoundException(nameof(Orders), orderId));


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Products(orderId, new BatchUpdateModel<OrdersProductUpdateModel, long>());


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<NotFoundResult>(response);
            Assert.Equal(404, ((NotFoundResult)response).StatusCode);
        }

        [Fact]
        public async Task ProductsUpdate_Failure_ValidationFailureException()
        {
            // Arrange
            var orderId = 1L;

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersProductsServiceMock.Setup(x => x.BatchUpdateOrdersProductsAsync(It.Is<long>(id => id == orderId), It.IsAny<BatchUpdateModel<OrdersProductUpdateModel, long>>()))
                .ThrowsAsync(new EntityValidationFailureException(nameof(OrdersProducts), 1L, new ValidationResult()));


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Products(orderId, new BatchUpdateModel<OrdersProductUpdateModel, long>());


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<BadRequestObjectResult>(response);
            Assert.Equal(400, ((BadRequestObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task ProductsUpdate_Failure_GeneralException()
        {
            // Arrange
            var orderId = 1L;

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersProductsServiceMock.Setup(x => x.BatchUpdateOrdersProductsAsync(It.Is<long>(id => id == orderId), It.IsAny<BatchUpdateModel<OrdersProductUpdateModel, long>>()))
                .ThrowsAsync(new Exception());


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Products(orderId, new BatchUpdateModel<OrdersProductUpdateModel, long>());


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task ProductsUpdate_Success()
        {
            // Arrange
            var orderId = 1L;

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersProductsServiceMock.Setup(x => x.BatchUpdateOrdersProductsAsync(It.Is<long>(id => id == orderId), It.IsAny<BatchUpdateModel<OrdersProductUpdateModel, long>>()))
                .Returns(Task.CompletedTask);


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Products(orderId, new BatchUpdateModel<OrdersProductUpdateModel, long>());


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkResult>(response);
            Assert.Equal(200, ((OkResult)response).StatusCode);
        }

        [Fact]
        public async Task Update_Failure_GeneralException()
        {
            // Arrange
            var orderModel = new OrderUpdateModel() { ID = 1, CustomerID = 1 };

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersServiceMock.Setup(x => x.UpdateExistingOrderAsync(It.IsAny<OrderUpdateModel>()))
                .ThrowsAsync(new Exception());


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Update(orderModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Update_Failure_ValidationFailureException()
        {
            // Arrange
            var orderModel = new OrderUpdateModel() { ID = 1, CustomerID = 0 };

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersServiceMock.Setup(x => x.UpdateExistingOrderAsync(It.IsAny<OrderUpdateModel>()))
                .ThrowsAsync(new EntityValidationFailureException(nameof(Orders), 1L, new ValidationResult()));


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Update(orderModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<BadRequestObjectResult>(response);
            Assert.Equal(400, ((BadRequestObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Update_Success()
        {
            // Arrange
            var orderModel = new OrderUpdateModel() { ID = 1, CustomerID = 1 };

            var ordersServiceMock = new Mock<IOrdersService>();
            var ordersProductsServiceMock = new Mock<IOrdersProductsService>();

            ordersServiceMock.Setup(x => x.UpdateExistingOrderAsync(It.IsAny<OrderUpdateModel>()))
                .ReturnsAsync(new OrderDisplayModel { ID = 1, CustomerID = 1, CreatedOn = DateTime.Now });


            // Act
            var ordersController = new OrdersController(ordersServiceMock.Object, ordersProductsServiceMock.Object);

            var response = await ordersController.Update(orderModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }
    }
}
