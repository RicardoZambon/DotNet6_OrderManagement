using Microsoft.EntityFrameworkCore;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Repositories.Stock;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;

namespace UnitTests.RepositoryTests
{
    public class ProductsRepositoryTest : IDisposable
    {
        private readonly SharedDatabaseFixture databaseFixture;

        public ProductsRepositoryTest()
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
            var product = new Products { Name = "Product 1", UnitPrice = 1 };


            // Act
            var productsRepository = new ProductsRepository(context);

            await productsRepository.AddAsync(product);
            await context.SaveChangesAsync();


            // Assert
            var dbProduct = await context.Set<Products>().FirstOrDefaultAsync();

            Assert.Equal(1, await context.Set<Products>().CountAsync());

            Assert.NotNull(dbProduct);
            Assert.Equal(dbProduct.Name, product.Name);
        }

        [Fact]
        public async Task FindByIdAsync_Success_InvalidProductId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var product = new Products { ID = 1, Name = "Product 1" };

            context.Set<Products>().Add(product);
            await context.SaveChangesAsync();


            // Act
            var productsRepository = new ProductsRepository(context);

            var dbProduct = await productsRepository.FindByIdAsync(2);
            await context.SaveChangesAsync();


            // Assert
            Assert.Null(dbProduct);
        }

        [Fact]
        public async Task FindByIdAsync_Success_ValidProductId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var product = new Products { ID = 1, Name = "Product 1" };

            context.Set<Products>().Add(product);
            await context.SaveChangesAsync();


            // Act
            var productsRepository = new ProductsRepository(context);

            var dbProduct = await productsRepository.FindByIdAsync(1);
            await context.SaveChangesAsync();


            // Assert
            Assert.NotNull(dbProduct);
            Assert.Equal(dbProduct.Name, product.Name);
        }

        [Fact]
        public async Task List_Fail_MissingParameters()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var products = new Products[]
            {
                new Products { Name = "Product 1" },
                new Products { Name = "Product 2" },
                new Products { Name = "Product 3" },
            };

            context.Set<Products>().AddRange(products);
            await context.SaveChangesAsync();


            // Act
            var productsRepository = new ProductsRepository(context);

            var method = () =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _ = productsRepository.List(null);
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
            var products = new List<Products>();
            for (var i = 0; i < 10; i++)
            {
                products.Add(new Products { Name = $"Product {i}" });
            }

            context.Set<Products>().AddRange(products);
            await context.SaveChangesAsync();


            // Act
            var productsRepository = new ProductsRepository(context);

            var productsList = productsRepository.List(new ListParametersModel { EndRow = 5, StartRow = 0 });


            // Assert
            Assert.NotNull(productsList);

            Assert.Equal(products.Skip(0).Take(5).Count(), await productsList.CountAsync());
        }

        [Fact]
        public async Task List_Success_WithStartAndEndRow()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var products = new List<Products>();
            for (var i = 0; i < 10; i++)
            {
                products.Add(new Products { Name = $"Product {i}" });
            }

            context.Set<Products>().AddRange(products);
            await context.SaveChangesAsync();


            // Act
            var productsRepository = new ProductsRepository(context);

            var productsList = productsRepository.List(new ListParametersModel { EndRow = 5, StartRow = 2 });


            // Assert
            Assert.NotNull(productsList);

            Assert.Equal(products.Skip(2).Take(5).Count(), await productsList.CountAsync());
        }

        [Fact]
        public async Task List_Success_WithFilterName()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var products = new Products[]
            {
                new Products { Name = "Product 1" },
                new Products { Name = "John Doe 2" },
                new Products { Name = "Company 3" },
            };

            context.Set<Products>().AddRange(products);
            await context.SaveChangesAsync();


            // Act
            var productsRepository = new ProductsRepository(context);

            var parameters = new ListParametersModel();
            parameters.Filters.Add(nameof(Products.Name), "Product");

            var productsList = productsRepository.List(parameters);


            // Assert
            Assert.NotNull(productsList);

            Assert.Equal(products.Count(x => x.Name != null && x.Name.Contains("Product")), await productsList.CountAsync());
        }

        [Fact]
        public async Task List_Success_WithoutFilters()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var products = new Products[]
            {
                new Products { Name = "Product 1" },
                new Products { Name = "Product 2" },
                new Products { Name = "Product 3" },
            };

            context.Set<Products>().AddRange(products);
            await context.SaveChangesAsync();


            // Act
            var productsRepository = new ProductsRepository(context);

            var productsList = productsRepository.List(new ListParametersModel());


            // Assert
            Assert.NotNull(productsList);

            Assert.Equal(products.Length, await productsList.CountAsync());
        }

        [Fact]
        public async Task RemoveAsync_Fail_InvalidProductId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var productId = 2;
            var product = new Products { ID = 1, Name = "Product 1" };

            context.Set<Products>().Add(product);
            await context.SaveChangesAsync();


            // Act
            var productsRepository = new ProductsRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await productsRepository.RemoveAsync(productId);
            });


            // Assert
            Assert.NotNull(ex);

            Assert.Equal(new EntityNotFoundException(nameof(Products), productId).Message, ex.Message);
        }

        [Fact]
        public async Task RemoveAsync_Success_ValidProductId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var productId = 1;
            var product = new Products { ID = productId, Name = "Product 1" };

            context.Set<Products>().Add(product);
            await context.SaveChangesAsync();


            // Act
            var productsRepository = new ProductsRepository(context);

            await productsRepository.RemoveAsync(productId);
            await context.SaveChangesAsync();


            // Assert
            var dbProduct = await productsRepository.FindByIdAsync(productId);

            Assert.Equal(0, await context.Set<Products>().CountAsync());
            Assert.NotNull(dbProduct);
            Assert.True(dbProduct.IsDeleted);
        }

        [Fact]
        public async Task UpdateAsync_Fail_NewProduct()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange


            // Act
            var productsRepository = new ProductsRepository(context);

            await productsRepository.UpdateAsync(new Products { ID = 1, Name = "Product 2" });

            var method = async () =>
            {
                await context.SaveChangesAsync();
            };


            // Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(method);
        }

        [Fact]
        public async Task UpdateAsync_Success_ExistingProduct()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var productId = 1;

            context.Set<Products>().Add(new Products { ID = productId, Name = "Product 1", UnitPrice = 1 });
            await context.SaveChangesAsync();


            // Act
            var productsRepository = new ProductsRepository(context);

            var product = await productsRepository.FindByIdAsync(productId);
            product!.Name = "New Product 1";

            await productsRepository.UpdateAsync(product);
            await context.SaveChangesAsync();


            // Assert
            var dbProduct = await productsRepository.FindByIdAsync(productId);

            Assert.NotNull(dbProduct);
            Assert.Equal(dbProduct.Name, product.Name);
        }

        [Fact]
        public async Task ValidateAsync_Fail_InvalidProduct_NameDuplicated()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            context.Set<Products>().Add(new Products { ID = 1, Name = "Product 1", UnitPrice = 1 });
            await context.SaveChangesAsync();


            // Act
            var productsRepository = new ProductsRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await productsRepository.ValidateAsync(new Products { Name = "Product 1" });
            });


            // Assert
            Assert.NotNull(ex);

            Assert.IsType<EntityValidationFailureException>(ex);

            Assert.Equal($"Entity '{nameof(Products)} ({0})' has validation problems.", ex.Message);

            Assert.True(((EntityValidationFailureException)ex).ValidationResult.Errors.ContainsKey(nameof(Products.Name)));

            Assert.Contains("exists", ((EntityValidationFailureException)ex).ValidationResult.Errors[nameof(Products.Name)]);
        }

        [Fact]
        public async Task ValidateAsync_Fail_InvalidProduct_NameInvalid()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange


            // Act
            var productsRepository = new ProductsRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await productsRepository.ValidateAsync(new Products { Name = string.Empty, UnitPrice = 1 });
            });


            // Assert
            Assert.NotNull(ex);

            Assert.IsType<EntityValidationFailureException>(ex);

            Assert.Equal($"Entity '{nameof(Products)} ({0})' has validation problems.", ex.Message);

            Assert.True(((EntityValidationFailureException)ex).ValidationResult.Errors.ContainsKey(nameof(Products.Name)));

            Assert.Contains("required", ((EntityValidationFailureException)ex).ValidationResult.Errors[nameof(Products.Name)]);
        }

        [Fact]
        public async Task ValidateAsync_Fail_InvalidProduct_UnitPriceInvalid()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange


            // Act
            var productsRepository = new ProductsRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await productsRepository.ValidateAsync(new Products { Name = "Product 1", UnitPrice = -1 });
            });


            // Assert
            Assert.NotNull(ex);

            Assert.IsType<EntityValidationFailureException>(ex);

            Assert.Equal($"Entity '{nameof(Products)} ({0})' has validation problems.", ex.Message);

            Assert.True(((EntityValidationFailureException)ex).ValidationResult.Errors.ContainsKey(nameof(Products.UnitPrice)));

            Assert.Contains("min", ((EntityValidationFailureException)ex).ValidationResult.Errors[nameof(Products.UnitPrice)]);
        }

        [Fact]
        public async Task ValidateAsync_Success_ValidProduct()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange


            // Act
            var productsRepository = new ProductsRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await productsRepository.ValidateAsync(new Products { Name = "Product 1", UnitPrice = 1 });
            });


            // Assert
            Assert.Null(ex);
        }
    }
}