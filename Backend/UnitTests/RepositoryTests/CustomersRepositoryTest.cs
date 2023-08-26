using Microsoft.EntityFrameworkCore;
using Zambon.OrderManagement.Core.BusinessEntities.General;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Repositories.General;
using Zambon.OrderManagement.WebApi.Helpers.Exceptions;
using Zambon.OrderManagement.WebApi.Models;

namespace UnitTests.RepositoryTests
{
    public class CustomersRepositoryTest : IDisposable
    {
        private readonly SharedDatabaseFixture databaseFixture;

        public CustomersRepositoryTest()
        {
            databaseFixture = new SharedDatabaseFixture();
        }

        public void Dispose()
        {
            databaseFixture.Dispose();
            GC.SuppressFinalize(this);
        }


        [Fact]
        public async Task AddAsync_Success_NewCustomer()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var customer = new Customers { Name = "Customer 1" };


            // Act
            var customersRepository = new CustomersRepository(context);

            await customersRepository.AddAsync(customer);
            await context.SaveChangesAsync();


            // Assert
            var dbCustomer = await context.Set<Customers>().FirstOrDefaultAsync();

            Assert.Equal(1, await context.Set<Customers>().CountAsync());

            Assert.NotNull(dbCustomer);
            Assert.Equal(dbCustomer.Name, customer.Name);
        }

        [Fact]
        public async Task FindByIdAsync_Success_InvalidCustomerId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var customer = new Customers { ID = 1, Name = "Customer 1" };

            context.Set<Customers>().Add(customer);
            await context.SaveChangesAsync();


            // Act
            var customersRepository = new CustomersRepository(context);

            var dbCustomer = await customersRepository.FindByIdAsync(2);
            await context.SaveChangesAsync();


