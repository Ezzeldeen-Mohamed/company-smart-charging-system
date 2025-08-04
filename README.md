# company-smart-charging-system


# 🔌 Smart Charging System - Backend (Clean Architecture)

## 📚 Project Description

This is a backend system for managing contracts, meters, charge transactions, and customers for a smart charging platform. The system tracks customer contracts, enforces business rules like minimum initial charge, and automatically closes contracts that haven't been charged in 6 months. It supports authentication, logging, reporting, background services, and import/export functionalities.

---

## 🧱 Entities and Relationships

### 1. **Contract**
- Fields: `unitqCode`, `customerId`, `meterId`, `customerCode`, `isClosed`, `onClosed`, `isDeleted`
- Relationships:
  - One Contract has **one Meter**
  - One Contract belongs to **one Customer**

### 2. **Customer**
- Fields: `id`, `chargeNumber`, `name`, `nationalId`, `serial`, `amountPadged`, `date`, `netPadged`
- Relationships:
  - One Customer has **many Contracts**

### 3. **Meter**
- Related to **one Contract**

### 4. **ChargeTransaction**
- Related to:
  - **One Contract**
  - **One Meter**

### 5. **User**
- Uses **ASP.NET Identity**

---

## ⚙️ Features

- ✅ Clean Architecture (Domain, Application, Infrastructure, API)
- 🔐 JWT Authentication (Access + Refresh Tokens)
- 🧠 Custom Middleware (Exception Handling + Logging)
- 📊 Customer Reports (via Stored Procedures)
- 📥 Import Excel Sheets
- 📤 Export PDF + Excel Sheets
- 🧾 Soft Delete with Query Filters
- 🕒 Background Services using Hangfire:
  - Automatically close contracts if no charge in 6 months
- 🧪 Unit Testing
- 🧰 Generic Repository Pattern
- 🧩 Extension Methods and API Configuration

---

## 🧷 Business Rules

1. First charge **must be at least 100**.
2. If a customer **does not charge for 6 months**, their contract will be **automatically closed**.

---

## 🧑‍💻 Tech Stack

- ✅ ASP.NET Core Web API
- ✅ Entity Framework Core
- ✅ SQL Server
- ✅ Clean Architecture
- ✅ JWT Authentication
- ✅ Hangfire
- ✅ ClosedXML (for Excel)
- ✅ iTextSharp / DinkToPdf (for PDF)
- ✅ AutoMapper (optional)
- ✅ GitHub + Git Flow

---

## 🚀 Getting Started

1. Clone the repo
2. Set your database connection string in `appsettings.json`
3. Run migrations and update the database
4. Run the API project
5. Access Swagger at: `/swagger/index.html`

---

## 📌 Task Breakdown (Sub Tasks)

### 🔹 Project Setup
- [ ] Create Clean Architecture layers
- [ ] Configure EF Core and database context
- [ ] Configure JWT authentication
- [ ] Setup custom middleware (logging & exception)

### 🔹 Entities & Database
- [ ] Implement Contract, Customer, Meter, ChargeTransaction entities
- [ ] Configure relationships using Fluent API
- [ ] Seed initial data (if needed)

### 🔹 Business Logic
- [ ] Add validation: First charge >= 100
- [ ] Background job: Auto-close contract after 6 months (Hangfire)
- [ ] Soft delete with global filters

### 🔹 Features
- [ ] Import Excel file (ClosedXML)
- [ ] Export Excel & PDF
- [ ] Reports using Stored Procedures
- [ ] Unit tests for services

### 🔹 Authentication
- [ ] User registration and login with JWT (access + refresh token)
- [ ] Protect endpoints with authorization

### 🔹 GitHub Workflow
- [ ] Create main/dev branches
- [ ] Each feature in a separate branch
- [ ] Pull Requests for review before merge

---

## 📬 Contact

Developed by the Engineering Team  
Powered by ASP.NET Core Clean Architecture  
