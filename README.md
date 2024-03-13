# RapidPay API

This is the README file for the RapidPay API project.

## Setup

### Prerequisites

- .NET 7 SDK
- MySQL Database Server
- Postman (optional for testing)

### Installation
- Clone the repository to your local machine.
- open RapidPay.sln in visual studio

### Configuration

- Update the connection string in `appsettings.json` with your database connection details.

### Database Migration

To apply database migrations, follow these steps:
- cd into RapidPay project
- Ensure EF Core Tools are Installed
    - dotnet tool install --global dotnet-ef
- Create Initial Migration
    - dotnet ef migrations add InitialMigration
- Update Database
    - dotnet ef database update

### Running the Application

- Build and run the application.
- The API will be accessible at the specified URL.
- Usage
- Endpoints
    - /api/v1/card/create: Create a new card.
    - /api/v1/card/pay: Make a payment.
    - /api/v1/card/{cardNumber}/balance: Get the balance of a card.
- For more detailed API documentation, refer to the Swagger UI by navigating to /swagger when the application is running.

### Testing

- Open a terminal or command prompt.
- Navigate to the test project directory.
- Run the tests using the following command:
    dotnet test