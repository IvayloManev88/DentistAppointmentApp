namespace DentistApp.Web.Controllers
{
    using DentistApp.Data.Models;
    using DentistApp.Web.ViewModels.ManipulationViewModels;
    using DentistApp.Data;
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
                .Where(m => m.IsDeleted == false)
                .OrderBy(m => m.Name)
                .Select(m => new ManipulationViewAllViewModel
                {
                    ManipulationId = m.ManipulationId.ToString(),
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
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            ManipulationType? manipulationToDelete = await dbContext
                .ManipulationTypes
                .SingleOrDefaultAsync(m => m.IsDeleted == false && m.ManipulationId.ToString().ToLower() == id.ToLower());

            if (manipulationToDelete == null)
            {
                return NotFound();
            }
            manipulationToDelete.IsDeleted = true;

            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            ManipulationType? manipulationToEdit = await dbContext
                .ManipulationTypes
                .SingleOrDefaultAsync(m => m.IsDeleted == false && m.ManipulationId.ToString().ToLower() == id.ToLower());

            if (manipulationToEdit == null)
            {
                return NotFound();
            }
            ManipulationEditViewModel editViewModel = new ManipulationEditViewModel
            {
                ManipulationId = manipulationToEdit.ManipulationId.ToString(),
                Name = manipulationToEdit.Name,
                PriceRange = manipulationToEdit.PriceRange
            };
            return View(editViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ManipulationEditViewModel editViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editViewModel);
            }

            if (await this.dbContext.ManipulationTypes
                .AsNoTracking()
                .AnyAsync(m => m.Name == editViewModel.Name && m.IsDeleted == false && editViewModel.ManipulationId.ToLower() != m.ManipulationId.ToString().ToLower()))
            {
                ModelState
                    .AddModelError(nameof(editViewModel.Name), "Duplicate manipulation name");
                return View(editViewModel);
            }

            ManipulationType? manipulationToEdit = await dbContext
                .ManipulationTypes
                .SingleOrDefaultAsync(m => m.IsDeleted == false && m.ManipulationId.ToString().ToLower() == editViewModel.ManipulationId.ToLower());

            if (manipulationToEdit == null)
            {
                return NotFound();
            }
            manipulationToEdit.Name = editViewModel.Name;
            manipulationToEdit.PriceRange = editViewModel.PriceRange;

            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));



        }
    }
}
