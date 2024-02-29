using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Entities.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    [Route("/Admin/Profile")]
    public class AdminProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly List<EMethod> userMethods;
        private readonly IToastNotification _toastNotification;
        private readonly IAccountService _accountService;
        private readonly long? loginUserId = null;

        public AdminProfileController(IUserService userService, IToastNotification toastNotification, IMapper mapper, IIdentityService identityService, IHttpContextAccessor contextAccessor, IAccountService accountService)
        {
            _userService = userService;
            _toastNotification = toastNotification;
            _mapper = mapper;
            _identityService = identityService;
            _contextAccessor = contextAccessor;
            _accountService = accountService;
            userMethods = new List<EMethod>();
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

        public async Task<IActionResult> Index()
        {
            var response = await _userService.Get((long)loginUserId);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(_mapper.Map<UserDto>(response.Result));
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View(); 
        }

        [HttpPost()]
        public async Task<IActionResult> Index(UserDto user)
        {
            user.Id =(long) loginUserId;
            var response = await _userService.Update(user);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }


        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(IdentityUpdateDto identity)
        {
            identity.UserId =(long) loginUserId;

            var response = await _identityService.Update(identity);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return Redirect("/Admin");
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }






    }
}
