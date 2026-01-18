using Microsoft.AspNetCore.Mvc;

namespace DentistApp.Web.Controllers
{
    public class AppointmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


    }
}