            // Assert
            Assert.Null(dbCustomer);
        }

        [Fact]
        public async Task FindByIdAsync_Success_ValidCustomerId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var customer = new Customers { ID = 1, Name = "Customer 1" };

            context.Set<Customers>().Add(customer);
            await context.SaveChangesAsync();


            // Act
            var customersRepository = new CustomersRepository(context);

            var dbCustomer = await customersRepository.FindByIdAsync(1);
            await context.SaveChangesAsync();


            // Assert
            Assert.NotNull(dbCustomer);
            Assert.Equal(dbCustomer.Name, customer.Name);
        }

        [Fact]
        public async Task List_Fail_MissingParameters()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var customers = new Customers[]
            {
                new Customers { Name = "Customer 1" },
                new Customers { Name = "Customer 2" },
                new Customers { Name = "Customer 3" },
            };

            context.Set<Customers>().AddRange(customers);
            await context.SaveChangesAsync();


            // Act
            var customersRepository = new CustomersRepository(context);

            var method = () =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _ = customersRepository.List(null);
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
            var customers = new List<Customers>();
            for (var i = 0; i < 10; i++)
            {
                customers.Add(new Customers { Name = $"Customer {i}" });
            }

            context.Set<Customers>().AddRange(customers);
            await context.SaveChangesAsync();


            // Act
            var customersRepository = new CustomersRepository(context);

            var customersList = customersRepository.List(new ListParametersModel { EndRow = 5, StartRow = 0 });


            // Assert
            Assert.NotNull(customersList);

            Assert.Equal(customers.Skip(0).Take(5).Count(), await customersList.CountAsync());
        }

        [Fact]
        public async Task List_Success_WithStartAndEndRow()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var customers = new List<Customers>();
            for (var i = 0; i < 10; i++)
            {
                customers.Add(new Customers { Name = $"Customer {i}" });
            }

            context.Set<Customers>().AddRange(customers);
            await context.SaveChangesAsync();


            // Act
            var customersRepository = new CustomersRepository(context);

            var customersList = customersRepository.List(new ListParametersModel { EndRow = 5, StartRow = 2 });


            // Assert
            Assert.NotNull(customersList);

            Assert.Equal(customers.Skip(2).Take(5).Count(), await customersList.CountAsync());
        }

        [Fact]
        public async Task List_Success_WithFilterByName()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var customers = new Customers[]
            {
                new Customers { Name = "Customer 1" },
                new Customers { Name = "John Doe 2" },
                new Customers { Name = "Company 3" },
            };

            context.Set<Customers>().AddRange(customers);
            await context.SaveChangesAsync();


            // Act
            var customersRepository = new CustomersRepository(context);

            var parameters = new ListParametersModel();
            parameters.Filters.Add(nameof(Customers.Name), "Customer");

            var customersList = customersRepository.List(parameters);


            // Assert
            Assert.NotNull(customersList);

            Assert.Equal(customers.Count(x => x.Name != null && x.Name.Contains("Customer")), await customersList.CountAsync());
        }

        [Fact]
        public async Task List_Success_WithoutFilters()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var customers = new Customers[]
            {
                new Customers { Name = "Customer 1" },
                new Customers { Name = "Customer 2" },
                new Customers { Name = "Customer 3" },
            };

            context.Set<Customers>().AddRange(customers);
            await context.SaveChangesAsync();


            // Act
            var customersRepository = new CustomersRepository(context);

            var customersList = customersRepository.List(new ListParametersModel());


            // Assert
            Assert.NotNull(customersList);

            Assert.Equal(customers.Length, await customersList.CountAsync());
        }

        [Fact]
        public async Task RemoveAsync_Fail_InvalidCustomerId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var customerId = 2;
            var customer = new Customers { ID = 1, Name = "Customer 1" };

            context.Set<Customers>().Add(customer);
            await context.SaveChangesAsync();


            // Act
            var customersRepository = new CustomersRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await customersRepository.RemoveAsync(customerId);
            });


            // Assert
            Assert.NotNull(ex);

            Assert.Equal(new EntityNotFoundException(nameof(Customers), customerId).Message, ex.Message);
        }

        [Fact]
        public async Task RemoveAsync_Success_ValidCustomerId()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var customerId = 1;
            var customer = new Customers { ID = customerId, Name = "Customer 1" };

            context.Set<Customers>().Add(customer);
            await context.SaveChangesAsync();


            // Act
            var customersRepository = new CustomersRepository(context);

            await customersRepository.RemoveAsync(customerId);
            await context.SaveChangesAsync();


            // Assert
            var dbCustomer = await customersRepository.FindByIdAsync(customerId);

            Assert.Equal(0, await context.Set<Customers>().CountAsync());
            Assert.NotNull(dbCustomer);
            Assert.True(dbCustomer.IsDeleted);
        }

        [Fact]
        public async Task UpdateAsync_Fail_NewCustomer()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange


            // Act
            var customersRepository = new CustomersRepository(context);

            await customersRepository.UpdateAsync(new Customers { ID = 1, Name = "Customer 2" });

            var method = async () =>
            {
                await context.SaveChangesAsync();
            };


            // Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(method);
        }

        [Fact]
        public async Task UpdateAsync_Success_ExistingCustomer()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            var customerId = 1;

            context.Set<Customers>().Add(new Customers { ID = customerId, Name = "Customer 1" });
            await context.SaveChangesAsync();


            // Act
            var customersRepository = new CustomersRepository(context);

            var customer = await customersRepository.FindByIdAsync(customerId);
            customer!.Name = "New Customer 1";

            await customersRepository.UpdateAsync(customer);
            await context.SaveChangesAsync();


            // Assert
            var dbCustomer = await customersRepository.FindByIdAsync(customerId);

            Assert.NotNull(dbCustomer);
            Assert.Equal(dbCustomer.Name, customer.Name);
        }

        [Fact]
        public async Task ValidateAsync_Fail_InvalidCustomer_NameDuplicated()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange
            context.Set<Customers>().Add(new Customers { ID = 1, Name = "Customer 1" });
            await context.SaveChangesAsync();


            // Act
            var customersRepository = new CustomersRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await customersRepository.ValidateAsync(new Customers { Name = "Customer 1" });
            });


            // Assert
            Assert.NotNull(ex);

            Assert.IsType<EntityValidationFailureException>(ex);

            Assert.Equal($"Entity '{nameof(Customers)} ({0})' has validation problems.", ex.Message);

            Assert.True(((EntityValidationFailureException)ex).ValidationResult.Errors.ContainsKey(nameof(Customers.Name)));

            Assert.Contains("exists", ((EntityValidationFailureException)ex).ValidationResult.Errors[nameof(Customers.Name)]);
        }

        [Fact]
        public async Task ValidateAsync_Fail_InvalidCustomer_NameInvalid()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange


            // Act
            var customersRepository = new CustomersRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await customersRepository.ValidateAsync(new Customers { Name = string.Empty });
            });


            // Assert
            Assert.NotNull(ex);

            Assert.IsType<EntityValidationFailureException>(ex);

            Assert.Equal($"Entity '{nameof(Customers)} ({0})' has validation problems.", ex.Message);

            Assert.True(((EntityValidationFailureException)ex).ValidationResult.Errors.ContainsKey(nameof(Customers.Name)));

            Assert.Contains("required", ((EntityValidationFailureException)ex).ValidationResult.Errors[nameof(Customers.Name)]);
        }

        [Fact]
        public async Task ValidateAsync_Success_ValidCustomer()
        {
            using var context = databaseFixture.CreateContext();

            // Arrange


            // Act
            var customersRepository = new CustomersRepository(context);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await customersRepository.ValidateAsync(new Customers { Name = "Customer 1" });
            });


            // Assert
            Assert.Null(ex);
        }
    }
}