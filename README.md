# Bowel Movement Tracker

A focused, open-source web application designed for a very specific use case: tracking and logging bowel movements on a scale of 1 to 7. 



This tool is intended for individuals managing health conditions or anyone looking to track, analyse, and understand their lifestyle and digestive health patterns.

**Please Note:** This application is strictly designed for personal or family use. It is not intended to be a high-performance app for public access. It is best suited for a home server or a tightly restricted cloud environment.

## Current Status: Minimum Viable Product (MVP)
This project is currently in its early stages. It supports only the core, essential functionality required to log and track entries. 

**Planned Updates:**
* **Account Creation UI:** A simple interface to create users without needing to write SQL queries.
* **Data Visualisation:** Statistical graphs to easily spot patterns and health changes over time.
* **Calendar View:** A dedicated calendar page to navigate historical data easily.
* **Expanded Tracking:** More options and detailed views for daily entries.

## Tech Stack
* **Framework:** ASP.NET Core MVC (.NET 10)
* **Database:** SQL Server

## Hosting and Installation

### Option 1: Microsoft Azure (Recommended)
There are many free solutions available, but I recommend setting up a free-tier server on Azure. You can host both the application and the database at no cost.
* [Quickstart: Deploy an ASP.NET web app to Azure App Service](https://learn.microsoft.com/en-gb/azure/app-service/quickstart-dotnetcore)
* [Quickstart: Create a single database in Azure SQL Database](https://learn.microsoft.com/en-gb/azure/azure-sql/database/single-database-create-quickstart)

### Option 2: Personal Server
If you have a personal home server, you can host it yourself. Assuming you manage your own server, standard ASP.NET Core and SQL Server deployment methods apply.

### ⚠️ Important: Initial Database Setup
Because the user registration interface is not yet built, you must set up your initial account manually via your SQL database client:
1. Ensure your database is configured to use auto-generated `GUID`s for all primary keys.
2. Manually insert a record into the `User` table.
3. **Crucial Step:** A `Diary` object requires a `User ID`. You must manually create a `Diary` record and link it to the `GUID` of the user you just created.

## Support the Developer

If you find this tool useful, I would highly appreciate any donations to help fund further development! 

Additionally, if you need any help setting up the database, deploying the app, or configuring your server, please reach out to me here:

🔗 **[Mateusz Podeszwa | Software, Computer Designs, Tutorials | Patreon](https://www.patreon.com/mateuszpodeszwa)**

Thank you for checking out the project!
