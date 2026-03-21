namespace DentistApp.Web.Controllers
{
    using DentistApp.Data.Models;
    using DentistApp.ViewModels;
    using static DentistApp.GCommon.Roles;
    using static DentistApp.GCommon.GlobalCommon;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using System.Diagnostics;
    
   
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                ApplicationUser? user = await _userManager.GetUserAsync(User);

                if (user != null && await _userManager.IsInRoleAsync(user, DentistRoleName))
                {
                    return RedirectToAction("Index", "Home", new { area = DentistArea });
                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
