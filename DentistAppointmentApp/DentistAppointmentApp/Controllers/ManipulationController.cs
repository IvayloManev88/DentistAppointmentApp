namespace DentistApp.Web.Controllers
{
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels.ManipulationViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class ManipulationController : Controller
    {
        private readonly IManipulationService manipulationService;
        public ManipulationController(IManipulationService manipulationService)
        {
            this.manipulationService = manipulationService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            IEnumerable<ManipulationViewAllViewModel> manipulations = await manipulationService.GetAllManipulationTypesAsync();

            return View(manipulations);
        }
    }
}
