# Zambon - Order Management - Testing

This project is testing the following classes and methods:

## Repositories

The repository tests are using ```Sqlite``` to execute the database operations on an in memory database, instead of mocking the DbSet from Entity Framework.

### Customers

* **AddAsync**
  * Success - New customer
* **FindByID**
  * Success - Invalid customer ID
  * Success - Valid customer ID
* **List**
  * Fail - Missing parameters
  * Success - With end row
  * Success - With start and end row
  * Success - With filter by ```Name```
  * Success - Without filters
* **RemoveAsync**
  * Fail - Invalid customer ID
  * Success - Valid customer ID
* **UpdateAsync**
  * Fail - New customer
  * Success - Existing customer
* **ValidateAsync**
  * Fail - Invalid customer - Name duplicated
  * Fail - Invalid customer - Name invalid
  * Success - Valid customer

### Orders Products

* **AddAsync**
  * Success - New order product
* **FindByID**
  * Success - Invalid order product ID
  * Success - Valid order product ID
* **List**
  * Fail - Missing parameters
  * Success - With end row
  * Success - With start and end row
  * Success - Without filters
* **RemoveAsync**
  * Fail - Invalid order product ID
  * Success - Valid order product ID
* **UpdateAsync**
  * Fail - New order product
  * Success - Existing order product
* **ValidateAsync**
  * Fail - Invalid order product - Product ID invalid
  * Fail - Invalid order product - Qty invalid
  * Success - Valid order product

### Orders

* **AddAsync**
  * Success - New order
* **FindByID**
  * Success - Invalid order ID
  * Success - Valid order ID
* **List**
  * Fail - Missing parameters
  * Success - With end row
  * Success - With start and end row
  * Success - With filter by ```CustomerID```
  * Success - Without filters
* **RemoveAsync**
  * Fail - Invalid order ID
  * Success - Valid order ID
* **UpdateAsync**
  * Fail - New customer
  * Success - Existing order
* **ValidateAsync**
  * Fail - Invalid order - Customer ID invalid
  * Success - Valid order
 
### Products

* **AddAsync**
  * Success - New product
* **FindByID**
  * Success - Invalid product ID
  * Success - Valid product ID
* **List**
  * Fail - Missing parameters
  * Success - With end row
  * Success - With start and end row
  * Success - With filter by ```Name```
  * Success - Without filters
* **RemoveAsync**
  * Fail - Invalid product ID
  * Success - Valid product ID
* **UpdateAsync**
  * Fail - New product
  * Success - Existing product
* **ValidateAsync**
  * Fail - Invalid product - Name duplicated
  * Fail - Invalid product - Name invalid
  * Fail - Invalid product - Unit price invalid
  * Success - Valid product

### Refresh Tokens

* **FindByIdAsync**
  * Success - Invalid token
  * Success - Invalid username
  * Success - Valid username and token
* **InsertAsync**
  * Fail - Invalid refresh token
  * Success - Valid refresh token
* **RevokeAsync**
  * Fail - Invlaid refresh token
  * Success - Valid refresh token

### Users

* **AddAsync**
  * Success - New user
* **FindByID**
  * Success - Invalid user ID
  * Success - Valid user ID
* **FindByIDs**
  * Success - Invalid user IDs
  * Success - Valid user IDs
* **FindByUsernameAsync**
  * Success - Invalid username
  * Success - Valid username
* **List**
  * Fail - Missing parameters
  * Success - With end row
  * Success - With start and end row
  * Success - With filter by ```Username```
  * Success - Without filters
* **RemoveAsync**
  * Fail - Invalid user ID
  * Success - Valid user ID
* **UpdateAsync**
  * Fail - New user
  * Success - Existing user
* **ValidateAsync**
  * Fail - Invalid user - Username duplicated
  * Fail - Invalid user - Username invalid
  * Success - Valid user


## Services

The services test uses ```Moq``` to mock repository interfaces, database, and other services.

### Authentication

* **RefreshTokenAsync**
  * Failure - Deactivated refresh token
  * Failure - Invalid model
  * Failure - Invalid refresh token
  * Failure - Invalid username
  * Failure - Rollback
  * Failure - User not found
  * Success - Valid authentication response
