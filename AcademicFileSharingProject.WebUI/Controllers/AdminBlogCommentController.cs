using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Entities.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    [Route("/Admin/BlogComment")]
    public class AdminBlogCommentController : Controller
    {
        private readonly IBlogCommentService _blogCommentService;

        private readonly IToastNotification _toastNotification;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly List<EMethod> userMethods;
        private readonly IAccountService _accountService;
        private readonly long? loginUserId = null;
        private readonly UserListDto? loginUser = null;
        private readonly IMapper _mapper;
        public AdminBlogCommentController(IBlogCommentService blogCommentService, IUserService userService, IToastNotification toastNotification, IMapper mapper, IHttpContextAccessor contextAccessor, IAccountService accountService)
        {
            _blogCommentService = blogCommentService;
            _userService = userService;
            _toastNotification = toastNotification;
            _mapper = mapper;
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
                        loginUser = result.Result.Result.User;
                        var roleResult = _accountService.GetUserRoleMethods(result.Result.Result.UserId);
                        roleResult.Wait();
                        if (roleResult.Result.ResultStatus == Dtos.Enums.ResultStatus.Success)
                        {
                            userMethods = roleResult.Result.Result;
                            ViewBag.CanUpdate = !(!userMethods.Contains(EMethod.BlogCommentUpdate) );
                            ViewBag.CanDelete = !(!userMethods.Contains(EMethod.BlogCommentRemove));
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

        [HttpGet]
        [HttpGet("{blogId}")]
        public async Task<IActionResult> Index(long? blogId, [FromQuery]int? page)
        {
            if (!userMethods.Contains(EMethod.BlogCommentAllList) || !userMethods.Contains(EMethod.BlogCommentList))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _blogCommentService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.BlogCommentFilter>
            {
                ContentCount = 10,
                PageCount = page ?? 0,
                Filter = new Dtos.Filters.BlogCommentFilter
                {
                    BlogId = blogId,
                    BloggerId = (!userMethods.Contains(EMethod.BlogCommentAllList)) ? loginUserId : null

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



        [HttpGet("Detail/{Id}")]
        public async Task<IActionResult> Detail(long id)
        {
            if (!userMethods.Contains(EMethod.BlogCommentDetail) )
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _blogCommentService.Get(id);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(result.Result);
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }

        [HttpGet("Add/{blogId}")]
        public async Task<IActionResult> Add(long blogId)
        {
            if (!userMethods.Contains(EMethod.BlogCommentAdd))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            return View();
        }

        [HttpPost("Add/{blogId}")]
        public async Task<IActionResult>Add([FromRoute]long blogId,[FromForm]BlogCommentDto comment)
        {
            if (!userMethods.Contains(EMethod.BlogCommentAdd))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            comment.SenderUserId = (long)loginUserId;
            comment.BlogId = blogId;
            var result = await _blogCommentService.Add(comment);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }


        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            if (((await _blogCommentService.Get(id))?.Result?.Blog.UserId != loginUserId && !userMethods.Contains(EMethod.BlogCommentAllRemove)) || !userMethods.Contains(EMethod.BlogCommentRemove))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }

            var result = await _blogCommentService.Delete(id);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            return View();
        }


        [HttpGet("Update/{Id}")]
        public async Task<IActionResult> Update(long id)
        {
            
            var result = await _blogCommentService.Get(id);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                if ((result?.Result?.Blog.UserId != loginUserId && !userMethods.Contains(EMethod.BlogCommentAllUpdate)) || !userMethods.Contains(EMethod.BlogCommentUpdate))
                {
                    _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                    return Redirect("/");
                }
                return View(_mapper.Map<BlogCommentDto>( result.Result));
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }

        [HttpPost("Update/{id}")]
        public async Task<IActionResult> Update( BlogCommentDto comment)
        {
            if (((await _blogCommentService.Get(comment.Id))?.Result?.Blog.UserId != loginUserId && !userMethods.Contains(EMethod.BlogCommentAllUpdate)) || !userMethods.Contains(EMethod.BlogCommentUpdate))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            comment.SenderUserId = (long)loginUserId;
            var result = await _blogCommentService.Update(comment);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }


    }
}
