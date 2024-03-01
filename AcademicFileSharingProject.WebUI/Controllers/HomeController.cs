using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using AcademicFileSharingProject.Entities.Enums;
using AcademicFileSharingProject.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NToastNotify;
using System.Diagnostics;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBlogService _blogService;
        private readonly IPostService _postService;
        private readonly List<EMethod> userMethods;
        private readonly IToastNotification _toastNotification;
        private readonly IAccountService _accountService;
        private readonly long? loginUserId = null;

        public HomeController(IUserService userService, IUserRoleService userRoleService, IHttpContextAccessor contextAccessor, IToastNotification toastNotification, IAccountService accountService, IRoleMethodService roleMethodService, IBlogService blogService, IPostService postService)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _contextAccessor = contextAccessor;
            _toastNotification = toastNotification;
            //            _roleMethodService = roleMethodService;
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

            }
            _blogService = blogService;
            _postService = postService;
        }


        public async Task<IActionResult> Index()
        {

            var academicianResult = await _userRoleService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.UserRoleFilter>
            {
                ContentCount = 15,
                PageCount =  0,
                Filter = new Dtos.Filters.UserRoleFilter
                {
                    Role = Entities.Enums.ERoles.Academician,
                    
                }
            });
            if (academicianResult.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                var list = academicianResult.Result.Values.Select(x => x.User).ToList();
                ViewBag.Users=(list);
            }
            else
            {
                var message = string.Join(Environment.NewLine, academicianResult.ErrorMessages.Select(x => x.Message).ToList());
                _toastNotification.AddErrorToastMessage(message);
            }



            var postResult = await _postService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.PostFilter>
            {
                ContentCount = 15,
                PageCount = 0,
                Filter = new Dtos.Filters.PostFilter
                {
                    IsAir= true,
                }
            });

            if (postResult.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                ViewBag.Posts = postResult.Result.Values;


            }
            else
            {
                var message = string.Join(Environment.NewLine, postResult.ErrorMessages.Select(x => x.Message).ToList());
                _toastNotification.AddErrorToastMessage(message);
            }

            var result = await _blogService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.BlogFilter>
            {
                ContentCount = 15,
                PageCount = 0,
                Filter = new Dtos.Filters.BlogFilter
                {
                    IsAir = true,
                }
            });

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                ViewBag.Blogs = result.Result.Values;


            }
            else
            {
                var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
                _toastNotification.AddErrorToastMessage(message);
            }


            return View();
        }

     
      
    }
}