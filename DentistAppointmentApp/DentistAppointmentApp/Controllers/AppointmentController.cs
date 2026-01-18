
using Microsoft.EntityFrameworkCore;

namespace DentistApp.Web.Controllers
{
    using DentistApp.Data.Models;
    using DentistApp.Services.Core;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.Services.Core.Models;
    using DentistApp.Web.ViewModels.AppointmentViewModels;
    using DentistAppointmentApp.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using static System.Net.Mime.MediaTypeNames;

    [Authorize]
    public class AppointmentController : Controller
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly DentistAppDbContext dbContext;
        private readonly IManipulationService manipulationService;

        public AppointmentController(UserManager<ApplicationUser> userManager, DentistAppDbContext dbContext, IManipulationService manipulationService)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.manipulationService = manipulationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            IEnumerable<LookupItem> manipulationTypes = await manipulationService.GetManipulationTypesAsync();
            AppointmentCreateViewModel createModel = new AppointmentCreateViewModel()
            {
                ManipulationTypes = manipulationTypes
                .Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text = mt.Name
                })
            };
            return View(createModel);
        }

    }
}
