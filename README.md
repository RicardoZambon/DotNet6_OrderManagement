
# Zambon - Order Management - API Docs

## Docs

### <a href="/Docks/API.md">> API</a>
### <a href="Docks/DevSetup.md">> Dev environment setup</a>
### <a href="Docks/Docker">> Docker</a>
### <a href="Docks/Testing.md">> Testing</a>


## About this project
<b>Order Management System REST API</b>

This project was intended to develop a simplified Order Management System to demonstrate my abilities in C#, .NET, Microsoft SQL Server, Entity Framework, T-SQL, and stored procedures.

The system has REST API endpoints that read, create, update, and delete entities:

* **General**
  * Customers
* **Security**
  * Users
* **Stock**
  * Orders
  * Products

This project implements SOLID principles, along with the following design patterns:

* Dependency Injection (DI) 
* Repository Pattern 
* Unit of work 


## Project specification

### Entity design:
* Product: Products have an ID, name, and price.
* Order: Orders have an ID, a customer ID, a product ID, the quantity of the product, and the total cost.

### Data validation:
* At least validate that product ID exists when creating an order and that the quantity is positive.

### API endpoints:
* Product: A GET endpoint to read a product by its ID.
* Order: A POST endpoint to create a new order and a GET endpoint to read an order by its ID.

### Database access:
* Microsoft SQL Server and Entity Framework to interact with the database.<br />
 Defined the DbContext and the DbSet for each entity and wrote LINQ queries to perform the read operations.
* T-SQL will be used to create the tables and pre-populate them with some initial data.

### Data persistence:
* Wrote a stored procedure to calculate the total cost and create a new order.
* Call this stored procedure from your C# code using Entity Framework when the POST endpoint for creating a new order is hit.

### Authentication:
* All POST endpoints require authentication;
* Using ASP.NET Identity with JWT.<br />
***:large_orange_diamond: TODO:*** Optional: role-based authorization (no specifications for roles) 

### Testing:
* Critical unit tests to validate the functionality of the API endpoints and the stored procedure.
* Tests created using xUnit and MOQ.


## Extra implementations
Here are some extra features added to this project to highlight my work.

### Entity Design
* :white_check_mark: Added the ```Customers``` entity

### Testing
* :white_check_mark: Mocked database operations
* :black_square_button: ***TODO:*** Integration testing to test the stored procedure

### Authentication
* :white_check_mark: Use JWT authentication for the POST endpoint

### Pagination and sorting using query parameters
* :white_check_mark: Paginate and sort all GET endpoints

### Logging
* :black_square_button: ***TODO:*** Implement logging with Serilog or NLog

### Others
* :white_check_mark: Dockerized the application
* :black_square_button: ***TODO:*** API documentation using Swagger
* :black_square_button: ***TODO:*** Create a basic web client using TypeScript 
