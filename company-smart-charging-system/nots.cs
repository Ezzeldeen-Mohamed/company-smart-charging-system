
// onion architecture:->

/*
 
MyApp.sln
│
├── MyApp.Domain
│   ├── Entities
│   ├── Interfaces
│   ├── ValueObjects
│   └── Enums
│
├── MyApp.Application
│   ├── Interfaces
│   ├── Services
│   ├── DTOs
│   ├── Validators
│   └── Mappers
│
├── MyApp.Infrastructure
│   ├── Repositories
│   ├── Persistence
│   ├── Configurations
│   └── ExternalServices
│   └── AppDbContext
│   └── Migrations
│
└── MyApp.API
    ├── Controllers
    ├── Middlewares
    ├── Filters
    ├── DependencyInjection
    ├── Program.cs
    └── appsettings.json



لو هتكتب Unit Tests → إعمل Dependencies على Application و Domain.                    do test for logic of application  (no Database)

لو هتكتب Integration Tests → إعمل Dependencies على API و Infrastructure كمان.        Do test for integration between controllers and database ( Inmomery DB)






    */



