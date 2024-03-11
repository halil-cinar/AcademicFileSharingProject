using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using AcademicFileSharingProject.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    [Route("/Academician")]
    public class AcademicianController : Controller
    {
        private readonly IUserRoleService _userRoleService;
        private readonly IUserService _userService;
        private readonly ISubscribeService _subscribeService;
        private readonly IPostMediaDownloadService _postMediaDownloadService;
        private readonly IPostService _postService;
        private readonly IToastNotification _toastNotification;
        private readonly long? loginUserId = null;
        private readonly UserListDto loginUser = null;
        private readonly IAccountService _accountService;
        private readonly List<EMethod> userMethods;
        private readonly IHttpContextAccessor _contextAccessor;

        public AcademicianController(IToastNotification toastNotification, IUserRoleService userRoleService, IUserService userService, IPostService postService, ISubscribeService subscribeService, IHttpContextAccessor httpContextAccessor, IAccountService accountService, IPostMediaDownloadService postMediaDownloadService)
        {
            _toastNotification = toastNotification;
            _userRoleService = userRoleService;
            _userService = userService;
            _postService = postService;
            _contextAccessor = httpContextAccessor;
            _postMediaDownloadService = postMediaDownloadService;
            _subscribeService = subscribeService;
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
        }

        public async Task<IActionResult> Index([FromQuery] int? page, [FromQuery] string? search)
        {
            var result = await _userRoleService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.UserRoleFilter>
            {
                ContentCount = 15,
                PageCount = page ?? 0,
                Filter = new Dtos.Filters.UserRoleFilter
                {
                    Role = Entities.Enums.ERoles.Academician,
                    Search=search
                }
            });
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                ViewBag.Search = search ;
                var list = result.Result.Values.Select(x => x.User).ToList();
                return View(new GenericLoadMoreDto<UserListDto>
                {
                    Values = list,
                    ContentCount = result.Result.ContentCount,
                    PageCount = result.Result.PageCount,
                    NextPage = result.Result.NextPage,
                    PrevPage = result.Result.PrevPage,
                    TotalContentCount = result.Result.TotalContentCount,
                    TotalPageCount = result.Result.TotalPageCount

                });
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Profile(long id, [FromQuery] int? page)
        {

            var result = await _userService.Get(id);
            var postResult = await _postService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.PostFilter>
            {
                ContentCount = 10,
                PageCount = page ?? 0,
                Filter = new Dtos.Filters.PostFilter
                {
                    UserId = id,
                }
            });
            if (postResult.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                ViewBag.Posts = postResult.Result;
            }
            else
            {
                var message2 = string.Join(Environment.NewLine, postResult.ErrorMessages.Select(x => x.Message).ToList());
                _toastNotification.AddErrorToastMessage(message2);
            }
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(result.Result);
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return RedirectToAction("Index");
        }

        [HttpGet("Download/{userId}/{postId}")]
        public async Task<IActionResult> DownloadPostMedia(long userId,long postId)
        {
            if (loginUserId == null)
            {
                _toastNotification.AddAlertToastMessage("Lütfen önce giriş yapınız");
                return Redirect("/Academician/" + userId);

            }

            
            var result = await _postMediaDownloadService.AddByPost(new Dtos.AddOrUpdateDtos.PostMediaDownloadDto
            {
                CreatedTime = DateTime.Now,
                PostMediaId = postId,
                UserId = loginUserId ?? 0,

            });
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
               // _toastNotification.AddSuccessToastMessage("İndirme işlemi başlatılmıştır");

                return Redirect("/Media?ids="+string.Join(",",result.Result));
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return Redirect("/Academician/" + userId);
        }



        [HttpGet("AddSubscribe/{userId:long}")]
        public async Task<IActionResult> AddSubscribe(long userId)
        {

            if (loginUserId == null)
            {
                _toastNotification.AddAlertToastMessage("Lütfen önce giriş yapınız");
                return Redirect("/Academician/"+userId);

            }

            var result = await _subscribeService.Add(new Dtos.AddOrUpdateDtos.SubscribeDto
            {
                CreatedTime = DateTime.Now,
                UserID = loginUserId,
                SubscribeUserID = userId,
                Email=loginUser.Email,
                SubscribeDate= DateTime.Now,
            });
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                _toastNotification.AddSuccessToastMessage("Aboneliğiniz alınmıştır");

                return Redirect("/Academician/" + userId);
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return Redirect("/Academician/" + userId);
        }


    }
}
