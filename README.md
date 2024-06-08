# HttpWebshopCookie

HttpWebshopCookie is a web application built using ASP.NET Core, designed to manage an online webshop. The application includes features for user authentication, product management, order processing, and session management. This project supports different configurations for development and production environments, with sensitive information securely managed through environment-specific configuration files and secrets.

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Environment Variables](#environment-variables)
- [Running the Application](#running-the-application)
- [License](#license)

## Features

- User authentication and authorization
- Product management
- Order processing
- Session management
- Environment-specific configurations

## Technologies Used

- ASP.NET Core
- Entity Framework Core
- Microsoft SQL Server
- MailKit for email services
- Razor Pages
- Smtp4Dev (during development)

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/HttpWebshopCookie.git
   cd HttpWebshopCookie
   ```

2. Install the required packages:
   ```bash
   dotnet restore
   ```

### Configuration

The application uses different configuration files for development and production environments.

#### appsettings.json

This file contains general configuration settings and serves as the base configuration.

#### appsettings.Development.json

This file contains settings specific to the development environment.

Example `appsettings.Development.json`:
```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=aspnet-HttpWebshopCookie;Trusted_Connection=True;MultipleActiveResultSets=true"
    },
    "SmtpSettings": {
        "Server": "localhost",
        "Port": 25,
        "SenderName": "BlomsterBinderiet",
        "SenderEmail": "blomsterbinderiet@cally.dk",
        "Username": "",
        "Password": ""
    }
}
```

#### appsettings.Production.json

This file contains settings specific to the production environment and should reference environment variables for sensitive data.

Example `appsettings.Production.json`:
```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Server=${DB_SERVER};Database=${DB_NAME};User Id=${DB_USER};Password=${DB_PASSWORD};TrustServerCertificate=true"
    },
    "SmtpSettings": {
        "Server": "${SMTP_SERVER}",
        "Port": 587,
        "SenderName": "${SMTP_SENDER_NAME}",
        "SenderEmail": "${SMTP_SENDER_EMAIL}",
        "Username": "${SMTP_USERNAME}",
        "Password": "${SMTP_PASSWORD}"
    }
}
```

### Environment Variables

For local development, you can use a `.env` file to manage your environment variables. Make sure to add this file to your `.gitignore` to avoid committing it to the repository.

Example `.env`:
```plaintext
DB_SERVER= ..
DB_NAME= ..
DB_USER= ..
DB_PASSWORD= ..
SMTP_SERVER= ..
SMTP_SENDER_NAME= ..
SMTP_SENDER_EMAIL= ..
SMTP_USERNAME= ..
SMTP_PASSWORD= .. 
```

### Running the Application

1. Ensure your database is set up and the connection strings in your configuration files are correct.

2. Run the application:
   ```bash
   dotnet run
   ```

3. Navigate to `https://localhost:5000` in your browser to access the application.

### License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
