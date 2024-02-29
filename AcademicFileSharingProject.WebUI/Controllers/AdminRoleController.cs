using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.Filters;
using AcademicFileSharingProject.Dtos.Result;
using AcademicFileSharingProject.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    [Route("/Admin/Role")]
    public class AdminRoleController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRoleMethodService _roleMethodService;
        private readonly List<EMethod> userMethods;
        private readonly IToastNotification _toastNotification;
        private readonly IAccountService _accountService;
        private readonly long? loginUserId = null;

        public AdminRoleController(IUserService userService, IUserRoleService userRoleService, IHttpContextAccessor contextAccessor, IToastNotification toastNotification, IAccountService accountService, IRoleMethodService roleMethodService)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _contextAccessor = contextAccessor;
            _toastNotification = toastNotification;
            _roleMethodService = roleMethodService;
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
                            ViewBag.CanUpdate = !(!userMethods.Contains(EMethod.UserUpdate) || !userMethods.Contains(EMethod.StudentUpdate));
                            ViewBag.CanDelete = !(!userMethods.Contains(EMethod.UserRemove) || !userMethods.Contains(EMethod.StudentRemove));

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



        public async Task<IActionResult> Index([FromQuery]int? page)
        {
            if (!userMethods.Contains(EMethod.RoleMethodAllList) || !userMethods.Contains(EMethod.RoleMethodList))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var response = await _roleMethodService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.RoleMethodFilter>
            {
                ContentCount = 20,
                PageCount = page??0,
            });
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(response.Result);
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return View();
        }
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            if (!userMethods.Contains(EMethod.RoleMethodAllList) || !userMethods.Contains(EMethod.RoleMethodList))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var response = await _roleMethodService.Delete(id);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return RedirectToAction("Index");
        }


        [HttpPost("Update")]
        public async Task<IActionResult> Update(RoleMethodAllUpdateDto roleMethod)
        {
            if (!userMethods.Contains(EMethod.RoleMethodUpdate) || !userMethods.Contains(EMethod.RoleMethodUpdate))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var response = await _roleMethodService.UpdateAll(roleMethod);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return View(roleMethod);
        }


        [HttpGet("Update")]
        public async Task<IActionResult> Update()
        {
            
            if (!userMethods.Contains(EMethod.RoleMethodUpdate) || !userMethods.Contains(EMethod.RoleMethodUpdate))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }

            var role = Request.Query["role"];
            if(!string.IsNullOrEmpty(role))
            {
                var response = await _roleMethodService.GetAll(new LoadMoreFilter<RoleMethodFilter>
                {
                    ContentCount = int.MaxValue,
                    PageCount = 0,
                    Filter = new RoleMethodFilter()
                    {
                        Role = (ERoles)Convert.ToInt32(role)
                    }
                }); ;
                if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
                {
                    ViewBag.SelectedRole = (ERoles)Convert.ToInt32(role);
                    ViewBag.SelectedMethods = response.Result.Values.Select(x => x.Method).ToList();

                    return View();
                }
                var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
                _toastNotification.AddErrorToastMessage(message);

            }
            
            return View();
        }
        


        [HttpGet("GetMethods/{roleId:int}")]
        public async Task<List<EMethod>> GetMethodsByRole(ERoles roleId)
        {
            if (!userMethods.Contains(EMethod.RoleMethodList) || !userMethods.Contains(EMethod.RoleMethodAllList))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return new List<EMethod>();
            }
            var response = await _roleMethodService.GetAll(new LoadMoreFilter<RoleMethodFilter>
            {
                ContentCount = int.MaxValue,
                PageCount = 0,
                Filter = new RoleMethodFilter()
                {
                    Role = roleId
                }
            });
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return response.Result.Values.Select(x => x.Method).ToList();
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return new List<EMethod>();
        }

       


    }
}
