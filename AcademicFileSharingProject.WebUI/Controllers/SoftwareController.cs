using AcademicFileSharingProject.Business.Abstract;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    public class SoftwareController : Controller
    {

        private readonly IToastNotification _toastNotification;
        private readonly ISoftwareService _softwareService;
        public SoftwareController(IToastNotification toastNotification, ISoftwareService softwareService)
        {
            _toastNotification = toastNotification;
            _softwareService = softwareService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var result = await _softwareService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.SoftwareFilter>
                {
                ContentCount = 10,
                PageCount = 0,
                Filter = new Dtos.Filters.SoftwareFilter
                {
                    IsAir = true,
                }
            });
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(result.Result);
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }
    }
}
