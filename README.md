# Event Booking System

## Description

The Event Booking System is a web application designed to facilitate the booking and management of events. It provides users with the ability to browse, book, and manage event reservations seamlessly.

## Features

-   User authentication and authorization
-   Event browsing and booking
-   Booking and reservation management
-   Admin panel for event management

## DEMO Images

### User Side

![Home Page](Images/Home%20Page.png)
![Home page after Login](Images/Home%20Page%20Logged%20in.png)
![Congratulations Page](Images/Congratulation%20Page.png)
![My Bookings Page](Images/My%20Bookings.png)
![Sample Ticket](Images/ticket-tech_conference_2024.jpg)

### Admin Side

![Admin panel](Images/Admin%20Panel.png)

## Frontend Information

-   **Technologies Used**: Razor Pages (CSHTML), CSS, JavaScript
-   **Port**: Shares the same ports as the backend: HTTP on port 5024 and HTTPS on port 5025

## Backend Information

-   **Technologies Used**: ASP.NET Core, Entity Framework Core, SQL Server
-   **Port**: Runs on port 5024 for HTTP and 5025 for HTTPS
-   **Database**: SQL Server with connection string configured in `appsettings.json`

## Technologies Used

-   ASP.NET Core 9.0
-   Entity Framework Core
-   SQL Server
-   BCrypt.Net for password hashing
-   JWT for authentication
-   BouncyCastle for cryptographic operations

### NuGet Packages

-   BCrypt.Net-Next 4.0.3
-   Microsoft.AspNetCore.Authentication.JwtBearer 9.0.4
-   Microsoft.EntityFrameworkCore.SqlServer 9.0.2
-   Microsoft.EntityFrameworkCore.Tools 9.0.2
-   Microsoft.Extensions.Configuration 9.0.2
-   Microsoft.Extensions.Configuration.Json 9.0.2
-   Microsoft.VisualStudio.Web.CodeGeneration.Design 9.0.0
-   Portable.BouncyCastle 1.9.0

## Installation Steps

1.  **Clone the Repository**:
    ```bash
    git clone <repository-url>
    ```
2.  **Install dotnet-ef Tool**:
    ```bash
    dotnet tool install --global dotnet-ef
    ```
3.  **Change Server Name in appsettings**:

    -   Open `appsettings.json` and update the `Server` value in the `constr` key to match your SQL Server instance.

4.  **Create the Database**:

    -   Create the database in SQL Server with the following command:

    ```sql
    CREATE DATABASE Event_Booking_System
    ```

5.  **Download NuGet Packages**:
    -   Restore the necessary NuGet packages using the following command:
    ```bash
    dotnet restore
    ```
6.  **Migration Step**:
    -   Apply migrations to set up the database schema:
    ```bash
    dotnet ef database update
    ```
7.  **Insert Data into Database**:
    -   Use the provided `database.sql` file to insert initial data into the database.
8.  **Build the Project**:
    -   Compile the project using:
    ```bash
    dotnet build
    ```
9.  **Run the Project**:
    -   Start the application with:
    ```bash
    dotnet run
    ```
