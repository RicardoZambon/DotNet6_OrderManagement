# Zambon - Order Management - Dev Setup

## Database

Make sure you have a SQL Server instance running, in case of need, you can use a local SQL Server instance running with Docker:

```
docker run --name SQLServer2019 -e ACCEPT_EULA=Y -e SA_PASSWORD=SqlServer2019! -e MSSQL_SA_PASSWORD=SqlServer2019! -e "TZ=America/Sao_Paulo" -p 1433:1433 -v C:\Temp\Docker\SQLServer2019:/var/opt/mssql -e MSSQL_PID=Standard --hostname SQLServer2019 -d mcr.microsoft.com/mssql/server:2019-latest
```

Just adjust the local path ```C:\Temp\Docker\SQLServer2019``` for your needs.

## Connection string

With the database service running, adjust the connection string in the ```appsettings.Development.json``` file by matching the ```Server```,  ```User Id```, and  ```Password``` to your needs.

Alternatively, you can set User Secrets, without the need to modify the appsettings file:

- Right-click in solution > <b>Manager user secrets</b><br />
You can use the following template:<br />
(Remember to adjust the connection string according to your needs):

```
{
  "ConnectionStrings:DefaultConnection": "Server=(LOCAL); Initial Catalog=PartnerAssessment; User Id=OrderManagement; Password=OrderManagement;"
}
```

## Migrations

The application will apply automatically the database migrations, this way you don't need to manually apply them during development.

During the application start, there is a method that checks for missing migrations and applies them, along with this, it also checks for data initialization and sets default data.

There exists a method called ```HasData``` in EfCore Migration, but the problem with this is that these entities are tracked in the model snapshot, due to this I preferred to implement a custom method to initialize database data.
