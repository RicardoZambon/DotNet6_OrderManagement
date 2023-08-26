# Zambon - Order Management - API Docs

## Running the API

From the repository root folder, run the ```docker-compose up```.

The API will be available at http://localhost:8080/swagger/index.html.

## API Endpoints

### Authentication
* **[POST] /Authentication/SiginIn**<br />
Validate the user credentials against the [Security].[Users] database, expects to receive a valid username and password as plain text.<br />
Returns an object with the access token (expiration in 600 minutes) and the refresh token (expiration in 10 days).

* **[POST] /Authentication/RefreshToken**<br />
Validate the refresh token against the [Security].[RefreshTokens] and updates the expired JWT token used to access the API, expecting to receive the username and a valid refresh token.
