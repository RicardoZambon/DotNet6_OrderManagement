using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Zambon.OrderManagement.Core;
using Zambon.OrderManagement.Core.BusinessEntities.General;
using Zambon.OrderManagement.Core.Helpers.Exceptions;
using Zambon.OrderManagement.Core.Interfaces;
using Zambon.OrderManagement.Core.Repositories.General.Interfaces;
using Zambon.OrderManagement.WebApi.Models;
using Zambon.OrderManagement.WebApi.Models.General;
using Zambon.OrderManagement.WebApi.Services.General;

namespace UnitTests.ServiceTests
{
    public class CustomersServiceDefaultTest
    {
        private readonly IMapper mapper;

        public CustomersServiceDefaultTest()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Zambon.OrderManagement.WebApi.Helpers.ValidationProblemEntityDetails).Assembly));
            configuration.CompileMappings();
            mapper = new Mapper(configuration);
        }


        [Fact]
        public async Task FindCustomerByIdAsync_Fail_InvalidCustomerId()
        {
            // Arrange
            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var customersRepositoryMock = new Mock<ICustomersRepository>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            customersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(await Task.FromResult<Customers>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.


            // Act
            var customerService = new CustomersServiceDefault(dbContextMock.Object, mapper, customersRepositoryMock.Object);

            var method = async () =>
            {
                _ = await customerService.FindCustomerByIdAsync(1);
            };


            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(method);
        }

        [Fact]
        public async Task FindCustomerByIdAsync_Success_ValidCustomerId()
        {
            // Arrange
            var customerId = 1L;
            var customer = new Customers { ID = customerId, Name = "Customer 1" };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var customersRepositoryMock = new Mock<ICustomersRepository>();

            customersRepositoryMock.Setup(x => x.FindByIdAsync(It.Is<long>(x => x == customerId)))
                .ReturnsAsync(customer);


            // Act
            var customerService = new CustomersServiceDefault(dbContextMock.Object, mapper, customersRepositoryMock.Object);

            var customerResult = await customerService.FindCustomerByIdAsync(customerId);


            // Assert
            customersRepositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<long>()), Times.Once);

            Assert.NotNull(customerResult);
            Assert.Equal(customer.ID, customerResult.ID);
            Assert.Equal(customer.Name, customerResult.Name);
        }

        [Fact]
        public async Task InsertNewCustomerAsync_Success()
        {
            // Arrange
            var customerModel = new CustomerInsertModel { Name = "Customer 1" };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var customersRepositoryMock = new Mock<ICustomersRepository>();


            // Act
            var customerService = new CustomersServiceDefault(dbContextMock.Object, mapper, customersRepositoryMock.Object);

            var customerResult = await customerService.InsertNewCustomerAsync(customerModel);


            // Assert
            customersRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Customers>()), Times.Once);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(customerModel.Name, customerResult.Name);
        }

        [Fact]
        public void ListCustomers_Success()
        {
            // Arrange
            var customers = new Customers[]
            {
                new Customers { Name = "Customer 1" },
                new Customers { Name = "Customer 2" },
                new Customers { Name = "Customer 3" },
            };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var customersRepositoryMock = new Mock<ICustomersRepository>();

            customersRepositoryMock.Setup(x => x.List(It.IsAny<IListParameters>()))
                .Returns(customers.AsQueryable());


            // Act
            var customerService = new CustomersServiceDefault(dbContextMock.Object, mapper, customersRepositoryMock.Object);

            var customersList = customerService.ListCustomers(new ListParametersModel());


            // Assert
            customersRepositoryMock.Verify(x => x.List(It.IsAny<IListParameters>()), Times.Once);

            Assert.NotNull(customersList);
            Assert.Equal(customers.Length, customersList.Count());
        }

        [Fact]
        public async Task RemoveCustomersAsync_Success()
        {
            // Arrange
            var customerIds = new long[] { 1, 2 };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var databaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();
            var customersRepositoryMock = new Mock<ICustomersRepository>();

            dbContextMock.Setup(x => x.Database)
                .Returns(databaseMock.Object);

            databaseMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            customersRepositoryMock.Setup(x => x.RemoveAsync(It.IsAny<long>()))
                .Returns(Task.CompletedTask);


            // Act
            var customerService = new CustomersServiceDefault(dbContextMock.Object, mapper, customersRepositoryMock.Object);

            await customerService.RemoveCustomersAsync(customerIds);


            // Assert
            databaseMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            customersRepositoryMock.Verify(x => x.RemoveAsync(It.IsAny<long>()), Times.Exactly(customerIds.Length));
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateExistingCustomerAsync_Failure_EntityNotFound()
        {
            // Arrange
            var customerModel = new CustomerUpdateModel { ID = 1, Name = "Customer 1" };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var customersRepositoryMock = new Mock<ICustomersRepository>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            customersRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(await Task.FromResult<Customers>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.


            // Act
            var customerService = new CustomersServiceDefault(dbContextMock.Object, mapper, customersRepositoryMock.Object);

            var method = async () =>
            {
                _ = await customerService.UpdateExistingCustomerAsync(customerModel);
            };


            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(method);
        }

        [Fact]
        public async Task UpdateExistingCustomerAsync_Success_ValidCustomerModel()
        {
            // Arrange
            var customer = new Customers { ID = 1, Name = "Customer 1" };
            var customerModel = new CustomerUpdateModel { ID = 1, Name = "Customer 2" };

            var dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var customersRepositoryMock = new Mock<ICustomersRepository>();

            customersRepositoryMock.Setup(x => x.FindByIdAsync(It.Is<long>(x => x == customer.ID)))
                .ReturnsAsync(customer);


            // Act
            var customerService = new CustomersServiceDefault(dbContextMock.Object, mapper, customersRepositoryMock.Object);

            var customerResult = await customerService.UpdateExistingCustomerAsync(customerModel);


            // Assert
            customersRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Customers>()), Times.Once);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(customerModel.Name, customerResult.Name);
        }
    }
}