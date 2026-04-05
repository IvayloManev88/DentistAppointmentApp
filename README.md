# 🚀 Project Name - DentistAppointmentApp

---

This is a web application aiming to cover creating and managing dentist appointments, procedures done to the patient and adding variety of Manipulations.
Short overview of the application could be found on the following links:
- Functionality for non-registered users -> https://www.youtube.com/watch?v=hNavj5bNEMA
- Functionality for registered users (without Dentist role) -> https://www.youtube.com/watch?v=F_p9gZgrWxY
- Functionality for registered Dentist users -> https://www.youtube.com/watch?v=7X2z-Ucr3tI

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

This is a simple appointment, procedures and manipulations management service. It aims to provide non registered users with information what Manipulations does the Dentist perform. Also not registered user will be able to see the Partial view that shows free slots for appointments but will not be able to reserve slot without registration.

A register user will be able to create his/hers's own appointments, edit or delete them and access his/hers's Procedures done history. This will be used like client file of dentist manipulations performed.

---

## 🛠️ Technologies Used

| Technology            | Version  | Purpose                          |
|-----------------------|----------|----------------------------------|
| ASP.NET Core MVC      | 10.0.2   | Web framework                    |
| Entity Framework Core | 10.0.2   | ORM / Database access            |
| SQL Server / SQLite   | -        | Database                         |
| Bootstrap             |          | Frontend styling                 |
| Razor Pages / Views   | -        | Server-side HTML rendering       |
| Xunit	    		| -        | Testing                          |
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
git clone https://github.com/IvayloManev88/DentistAppointmentApp.git
cd DentistAppointmentApp
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

Important -> You need to apply database migration before starting the program as I am seeding data Runtime in Program.cs using a class DatabaseSeeder and seeding Roles, Users, Manipulations, Appointments, Procedures and Feedbacks.
The order of seeding is very important because we need Roles to be seeded in order to add Users. Manipulations should be seeded because Appointments, Procedures are dependent on them. Also in order to seed Feedbacks we need real Procedures seeded because a Feedback is connected to a specific Procedure.
 
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
However, separate Area for Dentist user is created meaning that if the user is Dentist he/she will automatically be redirected to the Dentist Area. Normal users (user without role "Dentist") will not have access to the Dentist Area.

The application is designed to work with only one dentist at a time - this is because of the use case I am trying to cover. In future if there is a need to extend the application to work with more dentist it can be extended. However, due to the goal to provide faster user experience. 

Dentist role will be described in detail below as it is crucial.

Also password requirements for now are disabled but when the application goes live in the future the password will have requirements for length, special symbols, numbers etc (probably RequireConfirmedAccount will remain false).
Not that all seeded user share the same password for easy testing and project evaluation from the examiners:
Seeded users:
UserName = "TestDentist@abv.bg" - user with Dentist Role
UserName = "ivo@abv.bg" - regular user
UserName = "pesho@abv.bg" - regular user
UserName = "gorgi@abv.bg" - regular user
UserName = "Milen@abv.bg" - regular user
Password for all is "123456"


I used ConfigureApplicationCookie for proper redirect if the current user is not logged in but want to use some of the functionality that require login. Upon dentist login he/she is automatically redirected to Dentist area views.

Identity was created using default Scaffold and adding the ApplicationUser to extend the Identity user by adding First Name and Last Name.

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
├── Data/Repositories     # Separate Repositories for testing purposes
├── Services/             # Business logic / service layer
├── Tests/                # Unit test for all services using XUnit
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
- [ ] Ajax requests for extracting the appointments created from the database in order to load free slots as selectable buttons.

---

## 💻 Usage

1. Navigate to /Register to create an account.
2. Log in at /Login.
3. Use the dashboard to manage your Appointments. Only the user created the appointment and the dentist user can edit, delete appointments.
4. Only Dentist users can manage Manipulations (edit, create, delete).
5. Only Dentist users can manage Procedures (edit, create, delete).
6. Logged in users can access the menu Manipulations only to view the available manipulations. Can access Appointments - create, edit and delete his/her own and will be able to see only their appointments. Can access to view (but not manage) their Procedures done.
7. Only registered users that have not yet left Feedback on their last visit (last Procedure) can leave Feedback. Dentist users also cannot leave feedback.
8. Not logged in users can only review what type of manipulations are available.
9. Dentist user can manage Manipulations (edit, create, delete). Dentist user can manage Procedures (edit, create, delete). Dentist user can manage Appointments (edit, create, delete) - implemented the Dentist management as separate Area from the normal users area.

## Overview of the application 

---

### Manipulations


