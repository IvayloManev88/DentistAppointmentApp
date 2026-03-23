namespace DentistApp.Data.Configuration
{
    using static DentistApp.GCommon.Roles;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using DentistApp.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public static class DatabaseSeeder
    {
        public static void SeedRoles(IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles =
            {
                DentistRoleName,
                UserRoleName
            };
            foreach (var role in roles)
            {
                var roleExists = roleManager
                    .RoleExistsAsync(role)
                    .GetAwaiter()
                    .GetResult();

                if (!roleExists)
                {
                    var result = roleManager
                        .CreateAsync(new IdentityRole { Name = role })
                        .GetAwaiter()
                        .GetResult();
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role: {role}");
                    }
                }
            }
        }

        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            //Seed Dentist user - important because my application uses one dentist user and if there is no seed then it will have error exceptions thrown

            ApplicationUser? dentistUser = await userManager.FindByEmailAsync("TestDentist@abv.bg");

            if (dentistUser == null)
            {
                dentistUser = new ApplicationUser
                {
                    UserName = "TestDentist@abv.bg",
                    Email = "TestDentist@abv.bg",
                    FirstName = "Kristina",
                    LastName = "Maneva",
                    EmailConfirmed = true
                };
                //Just for exam purposes the passwords are easy. On prod they will be different 
                var result = await userManager.CreateAsync(dentistUser, "123456");

                if (!result.Succeeded)
                {
                    throw new Exception();
                }

                await userManager.AddToRoleAsync(dentistUser, DentistRoleName);
            }

            //Seed normal users
            ApplicationUser[] users = new ApplicationUser[]
            {
                new ApplicationUser
                 {
                    UserName = "ivo@abv.bg",
                    Email = "ivo@abv.bg",
                    FirstName = "Ivaylo",
                    LastName = "Manev",
                    EmailConfirmed = true
                },

                 new ApplicationUser
                 {
                    UserName = "pesho@abv.bg",
                    Email = "pesho@abv.bg",
                    FirstName = "Petur",
                    LastName = "Petrov",
                    EmailConfirmed = true
                },

                  new ApplicationUser
                 {
                    UserName = "gorgi@abv.bg",
                    Email = "gorgi@abv.bg",
                    FirstName = "Georgi",
                    LastName = "Georgiev",
                    EmailConfirmed = true
                },

                   new ApplicationUser
                 {
                    UserName = "Milen@abv.bg",
                    Email = "Milen@abv.bg",
                    FirstName = "Milen",
                    LastName = "Milenov",
                    EmailConfirmed = true
                }

            };
            foreach (var user in users)
            {
                ApplicationUser? normalUser = await userManager.FindByEmailAsync(user.Email!);
                if (normalUser == null)
                {
                    normalUser = user;

                    var result = await userManager.CreateAsync(normalUser, "123456");

                    if (!result.Succeeded)
                    {
                        throw new Exception();
                    }

                    await userManager.AddToRoleAsync(dentistUser, UserRoleName);
                }
            }
        }

        public static async Task SeedManipulationsAsync(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<DentistAppDbContext>();

            if (dbContext.ManipulationTypes.Any())
            {
                return;
            }

            ManipulationType[] manipulationsToSeed =
            {
                new ManipulationType
                {
                    ManipulationId = Guid.NewGuid(),
                    Name = "Dental Check-up",
                    PriceRange = "20-50",
                    IsDeleted = false
                },
                new ManipulationType
                {
                    ManipulationId = Guid.NewGuid(),
                    Name = "Teeth Cleaning",
                    PriceRange = "50-120",
                    IsDeleted = false
                },
                new ManipulationType
                {
                    ManipulationId = Guid.NewGuid(),
                    Name = "Filling",
                    PriceRange = "80",
                    IsDeleted = false
                },
                new ManipulationType
                {
                    ManipulationId = Guid.NewGuid(),
                    Name = "Root Canal Treatment",
                    PriceRange = "200",
                    IsDeleted = false
                }
            };
            await dbContext.AddRangeAsync(manipulationsToSeed);
            await dbContext.SaveChangesAsync();

        }



        public static async Task SeedAppointmentsAsync(IServiceProvider serviceProvider)
        {
            //Using this type of seeding as not sure what Guids will be generated when users are seeded
            var dbContext = serviceProvider.GetRequiredService<DentistAppDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (dbContext.Appointments.Any())
            {
                return;
            }

            ApplicationUser? dentist = await userManager.FindByEmailAsync("TestDentist@abv.bg");
            ApplicationUser? patientIvo = await userManager.FindByEmailAsync("ivo@abv.bg");
            ApplicationUser? patientPesho = await userManager.FindByEmailAsync("pesho@abv.bg");
            ApplicationUser? patientGeorgi = await userManager.FindByEmailAsync("gorgi@abv.bg");

            if (dentist == null || patientIvo == null || patientPesho == null || patientGeorgi == null)
            {
                throw new Exception("Users not found");
            }
            ManipulationType? manipulationCheckUp = await dbContext.ManipulationTypes
                .SingleOrDefaultAsync(m => m.Name == "Dental Check-up");
            ManipulationType? manipulationRoot = await dbContext.ManipulationTypes
                .SingleOrDefaultAsync(m => m.Name == "Root Canal Treatment");
            ManipulationType? manipulationFilling = await dbContext.ManipulationTypes
                .SingleOrDefaultAsync(m => m.Name == "Filling");
            ManipulationType? manipulationCleaning = await dbContext.ManipulationTypes
               .SingleOrDefaultAsync(m => m.Name == "Teeth Cleaning");

            List<Appointment> appointmentsToSeed = new List<Appointment>();

            if (manipulationCheckUp != null)
            {
                appointmentsToSeed.Add(new Appointment
                {
                    AppointmentId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientIvo.Id,
                    ManipulationTypeId = manipulationCheckUp.ManipulationId,
                    PatientPhoneNumber = "0876720270",
                    Date = new DateTime(2026, 4, 9, 10, 0, 0),
                    Note = "Initial consultation"
                });

                appointmentsToSeed.Add(new Appointment
                {
                    AppointmentId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientPesho.Id,
                    ManipulationTypeId = manipulationCheckUp.ManipulationId,
                    PatientPhoneNumber = "0876555555",
                    Date = new DateTime(2026, 4, 10, 13, 0, 0),
                    Note = "Initial consultation"
                });


                appointmentsToSeed.Add(new Appointment
                {
                    AppointmentId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientGeorgi.Id,
                    ManipulationTypeId = manipulationCheckUp.ManipulationId,
                    PatientPhoneNumber = "0876722122",
                    Date = new DateTime(2026, 4, 11, 15, 0, 0),
                    Note = "Initial consultation"
                });
            }

            if (manipulationCleaning != null)
            {
                appointmentsToSeed.Add(new Appointment
                {
                    AppointmentId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientIvo.Id,
                    ManipulationTypeId = manipulationCleaning.ManipulationId,
                    PatientPhoneNumber = "0876720270",
                    Date = new DateTime(2026, 4, 12, 9, 0, 0),
                    Note = "Teeth Cleaning"
                });

                appointmentsToSeed.Add(new Appointment
                {
                    AppointmentId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientPesho.Id,
                    ManipulationTypeId = manipulationCleaning.ManipulationId,
                    PatientPhoneNumber = "0876555555",
                    Date = new DateTime(2026, 4, 12, 10, 0, 0),
                    Note = "Teeth Cleaning"
                });

                appointmentsToSeed.Add(new Appointment
                {
                    AppointmentId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientGeorgi.Id,
                    ManipulationTypeId = manipulationCleaning.ManipulationId,
                    PatientPhoneNumber = "0876722122",
                    Date = new DateTime(2026, 4, 12, 14, 0, 0),
                    Note = "Teeth Cleaning"
                });
            }

            if (manipulationFilling != null)
            {
                appointmentsToSeed.Add(new Appointment
                {
                    AppointmentId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientIvo.Id,
                    ManipulationTypeId = manipulationFilling.ManipulationId,
                    PatientPhoneNumber = "0876720270",
                    Date = new DateTime(2026, 4, 15, 10, 0, 0),
                    Note = "Filling procedure"
                });

                appointmentsToSeed.Add(new Appointment
                {
                    AppointmentId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientPesho.Id,
                    ManipulationTypeId = manipulationFilling.ManipulationId,
                    PatientPhoneNumber = "0876555555",
                    Date = new DateTime(2026, 4, 16, 14, 0, 0),
                    Note = "Filling procedure"
                });

                appointmentsToSeed.Add(new Appointment
                {
                    AppointmentId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientGeorgi.Id,
                    ManipulationTypeId = manipulationFilling.ManipulationId,
                    PatientPhoneNumber = "0876722122",
                    Date = new DateTime(2026, 4, 17, 7, 0, 0),
                    Note = "Filling proceduren"
                });
            }

            if (manipulationRoot != null)
            {
                appointmentsToSeed.Add(new Appointment
                {
                    AppointmentId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientIvo.Id,
                    ManipulationTypeId = manipulationRoot.ManipulationId,
                    PatientPhoneNumber = "0876720270",
                    Date = new DateTime(2026, 4, 13, 17, 0, 0),
                    Note = "Root Canal"
                });

                appointmentsToSeed.Add(new Appointment
                {
                    AppointmentId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientPesho.Id,
                    ManipulationTypeId = manipulationRoot.ManipulationId,
                    PatientPhoneNumber = "0876555555",
                    Date = new DateTime(2026, 4, 13, 8, 0, 0),
                    Note = "Root Canal"
                });

                appointmentsToSeed.Add(new Appointment
                {
                    AppointmentId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientGeorgi.Id,
                    ManipulationTypeId = manipulationRoot.ManipulationId,
                    PatientPhoneNumber = "0876722122",
                    Date = new DateTime(2026, 4, 13, 11, 0, 0),
                    Note = "Root Canal"
                });
            }
            await dbContext.AddRangeAsync(appointmentsToSeed);
            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedProceduresAsync(IServiceProvider serviceProvider)
        {
            //Using this type of seeding as not sure what Guids will be generated when users are seeded
            var dbContext = serviceProvider.GetRequiredService<DentistAppDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (dbContext.Procedures.Any())
            {
                return;
            }

            ApplicationUser? dentist = await userManager.FindByEmailAsync("TestDentist@abv.bg");
            ApplicationUser? patientIvo = await userManager.FindByEmailAsync("ivo@abv.bg");
            ApplicationUser? patientPesho = await userManager.FindByEmailAsync("pesho@abv.bg");
            ApplicationUser? patientGeorgi = await userManager.FindByEmailAsync("gorgi@abv.bg");

            if (dentist == null || patientIvo == null || patientPesho == null || patientGeorgi == null)
            {
                throw new Exception("Users not found");
            }

            ManipulationType? manipulationCheckUp = await dbContext.ManipulationTypes
               .SingleOrDefaultAsync(m => m.Name == "Dental Check-up");
            ManipulationType? manipulationRoot = await dbContext.ManipulationTypes
                .SingleOrDefaultAsync(m => m.Name == "Root Canal Treatment");
            ManipulationType? manipulationFilling = await dbContext.ManipulationTypes
                .SingleOrDefaultAsync(m => m.Name == "Filling");
            ManipulationType? manipulationCleaning = await dbContext.ManipulationTypes
               .SingleOrDefaultAsync(m => m.Name == "Teeth Cleaning");

            List<Procedure> proceduresToSeed = new List<Procedure>();

            if (manipulationCheckUp != null)
            {
                proceduresToSeed.Add(new Procedure
                {
                    ProcedureId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientIvo.Id,
                    ManipulationTypeId = manipulationCheckUp.ManipulationId,
                    PatientPhoneNumber = "0876720270",
                    Date = new DateTime(2026, 4, 7, 0, 0, 0),
                    Note = "Initial consultation"
                });

                proceduresToSeed.Add(new Procedure
                {
                    ProcedureId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientPesho.Id,
                    ManipulationTypeId = manipulationCheckUp.ManipulationId,
                    PatientPhoneNumber = "0876555555",
                    Date = new DateTime(2026, 4, 8, 0, 0, 0),
                    Note = "Initial consultation"
                });


                proceduresToSeed.Add(new Procedure
                {
                    ProcedureId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientGeorgi.Id,
                    ManipulationTypeId = manipulationCheckUp.ManipulationId,
                    PatientPhoneNumber = "0876722122",
                    Date = new DateTime(2026, 4, 1, 0, 0, 0),
                    Note = "Initial consultation"
                });
            }

            if (manipulationCleaning != null)
            {
                proceduresToSeed.Add(new Procedure
                {
                    ProcedureId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientIvo.Id,
                    ManipulationTypeId = manipulationCleaning.ManipulationId,
                    PatientPhoneNumber = "0876720270",
                    Date = new DateTime(2026, 4, 8, 0, 0, 0),
                    Note = "Teeth Cleaning"
                });

                proceduresToSeed.Add(new Procedure
                {
                    ProcedureId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientPesho.Id,
                    ManipulationTypeId = manipulationCleaning.ManipulationId,
                    PatientPhoneNumber = "0876555555",
                    Date = new DateTime(2026, 4, 8, 0, 0, 0),
                    Note = "Teeth Cleaning"
                });

                proceduresToSeed.Add(new Procedure
                {
                    ProcedureId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientGeorgi.Id,
                    ManipulationTypeId = manipulationCleaning.ManipulationId,
                    PatientPhoneNumber = "0876722122",
                    Date = new DateTime(2026, 4, 4, 0, 0, 0),
                    Note = "Teeth Cleaning"
                });
            }

            if (manipulationFilling != null)
            {
                proceduresToSeed.Add(new Procedure
                {
                    ProcedureId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientIvo.Id,
                    ManipulationTypeId = manipulationFilling.ManipulationId,
                    PatientPhoneNumber = "0876720270",
                    Date = new DateTime(2026, 4, 9, 0, 0, 0),
                    Note = "Filling procedure"
                });

                proceduresToSeed.Add(new Procedure
                {
                    ProcedureId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientPesho.Id,
                    ManipulationTypeId = manipulationFilling.ManipulationId,
                    PatientPhoneNumber = "0876555555",
                    Date = new DateTime(2026, 4, 5, 0, 0, 0),
                    Note = "Filling procedure"
                });

                proceduresToSeed.Add(new Procedure
                {
                    ProcedureId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientGeorgi.Id,
                    ManipulationTypeId = manipulationFilling.ManipulationId,
                    PatientPhoneNumber = "0876722122",
                    Date = new DateTime(2026, 4, 4, 0, 0, 0),
                    Note = "Filling procedure"
                });
            }

            if (manipulationRoot != null)
            {
                proceduresToSeed.Add(new Procedure
                {
                    ProcedureId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientIvo.Id,
                    ManipulationTypeId = manipulationRoot.ManipulationId,
                    PatientPhoneNumber = "0876720270",
                    Date = new DateTime(2026, 4, 5, 0, 0, 0),
                    Note = "Root Canal"
                });

                proceduresToSeed.Add(new Procedure
                {
                    ProcedureId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientPesho.Id,
                    ManipulationTypeId = manipulationRoot.ManipulationId,
                    PatientPhoneNumber = "0876555555",
                    Date = new DateTime(2026, 4, 8, 0, 0, 0),
                    Note = "Root Canal"
                });

                proceduresToSeed.Add(new Procedure
                {
                    ProcedureId = Guid.NewGuid(),
                    DentistId = dentist.Id,
                    PatientId = patientGeorgi.Id,
                    ManipulationTypeId = manipulationRoot.ManipulationId,
                    PatientPhoneNumber = "0876722122",
                    Date = new DateTime(2026, 4, 7, 0, 0, 0),
                    Note = "Root Canal"
                });
            }
            await dbContext.AddRangeAsync(proceduresToSeed);
            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedFeedbackAsync(IServiceProvider serviceProvider)
        {
            //Using this type of seeding as not sure what Guids will be generated when users are seeded
            var dbContext = serviceProvider.GetRequiredService<DentistAppDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            
            if (dbContext.Feedbacks.Any())
            {
                return;
            }
            

            ApplicationUser? patientIvo = await userManager.FindByEmailAsync("ivo@abv.bg");
            ApplicationUser? patientPesho = await userManager.FindByEmailAsync("pesho@abv.bg");
            ApplicationUser? patientGeorgi = await userManager.FindByEmailAsync("gorgi@abv.bg");

            if (patientIvo == null || patientPesho == null || patientGeorgi == null)
            {
                throw new Exception("Users not found");
            }
            List<Feedback> feedbackToSeed = new List<Feedback>();
            Procedure? procedurePatientIvoFeedback = await dbContext.Procedures
                .Include(p => p.Feedback)
                .Where(p => p.PatientId == patientIvo.Id)
                .FirstOrDefaultAsync();

            if (procedurePatientIvoFeedback != null && procedurePatientIvoFeedback.Feedback==null)
            {
                feedbackToSeed.Add(new Feedback
                {
                    FeedbackId = Guid.NewGuid(),
                    CreatedOn = new DateTime(2026, 4, 9, 0, 0, 0),
                    Rating = 3,
                    FeedbackText = "Mid Service",
                    ProcedureId = procedurePatientIvoFeedback.ProcedureId
                });
            }

            Procedure? procedurePatientPeshoFeedback = await dbContext.Procedures
                .Include(p => p.Feedback)
                .Where(p => p.PatientId == patientPesho.Id)
                .FirstOrDefaultAsync();
            if (procedurePatientPeshoFeedback != null&& procedurePatientPeshoFeedback.Feedback==null)
            {
                feedbackToSeed.Add(new Feedback
                {
                    FeedbackId = Guid.NewGuid(),
                    CreatedOn = new DateTime(2026, 4, 8, 0, 0, 0),
                    Rating = 5,
                    FeedbackText = "Great Service",
                    ProcedureId = procedurePatientPeshoFeedback.ProcedureId
                });
            }

            Procedure? procedurePatientGoshoFeedback = await dbContext.Procedures
                .Include(p => p.Feedback)
                .Where(p => p.PatientId == patientGeorgi.Id)
                .FirstOrDefaultAsync();
            if (procedurePatientGoshoFeedback != null && procedurePatientGoshoFeedback.Feedback == null)
            {
                feedbackToSeed.Add(new Feedback
                {
                    FeedbackId = Guid.NewGuid(),
                    CreatedOn = new DateTime(2026, 4, 7, 0, 0, 0),
                    Rating = 1,
                    FeedbackText = "Bad Service",
                    ProcedureId = procedurePatientGoshoFeedback.ProcedureId
                });
            }

            await dbContext.Feedbacks.AddRangeAsync(feedbackToSeed);
            await dbContext.SaveChangesAsync();
        }
    }
}
