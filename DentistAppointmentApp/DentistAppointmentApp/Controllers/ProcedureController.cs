namespace DentistApp.Web.Controllers
{
    using DentistApp.Data.Models;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels.ProcedureViewModels;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using System.Collections.Generic;

    [Authorize]
    public class ProcedureController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IProcedureService procedureService;

        public ProcedureController(UserManager<ApplicationUser> userManager, IProcedureService procedureService)
        {
            this.userManager = userManager;
            this.procedureService = procedureService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchQuery, int page=1)
        {
            string currentUserId = userManager.GetUserId(User)!;
            ProcedurePaginationViewModel procedures = await procedureService
                .GetAllProceduresViewModelsAsync(currentUserId, searchQuery,page);

            ViewBag.SearchQuery = searchQuery;
            return View(procedures);
        }
    }
}
