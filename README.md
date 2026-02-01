##DentistApointmentApp

---

This is a web application aiming to cover creating and managing dentist appointments, procedures done to the patient and adding variety of Manipulations. Additionally it will have additional functionality later in the project's life

---

The aim of this document is to describe the way the application was meant to be used, it's core parts and specifications. Lastly I will describe the ways I plan to expand the application.

---

## Starting and initialization of the application



This service is created using \*\*.NET 10.0\*\* and the project initialization could be found in the "Program.cs" file.

The database configuration is located in `appsettings.json` under

`DefaultConnection`:



```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=DentistDb;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true"
  }
}


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

Identity was created using default Scaffold and adding the ApplicationUser to extend the Identity user.

---



## Overview of the application

---



### Appointments



The application provides the ability for not registered user to see only the type of Manipulations the KM Smile Studio provides along with price range.

The logged (non "Dentist" user) will have the ability to see all Appointments, to Create a new Appointment, to Edit an Appointment, to Delete an Appointment.

Note that I am currently working of an application that will be used by a single Dentist that is the reason that when creating Appointment I do not prompt the user to select Dentist but I assign the first one with that Role. This is on purpose and can easily be re-worked to accept new field for selecting Dentist. However, from user experience I want to make the Appointment creation process faster.

Note that an Appointment cannot be set for the same Time and Date. Also Appointments cannot be set for Date in the past. 

In future development I will validate Date and Time based on a new field for Manipulations - ManipulationDuration. Based on each ManipulationDuration I will not allow appointments to overlap. However, currently the application checks only for exact duplication of Date and Time of the Appointments.

Another future development for appointments will be the option to add files for example Xrays in order for the Dentist to have information in advance. 

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