* **SignInAsync**
  * Failure - Invalid model
  * Failure - Invalid password
  * Failure - Invalid username
  * Failure - User not found
  * Failure - User without password
  * Failure - User with wrong password
  * Success - User with valid password

### Customer

* **FindCustomerByIdAsync**
  * Fail - Invalid customer ID
  * Success - Valid customer ID
* **InsertNewCustomerAsync**
  * Success
* **ListCustomers**
  * Success
* **RemoveCustomersAsync**
  * Success
* **UpdateExistingCustomerAsync**
  * Failure - Entity not found
  * Success - Valid customer model

### Orders Products

* **BatchUpdateOrdersProductsAsync**
  * Failure - Add entities
  * Failure - Add invalid entities
  * Failure - Delete invalid entities
  * Failure - Entity not found
  * Failure - Update entities
  * Failure - Update invalid entity
  * Success - Add entities
  * Success - Delete entities
  * Success - No entities updated
  * Success - Update entities
* **ListOrdersProductsAsync**
  * Failure - Invalid order ID
  * Success - Empty results
  * Success - Valid results

### Orders

* **FindOrderByIdAsync**
  * Fail - Invalid order ID
  * Success - Valid order ID
* **InsertNewOrderAsync**
  * Success
* **ListOrders**
  * Success
* **RemoveOrdersAsync**
  * Success
* **UpdateExistingOrderAsync**
  * Failure - Entity not found
  * Success - Valid order model

### Products

* **FindProductByIdAsync**
  * Fail - Invalid product ID
  * Success - Valid product ID
* **InsertNewProductAsync**
  * Success
* **ListProducts**
  * Success
* **RemoveProductsAsync**
  * Success
* **UpdateExistingProductAsync**
  * Failure - Entity not found
  * Success - Valid product model

### Users

* **FindUserByIdAsync**
  * Fail - Invalid user ID
  * Success - Valid user ID
* **InsertNewUserAsync**
  * Success
* **ListUsers**
  * Success
* **RemoveUsersAsync**
  * Success
* **UpdateExistingUserAsync**
  * Failure - Entity not found
  * Success - Valid user model


## Controllers

The controller test uses ```Moq``` to mock service interfaces.

### Authentication

* **RefreshToken**
  * Failure - General exception
  * Failure - Invalid refresh token exception
  * Failure - Refresh token not found exception
  * Success
* **SignIn**
  * Failure - General exception
  * Failure - Invalid authentication exception
  * Success

### Customer

* **Add**
  * Failure - General exception
  * Failure - Validation failure exception
  * Success
* **Delete**
  * Failure - General exception
  * Failure - Entity not found exception
  * Success
* **Get**
  * Failure - Entity not found exception
  * Failure - General exception
  * Success
* **List**
  * Failure - General exception
  * Success
* **Update**
  * Failure - General exception
  * Failure - Validation failure exception
  * Success

### Orders

* **Add**
  * Failure - General exception
  * Failure - Validation failure exception
  * Success
* **Delete**
  * Failure - General exception
  * Failure - Entity not found exception
  * Success
* **Get**
  * Failure - Entity not found exception
  * Failure - General exception
  * Success
* **List**
  * Failure - General exception
  * Success
* **ProductsList**
  * Failure - Entity not found exception
  * Failure - General exception
  * Success
* **ProductsUpdate**
  * Failure - Entity not found exception
  * Failure - Validation failure exception
  * Failure - General exception
  * Success
* **Update**
  * Failure - General exception
  * Failure - Validation failure exception
  * Success

### Products

* **Add**
  * Failure - General exception
  * Failure - Validation failure exception
  * Success
* **Delete**
  * Failure - General exception
  * Failure - Entity not found exception
  * Success
* **Get**
  * Failure - Entity not found exception
  * Failure - General exception
  * Success
* **List**
  * Failure - General exception
  * Success
* **Update**
  * Failure - General exception
  * Failure - Validation failure exception
  * Success

### Users

* **Add**
  * Failure - General exception
  * Failure - Validation failure exception
  * Success
* **Delete**
  * Failure - General exception
  * Failure - Entity not found exception
  * Success
* **Get**
  * Failure - Entity not found exception
  * Failure - General exception
  * Success
* **List**
  * Failure - General exception
  * Success
* **Update**
  * Failure - General exception
  * Failure - Validation failure exception
  * Success
