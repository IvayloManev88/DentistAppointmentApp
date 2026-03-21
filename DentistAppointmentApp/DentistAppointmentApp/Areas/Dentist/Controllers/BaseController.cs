namespace DentistApp.Web.Areas.Dentist.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using static DentistApp.GCommon.GlobalCommon;
    using static DentistApp.GCommon.Roles;

    [Area(DentistArea)]
    [Authorize (Roles = DentistRoleName)]
    public abstract class BaseController : Controller
    {
       
    }
}
