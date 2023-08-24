using Microsoft.EntityFrameworkCore;
using Zambon.OrderManagement.Core.BusinessEntities.General;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Repositories.Stock;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;

namespace UnitTests.RepositoryTests
{
    public class OrdersProductsRepositoryTest : IDisposable
    {
        private readonly SharedDatabaseFixture databaseFixture;

        public OrdersProductsRepositoryTest()
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
            var product = new Products { ID = 1, Name = "Product 1", UnitPrice = 1 };

            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.Set<Products>().AddAsync(product);
            await context.SaveChangesAsync();

            await context.Set<Orders>().AddAsync(new Orders { ID = 1, CustomerID = 1 });
            await context.SaveChangesAsync();

            var orderProduct = new OrdersProducts { OrderID = 1, ProductID = 1, Qty = 1 };


            // Act
            var ordersProductsRepository = new OrdersProductsRepository(context);

            await ordersProductsRepository.AddAsync(orderProduct);
            await context.SaveChangesAsync();


            // Assert
            var dbOrderProduct = await context.Set<OrdersProducts>().FirstOrDefaultAsync();

            Assert.Equal(1, await context.Set<OrdersProducts>().CountAsync());

            Assert.NotNull(dbOrderProduct);
            Assert.Equal(dbOrderProduct.OrderID, orderProduct.OrderID);
            Assert.Equal(dbOrderProduct.ProductID, orderProduct.ProductID);
            Assert.Equal(dbOrderProduct.Qty, orderProduct.Qty);

            Assert.Equal(dbOrderProduct.UnitPrice, product.UnitPrice);
        }

        [Fact]
        public async Task FindByIdAsync_Success_InvalidOrderProductId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.Set<Products>().AddAsync(new Products { ID = 1, Name = "Product 1", UnitPrice = 1 });
            await context.SaveChangesAsync();

            await context.Set<Orders>().AddAsync(new Orders { ID = 1, CustomerID = 1 });
            await context.SaveChangesAsync();

            var orderProduct = new OrdersProducts { ID = 1, OrderID = 1, ProductID = 1, Qty = 1 };

            context.Set<OrdersProducts>().Add(orderProduct);
            await context.SaveChangesAsync();


            // Act
            var ordersProductsRepository = new OrdersProductsRepository(context);

            var dbOrderProduct = await ordersProductsRepository.FindByIdAsync(2);
            await context.SaveChangesAsync();


