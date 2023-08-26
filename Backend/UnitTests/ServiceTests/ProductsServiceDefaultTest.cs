using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.Stock.Interfaces;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.Stock;
using Zambon.OrderManagement.WebApi.Services.Stock;

namespace UnitTests.ServiceTests
{
    public class ProductsServiceDefaultTest
    {
        private readonly IMapper mapper;

        public ProductsServiceDefaultTest()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Zambon.OrderManagement.WebApi.Helpers.ValidationProblemEntityDetails).Assembly));
            configuration.CompileMappings();
            mapper = new Mapper(configuration);
        }


        [Fact]
        public async Task FindProductByIdAsync_Fail_InvalidProductId()
        {
            // Arrange
            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var productsRepositoryMock = new Mock<IProductsRepository>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            productsRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(await Task.FromResult<Products>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.


            // Act
            var productService = new ProductsServiceDefault(dbContextMock.Object, mapper, productsRepositoryMock.Object);

            var method = async () =>
            {
                _ = await productService.FindProductByIdAsync(1);
            };


            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(method);
        }

        [Fact]
        public async Task FindProductByIdAsync_Success_ValidProductId()
        {
            // Arrange
            var productId = 1L;
            var product = new Products { ID = productId, Name = "Product 1" };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var productsRepositoryMock = new Mock<IProductsRepository>();

            productsRepositoryMock.Setup(x => x.FindByIdAsync(It.Is<long>(x => x == productId)))
                .ReturnsAsync(product);


            // Act
            var productService = new ProductsServiceDefault(dbContextMock.Object, mapper, productsRepositoryMock.Object);

            var productResult = await productService.FindProductByIdAsync(productId);


            // Assert
            productsRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<long>()), Times.Once);

            Assert.NotNull(productResult);
            Assert.Equal(product.ID, productResult.ID);
            Assert.Equal(product.Name, productResult.Name);
        }

        [Fact]
        public async Task InsertNewProductAsync_Success()
        {
            // Arrange
            var productModel = new ProductInsertModel { Name = "Product 1" };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var productsRepositoryMock = new Mock<IProductsRepository>();


            // Act
            var productService = new ProductsServiceDefault(dbContextMock.Object, mapper, productsRepositoryMock.Object);

            var productResult = await productService.InsertNewProductAsync(productModel);


            // Assert
            productsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Products>()), Times.Once);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(productModel.Name, productResult.Name);
        }

        [Fact]
        public void ListProducts_Success()
        {
            // Arrange
            var products = new Products[]
            {
                new Products { Name = "Product 1" },
                new Products { Name = "Product 2" },
                new Products { Name = "Product 3" },
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var productsRepositoryMock = new Mock<IProductsRepository>();

            productsRepositoryMock.Setup(x => x.List(It.IsAny<IListParameters>()))
                .Returns(products.AsQueryable());


            // Act
            var productService = new ProductsServiceDefault(dbContextMock.Object, mapper, productsRepositoryMock.Object);

            var productsList = productService.ListProducts(new ListParametersModel());


            // Assert
            productsRepositoryMock.Verify(x => x.List(It.IsAny<IListParameters>()), Times.Once);

            Assert.NotNull(productsList);
            Assert.Equal(products.Length, productsList.Count());
        }

        [Fact]
        public async Task RemoveProductsAsync_Success()
        {
            // Arrange
            var productIds = new long[] { 1, 2 };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var databaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();
            var productsRepositoryMock = new Mock<IProductsRepository>();

            dbContextMock.Setup(x => x.Database)
                .Returns(databaseMock.Object);

            databaseMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            productsRepositoryMock.Setup(x => x.RemoveAsync(It.IsAny<long>()))
                .Returns(Task.CompletedTask);


            // Act
            var productService = new ProductsServiceDefault(dbContextMock.Object, mapper, productsRepositoryMock.Object);

            await productService.RemoveProductsAsync(productIds);


            // Assert
            databaseMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            productsRepositoryMock.Verify(x => x.RemoveAsync(It.IsAny<long>()), Times.Exactly(productIds.Length));
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateExistingProductAsync_Failure_EntityNotFound()
        {
            // Arrange
            var productModel = new ProductUpdateModel { ID = 1, Name = "Product 1" };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var productsRepositoryMock = new Mock<IProductsRepository>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            productsRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(await Task.FromResult<Products>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.


            // Act
            var productService = new ProductsServiceDefault(dbContextMock.Object, mapper, productsRepositoryMock.Object);

            var method = async () =>
            {
                _ = await productService.UpdateExistingProductAsync(productModel);
            };


            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(method);
        }

        [Fact]
        public async Task UpdateExistingProductAsync_Success_ValidProductModel()
        {
            // Arrange
            var product = new Products { ID = 1, Name = "Product 1" };
            var productModel = new ProductUpdateModel { ID = 1, Name = "Product 2" };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var productsRepositoryMock = new Mock<IProductsRepository>();

            productsRepositoryMock.Setup(x => x.FindByIdAsync(It.Is<long>(x => x == product.ID)))
                .ReturnsAsync(product);


            // Act
            var productService = new ProductsServiceDefault(dbContextMock.Object, mapper, productsRepositoryMock.Object);

            var productResult = await productService.UpdateExistingProductAsync(productModel);


            // Assert
            productsRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Products>()), Times.Once);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(productModel.Name, productResult.Name);
        }
    }
}