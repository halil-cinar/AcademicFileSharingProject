using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Security.Principal;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    [Route("/")]
    public class AccountController : Controller
    {

        private readonly IAccountService _accountService;
        private readonly IToastNotification _toastNotification;

        public AccountController(IToastNotification toastNotification, IAccountService accountService)
        {
            _toastNotification = toastNotification;
            _accountService = accountService;
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            var sessionKey = Request.Cookies["SessionKey"];

            var result = await _accountService.Logout(sessionKey);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                Response.Cookies.Delete("SessionKey");
                return Redirect("/");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(IdentityCheckDto identity)
        {
            var result=await _accountService.Login(identity);
            if(result.ResultStatus==Dtos.Enums.ResultStatus.Success)
            {
                Response.Cookies.Append("SessionKey", result.Result.Key);
                return Redirect("/");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View(identity);
        }
        

        [HttpGet("SignUp")]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserAddDto user)
        {
            var result = await _accountService.SignUp(user);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                Response.Cookies.Append("SessionKey", result.Result.Key);
                return Redirect("/");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View(user);
        }

    }
}