            // Assert
            Assert.Null(dbOrderProduct);
        }

        [Fact]
        public async Task FindByIdAsync_Success_ValidOrderProductId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.Set<Products>().AddAsync(new Products { ID = 1, Name = "Product 1", UnitPrice = 1 });
            await context.SaveChangesAsync();

            await context.Set<Orders>().AddAsync(new Orders { ID = 1, CustomerID = 1 });
            await context.SaveChangesAsync();

            var orderProduct = new OrdersProducts { ID = 1, OrderID = 1, ProductID = 1, Qty = 1 };

            context.Set<OrdersProducts>().Add(orderProduct);
            await context.SaveChangesAsync();


            // Act
            var ordersProductsRepository = new OrdersProductsRepository(context);

            var dbOrderProduct = await ordersProductsRepository.FindByIdAsync(1);
            await context.SaveChangesAsync();


            // Assert
            Assert.NotNull(dbOrderProduct);
            Assert.Equal(dbOrderProduct.OrderID, orderProduct.OrderID);
            Assert.Equal(dbOrderProduct.ProductID, orderProduct.ProductID);
            Assert.Equal(dbOrderProduct.Qty, orderProduct.Qty);
        }

        [Fact]
        public async Task List_Fail_MissingParameters()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.Set<Products>().AddAsync(new Products { ID = 1, Name = "Product 1", UnitPrice = 1 });
            await context.SaveChangesAsync();

            await context.Set<Orders>().AddAsync(new Orders { ID = 1, CustomerID = 1 });
            await context.SaveChangesAsync();

            var ordersProducts = new OrdersProducts[]
            {
                new OrdersProducts { OrderID = 1, ProductID = 1, Qty = 1 },
                new OrdersProducts { OrderID = 1, ProductID = 1, Qty = 2 },
                new OrdersProducts { OrderID = 1, ProductID = 1, Qty = 3 },
            };

            context.Set<OrdersProducts>().AddRange(ordersProducts);
            await context.SaveChangesAsync();


            // Act
            var ordersProductsRepository = new OrdersProductsRepository(context);

            var method = () =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _ = ordersProductsRepository.List(1, null);
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
            await context.Set<Products>().AddAsync(new Products { ID = 1, Name = "Product 1", UnitPrice = 1 });
            await context.SaveChangesAsync();

            await context.Set<Orders>().AddAsync(new Orders { ID = 1, CustomerID = 1 });
            await context.SaveChangesAsync();

            var ordersProducts = new List<OrdersProducts>();
            for (var i = 0; i < 10; i++)
            {
                ordersProducts.Add(new OrdersProducts { OrderID = 1, ProductID = 1, Qty = i });
            }

            context.Set<OrdersProducts>().AddRange(ordersProducts);
            await context.SaveChangesAsync();


            // Act
            var ordersProductsRepository = new OrdersProductsRepository(context);

            var ordersProductsList = ordersProductsRepository.List(1, new ListParametersModel { EndRow = 5, StartRow = 0 });


            // Assert
            Assert.NotNull(ordersProductsList);

            Assert.Equal(ordersProducts.Skip(0).Take(5).Count(), await ordersProductsList.CountAsync());
        }

        [Fact]
        public async Task List_Success_WithStartAndEndRow()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.Set<Products>().AddAsync(new Products { ID = 1, Name = "Product 1", UnitPrice = 1 });
            await context.SaveChangesAsync();

            await context.Set<Orders>().AddAsync(new Orders { ID = 1, CustomerID = 1 });
            await context.SaveChangesAsync();

            var ordersProducts = new List<OrdersProducts>();
            for (var i = 0; i < 10; i++)
            {
                ordersProducts.Add(new OrdersProducts { OrderID = 1, ProductID = 1, Qty = i });
            }

            context.Set<OrdersProducts>().AddRange(ordersProducts);
            await context.SaveChangesAsync();


            // Act
            var ordersProductsRepository = new OrdersProductsRepository(context);

            var ordersProductsList = ordersProductsRepository.List(1, new ListParametersModel { EndRow = 5, StartRow = 2 });


            // Assert
            Assert.NotNull(ordersProductsList);

            Assert.Equal(ordersProducts.Skip(2).Take(5).Count(), await ordersProductsList.CountAsync());
        }

        [Fact]
        public async Task List_Success_WithoutFilters()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var orderId = 1L;

            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.Set<Products>().AddAsync(new Products { ID = 1, Name = "Product 1", UnitPrice = 1 });
            await context.SaveChangesAsync();

            await context.Set<Orders>().AddAsync(new Orders { ID = orderId, CustomerID = 1 });
            await context.Set<Orders>().AddAsync(new Orders { ID = 2, CustomerID = 1 });
            await context.SaveChangesAsync();

            var ordersProducts = new OrdersProducts[]
            {
                new OrdersProducts { OrderID = orderId, ProductID = 1, Qty = 1 },
                new OrdersProducts { OrderID = orderId, ProductID = 1, Qty = 2 },
                new OrdersProducts { OrderID = 2, ProductID = 1, Qty = 3 },
            };

            context.Set<OrdersProducts>().AddRange(ordersProducts);
            await context.SaveChangesAsync();


            // Act
            var ordersProductsRepository = new OrdersProductsRepository(context);

            var ordersProductsList = ordersProductsRepository.List(orderId, new ListParametersModel());


            // Assert
            Assert.NotNull(ordersProductsList);

            Assert.Equal(ordersProducts.Where(x => x.OrderID == orderId).Count(), await ordersProductsList.CountAsync());
        }

        [Fact]
        public async Task RemoveAsync_Fail_InvalidOrderProductId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.Set<Products>().AddAsync(new Products { ID = 1, Name = "Product 1", UnitPrice = 1 });
            await context.SaveChangesAsync();

            await context.Set<Orders>().AddAsync(new Orders { ID = 1, CustomerID = 1 });
            await context.SaveChangesAsync();

            var orderProductId = 2;
            var orderProduct = new OrdersProducts { ID = 1, OrderID = 1, ProductID = 1, Qty = 1 };

            context.Set<OrdersProducts>().Add(orderProduct);
            await context.SaveChangesAsync();


            // Act
            var ordersProductsRepository = new OrdersProductsRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await ordersProductsRepository.RemoveAsync(orderProductId);
            });


            // Assert
            Assert.NotNull(ex);

            Assert.Equal(new EntityNotFoundException(nameof(OrdersProducts), orderProductId).Message, ex.Message);
        }

        [Fact]
        public async Task RemoveAsync_Success_ValidOrderProductId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.Set<Products>().AddAsync(new Products { ID = 1, Name = "Product 1", UnitPrice = 1 });
            await context.SaveChangesAsync();

            await context.Set<Orders>().AddAsync(new Orders { ID = 1, CustomerID = 1 });
            await context.SaveChangesAsync();

            var orderProductId = 1;
            var orderProduct = new OrdersProducts { ID = orderProductId, OrderID = 1, ProductID = 1, Qty = 1 };

            context.Set<OrdersProducts>().Add(orderProduct);
            await context.SaveChangesAsync();


            // Act
            var ordersProductsRepository = new OrdersProductsRepository(context);

            await ordersProductsRepository.RemoveAsync(orderProductId);
            await context.SaveChangesAsync();


            // Assert
            var dbOrderProduct = await ordersProductsRepository.FindByIdAsync(orderProductId);

            Assert.Equal(0, await context.Set<OrdersProducts>().CountAsync());
            Assert.NotNull(dbOrderProduct);
            Assert.True(dbOrderProduct.IsDeleted);
        }

        [Fact]
        public async Task UpdateAsync_Fail_NewOrderProduct()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.Set<Products>().AddAsync(new Products { ID = 1, Name = "Product 1", UnitPrice = 1 });
            await context.SaveChangesAsync();

            await context.Set<Orders>().AddAsync(new Orders { ID = 1, CustomerID = 1 });
            await context.SaveChangesAsync();


            // Act
            var ordersProductsRepository = new OrdersProductsRepository(context);

            await ordersProductsRepository.UpdateAsync(new OrdersProducts { ID = 1, OrderID = 1, ProductID = 1, Qty = 1 });

            var method = async () =>
            {
                await context.SaveChangesAsync();
            };


            // Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(method);
        }

        [Fact]
        public async Task UpdateAsync_Success_ExistingOrderProduct()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.Set<Products>().AddAsync(new Products { ID = 1, Name = "Product 1", UnitPrice = 1 });
            await context.SaveChangesAsync();

            await context.Set<Orders>().AddAsync(new Orders { ID = 1, CustomerID = 1 });
            await context.SaveChangesAsync();

            var orderProductId = 1;

            context.Set<OrdersProducts>().Add(new OrdersProducts { ID = orderProductId, OrderID = 1, ProductID = 1, Qty = 1 });
            await context.SaveChangesAsync();


            // Act
            var ordersProductsRepository = new OrdersProductsRepository(context);

            var orderProduct = await ordersProductsRepository.FindByIdAsync(orderProductId);
            orderProduct!.Qty = 2;

            await ordersProductsRepository.UpdateAsync(orderProduct);
            await context.SaveChangesAsync();


            // Assert
            var dbOrderProduct = await ordersProductsRepository.FindByIdAsync(orderProductId);

            Assert.NotNull(dbOrderProduct);
            Assert.Equal(orderProduct.Qty, dbOrderProduct.Qty);
        }

        [Fact]
        public async Task ValidateAsync_Fail_InvalidOrderProduct_ProductIDInvalid()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.SaveChangesAsync();

            await context.Set<Orders>().AddAsync(new Orders { ID = 1, CustomerID = 1 });
            await context.SaveChangesAsync();


            // Act
            var ordersProductsRepository = new OrdersProductsRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await ordersProductsRepository.ValidateAsync(new OrdersProducts { ProductID = 1, Qty = 1 });
            });


            // Assert
            Assert.NotNull(ex);

            Assert.IsType<EntityValidationFailureException>(ex);

            Assert.Equal($"Entity '{nameof(OrdersProducts)} ({0})' has validation problems.", ex.Message);

            Assert.True(((EntityValidationFailureException)ex).ValidationResult.Errors.ContainsKey(nameof(OrdersProducts.ProductID)));

            Assert.Contains("invalid", ((EntityValidationFailureException)ex).ValidationResult.Errors[nameof(OrdersProducts.ProductID)]);
        }
        
        [Fact]
        public async Task ValidateAsync_Fail_InvalidOrderProduct_QtyInvalid()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.Set<Products>().AddAsync(new Products { ID = 1, Name = "Product 1", UnitPrice = 1 });
            await context.SaveChangesAsync();

            await context.Set<Orders>().AddAsync(new Orders { ID = 1, CustomerID = 1 });
            await context.SaveChangesAsync();


            // Act
            var ordersProductsRepository = new OrdersProductsRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await ordersProductsRepository.ValidateAsync(new OrdersProducts { ProductID = 1, Qty = 0 });
            });


            // Assert
            Assert.NotNull(ex);

            Assert.IsType<EntityValidationFailureException>(ex);

            Assert.Equal($"Entity '{nameof(OrdersProducts)} ({0})' has validation problems.", ex.Message);

            Assert.True(((EntityValidationFailureException)ex).ValidationResult.Errors.ContainsKey(nameof(OrdersProducts.Qty)));

            Assert.Contains("min", ((EntityValidationFailureException)ex).ValidationResult.Errors[nameof(OrdersProducts.Qty)]);
        }

        [Fact]
        public async Task ValidateAsync_Success_ValidOrderProduct()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            await context.Set<Customers>().AddAsync(new Customers { ID = 1, Name = "Customer 1" });
            await context.Set<Products>().AddAsync(new Products { ID = 1, Name = "Product 1", UnitPrice = 1 });
            await context.SaveChangesAsync();

            await context.Set<Orders>().AddAsync(new Orders { ID = 1, CustomerID = 1 });
            await context.SaveChangesAsync();


            // Act
            var ordersProductsRepository = new OrdersProductsRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await ordersProductsRepository.ValidateAsync(new OrdersProducts { OrderID = 1, ProductID = 1, Qty = 1 });
            });


            // Assert
            Assert.Null(ex);
        }
    }
}