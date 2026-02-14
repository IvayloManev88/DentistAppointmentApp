# 🚀 Project Name - DentistAppointmentApp

---

This is a web application aiming to cover creating and managing dentist appointments, procedures done to the patient and adding variety of Manipulations.

---
![.NET Version] Version="10.0.2";
![ASP.NET Core] Version="10.0.2";
![License] Apache-2.0 license;
* Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.2";
* Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.2";
* Microsoft.EntityFrameworkCore.Tools" Version="10.0.2"

---

## 📋 Table of Contents

- [About the Project](#about-the-project)
- [Technologies Used](#technologies-used)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Features](#features)
- [Usage](#usage)
- [Database Setup](#database-setup)
- [Configuration](#configuration)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

---

## 📖 About the Project

This is a simple appointment, procedures and manipulations management service. It aims to provide non registered users with information what Manipulations does the Dentist perform. A register user will be able to create his/hers's own appointments, edit or delete them and access his/hers's Procedures done history. This will be used like client file of dentist manipulations performed.

---

## 🛠️ Technologies Used

| Technology            | Version  | Purpose                          |
|-----------------------|----------|----------------------------------|
| ASP.NET Core MVC      | 10.0.2   | Web framework                    |
| Entity Framework Core | 10.0.2   | ORM / Database access            |
| SQL Server / SQLite   | -        | Database                         |
| Bootstrap             |          | Frontend styling                 |
| Razor Pages / Views   | -        | Server-side HTML rendering       |

---

## ✅ Prerequisites

Make sure you have the following installed before running the project:

- [.NET SDK 10.0.2](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) or SQLite (if used)
- [Git](https://git-scm.com/)

---

## 🚀 Getting Started

Follow these steps to get the project running locally.

### 1. Clone the repository

```bash
git clone https://github.com/your-username/your-repo-name.git
cd your-repo-name
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Apply database migrations

```bash
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run
```

The app will be available at `https://localhost:5001` or `http://localhost:5000` (for me it is 7286).


This service is created using \*\*.NET 10\*\* and the project initialization could be found in the "Program.cs" file.

The database configuration is located in `appsettings.json` under

`DefaultConnection`:



```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=DentistDb;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true"
  }
}
```

Added packages are the following:

* Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.2";
* Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.2";
* Microsoft.EntityFrameworkCore.Tools" Version="10.0.2"



After setting up the Connection string an initial DB migration should be performed -> using Package Manager Console (make sure you select the "Dentistapp.Data" as Default project as the DentistAppDbContext is situated there).

The Database uses EntityFramework core and is created using the "Code first from Database" method. Using the SQL Server Management Studio 21.



In the Program.cs file are situated and initialized all dependencies for correct Dependency Injection.

Also the default MVC Controller route is described.

Note that I am using "Dentist" role user for managing most of the functionality. This role will be described in detail below as it is crucial.

Also password requirements for now are disabled but when the application goes live in the future the password will have requirements for length, special symbols, numbers etc (probably RequireConfirmedAccount will remain false).

I used ConfigureApplicationCookie for proper redirect if the current user is not logged in but want to use some of the functionality that require login or access a menu used specifically by the "Dentist" Role but the current user does not have this role applied.

Identity was created using default Scaffold and adding the ApplicationUser to extend the Identity user by adding First Name and Last Name.

In order to properly use the application I added a ManagerController which makes the currently signed in user a Dentist. Meaning that one user should access the button "Make me dentist" next to "Procedures" in navigation bar.
This button will not be available in the production phase of the app and is made only for Softuni examiner to easily assign Dentist role to one user.

My application will be working with one Dentist because this is my business use case. 

If I need multiple dentists to work in the same workspace I will add functionality to support that behavior (drop-downs, additional validations etc).

---

## 📁 Project Structure

```
DentistAppointmentApp/
│
├── Controllers/          # MVC Controllers
├── ViewModels/           # ViewModels
├── Views/                # Razor Views (.cshtml)
├── Data/                 # DbContext and migrations
├── Data/Models           # Domain models
├── Services/             # Business logic / service layer
├── wwwroot/              # Static files (CSS, JS, images)
├── appsettings.json      # App configuration
└── Program.cs            # App entry point and middleware setup
```
---

## ✨ Features

- [ ] User registration and login (ASP.NET Identity)
- [ ] CRUD operations for [Manipulations, Appointments, Procedures]
- [ ] RESTful API endpoints - currently no
- [ ] Input validation (server-side & client-side)
- [ ] Responsive UI with Bootstrap

---

## 💻 Usage

1. Navigate to /Register to create an account.
2. Log in at /Login.
3. Create one Dentist by navigating after Login to "Make me dentist" in the navbar.
4. Use the dashboard to manage your Appointments. Only the user created the appointment and the dentist user can edit, delete appointments.
5. Only Dentist users can manage Manipulations (edit, create, delete).
5. Only Dentist users can manage Procedures (edit, create, delete).
6. Logged in users can access the menu Manipulations only to view the available manipulations. Can access Appointments - create, edit and delete his/her own and will be able to see all appointments in order to know not to create duplicate date and time appointments (I will be implementing better solution here in the future -> calendar with timeslots that are available and that are busy and the date and time from these timeslots will be preloaded in the Application Create page). Can access to view (but not manage) their Procedures done.
7. Not logged in users can only review what type of manipulations are available.
8. Dentist user can manage Manipulations (edit, create, delete). Dentist user can manage Procedures (edit, create, delete). Dentist user can manage Appointments (edit, create, delete) - in the advanced part of the exam I will implement the Dentist management as separate Area from the normal users area.

## Overview of the application 

---

### Appointments

The application provides the ability for not registered user to see only the type of Manipulations the KM Smile Studio provides along with price range.

The logged (non "Dentist" user) will have the ability to see all Appointments, to Create a new Appointment, to Edit an Appointment, to Delete an Appointment.

Note that I am currently working of an application that will be used by a single Dentist that is the reason that when creating Appointment I do not prompt the user to select Dentist but I assign the first one with that Role. This is on purpose and can easily be re-worked to accept new field for selecting Dentist. However, from user experience I want to make the Appointment creation process faster.

Note that an Appointment cannot be set for the same Time and Date. Also Appointments cannot be set for Date in the past. 

In future development I will validate Date and Time based on a new field for Manipulations - ManipulationDuration. Based on each ManipulationDuration I will not allow appointments to overlap. However, currently the application checks only for exact duplication of Date and Time of the Appointments.

---

### Manipulations


Manipulations could be seen by everyone, however, only Dentist users can Create, Edit and Delete Manipulations. Normal user cannot and should not be able to change the Manipulations.


Future developments here will be implementing the ManipulationDuration.

---

### Procedures


Procedures are created only by the Dentist user and represent personal patient record. 

A Patient (logged but not "Dentist" user) can see only their own Procedures created by their Dentist.

The Dentist could not create Procedures in the future. Meaning that Date of the Procedure should not be greater than the Today's date. Note that Procedures do not have Time as only Date is enough for client file.

The Dentist user can Create, Edit, Delete procedures and can View all Procedures done by him.

---

### Creating "Dentist"

A ManagerController is implemented for logged-in users to assign the Dentist role by accessing:

/Manager/AssignDentist



This controller will not be active when the application goes live. However, at least one user with "Dentist" role is needed in order to manage Manipulations and Procedures functionality.

---

### Future developments

* A scheduled daily job that automatically converts completed Appointments into Procedures. This will dramatically improve client file management as the Dentist should only edit manipulations(if needed) and mark some of the Procedures as "No show" for example - could be managed by using separate button for ease of use.
* Significant UI improvements
* Improved experience during appointment creation by using Time-slot selection directly from a calendar - By selecting the timeslot the Date and Time will be preloaded in the Application creation. Leaving the patient to select Phone number and Manipulation type only (phone number will be required as the user and the actual patient could differ - example is that older person or child will not have registration but a relative still can create Appointment using their account and entering different Phone number). 

---

## 🗄️ Database Setup

The project uses **Entity Framework Core** with a Code-First approach.

Connection string is configured in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DentistDb;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true"
}
```

To create the database:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## ⚙️ Configuration

Key settings in `appsettings.json`:

```json
{
   "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=DentistDb;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```
---

## 🤝 Contributing

Contributions are welcome! To contribute:

1. Fork the repository
2. Create a new branch: `git checkout -b feature/your-feature-name`
3. Commit your changes: `git commit -m "Add some feature"`
4. Push to the branch: `git push origin feature/your-feature-name`
5. Open a Pull Request

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

## 📬 Contact

**Your Name** – Ivaylo Manev [@your-github](https://github.com/IvayloManev88)

Project Link: [https://github.com/your-username/your-repo-name](https://github.com/IvayloManev88/DentistAppointmentApp)

---

*Built as part of the **ASP.NET Fundamentals** course.*
