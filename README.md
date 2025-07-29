# Smart Charging System - Backend

## Project Overview

The Smart Charging System is designed to manage the relationship between customers, their contracts, electric meters, and charging operations. The goal is to build a secure and scalable backend system that ensures accurate tracking of charging transactions, monitors customer contract activity, and enforces specific business rules.

This backend is built using **ASP.NET Core** and follows the principles of **Clean Architecture**, providing separation of concerns between business logic, infrastructure, and presentation layers.

## Key Modules

### 1. Contracts
Each contract links a customer to a specific meter. It contains unique identification, customer and meter references, and flags to indicate if the contract is closed or deleted.

### 2. Customers
A customer can have multiple contracts. Each customer record includes personal details, charge history, and other key identifiers.

### 3. Meters
Each meter is tied to a single contract and is used to track the power usage and charging operations.

### 4. Charge Transactions
Represents individual charging operations and is associated with both a meter and a contract.

### 5. Users
User authentication and authorization are handled via ASP.NET Identity, integrated with JWT tokens for secure API access.

## System Features

- **Authentication & Authorization** using JWT (access and refresh tokens)
- **Clean Architecture** structure (Domain, Application, Infrastructure, API)
- **Soft Delete** support with global query filters
- **Custom Middleware** for centralized exception handling and logging
- **Background Services** using Hangfire for automated operations
- **Excel Import** and **PDF/Excel Export** of data
- **Stored Procedures** for efficient reporting
- **Generic Repository Pattern** for data access
- **Unit Testing** to ensure reliability and maintainability

## Stored Procedures

The system uses **stored procedures** in the database to handle specific operations that require optimized performance or complex querying logic.

### Use Cases:
- Generating detailed **customer reports** with aggregated charging data.
- Retrieving historical charge transactions filtered by date or status.
- Supporting export features (Excel/PDF) by preparing pre-joined result sets.
- Centralizing critical business logic to ensure consistency and reduce duplication in the application layer.

Using stored procedures helps improve **performance**, **security**, and **data integrity**, especially for reporting and read-heavy operations.


## Business Rules

- The **first charge** for a customer must be **at least 100 units**.
- If a customer does **not perform any charge for 6 months**, their contract will be automatically **closed** by the system.

## Technology Stack

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Hangfire
- JWT (Access & Refresh Tokens)
- ClosedXML (Excel import/export)
- iTextSharp / DinkToPdf (PDF generation)
- AutoMapper (optional)
- GitHub for version control and collaboration

## Future Scope

After completing the backend, the system will be extended with a frontend interface using modern web technologies. The backend has been designed in a way that makes it easy to integrate with any frontend framework.

---

