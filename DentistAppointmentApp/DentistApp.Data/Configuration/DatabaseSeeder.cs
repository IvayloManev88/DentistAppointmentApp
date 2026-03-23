namespace DentistApp.Data.Configuration
{
    using static DentistApp.GCommon.Roles;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using DentistApp.Data.Models;

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
                    UserName = "Gorgi@abv.bg",
                    Email = "Gorgi@abv.bg",
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
            /*
           if (dbContext.Manipulations.Any())
           {
               return;
           }
           */
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
            var dbContext = serviceProvider.GetRequiredService<DentistAppDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            /*
            if (dbContext.Appointments.Any())
            {
                return;
            }
            */
            ApplicationUser? dentist = await userManager.FindByEmailAsync("TestDentist@abv.bg");
            ApplicationUser? patient = await userManager.FindByEmailAsync("ivo@abv.bg");

            if (dentist == null || patient == null)
            {
                throw new Exception("Users not found");
            }

            Appointment appointment = new Appointment
            {
                AppointmentId = Guid.NewGuid(),
                DentistId = dentist.Id,
                PatientId = patient.Id,
                Date = new DateTime(2026, 4, 1, 10, 0, 0),
                Note = "Initial consultation"
            };

            await dbContext.Appointments.AddAsync(appointment);
            await dbContext.SaveChangesAsync();
        }
    }
}
