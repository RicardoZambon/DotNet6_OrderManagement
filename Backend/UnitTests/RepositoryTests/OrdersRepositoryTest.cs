using Microsoft.EntityFrameworkCore;
using Zambon.OrderManagement.Core.BusinessEntities.General;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Repositories.Stock;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;

namespace UnitTests.RepositoryTests
{
    public class OrdersRepositoryTest : IDisposable
    {
        private readonly SharedDatabaseFixture databaseFixture;

        public OrdersRepositoryTest()
        {
            databaseFixture = new SharedDatabaseFixture();
        }

        public void Dispose()
        {
            databaseFixture.Dispose();
            GC.SuppressFinalize(this);
        }


        [Fact]
        public async Task AddAsync_Success()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.SaveChangesAsync();

            var order = new Orders { CustomerID = 1 };


            // Act
            var ordersRepository = new OrdersRepository(context);

            await ordersRepository.AddAsync(order);
            await context.SaveChangesAsync();


            // Assert
            var dbOrder = await context.Set<Orders>().FirstOrDefaultAsync();

            Assert.Equal(1, await context.Set<Orders>().CountAsync());

            Assert.NotNull(dbOrder);
            Assert.Equal(dbOrder.CustomerID, order.CustomerID);
        }

        [Fact]
        public async Task FindByIdAsync_Success_InvalidOrderId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.SaveChangesAsync();

            var order = new Orders { ID = 1, CustomerID = 1 };

            context.Set<Orders>().Add(order);
            await context.SaveChangesAsync();


            // Act
            var ordersRepository = new OrdersRepository(context);

            var dbOrder = await ordersRepository.FindByIdAsync(2);
            await context.SaveChangesAsync();


