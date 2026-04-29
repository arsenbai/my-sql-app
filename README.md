# C# Dapper Practice Project

## 📌 Overview

This repository contains a **C# console application** created with **`dotnet new console`**.  
The main goal of this project is to **practice using the Dapper ORM** with **Microsoft SQL Server**, focusing on lightweight data access, raw SQL, and mapping results to C# objects.

The project is intentionally simple and educational.

---

## 🛠️ Prerequisites

- .NET SDK (6.0 or later recommended)
- Microsoft SQL Server (LocalDB or full SQL Server)
- SQL Server Management Studio (SSMS) or any SQL client
- Git

---

## 🚀 Getting Started

### Step 0: Fork the repository
Fork this repository to your own GitHub account and clone it locally:

```bash
git clone https://github.com/arsenbai/my-sql-app.git
cd my-sql-app
```

### Step 1: Restore dependencies
Go to Data/Connection/ConnectionToDb.cs
Change the following for your setting:
```csharp
string SERVER_NAME = "_____";
string DB_NAME = "_____";
```

### Step 1: Restore dependencies
Restore NuGet packages:
```bash
dotnet restore
```

### Step 2: Create the database
Create a SQL Server database named:
`TestDb`


### Step 3: Initialize the database schema
Run the SQL script provided with the assignment to:
- Create all required tables


### ✅ What This Project Covers
- Using NUnit Framework
- Using Dapper for data access
- Executing raw SQL queries
- Mapping query results to C# models
- Working with SQL Server from a .NET console application

### 📚 Notes
- This project is for learning and practice purposes
- No complex frameworks or abstractions are used
- Focus is on understanding how Dapper works internally