# Document Processor Application - PhoneBook Tracker
A phonebook tracker application, allowing the user to seed and export data via local files, as well as via Azure Blob storage.

## Project Overview
This is a project that:
- Allows the user to create and store phonebook user details.
- The user can seed data from an XLS, XLSX or CSV File (local file).
- The user can export the databases items into a CSV, XLSX or CSV file, as well as exporting to an Azure Blob Storage instance.
- The data seeding element of the application will only be presented to the user if the database contains 0 elements upon startup.
- Finally, we also run an export of data based on a scheduled task, which by default is executed daily at 09:00 am (BST / GMT UK time).
- Import and Export local files are stored in the following directory:

```JJHH17\bin\Debug\net8.0\```

## Technologies Used
- SQL Server (Local DB Instance) for database storage
- Entity Framework Core
- Spectre Console (for UI / Console navigation)
- Azure Blob Storage
- Excel Data Reader (package)
- CSV Helper (CSV Writing Package)
- Configuration Manager
- CRONOS package - For running the file export on an automated basis.

## Usage Steps
### Creating an SQL Server Local DB Instance
- This project uses a Local DB instance of SQL Server, meaning a Database file will need to be created in order for the program to run
- You can create a Local DB instance by running the following terminal command, with SQL Server installed:

```sqllocaldb create documentProcessor```

### Connection strings to Local DB
- Connection strings are managed via the "app.config" file.
- Keep the "Server" string as "localdb".
- Change the "Database Name" string to whatever value you name your local db instance as.
- These values will then be appended to the full database connection string.

### Creating and using the application
- Clone the application and open it in your IDE of choice.
- Run the migrations command for Entity Framework - this is done via the following commands:

```dotnet ef migrations add InitialCreate```

then

```dotnet ef database update```

This will create the relevant EF tables.

### File Importing
- The app allows users to import data via CSV, XLSX or XLS files.
If the apps database instance is empty when the app is started, the user is prompted to import data via the selected file type:

<img width="263" height="115" alt="image" src="https://github.com/user-attachments/assets/34bf4fdd-a49d-4938-9bd2-bd0b18e095aa" />

All Files must be stored within the following directory, with the following naming conventions:

```JJHH17\bin\Debug\net8.0\```

- For CSV: Import Data - Sheet1.csv
- For XLSX: Import Data - Sheet1.xlsx
- For XLS: Import Data - Sheet1.xls

### Import File Formats:
In order for data to be imported correctly, they must be in the correct format:

CSV:
- Name,Email Address,Telephone Number
  
XLSX and XLS:
These must contain the following cell headers:
Name     EmailAddress    TelephoneNumber

### File Exporting

<img width="302" height="98" alt="image" src="https://github.com/user-attachments/assets/8c8701a8-a21e-41f3-bbef-f24cf0d286ff" />

- All files exported can be located in the following location, inside of the projects directory:
```JJHH17\bin\Debug\net8.0\```

- Users can export as a CSV, PDF, or route the export to an Azure Blob Storage instance (more details on this found below).

### Automated File Export
- We use the CRONOS package for automating the export of the file export to a CSV file.
- This runs daily at 9am (BST / GMT (UK local time)).
- The class for this work can be found inside of the "Scheduled Export" folder.

### Azure Blob Storage Connection
For data exporting, users can export the file to an Azure Blob instance.

All connection strings can be located and adjusted in the App.Config file of the project.

The main requirements here are:
- The connection string - This can be gathered when a Blob container has been created.
- The container name - This is the blob container name where you will store the file.

Once these values have been added, any data found in the Database will be exported to your blob instance.
