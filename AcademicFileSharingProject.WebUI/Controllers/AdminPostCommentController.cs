using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Entities.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    [Route("/Admin/PostComment")]
    public class AdminPostCommentController : Controller
    {
        private readonly IPostCommentService _postCommentService;

        private readonly IToastNotification _toastNotification;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly List<EMethod> userMethods;
        private readonly IAccountService _accountService;
        private readonly long? loginUserId = null; private readonly IMapper _mapper;

        public AdminPostCommentController(IMapper mapper, IUserService userService, IToastNotification toastNotification, IPostCommentService postCommentService, IHttpContextAccessor contextAccessor, IAccountService accountService)
        {
            _mapper = mapper;
            _userService = userService;
            _toastNotification = toastNotification;
            _postCommentService = postCommentService;
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
                            ViewBag.CanUpdate = !(!userMethods.Contains(EMethod.PostCommentUpdate) || !userMethods.Contains(EMethod.PostCommentAllUpdate));
                            ViewBag.CanDelete = !(!userMethods.Contains(EMethod.PostCommentRemove) || !userMethods.Contains(EMethod.PostCommentAllRemove));

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
        [HttpGet("{postId}")]
        public async Task<IActionResult> Index(long? postId, [FromQuery] int? page)
        {
            if (!userMethods.Contains(EMethod.PostCommentAllList) || !userMethods.Contains(EMethod.PostCommentList))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            //Todo: Admin ve akademisyen ayrımı eklenecek
            var result = await _postCommentService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.PostCommentFilter>
            {
                ContentCount = 10,
                PageCount =page?? 0,
                Filter = new Dtos.Filters.PostCommentFilter
                {
                   PostId= postId,
                   SharedUserId= (!userMethods.Contains(EMethod.PostCommentAllList)) ? loginUserId : null
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
            if (!userMethods.Contains(EMethod.PostCommentDetail))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _postCommentService.Get(id);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(result.Result);
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }

        [HttpGet("Add/{postId}")]
        public async Task<IActionResult> Add(long postId)
        {

            if (!userMethods.Contains(EMethod.PostCommentAdd))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            return View();
        }

        [HttpPost("Add/{postId}")]
        public async Task<IActionResult> Add([FromRoute] long postId, [FromForm] PostCommentDto comment)
        {
            if (!userMethods.Contains(EMethod.PostCommentAdd))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            comment.SenderUserId =(long) loginUserId;
            comment.PostId = postId;
            var result = await _postCommentService.Add(comment);
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
            if (((await _postCommentService.Get(id))?.Result?.Post.UserId != loginUserId && !userMethods.Contains(EMethod.PostCommentAllRemove)) || !userMethods.Contains(EMethod.PostCommentRemove))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _postCommentService.Delete(id);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            return View();
        }


        [HttpGet("Update/{Id}")]
        public async Task<IActionResult> Update(long id)
        {
            var result = await _postCommentService.Get(id);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                if ((result?.Result?.Post.UserId != loginUserId && !userMethods.Contains(EMethod.PostCommentAllUpdate)) || !userMethods.Contains(EMethod.PostCommentUpdate))
                {
                    _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                    return Redirect("/");
                }
                return View(_mapper.Map<PostCommentDto>(result.Result));
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }

        [HttpPost("Update/{id}")]
        public async Task<IActionResult> Update(PostCommentDto comment)
        {
            if (((await _postCommentService.Get(comment.Id))?.Result?.Post.UserId != loginUserId && !userMethods.Contains(EMethod.PostCommentAllUpdate)) || !userMethods.Contains(EMethod.PostCommentUpdate))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            comment.SenderUserId =(long) loginUserId;
            var result = await _postCommentService.Update(comment);
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
