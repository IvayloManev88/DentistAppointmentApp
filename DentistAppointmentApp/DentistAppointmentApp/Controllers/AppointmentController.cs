
using Microsoft.EntityFrameworkCore;

namespace DentistApp.Web.Controllers
{
    using DentistApp.Data.Models;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.Services.Core.Models;
    using DentistApp.Web.ViewModels.AppointmentViewModels;
    using DentistApp.Web.ViewModels.ManipulationViewModels;
    using DentistAppointmentApp.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using static GCommon.AppointmentConstants;

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

        public async Task<IActionResult> Index()
        {
            AppointmentViewAppointmentViewModel[] appointments = await dbContext
                .Appointments
                .AsNoTracking()
                .OrderBy(a => a.Date)
                .Select(a => new AppointmentViewAppointmentViewModel
                {
                    PatientAppointmentName = $"{a.Patient.FirstName} {a.Patient.LastName}",
                    DentistAppointmentName = $"{a.Dentist.FirstName} {a.Dentist.LastName}",
                    AppointmentDate = a.Date.ToString("hh:mm dd.MM.yyyy"),
                    PatientAppointmentPhoneNumber = a.PatientPhoneNumber,
                    ManipulationName = a.ManipulationType.Name,
                    AppointmentNote = a.Note

                }).ToArrayAsync();

            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            IEnumerable<LookupItem> manipulationTypes = await manipulationService.GetManipulationTypesAsync();
            AppointmentCreateViewModel createModel = new AppointmentCreateViewModel();
            await PopulateManipulationTypesAsync(createModel);
            return View(createModel);
        }

        public async Task<IActionResult> Create(AppointmentCreateViewModel createModel)
        {
            if (!ModelState.IsValid)
            {
                return View(createModel);
            }

            DateTime appointmentDate = createModel.AppointmentDate.Date + createModel.AppointmentTime;
            if (await this.dbContext.Appointments.AsNoTracking().AnyAsync(a => a.Date == appointmentDate && a.IsDeleted == false))
            {
                ModelState
                    .AddModelError(nameof(createModel.AppointmentDate), "The selected combination Date/Time is not available. Please try different Date/Time");

                await PopulateManipulationTypesAsync(createModel);
                return View(createModel);
            }

            if (appointmentDate < DateTime.Now)
            {
                ModelState
                   .AddModelError(nameof(createModel.AppointmentDate), "You should not set an appintment in the past");
                await PopulateManipulationTypesAsync(createModel);
                return View(createModel);
            }
            Appointment currentAppointment = new Appointment
            {
                PatientId = userManager.GetUserId(User)!,
                DentistId = DentistId,
                Date = appointmentDate,
                PatientPhoneNumber = createModel.PatientPhoneNumber,
                ManipulationTypeId = createModel.ManipulationTypeId,


            };

            await dbContext.Appointments.AddAsync(currentAppointment);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

       private async Task PopulateManipulationTypesAsync(AppointmentCreateViewModel createViewModel)
        {
            var manipulationTypes = await manipulationService.GetManipulationTypesAsync();

            createViewModel.ManipulationTypes = manipulationTypes
                .Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text = mt.Name
                });
                
        }

    }
}
