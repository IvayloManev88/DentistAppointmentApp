namespace DentistApp.Web.Controllers
{
    using DentistApp.Data.Models;
    using static DentistApp.GCommon.Roles;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class ManagerController: Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public ManagerController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
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
