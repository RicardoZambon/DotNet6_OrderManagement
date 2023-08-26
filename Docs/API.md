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

### Users
* **[PUT] /Users/Add**<br />
Validate and add a new user in the ```[Security].[Users]```, expects to receive a model with the user to be inserted, and returns an object with the user representation.<br />
Response with status code 400 indicates validation issues with the insert model.

* **[PUT] /Users/Delete**<br />
Delete existing users in the ```[Security].[Users]```, expects to receive from the query a list of user IDs to be deleted.<br />
Response with status code 404 indicates one or more users were not found, in this case, the transaction is rolled back.

* **[GET] /Users/Get**<br />
Return a user in the ```[Security].[Users]```, expects to receive the entity ID to search for.<br />
Response with status code 404 indicates the user was not found.

* **[POST] /Users/List**<br />
Return a list of users in the ```[Security].[Users]```, expects to receive a parameters object for pagination (```StartRow``` and ```EndRow```) and filtering (```Filters```).

* **[POST] /Users/Update**<br />
Validate and update an existing new user in the ```[Security].[Users]```, expects to receive a model with the user to be updated, and returns an object with the user representation.<br />
Response with status code 400 indicates validation issues with the update model.
