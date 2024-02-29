using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.Filters;
using AcademicFileSharingProject.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    [Route("/Admin/User")]
    public class AdminUserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly List<EMethod> userMethods;
        private readonly IToastNotification _toastNotification;
        private readonly IAccountService _accountService;
        private readonly long? loginUserId = null;

        //Todo: password reset ve profil foto değiştirme eklenecek
        public AdminUserController(IUserService userService, IUserRoleService userRoleService, IHttpContextAccessor contextAccessor, IToastNotification toastNotification, IAccountService accountService)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _contextAccessor = contextAccessor;
            _toastNotification = toastNotification;
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
                            ViewBag.CanUpdate = !(!userMethods.Contains(EMethod.UserUpdate));
                            ViewBag.CanDelete = !(!userMethods.Contains(EMethod.UserRemove));
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


        [HttpGet("Roles")]
        public async Task<IActionResult> UserRoles([FromQuery] int? page)
        {

            if (!userMethods.Contains(EMethod.UserRoleAllList) || !userMethods.Contains(EMethod.UserRoleList))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var response = await _userService.GetAll(new LoadMoreFilter<UserFilter>
            {
                ContentCount = 20,
                PageCount = page ?? 0,
                
            });
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(response.Result);
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return RedirectToAction("Index");


        }


        [HttpPost("ChangeUserRole/{userId}")]
        public async Task<IActionResult> ChangeUserRole(UserRoleAllUpdateDto role)
        {
            if (!userMethods.Contains(EMethod.UserRoleUpdate) || !userMethods.Contains(EMethod.UserRoleUpdate))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var response = await _userRoleService.UpdateAll(role);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("UserRoles");
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return View(role);
        }


        [HttpGet("ChangeUserRole/{userId}")]
        public async Task<IActionResult> ChangeUserRole(long userId)
        {

            if (!userMethods.Contains(EMethod.UserRoleUpdate) || !userMethods.Contains(EMethod.UserRoleUpdate))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }


            var response = await _userRoleService.GetAll(new LoadMoreFilter<UserRoleFilter>
            {
                ContentCount = int.MaxValue,
                PageCount = 0,
                Filter = new UserRoleFilter()
                {
                   UserId= userId,
                }
            }); 
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                ViewBag.SelectedRoles = response.Result.Values.Select(x=>x.Role).ToList(); 
                ViewBag.UserId= userId;
                return View();
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);



            return View();
        }


    }
}