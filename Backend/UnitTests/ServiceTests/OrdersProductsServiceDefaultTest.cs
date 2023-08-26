using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.BusinessEntities.General;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Helpers.Validations;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.Stock.Interfaces;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.Stock;
using Zambon.OrderManagement.WebApi.Services.Stock;

namespace UnitTests.ServiceTests
{
    public class OrdersProductsServiceDefaultTest
    {
        private readonly IMapper mapper;

        public OrdersProductsServiceDefaultTest()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Zambon.OrderManagement.WebApi.Helpers.ValidationProblemEntityDetails).Assembly));
            configuration.CompileMappings();
            mapper = new Mapper(configuration);
        }


        [Fact]
        public async Task BatchUpdateOrdersProductsAsync_Failure_AddEntities()
        {
            // Arrange
            var batchUpdateModel = new BatchUpdateModel<OrdersProductUpdateModel, long>()
            {
                EntitiesToAddUpdate = new OrdersProductUpdateModel[] {
                    new OrdersProductUpdateModel { ID = -1, ProductID = 1, Qty = 1 },
                },
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var databaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();
            var ordersProductsRepositoryMock = new Mock<IOrdersProductsRepository>();
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

            dbContextMock.Setup(x => x.Database)
                .Returns(databaseMock.Object);

            databaseMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new Orders { ID = 1, CustomerID = 1 });

            ordersProductsRepositoryMock.Setup(x => x.AddAsync(It.IsAny<OrdersProducts>()))
                .ThrowsAsync(new Exception());


            // Act
            var orderProductsService = new OrdersProductsServiceDefault(dbContextMock.Object, mapper, ordersProductsRepositoryMock.Object, ordersRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                await orderProductsService.BatchUpdateOrdersProductsAsync(1, batchUpdateModel);
            });


            // Assert
            Assert.NotNull(exception);
            Assert.IsType<Exception>(exception);

            databaseMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            ordersProductsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrdersProducts>()), Times.Once);
            ordersProductsRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<long>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OrdersProducts>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.RemoveAsync(It.IsAny<long>()), Times.Never);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BatchUpdateOrdersProductsAsync_Failure_AddInvalidEntities()
        {
            // Arrange
            var batchUpdateModel = new BatchUpdateModel<OrdersProductUpdateModel, long>()
            {
                EntitiesToAddUpdate = new OrdersProductUpdateModel[] {
                    new OrdersProductUpdateModel { ID = -1, ProductID = 1, Qty = 1 },
                },
            };

            var validationResult = new ValidationResult();

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var databaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();
            var ordersProductsRepositoryMock = new Mock<IOrdersProductsRepository>();
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

            dbContextMock.Setup(x => x.Database)
                .Returns(databaseMock.Object);

            databaseMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new Orders { ID = 1, CustomerID = 1 });

            ordersProductsRepositoryMock.Setup(x => x.AddAsync(It.IsAny<OrdersProducts>()))
                .ThrowsAsync(new EntityValidationFailureException(nameof(OrdersProducts), 0, validationResult));


            // Act
            var orderProductsService = new OrdersProductsServiceDefault(dbContextMock.Object, mapper, ordersProductsRepositoryMock.Object, ordersRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                await orderProductsService.BatchUpdateOrdersProductsAsync(1, batchUpdateModel);
            });


            // Assert
            Assert.NotNull(exception);
            Assert.IsType<EntityValidationFailureException>(exception);
            Assert.Contains(nameof(OrdersProducts), ((EntityValidationFailureException)exception).Message);
            Assert.Equal(batchUpdateModel.EntitiesToAddUpdate.First().ID, ((EntityValidationFailureException)exception).EntityKey);
            Assert.Equal(validationResult, ((EntityValidationFailureException)exception).ValidationResult);

            databaseMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            ordersProductsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrdersProducts>()), Times.Once);
            ordersProductsRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<long>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OrdersProducts>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.RemoveAsync(It.IsAny<long>()), Times.Never);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BatchUpdateOrdersProductsAsync_Failure_DeleteInvalidEntities()
        {
            // Arrange
            var batchUpdateModel = new BatchUpdateModel<OrdersProductUpdateModel, long>()
            {
                EntitiesToDelete = new long[] { 1, 2 },
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var databaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();
            var ordersProductsRepositoryMock = new Mock<IOrdersProductsRepository>();
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

            dbContextMock.Setup(x => x.Database)
                .Returns(databaseMock.Object);

            databaseMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new Orders { ID = 1, CustomerID = 1 });

            ordersProductsRepositoryMock.Setup(x => x.RemoveAsync(It.IsAny<long>()))
                .ThrowsAsync(new Exception());


            // Act
            var orderProductsService = new OrdersProductsServiceDefault(dbContextMock.Object, mapper, ordersProductsRepositoryMock.Object, ordersRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                await orderProductsService.BatchUpdateOrdersProductsAsync(1, batchUpdateModel);
            });


            // Assert
            Assert.NotNull(exception);

            databaseMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            ordersProductsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrdersProducts>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<long>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OrdersProducts>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.RemoveAsync(It.IsAny<long>()), Times.Once);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BatchUpdateOrdersProductsAsync_Failure_EntityNotFound()
        {
            // Arrange
            var batchUpdateModel = new BatchUpdateModel<OrdersProductUpdateModel, long>() { };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var ordersProductsRepositoryMock = new Mock<IOrdersProductsRepository>();
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(await Task.FromResult<Orders>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.


            // Act
            var orderProductsService = new OrdersProductsServiceDefault(dbContextMock.Object, mapper, ordersProductsRepositoryMock.Object, ordersRepositoryMock.Object);

            var method = async () =>
            {
                await orderProductsService.BatchUpdateOrdersProductsAsync(1, batchUpdateModel);
            };


            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(method);
        }

        [Fact]
        public async Task BatchUpdateOrdersProductsAsync_Failure_UpdateEntities()
        {
            // Arrange
            var batchUpdateModel = new BatchUpdateModel<OrdersProductUpdateModel, long>()
            {
                EntitiesToAddUpdate = new OrdersProductUpdateModel[] {
                    new OrdersProductUpdateModel { ID = 1, ProductID = 1, Qty = 1 },
                },
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var databaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();
            var ordersProductsRepositoryMock = new Mock<IOrdersProductsRepository>();
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

            dbContextMock.Setup(x => x.Database)
                .Returns(databaseMock.Object);

            databaseMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new Orders { ID = 1, CustomerID = 1 });

            ordersProductsRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new OrdersProducts
                {
                    ID = 1,
                    ProductID = 1,
                    Product = new Products { ID = 1, Name = "Product 1", UnitPrice = 1 },
                    Qty = 1,
                    UnitPrice = 1
                });

            ordersProductsRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<OrdersProducts>()))
                .ThrowsAsync(new Exception());


            // Act
            var orderProductsService = new OrdersProductsServiceDefault(dbContextMock.Object, mapper, ordersProductsRepositoryMock.Object, ordersRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                await orderProductsService.BatchUpdateOrdersProductsAsync(1, batchUpdateModel);
            });


            // Assert
            Assert.NotNull(exception);
            Assert.IsType<Exception>(exception);

            databaseMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            ordersProductsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrdersProducts>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<long>()), Times.Once);
            ordersProductsRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OrdersProducts>()), Times.Once);
            ordersProductsRepositoryMock.Verify(x => x.RemoveAsync(It.IsAny<long>()), Times.Never);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BatchUpdateOrdersProductsAsync_Failure_UpdateInvalidEntity()
        {
            // Arrange
            var batchUpdateModel = new BatchUpdateModel<OrdersProductUpdateModel, long>()
            {
                EntitiesToAddUpdate = new OrdersProductUpdateModel[] {
                    new OrdersProductUpdateModel { ID = 1, ProductID = 1, Qty = 1 },
                },
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var databaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();
            var ordersProductsRepositoryMock = new Mock<IOrdersProductsRepository>();
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

            dbContextMock.Setup(x => x.Database)
                .Returns(databaseMock.Object);

            databaseMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new Orders { ID = 1, CustomerID = 1 });

            ordersProductsRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ThrowsAsync(new EntityNotFoundException(nameof(OrdersProducts), batchUpdateModel.EntitiesToAddUpdate.First().ID));


            // Act
            var orderProductsService = new OrdersProductsServiceDefault(dbContextMock.Object, mapper, ordersProductsRepositoryMock.Object, ordersRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                await orderProductsService.BatchUpdateOrdersProductsAsync(1, batchUpdateModel);
            });


            // Assert
            Assert.NotNull(exception);
            Assert.IsType<EntityNotFoundException>(exception);
            Assert.Equal(new EntityNotFoundException(nameof(OrdersProducts), batchUpdateModel.EntitiesToAddUpdate.First().ID).Message, exception.Message);

            databaseMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            ordersProductsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrdersProducts>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<long>()), Times.Exactly(batchUpdateModel.EntitiesToAddUpdate.Count(x => x.ID > 0)));
            ordersProductsRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OrdersProducts>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.RemoveAsync(It.IsAny<long>()), Times.Never);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BatchUpdateOrdersProductsAsync_Success_AddEntities()
        {
            // Arrange
            var batchUpdateModel = new BatchUpdateModel<OrdersProductUpdateModel, long>()
            {
                EntitiesToAddUpdate = new OrdersProductUpdateModel[] {
                    new OrdersProductUpdateModel { ID = -1, ProductID = 1, Qty = 1 },
                },
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var databaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();
            var ordersProductsRepositoryMock = new Mock<IOrdersProductsRepository>();
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

            dbContextMock.Setup(x => x.Database)
                .Returns(databaseMock.Object);

            databaseMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new Orders { ID = 1, CustomerID = 1 });


            // Act
            var orderProductsService = new OrdersProductsServiceDefault(dbContextMock.Object, mapper, ordersProductsRepositoryMock.Object, ordersRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                await orderProductsService.BatchUpdateOrdersProductsAsync(1, batchUpdateModel);
            });


            // Assert
            Assert.Null(exception);

            databaseMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            ordersProductsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrdersProducts>()), Times.Exactly(batchUpdateModel.EntitiesToAddUpdate.Count(x => x.ID <= 0)));
            ordersProductsRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<long>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OrdersProducts>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.RemoveAsync(It.IsAny<long>()), Times.Never);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task BatchUpdateOrdersProductsAsync_Success_DeleteEntities()
        {
            // Arrange
            var batchUpdateModel = new BatchUpdateModel<OrdersProductUpdateModel, long>()
            {
                EntitiesToDelete = new long[] { 1, 2 },
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var databaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();
            var ordersProductsRepositoryMock = new Mock<IOrdersProductsRepository>();
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

            dbContextMock.Setup(x => x.Database)
                .Returns(databaseMock.Object);

            databaseMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new Orders { ID = 1, CustomerID = 1 });


            // Act
            var orderProductsService = new OrdersProductsServiceDefault(dbContextMock.Object, mapper, ordersProductsRepositoryMock.Object, ordersRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                await orderProductsService.BatchUpdateOrdersProductsAsync(1, batchUpdateModel);
            });


            // Assert
            Assert.Null(exception);

            databaseMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            ordersProductsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrdersProducts>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<long>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OrdersProducts>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.RemoveAsync(It.IsAny<long>()), Times.Exactly(batchUpdateModel.EntitiesToDelete.Length));
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task BatchUpdateOrdersProductsAsync_Success_NoEntitiesUpdated()
        {
            // Arrange
            var batchUpdateModel = new BatchUpdateModel<OrdersProductUpdateModel, long>() { };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var databaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();
            var ordersProductsRepositoryMock = new Mock<IOrdersProductsRepository>();
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

            dbContextMock.Setup(x => x.Database)
                .Returns(databaseMock.Object);

            databaseMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new Orders { ID = 1, CustomerID = 1 });


            // Act
            var orderProductsService = new OrdersProductsServiceDefault(dbContextMock.Object, mapper, ordersProductsRepositoryMock.Object, ordersRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                await orderProductsService.BatchUpdateOrdersProductsAsync(1, batchUpdateModel);
            });


            // Assert
            Assert.Null(exception);

            databaseMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrdersProducts>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<long>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OrdersProducts>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.RemoveAsync(It.IsAny<long>()), Times.Never);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task BatchUpdateOrdersProductsAsync_Success_UpdateEntities()
        {
            // Arrange
            var batchUpdateModel = new BatchUpdateModel<OrdersProductUpdateModel, long>()
            {
                EntitiesToAddUpdate = new OrdersProductUpdateModel[] {
                    new OrdersProductUpdateModel { ID = 1, ProductID = 1, Qty = 1 },
                },
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var databaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();
            var ordersProductsRepositoryMock = new Mock<IOrdersProductsRepository>();
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

            dbContextMock.Setup(x => x.Database)
                .Returns(databaseMock.Object);

            databaseMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new Orders { ID = 1, CustomerID = 1 });

            ordersProductsRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new OrdersProducts
                {
                    ID = 1,
                    ProductID = 1,
                    Product = new Products { ID = 1, Name = "Product 1", UnitPrice = 1 },
                    Qty = 1,
                    UnitPrice = 1
                });


            // Act
            var orderProductsService = new OrdersProductsServiceDefault(dbContextMock.Object, mapper, ordersProductsRepositoryMock.Object, ordersRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                await orderProductsService.BatchUpdateOrdersProductsAsync(1, batchUpdateModel);
            });


            // Assert
            Assert.Null(exception);

            databaseMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            ordersProductsRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrdersProducts>()), Times.Never);
            ordersProductsRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<long>()), Times.Exactly(batchUpdateModel.EntitiesToAddUpdate.Count(x => x.ID > 0)));
            ordersProductsRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OrdersProducts>()), Times.Exactly(batchUpdateModel.EntitiesToAddUpdate.Count(x => x.ID > 0)));
            ordersProductsRepositoryMock.Verify(x => x.RemoveAsync(It.IsAny<long>()), Times.Never);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ListOrdersProductsAsync_Failure_InvalidOrderId()
        {
            // Arrange
            var orderId = 1L;

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var ordersProductsRepositoryMock = new Mock<IOrdersProductsRepository>();
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(await Task.FromResult<Orders>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.


            // Act
            var orderProductsService = new OrdersProductsServiceDefault(dbContextMock.Object, mapper, ordersProductsRepositoryMock.Object, ordersRepositoryMock.Object);

            var exception = await Record.ExceptionAsync(async () =>
            {
                _ = await orderProductsService.ListOrdersProductsAsync(orderId, new ListParametersModel());
            });


            // Assert
            ordersProductsRepositoryMock.Verify(x => x.List(It.IsAny<long>(), It.IsAny<IListParameters>()), Times.Never);

            Assert.NotNull(exception);
            Assert.IsType<EntityNotFoundException>(exception);
            Assert.Equal(new EntityNotFoundException(nameof(Orders), orderId).Message, exception.Message);
        }

        [Fact]
        public async Task ListOrdersProductsAsync_Success_EmptyResults()
        {
            // Arrange
            var order = new Orders
            {
                ID = 1,
                CustomerID = 1,
                Customer = new Customers() { ID = 1, Name = "Customer 1" },
            };

            var ordersProducts = new OrdersProducts[0];

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var ordersProductsRepositoryMock = new Mock<IOrdersProductsRepository>();
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.Is<long>(x => x == order.ID)))
                .ReturnsAsync(order);

            ordersProductsRepositoryMock.Setup(x => x.List(It.Is<long>(x => x == order.ID), It.IsAny<IListParameters>()))
                .Returns(ordersProducts.AsQueryable());


            // Act
            var orderProductsService = new OrdersProductsServiceDefault(dbContextMock.Object, mapper, ordersProductsRepositoryMock.Object, ordersRepositoryMock.Object);

            var ordersProductsList = await orderProductsService.ListOrdersProductsAsync(order.ID, new ListParametersModel());


            // Assert
            ordersProductsRepositoryMock.Verify(x => x.List(It.IsAny<long>(), It.IsAny<IListParameters>()), Times.Once);

            Assert.NotNull(ordersProductsList);
            Assert.Equal(ordersProducts.Length, ordersProductsList.Count());
        }

        [Fact]
        public async Task ListOrdersProductsAsync_Success_ValidResults()
        {
            // Arrange
            var order = new Orders
            {
                ID = 1,
                CustomerID = 1,
                Customer = new Customers() { ID = 1, Name = "Customer 1" },
            };

            var ordersProducts = new OrdersProducts[]
            {
                new OrdersProducts {
                    ID = 1,
                    OrderID = 1,
                    Order = order,
                    ProductID = 1,
                    Product = new Products() { ID = 1, Name = "Product 1", UnitPrice = 1},
                    Qty = 1,
                    UnitPrice = 1,
                },
                new OrdersProducts {
                    ID = 2,
                    OrderID = 1,
                    Order = order,
                    ProductID = 2,
                    Product = new Products() { ID = 2, Name = "Product 2", UnitPrice = 1},
                    Qty = 1,
                    UnitPrice = 1,
                },
                new OrdersProducts {
                    ID = 3,
                    OrderID = 1,
                    Order = order,
                    ProductID = 3,
                    Product = new Products() { ID = 3, Name = "Product 3", UnitPrice = 1},
                    Qty = 1,
                    UnitPrice = 1,
                },
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var ordersProductsRepositoryMock = new Mock<IOrdersProductsRepository>();
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.Is<long>(x => x == order.ID)))
                .ReturnsAsync(order);

            ordersProductsRepositoryMock.Setup(x => x.List(It.Is<long>(x => x == order.ID), It.IsAny<IListParameters>()))
                .Returns(ordersProducts.AsQueryable());


            // Act
            var orderProductsService = new OrdersProductsServiceDefault(dbContextMock.Object, mapper, ordersProductsRepositoryMock.Object, ordersRepositoryMock.Object);

            var ordersProductsList = await orderProductsService.ListOrdersProductsAsync(order.ID, new ListParametersModel());


            // Assert
            ordersProductsRepositoryMock.Verify(x => x.List(It.IsAny<long>(), It.IsAny<IListParameters>()), Times.Once);

            Assert.NotNull(ordersProductsList);
            Assert.Equal(ordersProducts.Length, ordersProductsList.Count());
        }
    }
}