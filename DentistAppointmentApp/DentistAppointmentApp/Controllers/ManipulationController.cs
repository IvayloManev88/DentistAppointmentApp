namespace DentistApp.Web.Controllers
{
    using DentistApp.Data.Models;
    using DentistApp.Web.ViewModels.ManipulationViewModels;
    using DentistAppointmentApp.Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

    public class ManipulationController : Controller
    {
        private readonly DentistAppDbContext dbContext;
        public ManipulationController(DentistAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ManipulationViewAllViewModel[] manipulations = await dbContext
                .ManipulationTypes
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .Select(m => new ManipulationViewAllViewModel
                {
                    Name = m.Name,
                    PriceRange = m.PriceRange
                }).ToArrayAsync();

            return View(manipulations);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ManipulationCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ManipulationCreateViewModel inputViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(inputViewModel);
            }

            if (await this.dbContext.ManipulationTypes.AsNoTracking().AnyAsync(m => m.Name == inputViewModel.Name && m.IsDeleted == false))
            {
                ModelState
                    .AddModelError(nameof(inputViewModel.Name), "Duplicate manipulation name");
                return View(inputViewModel);
            }
            ManipulationType currentManipulation = new ManipulationType
            {
                Name = inputViewModel.Name.TrimEnd(),
                PriceRange = inputViewModel.PriceRange

            };
            await dbContext.ManipulationTypes.AddAsync(currentManipulation);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
