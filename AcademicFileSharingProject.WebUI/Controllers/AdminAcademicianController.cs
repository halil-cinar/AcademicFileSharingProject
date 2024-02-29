using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using AcademicFileSharingProject.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    [Route("Admin/Academician")]
    public class AdminAcademicianController : Controller
    {

        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly List<EMethod> userMethods;
        private readonly IToastNotification _toastNotification;
        private readonly IAccountService _accountService;
        private readonly long? loginUserId = null;
        //Todo: password reset ve profil foto değiştirme eklenecek
        public AdminAcademicianController(IUserService userService, IUserRoleService userRoleService, IHttpContextAccessor contextAccessor, IToastNotification toastNotification, IAccountService accountService)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _contextAccessor = contextAccessor;
            _toastNotification = toastNotification;
            _accountService = accountService;
            userMethods= new List<EMethod>();

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

        public async Task<IActionResult> Index([FromQuery]int? page)
        {
            if (!userMethods.Contains(EMethod.UserAllList)||!userMethods.Contains(EMethod.AcademicianAllList))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }

            var result = await _userRoleService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.UserRoleFilter>
            {
                ContentCount = 10,
                PageCount = page??0,
                Filter = new Dtos.Filters.UserRoleFilter
                {
                    Role = Entities.Enums.ERoles.Academician
                }
            });
            
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                var list = result.Result.Values.Select(x => x.User).ToList() ;
                return View(new GenericLoadMoreDto<UserListDto>
                {
                    Values=list,
                    ContentCount=result.Result.ContentCount,
                    PageCount=result.Result.PageCount,
                    NextPage=result.Result.NextPage,
                    PrevPage=result.Result.PrevPage,
                    TotalContentCount=result.Result.TotalContentCount,
                    TotalPageCount = result.Result.TotalPageCount

                });
            }
            

            return View();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(long id)
        {
            if (!userMethods.Contains(EMethod.UserDetail) || !userMethods.Contains(EMethod.AcademicianDetail))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _userService.Get(id);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(result.Result);
            }
            return View();


        }


        [HttpGet("Update/{id}")]
        public async Task<IActionResult> Update(long id)
        {
            if ((!userMethods.Contains(EMethod.UserUpdate) || !userMethods.Contains(EMethod.AcademicianUpdate))&&id!=loginUserId)
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _userService.Get(id);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(result.Result);
            }
            return View();


        }

        [HttpPost("Update/{id}")]
        public async Task<IActionResult> Update(UserDto user)
        {
            if ((!userMethods.Contains(EMethod.UserUpdate) || !userMethods.Contains(EMethod.AcademicianUpdate)) && user.Id != loginUserId)
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _userService.Update(user);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            return View();


        }


        [HttpGet("Add")]
        public async Task<IActionResult> Add()
        {
            if (!userMethods.Contains(EMethod.UserAdd) || !userMethods.Contains(EMethod.AcademicianAdd))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            return View();
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(UserAddDto user)
        {
            if (!userMethods.Contains(EMethod.UserAdd) || !userMethods.Contains(EMethod.AcademicianAdd))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            user.Role = Entities.Enums.ERoles.Academician;

            var result = await _userService.Add(user);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            return View(user);


        }


        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            if (!userMethods.Contains(EMethod.UserRemove) || !userMethods.Contains(EMethod.AcademicianRemove))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _userService.Delete(id);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            return View();
        }


        [HttpGet("ChangePhoto/{id}")]
        public async Task<IActionResult> ChangePhoto(long id)
        {
            if ((!userMethods.Contains(EMethod.UserUpdate) || !userMethods.Contains(EMethod.AcademicianUpdate)) && id != loginUserId)
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _userService.Get(id);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(new UserDto
                {
                    Id = id,

                });
            }
            return View();


        }

        [HttpPost("ChangePhoto/{id}")]
        public async Task<IActionResult> ChangePhoto(UserDto user)
        {
            if ((!userMethods.Contains(EMethod.UserUpdate) || !userMethods.Contains(EMethod.AcademicianUpdate)) && user.Id != loginUserId)
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _userService.ChangeProfilePhoto(user);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            return View(user);


        }









    }
}
