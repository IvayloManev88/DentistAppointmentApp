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
    }
}
