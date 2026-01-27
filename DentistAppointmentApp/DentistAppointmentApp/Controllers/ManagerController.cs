namespace DentistApp.Web.Controllers
{
    using DentistApp.Data;
    using DentistApp.Data.Models;
    using DentistApp.Services.Core.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using static DentistApp.GCommon.Roles;

    [Authorize]
    public class ManagerController: Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly DentistAppDbContext dbContext;
        private readonly RoleManager<IdentityRole> roleManager;
        public ManagerController(UserManager<ApplicationUser> userManager, DentistAppDbContext dbContext, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.roleManager = roleManager;

        }
        public async Task<IActionResult> AssignDentist()
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            ApplicationUser? currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }
            if (!await userManager.IsInRoleAsync(currentUser,roleName))
            {
                await userManager.AddToRoleAsync(currentUser,roleName)  ;
            }

            return Ok("You are a Dentist");
        }

    }
}