            // Assert
            Assert.Null(dbOrder);
        }

        [Fact]
        public async Task FindByIdAsync_Success_ValidOrderId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.SaveChangesAsync();

            var order = new Orders { ID = 1, CustomerID = 1 };

            context.Set<Orders>().Add(order);
            await context.SaveChangesAsync();


            // Act
            var ordersRepository = new OrdersRepository(context);

            var dbOrder = await ordersRepository.FindByIdAsync(1);
            await context.SaveChangesAsync();


            // Assert
            Assert.NotNull(dbOrder);
            Assert.Equal(dbOrder.CustomerID, order.CustomerID);
        }

        [Fact]
        public async Task List_Fail_MissingParameters()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.SaveChangesAsync();

            var orders = new Orders[]
            {
                new Orders { CustomerID = 1 },
                new Orders { CustomerID = 1 },
                new Orders { CustomerID = 1 },
            };

            context.Set<Orders>().AddRange(orders);
            await context.SaveChangesAsync();


            // Act
            var ordersRepository = new OrdersRepository(context);

            var method = () =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _ = ordersRepository.List(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            };


            // Assert
            Assert.Throws<ArgumentNullException>(method);
        }

        [Fact]
        public async Task List_Success_WithEndRow()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.SaveChangesAsync();

            var orders = new List<Orders>();
            for (var i = 0; i < 10; i++)
            {
                orders.Add(new Orders { CustomerID = 1 });
            }

            context.Set<Orders>().AddRange(orders);
            await context.SaveChangesAsync();


            // Act
            var ordersRepository = new OrdersRepository(context);

            var ordersList = ordersRepository.List(new ListParametersModel { EndRow = 5, StartRow = 0 });


            // Assert
            Assert.NotNull(ordersList);

            Assert.Equal(orders.Skip(0).Take(5).Count(), await ordersList.CountAsync());
        }

        [Fact]
        public async Task List_Success_WithStartAndEndRow()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.SaveChangesAsync();

            var orders = new List<Orders>();
            for (var i = 0; i < 10; i++)
            {
                orders.Add(new Orders { CustomerID = 1 });
            }

            context.Set<Orders>().AddRange(orders);
            await context.SaveChangesAsync();


            // Act
            var ordersRepository = new OrdersRepository(context);

            var ordersList = ordersRepository.List(new ListParametersModel { EndRow = 5, StartRow = 2 });


            // Assert
            Assert.NotNull(ordersList);

            Assert.Equal(orders.Skip(2).Take(5).Count(), await ordersList.CountAsync());
        }

        [Fact]
        public async Task List_Success_WithFilterCustomerID()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.Set<Customers>().AddAsync(new Customers { ID = 2, Name = "Customer 2" });
            await context.SaveChangesAsync();

            var orders = new Orders[]
            {
                new Orders { CustomerID = 1 },
                new Orders { CustomerID = 1 },
                new Orders { CustomerID = 2 },
            };

            context.Set<Orders>().AddRange(orders);
            await context.SaveChangesAsync();


            // Act
            var ordersRepository = new OrdersRepository(context);

            var parameters = new ListParametersModel();
            parameters.Filters.Add(nameof(Orders.CustomerID), 1);

            var ordersList = ordersRepository.List(parameters);


            // Assert
            Assert.NotNull(ordersList);

            Assert.Equal(orders.Count(x => x.CustomerID == 1), await ordersList.CountAsync());
        }

        [Fact]
        public async Task List_Success_WithoutFilters()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.SaveChangesAsync();

            var orders = new Orders[]
            {
                new Orders { CustomerID = 1 },
                new Orders { CustomerID = 1 },
                new Orders { CustomerID = 1 },
            };

            context.Set<Orders>().AddRange(orders);
            await context.SaveChangesAsync();


            // Act
            var ordersRepository = new OrdersRepository(context);

            var ordersList = ordersRepository.List(new ListParametersModel());


            // Assert
            Assert.NotNull(ordersList);

            Assert.Equal(orders.Length, await ordersList.CountAsync());
        }

        [Fact]
        public async Task RemoveAsync_Fail_InvalidOrderId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.SaveChangesAsync();

            var orderId = 2;
            var order = new Orders { ID = 1, CustomerID = 1 };

            context.Set<Orders>().Add(order);
            await context.SaveChangesAsync();


            // Act
            var ordersRepository = new OrdersRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await ordersRepository.RemoveAsync(orderId);
            });


            // Assert
            Assert.NotNull(ex);

            Assert.Equal(new EntityNotFoundException(nameof(Orders), orderId).Message, ex.Message);
        }

        [Fact]
        public async Task RemoveAsync_Success_ValidOrderId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.SaveChangesAsync();

            var orderId = 1;
            var order = new Orders { ID = orderId, CustomerID = 1 };

            context.Set<Orders>().Add(order);
            await context.SaveChangesAsync();


            // Act
            var ordersRepository = new OrdersRepository(context);

            await ordersRepository.RemoveAsync(orderId);
            await context.SaveChangesAsync();


            // Assert
            var dbOrder = await ordersRepository.FindByIdAsync(orderId);

            Assert.Equal(0, await context.Set<Orders>().CountAsync());
            Assert.NotNull(dbOrder);
            Assert.True(dbOrder.IsDeleted);
        }

        [Fact]
        public async Task UpdateAsync_Fail_NewOrder()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.SaveChangesAsync();


            // Act
            var ordersRepository = new OrdersRepository(context);

            await ordersRepository.UpdateAsync(new Orders { ID = 1, CustomerID = 1 });

            var method = async () =>
            {
                await context.SaveChangesAsync();
            };


            // Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(method);
        }

        [Fact]
        public async Task UpdateAsync_Success_ExistingOrder()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.Set<Customers>().AddAsync(new Customers { ID = 2, Name = "Customer 2" });
            await context.SaveChangesAsync();

            var orderId = 1;

            context.Set<Orders>().Add(new Orders { ID = orderId, CustomerID = 1 });
            await context.SaveChangesAsync();


            // Act
            var ordersRepository = new OrdersRepository(context);

            var order = await ordersRepository.FindByIdAsync(orderId);
            order!.CustomerID = 2;

            await ordersRepository.UpdateAsync(order);
            await context.SaveChangesAsync();


            // Assert
            var dbOrder = await ordersRepository.FindByIdAsync(orderId);

            Assert.NotNull(dbOrder);
            Assert.Equal(order.CustomerID, dbOrder.CustomerID);
        }

        [Fact]
        public async Task ValidateAsync_Fail_InvalidOrder_CustomerIDInvalid()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.SaveChangesAsync();


            // Act
            var ordersRepository = new OrdersRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await ordersRepository.ValidateAsync(new Orders { CustomerID = 2 });
            });


            // Assert
            Assert.NotNull(ex);

            Assert.IsType<EntityValidationFailureException>(ex);

            Assert.Equal($"Entity '{nameof(Orders)} ({0})' has validation problems.", ex.Message);

            Assert.True(((EntityValidationFailureException)ex).ValidationResult.Errors.ContainsKey(nameof(Orders.CustomerID)));

            Assert.Contains("invalid", ((EntityValidationFailureException)ex).ValidationResult.Errors[nameof(Orders.CustomerID)]);
        }

        [Fact]
        public async Task ValidateAsync_Success_ValidOrder()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.SaveChangesAsync();


            // Act
            var ordersRepository = new OrdersRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await ordersRepository.ValidateAsync(new Orders { CustomerID = 1 });
            });


            // Assert
            Assert.Null(ex);
        }
    }
}