namespace DentistApp.Web.Controllers
{
    using DentistApp.Data;
    using DentistApp.Data.Models;

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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AssignDentist()
        {
            if (!await roleManager.RoleExistsAsync(DentistRoleName))
            {
                await roleManager.CreateAsync(new IdentityRole(DentistRoleName));
            }

            ApplicationUser? currentUser = await userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return NotFound();
            }

            if (!await userManager.IsInRoleAsync(currentUser,DentistRoleName))
            {
                await userManager.AddToRoleAsync(currentUser,DentistRoleName)  ;
            }

            return Ok("You are a Dentist");
        }

    }
}
