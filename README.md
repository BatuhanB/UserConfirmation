# User Confirmation Project

This project demonstrates a user confirmation system using .NET 8, Web API, Identity, Entity Framework, RabbitMQ, and PostgreSQL. The project is designed with clean code principles and separation of concerns in mind.

## Table of Contents

- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Setup](#setup)
- [Usage](#usage)
- [Technologies Used](#technologies-used)
- [Contributing](#contributing)

## Project Structure
    UserConfirmation
    │
    ├── UserConfirmation.sln
    ├── src
    │ ├── UserConfirmation.Api
    │ │ ├── Controllers
    │ │ │ └── AccountController.cs
    │ │ ├── Properties
    │ │ ├── appsettings.json
    │ │ ├── Program.cs
    │ │ └── UserConfirmation.Api.csproj
    │ │
    │ ├── UserConfirmation.Data
    │ │ ├── Migrations
    │ │ │ ├── 20240629101409_initial-mig.cs
    │ │ │ └── DbContextModelSnapshot.cs
    │ │ ├── Models
    │ │ │ ├── ApplicationRole.cs
    │ │ │ └── ApplicationUser.cs
    │ │ ├── DbContext.cs
    │ │ └── UserConfirmation.Data.csproj
    │ │
    │ ├── UserConfirmation.Services
    │ │ ├── Accounts
    │ │ │ ├── AccountService.cs
    │ │ │ └── IAccountService.cs
    │ │ ├── Confirmations
    │ │ │ ├── ConfirmationService.cs
    │ │ │ └── IConfirmationService.cs
    │ │ ├── MessageQueue
    │ │ │ ├── IMessageQueueService.cs
    │ │ │ └── MessageQueueService.cs
    │ │ ├── Worker
    │ │ │ └── ConfirmationWorker.cs
    │ │ ├── DependencyResolver.cs
    │ │ └── UserConfirmation.Services.csproj
    │ │
    │ ├── UserConfirmation.Shared
    │ │ ├── Models
    │ │ │ ├── ConfirmationRequest.cs
    │ │ │ ├── LoginModel.cs
    │ │ │ └── RegisterModel.cs
    │ │ └── UserConfirmation.Shared.csproj
    │
    ├── tests
    │ └── UserConfirmation.Tests
    │ ├── AccountServiceTests.cs
    │ ├── ConfirmationServiceTests.cs
    │ └── UserConfirmation.Tests.csproj
## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [RabbitMQ](https://www.rabbitmq.com/download.html)
- [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

## Setup

1. **Clone the repository:**
   ```sh
   git clone https://github.com/BatuhanB/UserConfirmation.git
   cd UserConfirmation
2. **Clone the repository:**
    - Create a PostgreSQL database.
    - Update the connection string in appsettings.json:
    ```sh
    "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=your_db;Username=your_user;Password=your_password"
    }
3. **Install RabbitMQ:**
    - Follow the installation guide for your operating system.
4. **Build the project:**
    ```sh
    dotnet build
5. **Run the migrations:**
    ```sh
    dotnet ef database update --project src/UserConfirmation.Api
6. **Run the application:**
    ```sh
    dotnet run --project src/UserConfirmation.Api
## Usage

### **API Endpoints**

- **Register:**
    ```sh
    POST /api/account/register
    Body: {
      "Email": "user@example.com",
      "Password": "Password123!"
    }
- **Login:**
    ```sh
    POST /api/account/login
    Body: {
      "Email": "user@example.com",
      "Password": "Password123!"
    }
The application will send a confirmation message to the RabbitMQ queue upon successful login. The ConfirmationWorker will process these messages and handle user confirmation.

### **Technologies Used**

- **.NET 8**: Modern, fast, and versatile framework for building web applications.
- **Entity Framework Core**: ORM for database access.
- **ASP.NET Identity**: Authentication and authorization.
- **RabbitMQ**: Message broker for handling background tasks.
- **PostgreSQL**: Relational database management system.
- **Moq**: Mocking library for unit tests.

### **Contributing**
Contributions are welcome! Please open an issue or submit a pull request.