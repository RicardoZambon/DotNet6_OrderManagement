using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.BusinessEntities.General;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.Stock.Interfaces;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.Stock;
using Zambon.OrderManagement.WebApi.Services.Stock;

namespace UnitTests.ServiceTests
{
    public class OrdersServiceDefaultTest
    {
        private readonly IMapper mapper;

        public OrdersServiceDefaultTest()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Zambon.OrderManagement.WebApi.Helpers.ValidationProblemEntityDetails).Assembly));
            configuration.CompileMappings();
            mapper = new Mapper(configuration);
        }


        [Fact]
        public async Task FindOrderByIdAsync_Fail_InvalidOrderId()
        {
            // Arrange
            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(await Task.FromResult<Orders>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.


            // Act
            var orderService = new OrdersServiceDefault(dbContextMock.Object, mapper, ordersRepositoryMock.Object);

            var method = async () =>
            {
                _ = await orderService.FindOrderByIdAsync(1);
            };


            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(method);
        }

        [Fact]
        public async Task FindOrderByIdAsync_Success_ValidOrderId()
        {
            // Arrange
            var orderId = 1L;
            var order = new Orders {
                ID = orderId,
                CustomerID = 1,
                Customer = new Customers { Name = "Customer 1" },
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.Is<long>(x => x == orderId)))
                .ReturnsAsync(order);


            // Act
            var orderService = new OrdersServiceDefault(dbContextMock.Object, mapper, ordersRepositoryMock.Object);

            var orderResult = await orderService.FindOrderByIdAsync(orderId);


            // Assert
            ordersRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<long>()), Times.Once);

            Assert.NotNull(orderResult);
            Assert.Equal(order.ID, orderResult.ID);
            Assert.Equal(order.CustomerID, orderResult.CustomerID);
            Assert.Equal(order.Customer.Name, orderResult.CustomerID_Name);
        }

        [Fact]
        public async Task InsertNewOrderAsync_Success()
        {
            // Arrange
            var orderModel = new OrderInsertModel { CustomerID = 1 };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var ordersRepositoryMock = new Mock<IOrdersRepository>();


            // Act
            var orderService = new OrdersServiceDefault(dbContextMock.Object, mapper, ordersRepositoryMock.Object);

            var orderResult = await orderService.InsertNewOrderAsync(orderModel);


            // Assert
            ordersRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Orders>()), Times.Once);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(orderModel.CustomerID, orderResult.CustomerID);
        }

        [Fact]
        public void ListOrders_Success()
        {
            // Arrange
            var orders = new Orders[]
            {
                new Orders {
                    ID = 1,
                    CustomerID = 1,
                    Customer = new Customers { Name = "Customer 1" },
                },
                new Orders {
                    ID = 2,
                    CustomerID = 1,
                    Customer = new Customers { Name = "Customer 1" },
                },
                new Orders {
                    ID = 3,
                    CustomerID = 2,
                    Customer = new Customers { Name = "Customer 2" },
                },
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

            ordersRepositoryMock.Setup(x => x.List(It.IsAny<IListParameters>()))
                .Returns(orders.AsQueryable());


            // Act
            var orderService = new OrdersServiceDefault(dbContextMock.Object, mapper, ordersRepositoryMock.Object);

            var ordersList = orderService.ListOrders(new ListParametersModel());


            // Assert
            ordersRepositoryMock.Verify(x => x.List(It.IsAny<IListParameters>()), Times.Once);

            Assert.NotNull(ordersList);
            Assert.Equal(orders.Length, ordersList.Count());
        }

        [Fact]
        public async Task RemoveOrdersAsync_Success()
        {
            // Arrange
            var orderIds = new long[] { 1, 2 };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var databaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

            dbContextMock.Setup(x => x.Database)
                .Returns(databaseMock.Object);

            databaseMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ordersRepositoryMock.Setup(x => x.RemoveAsync(It.IsAny<long>()))
                .Returns(Task.CompletedTask);


            // Act
            var orderService = new OrdersServiceDefault(dbContextMock.Object, mapper, ordersRepositoryMock.Object);

            await orderService.RemoveOrdersAsync(orderIds);


            // Assert
            databaseMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            ordersRepositoryMock.Verify(x => x.RemoveAsync(It.IsAny<long>()), Times.Exactly(orderIds.Length));
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateExistingOrderAsync_Failure_EntityNotFound()
        {
            // Arrange
            var orderModel = new OrderUpdateModel { ID = 1, CustomerID = 1 };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(await Task.FromResult<Orders>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.


            // Act
            var orderService = new OrdersServiceDefault(dbContextMock.Object, mapper, ordersRepositoryMock.Object);

            var method = async () =>
            {
                _ = await orderService.UpdateExistingOrderAsync(orderModel);
            };


            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(method);
        }

        [Fact]
        public async Task UpdateExistingOrderAsync_Success_ValidOrderModel()
        {
            // Arrange
            var order = new Orders
            {
                ID = 1,
                CustomerID = 1,
                Customer = new Customers { Name = "Customer 1" },
            };
            var orderModel = new OrderUpdateModel { ID = 1, CustomerID = 2 };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var ordersRepositoryMock = new Mock<IOrdersRepository>();

            ordersRepositoryMock.Setup(x => x.FindByIdAsync(It.Is<long>(x => x == order.ID)))
                .ReturnsAsync(order);


            // Act
            var orderService = new OrdersServiceDefault(dbContextMock.Object, mapper, ordersRepositoryMock.Object);

            var orderResult = await orderService.UpdateExistingOrderAsync(orderModel);


            // Assert
            ordersRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Orders>()), Times.Once);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(orderModel.CustomerID, orderResult.CustomerID);
        }
    }
}