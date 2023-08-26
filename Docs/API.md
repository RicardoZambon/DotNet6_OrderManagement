# Zambon - Order Management - API Docs

## Running the API

From the repository root folder, run the ```docker-compose up```.

The API will be available at http://localhost:8080/swagger/index.html.


## API Endpoints

### Authentication
* **[POST] /Authentication/RefreshToken**<br />
Refresh the JWT token using a valid Refresh Token in the ```[Security].[RefreshTokens]```, expects to receive the user owner of the Refresh Token and the Refresh Token, return an object with a new JWT Token (expiration in 600 minutes by default) and a new Refresh Token (expiration in 10 days by default).

* **[POST] /Authentication/SiginIn**<br />
Validate the user credentials to grant access to the API and expect to receive a username and password to check against the ```[Security].[Users]``` and return an object with the JWT Token (expiration in 600 minutes by default) and the Refresh Token (expiration in 10 days by default).


### Customers
* **[PUT] /Customers/Add**<br />
Validate and add a new customer in the ```[General].[Customers]```, expect to receive a model with the customer to be inserted, and return an object with the customer representation.<br />
Response with status code 400 indicates validation issues with the insert model.

* **[PUT] /Customers/Delete**<br />
Delete existing customers in the ```[General].[Customers]```, expect to receive from the query a list of customer IDs to be deleted.<br />
Response with status code 404 indicates one or more customers were not found, in this case, the transaction is rolled back.

* **[GET] /Customers/Get**<br />
Return a customer in the ```[General].[Customers]```, expects to receive the entity ID to search for.<br />
Response with status code 404 indicates the customer was not found.

* **[POST] /Customers/List**<br />
Return a list of customers in the ```[General].[Customers]```, expects to receive a parameters object for pagination (```StartRow``` and ```EndRow```) and filtering (```Filters```).

* **[POST] /Customers/Update**<br />
Validate and update an existing new customer in the ```[General].[Customers]```, expects to receive a model with the customer to be updated, and returns an object with the customer representation.<br />
Response with status code 400 indicates validation issues with the update model.


### Products
* **[PUT] /Products/Add**<br />
Validate and add a new product in the ```[Stock].[Products]```, expect to receive a model with the product to be inserted, and return an object with the product representation.<br />
Response with status code 400 indicates validation issues with the insert model.

* **[PUT] /Products/Delete**<br />
Delete existing products in the ```[Stock].[Products]```, expect to receive from the query a list of product IDs to be deleted.<br />
Response with status code 404 indicates one or more products were not found, in this case, the transaction is rolled back.

* **[GET] /Products/Get**<br />
Return a product in the ```[Stock].[Products]```, expects to receive the entity ID to search for.<br />
Response with status code 404 indicates the product was not found.

* **[POST] /Products/List**<br />
Return a list of products in the ```[Stock].[Products]```, expects to receive a parameters object for pagination (```StartRow``` and ```EndRow```) and filtering (```Filters```).

* **[POST] /Products/Update**<br />
Validate and update an existing new product in the ```[Stock].[Products]```, expects to receive a model with the product to be updated, and returns an object with the product representation.<br />
Response with status code 400 indicates validation issues with the update model.


### Users
* **[PUT] /Users/Add**<br />
Validate and add a new user in the ```[Security].[Users]```, expect to receive a model with the user to be inserted, and return an object with the user representation.<br />
Response with status code 400 indicates validation issues with the insert model.

* **[PUT] /Users/Delete**<br />
Delete existing users in the ```[Security].[Users]```, expect to receive from the query a list of user IDs to be deleted.<br />
Response with status code 404 indicates one or more users were not found, in this case, the transaction is rolled back.

* **[GET] /Users/Get**<br />
Return a user in the ```[Security].[Users]```, expects to receive the entity ID to search for.<br />
Response with status code 404 indicates the user was not found.

* **[POST] /Users/List**<br />
Return a list of users in the ```[Security].[Users]```, expects to receive a parameters object for pagination (```StartRow``` and ```EndRow```) and filtering (```Filters```).

* **[POST] /Users/Update**<br />
Validate and update an existing new user in the ```[Security].[Users]```, expects to receive a model with the user to be updated, and returns an object with the user representation.<br />
Response with status code 400 indicates validation issues with the update model.