Manipulations could be seen by everyone, however, only Dentist users can Create, Edit and Delete Manipulations. Normal user cannot and should not be able to change the Manipulations.

The application provides the ability for not registered user to see only the type of Manipulations the KM Smile Studio provides along with price range.

---

### Appointments

The logged (non "Dentist" user) will have the ability to only their Appointments, to Create a new Appointment, to Edit an Appointment, to Delete an Appointment.
Create Appointments could be done by two ways:
  1. First (easiest one) is by using the Partial view that shows weekly calendar. The way the Partial view works is it retrieves all currently created appointments for the week and list them by days. If a slot for a particular day is empty then the application creates a clickable button. By clicking the button the application will send the user to the Application creation page pre-loading selected date and time and the user should only input his/her phone number and manipulation. Also note that we cannot create Appointments in the past. Meaning that all buttons that are before the current Date/Time combination will also be inactive. Example -> we are trying to create an Appointment on Tuesday - 10:30 AM, Our system will show every slot for Monday as grayed out, also 09.00 and 10.00 Am slots will be grayed out. 11:00 will be available if not already taken.
  2. Second method is by "Create Appointment" button on home page or in tab "Appointments". Here automatically current date and time will be preloaded (because by default is preloaded 01.01.0001 which is completely horrible for the user). From here the user can create appointment more flexibly (he/she should not hit an appointment that exactly matches Date/Time of another created appointment). I have not yet implemented stricter logic - meaning that we can create appointment currently on 04.05.2026 at 10:00AM and then 04.05.2026 at 10:10 AM. I am leaving this as flexibility for Dentist user to create more flexibly appointments without the system limiting. However, in the use process this can change easily.

Additionally if we create manually Appointment for 10:10AM which is not in the regular slot list it will still be shown in the Partial view.
Partial view is considered as a weekly (not monthly view) because it will be cleaner this way. In most cases the users call for slots in the current or the following week. The partial has the ability to change weeks with two buttons at the top.

Note that I am currently working of an application that will be used by a single Dentist that is the reason that when creating Appointment I do not prompt the user to select Dentist but I assign the first one with that Role. This is on purpose and can easily be re-worked to accept new field for selecting Dentist. However, from user experience I want to make the Appointment creation process faster.

Note that an Appointment cannot be set for the same Time and Date. Also Appointments cannot be set for Date in the past. 

In future development I will create night or hourly job that automatically created Procedure when the Appointment has passed. However, it is not implemented in the current version because I was not sure when my project will be examined and if all Appointments are deleted my seeding will be useless. After the project is graded I will add this functionality which will ease the Dentist job in managing Appointments and manually creating Procedures. After this the Dentist should only change "ManipulationType" of the Procedure if needed and add Note - not mandatory. Meaning that most of the Procedures in the users files will be correct even without the Dentist making any change.
---

### Procedures


Procedures are created only by the Dentist user and represent personal patient record. 

A Patient (logged but not "Dentist" user) can see only their own Procedures created by their Dentist. Pagination was implemented. Also they can filter by Manipulation Type.

The Dentist could not create Procedures in the future. Meaning that Date of the Procedure should not be greater than the Today's date. Note that Procedures do not have Time as only Date is enough for client file.

The Dentist user can Create, Edit, Delete procedures and can View all Procedures done by him. Also the filter and Pagination are part of the Procedures for the Dentist users as well. Here they will be able to filter by User FistName + Last Name or Manipulation type.
Note that when a Dentist is working on a specific user (patient) client file and filtered by patient name, after editing, deleting a procedure, we save the filtering by name. 

---

### Feedback

Feedback should be available to be viewed by everyone - registered or not registered users.
Feedback should include text and rating 1-5 (teeth). The application then calculates average rating and displays it on top of the Feedback page. Also all left feedbacks will be available to read.

Non-registered users and dentist will never be able to leave feedback. However, they can view the page.
Registered users will be able to leave feedback only if all the below are true:
1. The user had Procedure done
2. The user did not leave feedback for their latest Procedure.

This flow means that we will be able to leave feedback for only recent visits which will provide more accurate representation of the Dentist's work.

---

### Future developments

* A scheduled daily job that automatically converts completed Appointments into Procedures. This will dramatically improve client file management as the Dentist should only edit manipulations(if needed) and mark some of the Procedures as "No show" for example - could be managed by using separate button for ease of use.
* Improving password security
* Preparing database settings for production - definitely better Connection string handling.
* Translations - for the current project examination the application is in English which will be saved. However, it will be used in Bulgarian so a translation should be implemented with the ability to change languages.
* Running the application on prod environment.

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

*Built as part of the **ASP.NET Advanced** course.*
