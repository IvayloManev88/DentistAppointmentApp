namespace DentistApp.Web.Controllers
{
    using DentistApp.Data.Models;
    using DentistApp.Services.Core;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels.AppointmentViewModels;
    using DentistApp.ViewModels.FeedbackViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class FeedbackController:Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IFeedbackService feedbackService;
        public FeedbackController(UserManager<ApplicationUser> userManager, IFeedbackService feedbackService)
        {
            this.userManager = userManager;
            this.feedbackService = feedbackService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<FeedbackViewViewModel> feedbacks = await feedbackService.GetAllFeedbacksViewModelsAsync();
            return View(feedbacks);
        }


    }
}
