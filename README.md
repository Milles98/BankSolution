# BankSolution ReadMe

## Table of Contents
1. [Overview](#overview)
2. [BankWeb](#bankweb)
3. [DataLibrary](#datalibrary)
4. [MoneyLaunderingSafetyMeasure](#moneylaunderingsafetymeasure)
5. [WebAPI](#webapi)
6. [Getting Started](#getting-started)
7. [Building the Solution](#building-the-solution)
8. [Running the Solution](#running-the-solution)
9. [Testing the Solution](#testing-the-solution)

## Overview
This solution consists of four projects:
- **[BankWeb](#bankweb)**: The web application for user interface and interaction.
- **[DataLibrary](#datalibrary)**: The data access layer of the solution.
- **[MoneyLaunderingSafetyMeasure](#moneylaunderingsafetymeasure)**: Console application implementing safety measures.
- **[WebAPI](#webapi)**: Exposes endpoints for programmatic interactions.

Each project serves a specific purpose in the overall solution.

**Azure link:** [https://millesbankapp.azurewebsites.net/](https://millesbankapp.azurewebsites.net/)

## BankWeb
**BankWeb** is a web application project that uses **ASP.NET Core 8.0**.

- **Responsibilities:**
  - User interface and interaction.
  - User management with **Microsoft.AspNetCore.Identity**.
  - Data access with **Microsoft.EntityFrameworkCore**.

- **Dependencies:**
  - References the **DataLibrary** project for data-related operations.

## DataLibrary
**DataLibrary** is a class library project that serves as the data access layer for the solution.

- **Technologies:**
  - **Entity Framework Core 8.0** for data access.
  - **AutoMapper** for object-to-object mapping.

- **Functionality:**
  - Includes services for handling business logic.

## MoneyLaunderingSafetyMeasure
**MoneyLaunderingSafetyMeasure** is a console application project using **.NET 8.0**. It implements safety measures against money laundering.

- **Technologies:**
  - **Microsoft.Extensions.Configuration** for configuration management.

- **Dependencies:**
  - References the **DataLibrary** project for data-related operations.

## WebAPI
**WebAPI** is a web API project that uses **ASP.NET Core 8.0**.

- **Functionality:**
  - Exposes endpoints for programmatic interactions.

- **Technologies:**
  - **AutoMapper** for object-to-object mapping.
  - **Swashbuckle** for API documentation.

- **Dependencies:**
  - References the **DataLibrary** project for data-related operations.

## Getting Started
To get started with this solution:

1. Ensure you have the **.NET 8.0 SDK** installed.
2. Clone this repository.
3. Open the solution in your preferred IDE (e.g., Visual Studio).

## Building the Solution
To build the solution, navigate to the root directory of the solution in your terminal and run:

bash dotnet build

## Running the Solution
To run the solution, navigate to the root directory of each project in your terminal and run:

bash dotnet run

## Testing the Solution
To test the solution, navigate to the root directory of the solution in your terminal and run:

bash dotnet test
