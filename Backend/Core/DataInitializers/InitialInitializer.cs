using System.Data;
using System.Numerics;
using System;
using Zambon.OrderManagement.Core.BusinessEntities.Security;
using Microsoft.EntityFrameworkCore;
using Zambon.OrderManagement.Core.BusinessEntities.Stock;
using Zambon.OrderManagement.Core.BusinessEntities.General;

namespace Zambon.OrderManagement.Core.DataInitializers
{
    public class InitialInitializer : BaseInitializer
    {
        public InitialInitializer(AppDbContext dbContext) : base(dbContext)
        {
        }

        public override void Initialize()
        {
            InitializeDefaultUser();

            // General
            InitializeCustomers();

            // Stock
            InitializeProducts();
            InitializeOrders();
            InitializeOrdersProducts();

            // Security
            InitializeUsers();
        }

        #region General

        private void InitializeCustomers()
        {
            var users = new Customers[]
            {
                new Customers { ID = 0, Name = "Customer 1" },
                new Customers { ID = 0, Name = "Customer 2" },
                new Customers { ID = 0, Name = "Customer 3" },
            };

            SaveContext(users, nameof(InitializeCustomers), x => x.Name);
        }

        #endregion

        #region Stock

        private void InitializeProducts()
        {
            var users = new Products[]
            {
                new Products { ID = 0, Name = "Product A", UnitPrice = 100 },
                new Products { ID = 0, Name = "Product B", UnitPrice = 200 },
                new Products { ID = 0, Name = "Product C", UnitPrice = 300 },
            };

            SaveContext(users, nameof(InitializeProducts), x => x.Name);
        }

        private void InitializeOrders()
        {
            var users = new Orders[]
            {
                new Orders { ID = 1, CustomerID = 1 },
                new Orders { ID = 2, CustomerID = 1 },
                new Orders { ID = 3, CustomerID = 1 },
                new Orders { ID = 4, CustomerID = 2 },
                new Orders { ID = 5, CustomerID = 2 },
                new Orders { ID = 6, CustomerID = 3 },
            };

            SaveContext(users, nameof(InitializeOrders));
        }

        private void InitializeOrdersProducts()
        {
            var users = new OrdersProducts[]
            {
                new OrdersProducts { ID = 1, OrderID = 1, ProductID = 1, Qty = 50, UnitPrice = 100 },
                new OrdersProducts { ID = 2, OrderID = 1, ProductID = 2, Qty = 20, UnitPrice = 200 },
                new OrdersProducts { ID = 3, OrderID = 1, ProductID = 3, Qty = 10, UnitPrice = 300 },

                new OrdersProducts { ID = 4, OrderID = 2, ProductID = 2, Qty = 80, UnitPrice = 200 },
                new OrdersProducts { ID = 5, OrderID = 2, ProductID = 3, Qty = 60, UnitPrice = 300 },

                new OrdersProducts { ID = 6, OrderID = 3, ProductID = 3, Qty = 250, UnitPrice = 300 },
            };

            SaveContext(users, nameof(InitializeOrdersProducts));
        }

        #endregion

        #region Security

        private void InitializeDefaultUser()
        {
            var users = new Users[]
            {
                new Users { ID = DefaultUserID, Username = "administartor", Password = "password", Name = "Administrator", Email = "admin@ordermanagement.com" }
            };

            SaveContext(users, nameof(InitializeDefaultUser));
        }

        private void InitializeUsers()
        {
            var users = new Users[]
            {
                new Users { ID = 0, Username = "ricardo.zambon", Name = "Ricardo Zambon", Email = "ricardo.zambon@ordermanagement.com" },
            };

            SaveContext(users, nameof(InitializeUsers), x => x.Username);
        }

        #endregion
    }
}