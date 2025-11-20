# WinstantPay Web Wallet (SOAP Version)

## Overview

This repository contains the source code for the WinstantPay Web Wallet application. It is a comprehensive solution for managing digital wallets, including features for merchants, orders, bank transfers, currency exchange, and blockchain integration.

**IMPORTANT NOTE:** This version of the application relies on SOAP services which are scheduled for deprecation. Future development should focus on migrating these dependencies to RESTful alternatives.

## Project Structure

The solution `TSG.sln` consists of the following key projects:

* **`Tsg.UI.Main`**: The main ASP.NET MVC 5 and Web API 2 application. This serves as the user interface and the primary API entry point.
* **`TSGWebApi`**: A separate Web API project, likely handling specific integrations or legacy API endpoints.
* **`Tsg.Business.Model`**: Contains business logic and domain models.
* **`TSG.DAL`**: Data Access Layer, handling database interactions.
* **`Tsg.Data.Repository`**: Repository pattern implementation for data access.
* **`TSG.ServiceLayer`**: Service layer orchestrating business logic and data access.
* **`TSG.Common`**: Shared utility classes, extensions, and helper methods.
* **`TSG.Models`**: Data Transfer Objects (DTOs) and view models.
* **`WinstantPayDb`**: Contains the Entity Framework data models (`.edmx`) for the WinstantPay database.
* **`TSG.Crypto`**: **IGNORE THIS PROJECT.** It contains example code only and is not part of the production build.

## Technologies

* **Framework**: .NET Framework 4.5.1 / 4.6 / 4.8
* **Web Framework**: ASP.NET MVC 5.2.6, ASP.NET Web API 5.2.6
* **ORM**: Entity Framework 6.4.0
* **Dependency Injection**: Autofac
* **Object Mapping**: AutoMapper
* **Logging**: log4net
* **API Documentation**: Swashbuckle (Swagger)
* **Frontend**: jQuery, KnockoutJS

## Setup and Installation

1. **Prerequisites**:
    * Visual Studio 2019 or later.
    * SQL Server (LocalDB or full instance).
    * .NET Framework Developer Pack (4.5.1, 4.6, 4.8).

2. **Clone the Repository**:

    ```bash
    git clone <repository-url>
    ```

3. **Restore Packages**:
    Open `TSG.sln` in Visual Studio. NuGet packages should restore automatically on build. If not, run the following command in the Package Manager Console:

    ```powershell
    Update-Package -reinstall
    ```

4. **Database Configuration**:
    * Update the connection strings in `Tsg.UI.Main/Web.config`.
    * **`ewalletDbCon`**: Standard SQL connection string.
    * **`WinstantPayEntitiesEntities`**: Entity Framework connection string.
    * Ensure your SQL Server instance is running and accessible.

5. **Run the Application**:
    * Set `Tsg.UI.Main` as the startup project.
    * Press `F5` to run in Debug mode.

## Configuration

Key configuration settings can be found in `Tsg.UI.Main/Web.config`:

* **`appSettings`**: Contains API keys, service endpoints (Pipit, Blockchain info, KYC), and email settings.
* **`system.serviceModel`**: Configures the SOAP service endpoints (`IGPWebService1`, `GPWebServiceSoap`).

## API Documentation

The API is documented using Swagger. Once the application is running, you can access the Swagger UI (typically at `/swagger`) to explore and test the API endpoints.

## Disclaimer

* **`TSG.Crypto`**: This project is for demonstration purposes only and should not be used in production.
* **SOAP Deprecation**: Be aware that the SOAP services integrated into this application are legacy and will be deprecated. Plan for migration accordingly.
