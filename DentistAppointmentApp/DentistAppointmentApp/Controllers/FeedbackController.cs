namespace DentistApp.Web.Controllers
{
    using DentistApp.Data.Models;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels.FeedbackViewModels;
    using static DentistApp.GCommon.ControllersOutputMessages;

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
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            IEnumerable<FeedbackViewViewModel> feedbacks = await feedbackService.GetAllFeedbacksViewModelsAsync();

            ViewBag.AverageRating = await feedbackService.GetAverageRatingAsync();

            if (User.Identity?.IsAuthenticated == true)
            {
                string? userId = userManager.GetUserId(User);
                ViewBag.CanCreateFeedback = await feedbackService.CanUserLeaveFeedbackAsync(userId!);
            }

            return View(feedbacks);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            return View(new FeedBackCreateViewModel());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(FeedBackCreateViewModel inputViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(inputViewModel);
            }
            string patientId = userManager.GetUserId(User)!;

            try
            {
                await feedbackService.CreateFeedbackAsync(inputViewModel, patientId);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, FeedbackCreationError);
                return View(inputViewModel);
            }
        }
    }
}
