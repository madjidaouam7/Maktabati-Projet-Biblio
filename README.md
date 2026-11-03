# 📚 Maktabati – Library Management System

<p align="center">
  A web-based library management application built with .NET 8, ASP.NET Core and Blazor.
</p>

<p align="center">
  <a href="README.fr.md">🇫🇷 Lire la documentation en français</a>
</p>

---

## 📖 About the Project

**Maktabati** is a library management web application designed to simplify library operations and provide an intuitive interface for managing books, authors, categories, loans and reservations.

The application follows a layered architecture inspired by **Clean Architecture**, with a lightweight implementation of **Domain-Driven Design (DDD)** and **CQRS** using **MediatR**.

The solution is developed with the .NET 8 ecosystem and is composed of an ASP.NET Core Web API, a domain layer, an application layer, an infrastructure layer and Blazor web interfaces.

## ✨ Features

### Authentication and Authorization
- User registration and login.
- JWT-based authentication.
- Role-based authorization.
- Three application roles:
  - Admin
  - Bibliothecaire
  - Lecteur
- ASP.NET Core Identity integration.

### Book Management
- Create, read, update and delete books.
- Associate books with authors and categories.
- ISBN uniqueness constraint.
- Track book stock and availability.
- Retrieve available books.

### Authors and Categories
- Create, read, update and delete authors.
- Create, read, update and delete categories.
- Search functionality for authors and categories.
- Administrative deletion permissions.

### Loans and Returns
- Create and manage book loans.
- View active loans.
- Consult loan history.
- Register book returns.
- Calculate book availability based on active loans.

### Reservations
- Reserve a book when its available stock is exhausted.
- Cancel reservations.
- Update reservation status when a reservation is fulfilled.

*Note: Reservation management is currently available in the older Blazor interface (`Biblio.Web`). It has not yet been integrated into the newer interface (`Biblio.Web.New`).*

### User Interface
- Modern landing page.
- Responsive layouts.
- Book catalog and search interface.
- Login and registration forms.
- Form validation and error handling.
- Interactive components and navigation.

## 🏗️ Architecture

Maktabati uses a layered architecture inspired by Clean Architecture principles.

The solution is divided into the following projects:

```text
Biblio/
│
├── Biblio.Domain/
│   └── Domain entities and business rules
│
├── Biblio.Application/
│   └── Commands, Queries and application messages
│
├── Biblio.Infrastructure/
│   └── Database, Identity, EF Core and MediatR handlers
│
├── Biblio.Api/
│   └── ASP.NET Core Web API and REST controllers
│
├── Biblio.Web/
│   └── Original Blazor web interface
│
├── Biblio.Web.New/
│   └── Newer Blazor web interface
│
└── Biblio.sln
```

### Domain Layer – Biblio.Domain

Contains the core business entities and domain rules.

Main entities include:
- Livre
- Auteur
- Categorie
- Emprunt
- Reservation

Some entities encapsulate business behavior and protect their state through domain methods and state transitions.

### Application Layer – Biblio.Application

Contains application messages organized by feature.

It defines:
- Commands for write operations.
- Queries for read operations.
- Application request contracts dispatched through MediatR.

### Infrastructure Layer – Biblio.Infrastructure

Provides the technical implementation required by the application.

Responsibilities include:
- Entity Framework Core database access.
- SQLite integration.
- Identity persistence.
- Database migrations.
- MediatR command and query handlers.

The handlers currently access `BiblioDbContext` directly. A separate Repository layer is not implemented.

### API Layer – Biblio.Api

Provides the REST API and acts as the entry point for backend operations.

Responsibilities include:
- REST controllers.
- Authentication and JWT generation.
- Identity and role configuration.
- API request validation.
- Dispatching application commands and queries through MediatR.
- API error handling.
- Swagger / OpenAPI documentation.

### Presentation Layer – Blazor

The solution contains two Blazor applications:

- `Biblio.Web`: the original interface.
- `Biblio.Web.New`: the newer interface, used in the setup instructions below.

Both communicate with the backend API. The newer interface includes JWT-based authentication state management.

The older interface is retained in the solution, but its authentication synchronization should be reviewed before choosing it as the primary interface.

## 🧩 Architectural Principles and Design Patterns

### Clean Architecture

The solution follows a layered organization inspired by Clean Architecture, separating domain logic, application messages, infrastructure and presentation.

Some implementation details differ from strict Clean Architecture: MediatR handlers are located in Infrastructure, and API controllers interact directly with some Identity and data-access components.

### CQRS – Command Query Responsibility Segregation

Maktabati uses a lightweight CQRS approach.

- **Commands** represent operations that modify application data.
- **Queries** represent operations that retrieve data.
- Dedicated handlers process these messages.

The read and write operations are separated at the application-message level, without separate read and write database models.

### MediatR

MediatR is used to dispatch Commands and Queries from API controllers to their corresponding handlers.

### Lightweight DDD

The domain layer contains entities that encapsulate some business rules and state transitions.

For example:
- `Livre` manages availability-related behavior.
- `Emprunt` handles the return operation.
- `Reservation` manages reservation state transitions.

The domain model remains intentionally lightweight. No explicit Value Objects, Domain Events or separately named Aggregates have been identified.

### Other Patterns and Practices
- Data Transfer Objects (DTOs).
- Dependency Injection.
- Entity Framework Core DbContext.
- Dedicated request handlers.
- Data Annotations validation.

