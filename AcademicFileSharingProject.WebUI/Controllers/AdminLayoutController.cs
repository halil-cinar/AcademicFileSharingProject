using Microsoft.AspNetCore.Mvc;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    public class AdminLayoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
