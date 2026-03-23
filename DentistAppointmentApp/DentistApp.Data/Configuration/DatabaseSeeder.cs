namespace DentistApp.Data.Configuration
{
    using static DentistApp.GCommon.Roles;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    
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


    }
}
