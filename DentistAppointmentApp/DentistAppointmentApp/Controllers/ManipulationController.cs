namespace DentistApp.Web.Controllers
{
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels.ManipulationViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using static DentistApp.GCommon.Roles;

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
            IEnumerable<ManipulationViewAllViewModel> manipulations= await manipulationService.GetAllManipulationTypesAsync();

            return View(manipulations);
        }

        [HttpGet]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Create()
        {
            return View(new ManipulationCreateViewModel());
        }

        [HttpPost]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Create(ManipulationCreateViewModel inputViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(inputViewModel);
            }

            if (await manipulationService.IsManipulationNameDuplicatedAsync(inputViewModel.Name))
            {
                ModelState
                    .AddModelError(nameof(inputViewModel.Name), "Duplicate manipulation name");
                return View(inputViewModel);
            }
            try
            {
                await manipulationService.CreateManipulationAsync(inputViewModel);
                return RedirectToAction(nameof(Index));
            }
            catch 
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating Manipulation.Please try again!");
                return View(inputViewModel);
            }                    
        }

        [HttpPost]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await manipulationService.ValidateManipulationTypesAsync(id))
            {
                return NotFound();
            }

            try
            {
                await manipulationService.DeleteManipulationAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!await manipulationService.ValidateManipulationTypesAsync(id))
            {
                return NotFound();
            }
            try
            {
                ManipulationEditViewModel editViewModel = await manipulationService.GetManipulationEditViewModelAsync(id);
                return View(editViewModel);
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Edit(ManipulationEditViewModel editViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editViewModel);
            }

            if (await manipulationService.IsManipulationNameDuplicatedAsync(editViewModel.Name,editViewModel.ManipulationId))
            {
                ModelState
                    .AddModelError(nameof(editViewModel.Name), "Duplicate manipulation name");
                return View(editViewModel);
            }
            if (!editViewModel.ManipulationId.HasValue)
            {
                return BadRequest();
            }          
                   
            if (!await manipulationService.ValidateManipulationTypesAsync(editViewModel.ManipulationId.Value))
            {
                return NotFound();
            }

            try
            {
                await manipulationService.EditManipulationAsync(editViewModel);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred while editing Manipulation.Please try again!");
                return View(editViewModel);
            }
        }
    }
}
