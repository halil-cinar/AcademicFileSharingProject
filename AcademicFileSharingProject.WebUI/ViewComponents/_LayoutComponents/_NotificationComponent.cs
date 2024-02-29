using Microsoft.AspNetCore.Mvc;

namespace AcademicFileSharingProject.WebUI.ViewComponents._LayoutComponents
{
    public class _NotificationComponent:ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
