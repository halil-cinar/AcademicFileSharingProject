using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.Filters;
using AcademicFileSharingProject.Entities.Enums;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    [Route("/Admin/SystemSettings")]
    public class AdminSystemSettingsController : Controller
    {
        private readonly IUserService _userService;
        private readonly ISystemSettingsService _systemSettingsService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly List<EMethod> userMethods;
        private readonly IToastNotification _toastNotification;
        private readonly IAccountService _accountService;
        private readonly long? loginUserId = null;
        //Todo: password reset ve profil foto değiştirme eklenecek
        public AdminSystemSettingsController(IUserService userService, IHttpContextAccessor contextAccessor, IToastNotification toastNotification, IAccountService accountService, ISystemSettingsService systemSettingsService)
        {
            _userService = userService;
            _contextAccessor = contextAccessor;
            _toastNotification = toastNotification;
            _accountService = accountService;
            userMethods = new List<EMethod>();
            _systemSettingsService = systemSettingsService;

            var session = _contextAccessor.HttpContext.Request.Cookies["SessionKey"];
            if (session != null)
            {
                var result = _accountService.GetSession(session);
                result.Wait();
                if (result.Result.ResultStatus == Dtos.Enums.ResultStatus.Success)
                {
                    if (result.Result.Result == null)
                    {
                        session = null;
                    }
                    else
                    {
                        loginUserId = result.Result.Result.UserId;
                        var roleResult = _accountService.GetUserRoleMethods(result.Result.Result.UserId);
                        roleResult.Wait();
                        if (roleResult.Result.ResultStatus == Dtos.Enums.ResultStatus.Success)
                        {
                            userMethods = roleResult.Result.Result;
                            ViewBag.CanUpdate = !(!userMethods.Contains(EMethod.UserUpdate) || !userMethods.Contains(EMethod.AcademicianUpdate));
                            ViewBag.CanDelete = !(!userMethods.Contains(EMethod.UserRemove) || !userMethods.Contains(EMethod.AcademicianRemove));
                        }
                        else
                        {
                            var message = string.Join(Environment.NewLine, roleResult.Result.ErrorMessages.Select(m => m.Message));
                            _toastNotification.AddErrorToastMessage(message);
                        }

                    }
                }
                else
                {
                    var message = string.Join(Environment.NewLine, result.Result.ErrorMessages.Select(m => m.Message));
                    _toastNotification.AddErrorToastMessage(message);
                }
            }
            if (session == null)
            {
                _toastNotification.AddAlertToastMessage("Bu kısma girmek için yetkiniz yoktur");

                _contextAccessor.HttpContext.Response.Redirect("/");
            }
        }


        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            if (!userMethods.Contains(EMethod.SystemSettingsAllList)||!userMethods.Contains(EMethod.SystemSettingsList))
            {
                return Redirect("/");
            }
            var result = await _systemSettingsService.GetAll(new LoadMoreFilter<SystemSettingsFilter>
            {
                ContentCount = 10,
                PageCount = 0,
            });
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(result.Result);
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }

        [HttpPost("")]
        public async Task<IActionResult> Index(SystemSettingsDto systemSettings)
        {
            if (!userMethods.Contains(EMethod.SystemSettingsUpdate))
            {
                return Redirect("/");
            }
            var result = await _systemSettingsService.Update(systemSettings);
            if (result.ResultStatus ==Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return RedirectToAction("Index");
        }



        [HttpGet("Logo")]
        public async Task<IActionResult> LogoUpdate()
        {
            if (!userMethods.Contains(EMethod.SystemSettingsAllList) || !userMethods.Contains(EMethod.SystemSettingsList))
            {
                return Redirect("/");
            }
            var result = await _systemSettingsService.GetLogo();
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(result.Result);
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return RedirectToAction("Index");
        }
        [HttpPost("Logo")]
        public async Task<IActionResult> LogoUpdate(LogoDto logo)
        {
            if (!userMethods.Contains(EMethod.SystemSettingsUpdate))
            {
                return Redirect("/");
            }
            var result = await _systemSettingsService.ChangeLogo(logo);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return Redirect("/Admin");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return RedirectToAction("Index");
        }
    }
}
