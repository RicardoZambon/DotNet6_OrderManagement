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
    public class ProductControllerTest
    {
        [Fact]
        public async Task Add_Failure_GeneralException()
        {
            // Arrange
            var productModel = new ProductInsertModel() { Name = "Product 1" };

            var productsServiceMock = new Mock<IProductsService>();

            productsServiceMock.Setup(x => x.InsertNewProductAsync(It.IsAny<ProductInsertModel>()))
                .ThrowsAsync(new Exception());


            // Act
            var productsController = new ProductsController(productsServiceMock.Object);

            var response = await productsController.Add(productModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Add_Failure_ValidationFailureException()
        {
            // Arrange
            var productModel = new ProductInsertModel() { Name = string.Empty };

            var productsServiceMock = new Mock<IProductsService>();

            productsServiceMock.Setup(x => x.InsertNewProductAsync(It.IsAny<ProductInsertModel>()))
                .ThrowsAsync(new EntityValidationFailureException(nameof(Products), 1L, new ValidationResult()));


            // Act
            var productsController = new ProductsController(productsServiceMock.Object);

            var response = await productsController.Add(productModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<BadRequestObjectResult>(response);
            Assert.Equal(400, ((BadRequestObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Add_Success()
        {
            // Arrange
            var productModel = new ProductInsertModel() { Name = "Product 1" };

            var productsServiceMock = new Mock<IProductsService>();

            productsServiceMock.Setup(x => x.InsertNewProductAsync(It.IsAny<ProductInsertModel>()))
                .ReturnsAsync(new ProductUpdateModel { ID = 1, Name = "Product 1" });


            // Act
            var productsController = new ProductsController(productsServiceMock.Object);

            var response = await productsController.Add(productModel);


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

            var productsServiceMock = new Mock<IProductsService>();

            productsServiceMock.Setup(x => x.RemoveProductsAsync(It.IsAny<long[]>()))
                .ThrowsAsync(new Exception());


            // Act
            var productsController = new ProductsController(productsServiceMock.Object);

            var response = await productsController.Delete(entityIds);


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

            var productsServiceMock = new Mock<IProductsService>();

            productsServiceMock.Setup(x => x.RemoveProductsAsync(It.IsAny<long[]>()))
                .ThrowsAsync(new EntityNotFoundException(nameof(Products), entityId));


            // Act
            var productsController = new ProductsController(productsServiceMock.Object);

            var response = await productsController.Delete(new long[] { entityId });


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

            var productsServiceMock = new Mock<IProductsService>();

            productsServiceMock.Setup(x => x.RemoveProductsAsync(It.IsAny<long[]>()))
                .Returns(Task.CompletedTask);


            // Act
            var productsController = new ProductsController(productsServiceMock.Object);

            var response = await productsController.Delete(entityIds);


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

            var productsServiceMock = new Mock<IProductsService>();

            productsServiceMock.Setup(x => x.FindProductByIdAsync(It.IsAny<long>()))
                .Throws(new EntityNotFoundException(nameof(Products), entityId));


            // Act
            var productsController = new ProductsController(productsServiceMock.Object);

            var response = await productsController.Get(entityId);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<NotFoundResult>(response);
            Assert.Equal(404, ((NotFoundResult)response).StatusCode);
        }

        [Fact]
        public async Task Get_Failure_GeneralException()
        {
            // Arrange
            var productsServiceMock = new Mock<IProductsService>();

            productsServiceMock.Setup(x => x.FindProductByIdAsync(It.IsAny<long>()))
                .Throws(new Exception());


            // Act
            var productsController = new ProductsController(productsServiceMock.Object);

            var response = await productsController.Get(1L);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Get_Success()
        {
            // Arrange
            var productsServiceMock = new Mock<IProductsService>();

            productsServiceMock.Setup(x => x.FindProductByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new ProductUpdateModel { ID = 1, Name = "Product 1" });


            // Act
            var productsController = new ProductsController(productsServiceMock.Object);

            var response = await productsController.Get(1L);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }

        [Fact]
        public void List_Failure_GeneralException()
        {
            // Arrange
            var productsServiceMock = new Mock<IProductsService>();

            productsServiceMock.Setup(x => x.ListProducts(It.IsAny<IListParameters>()))
                .Throws(new Exception());


            // Act
            var productsController = new ProductsController(productsServiceMock.Object);

            var response = productsController.List(new ListParametersModel());


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public void List_Success()
        {
            // Arrange
            var productsServiceMock = new Mock<IProductsService>();

            productsServiceMock.Setup(x => x.ListProducts(It.IsAny<IListParameters>()))
                .Returns(new List<ProductsListModel>().AsQueryable());


            // Act
            var productsController = new ProductsController(productsServiceMock.Object);

            var response = productsController.List(new ListParametersModel());


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Update_Failure_GeneralException()
        {
            // Arrange
            var productModel = new ProductUpdateModel() { ID = 1, Name = "Product 1" };

            var productsServiceMock = new Mock<IProductsService>();

            productsServiceMock.Setup(x => x.UpdateExistingProductAsync(It.IsAny<ProductUpdateModel>()))
                .ThrowsAsync(new Exception());


            // Act
            var productsController = new ProductsController(productsServiceMock.Object);

            var response = await productsController.Update(productModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<ObjectResult>(response);
            Assert.Equal(500, ((ObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Update_Failure_ValidationFailureException()
        {
            // Arrange
            var productModel = new ProductUpdateModel() { ID = 1, Name = string.Empty };

            var productsServiceMock = new Mock<IProductsService>();

            productsServiceMock.Setup(x => x.UpdateExistingProductAsync(It.IsAny<ProductUpdateModel>()))
                .ThrowsAsync(new EntityValidationFailureException(nameof(Products), 1L, new ValidationResult()));


            // Act
            var productsController = new ProductsController(productsServiceMock.Object);

            var response = await productsController.Update(productModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<BadRequestObjectResult>(response);
            Assert.Equal(400, ((BadRequestObjectResult)response).StatusCode);
        }

        [Fact]
        public async Task Update_Success()
        {
            // Arrange
            var productModel = new ProductUpdateModel() { ID = 1, Name = "Product 1" };

            var productsServiceMock = new Mock<IProductsService>();

            productsServiceMock.Setup(x => x.UpdateExistingProductAsync(It.IsAny<ProductUpdateModel>()))
                .ReturnsAsync(new ProductUpdateModel { ID = 1, Name = "Product 1" });


            // Act
            var productsController = new ProductsController(productsServiceMock.Object);

            var response = await productsController.Update(productModel);


            // Assert
            Assert.NotNull(response);
            Assert.IsAssignableFrom<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }
    }
}