## 🛠️ Technologies

| Technology | Version / Usage |
|---|---|
| C# | Main programming language |
| .NET | 8 |
| ASP.NET Core Web API | Backend REST API |
| Blazor Web App | Frontend |
| Blazor Interactive Server | Interactive rendering |
| Entity Framework Core | 8.0.11 |
| SQLite | Database |
| ASP.NET Core Identity | User and role management |
| JWT | Authentication |
| MediatR | 12.4.1 |
| Swagger / OpenAPI | 6.6.2 – Swashbuckle |
| Bootstrap | UI styling |
| JavaScript | Browser interoperability |

## 🗄️ Database

Maktabati currently uses **SQLite** as its database provider.

The database context, `BiblioDbContext`, derives from `IdentityDbContext<Utilisateur>` and manages the following main entities:

- Users and Identity data.
- Books.
- Authors.
- Categories.
- Loans.
- Reservations.

Entity Framework Core migrations are used to manage database schema changes.

The project includes migrations for the initial database creation and the addition of reservations.

The ISBN field has a unique index, and several relationships use restrictive deletion behavior to protect related data.

## 🔐 Authentication and Security

Authentication is implemented through ASP.NET Core Identity and JWT.

The authentication flow is as follows:

1. The user submits login credentials through the Blazor interface.
2. The frontend sends an authentication request to the API.
3. The API validates the credentials using Identity.
4. A signed JWT is generated and returned.
5. The frontend stores the token and updates its authentication state.
6. API endpoints use JWT validation and role-based authorization.

The JWT validation checks the signature, issuer, audience and expiration.

**Security considerations:**
- Never commit JWT signing keys or passwords to the repository.
- Configure secrets through environment variables or a secure configuration provider.
- Review API authorization rules for all operations that modify data.
- The current frontend token storage approach should be reviewed before production deployment.

## ⚙️ Prerequisites

Before running the project, make sure you have:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Git](https://git-scm.com/)
- Visual Studio 2022 or Visual Studio Code (optional)

SQLite is included through the EF Core provider. No separate database server is required for the current local setup.

## 🚀 Installation and Setup

### 1. Clone the repository

```bash
git clone https://github.com/madjidaouam7/Maktabati-Projet-Biblio
cd Maktabati
```

### 2. Restore dependencies

```powershell
dotnet restore .\Biblio.sln
```

### 3. Configure application secrets

Before launching the application, configure the JWT signing key securely.

The API and the newer Blazor interface must use compatible JWT signing configuration.

`Biblio.Web.New` expects the configuration variable:

```text
Jwt__Cle
```

Configure the corresponding API signing-key setting according to the API configuration.

Do not place real secrets in committed `appsettings.json` files.

### 4. Install Entity Framework Core tools

If `dotnet-ef` is not already installed:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.11
```

### 5. Apply database migrations

From the solution root:

```powershell
dotnet ef database update --project .\Biblio.Infrastructure\Biblio.Infrastructure.csproj --startup-project .\Biblio.Api\Biblio.Api.csproj
```

This command applies the available EF Core migrations to the configured SQLite database.

### 6. Configure the API URL

The web applications currently use:

```text
http://localhost:5117/
```

If you change the API address or deploy the applications elsewhere, update the API base URL in the relevant frontend configuration.

### 7. Run the backend API

Open a terminal and execute:

```powershell
dotnet run --project .\Biblio.Api\Biblio.Api.csproj --launch-profile http
```

The API is available at:

```text
http://localhost:5117
```

Swagger documentation is available in development mode at:

```text
http://localhost:5117/swagger
```

### 8. Run the Blazor frontend

Open another terminal and execute:

```powershell
dotnet run --project .\Biblio.Web.New\Biblio.Web.New.csproj --launch-profile http
```

The newer Blazor interface is available at:

```text
http://localhost:5150
```

## 🧪 Testing and Code Quality

The current solution includes:
- Data Annotations validation.
- Domain-level business rules in selected entities.
- API error handling.
- ASP.NET Core logging.
- Swagger / OpenAPI documentation.

Automated unit and integration test projects were not identified in the analyzed solution.

## ⚠️ Current Limitations and Future Improvements

The project is actively evolving. The following areas have been identified for improvement:

- Consolidate the two Blazor interfaces and establish one official frontend.
- Complete reservation management in the newer interface.
- Connect frontend search, sorting and pagination to the corresponding API parameters.
- Improve user management operations.
- Implement automatic overdue-loan calculation and updates.
- Improve dashboard statistics.
- Complete the notification system.
- Review and strengthen server-side authorization for loan returns and reservations.
- Improve JWT secret management and frontend token storage.
- Add unit and integration tests.
- Consider email confirmation and password recovery features.

## 🤝 Contributing

Contributions, suggestions and improvements are welcome.

If you want to contribute:

1. Fork the repository.
2. Create a feature branch.
3. Implement your changes.
4. Test your changes locally.
5. Submit a Pull Request with a clear description.

## 👨‍💻 Author

**Madjid AOUAM**

GitHub: [@madjidaouam7](https://github.com/madjidaouam7)

---

<p align="center">
  Built with .NET 8, ASP.NET Core and Blazor.
</p>

<p align="center">
  <a href="README.fr.md">🇫🇷 Documentation française</a>
</p>