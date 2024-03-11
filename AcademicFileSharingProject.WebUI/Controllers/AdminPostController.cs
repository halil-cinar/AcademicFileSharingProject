using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Entities.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NToastNotify;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    [Route("/Admin/Post")]
    public class AdminPostController : Controller
    {

        private readonly IPostService _postService;
        private readonly IPostMediaService _postMediaService;
        private readonly IPostMediaDownloadService _postMediaDownloadService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly List<EMethod> userMethods;
        private readonly IToastNotification _toastNotification;
        private readonly IAccountService _accountService;
        private readonly long? loginUserId = null;
        private readonly IMapper _mapper;

        public AdminPostController(IPostService postService, IToastNotification toastNotification, IMapper mapper, IPostMediaService postMediaService, IHttpContextAccessor contextAccessor, IAccountService accountService, IPostMediaDownloadService postMediaDownloadService)
        {
            _postService = postService;
            _toastNotification = toastNotification;
            _mapper = mapper;
            _postMediaService = postMediaService;
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
                            ViewBag.CanUpdate = !(!userMethods.Contains(EMethod.PostUpdate) || !userMethods.Contains(EMethod.PostAllUpdate));
                            ViewBag.CanDelete = !(!userMethods.Contains(EMethod.PostRemove) || !userMethods.Contains(EMethod.PostAllRemove));

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
            _postMediaDownloadService = postMediaDownloadService;
        }


        public async Task<IActionResult> Index()
        {
            if (!userMethods.Contains(EMethod.PostAllList) || !userMethods.Contains(EMethod.PostList))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var response = await _postService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.PostFilter>
            {
                ContentCount = 20,
                PageCount = 0,
                Filter = new Dtos.Filters.PostFilter
                {
                    UserId = (!userMethods.Contains(EMethod.PostAllList)) ? loginUserId : null
                }
            });
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(response.Result);
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return View();
        }




        [HttpGet("Add")]
        public IActionResult Add()
        {
            if (!userMethods.Contains(EMethod.PostAdd))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            Response.Cookies.Append("Ad", "Halil");
            return View();
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(PostDto post)
        {
            if (!userMethods.Contains(EMethod.PostAdd))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            post.UserId = (long)loginUserId;
            var response = await _postService.Add(post);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return View();

        }


        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            if (((await _postService.Get(id))?.Result?.UserId != loginUserId && !userMethods.Contains(EMethod.PostAllRemove)) || !userMethods.Contains(EMethod.PostRemove))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }

            var response = await _postService.Delete(id);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return RedirectToAction("Index");
        }

        [HttpGet("Update/{id}")]
        public async Task<IActionResult> Update(long id)
        {
            var response = await _postService.Get(id);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                if ((response?.Result?.UserId != loginUserId && !userMethods.Contains(EMethod.PostAllRemove)) || !userMethods.Contains(EMethod.PostRemove))
                {
                    _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                    return Redirect("/");
                }
                return View(_mapper.Map<PostDto>(response.Result));
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return RedirectToAction("Index");
        }



        [HttpPost("Update/{id}")]
        public async Task<IActionResult> Update(PostDto post)
        {
            if ((post.UserId != loginUserId && !userMethods.Contains(EMethod.PostAllUpdate)) || !userMethods.Contains(EMethod.PostUpdate))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            post.UserId = (long)loginUserId;
            var response = await _postService.Update(post);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return View(response.Result);
        }

        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(long id)
        {
            if (!userMethods.Contains(EMethod.PostDetail))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var mediaResult = await _postMediaService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.PostMediaFilter>
            {
                ContentCount = int.MaxValue,
                PageCount = 0,
                Filter = new Dtos.Filters.PostMediaFilter
                {
                    PostId = id
                }
            });
            if (mediaResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
            {
                var message2 = string.Join(Environment.NewLine, mediaResult.ErrorMessages.Select(x => x.Message).ToList());
                _toastNotification.AddErrorToastMessage(message2);

                return RedirectToAction("Index");
            }
            ViewBag.Files = mediaResult.Result.Values.Select(x => x.Media).ToList();
            var response = await _postService.Get(id);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View((response.Result));
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return RedirectToAction("Index");
        }



        [HttpGet("FilesUpdate/{id}")]
        public async Task<IActionResult> FilesUpdate(long id)
        {

            var response = await _postService.Get(id);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                if ((response?.Result?.UserId != loginUserId && !userMethods.Contains(EMethod.PostAllRemove)) || !userMethods.Contains(EMethod.PostRemove))
                {
                    _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                    return Redirect("/");
                }
                return View(new PostDto
                {
                    Id = id,
                    UserId = response.Result.UserId,

                });
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return RedirectToAction("Index");
        }

        [HttpPost("FilesUpdate/{id}")]
        public async Task<IActionResult> FilesUpdate(PostDto post)
        {
            if ((post.UserId != loginUserId && !userMethods.Contains(EMethod.PostAllUpdate)) || !userMethods.Contains(EMethod.PostUpdate))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var response = await _postService.ChangeFiles(post);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return View();
        }
        [HttpGet("PhotoUpdate/{id}")]
        public async Task<IActionResult> PhotoUpdate(long id)
        {

            var response = await _postService.Get(id);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                if ((response?.Result?.UserId != loginUserId && !userMethods.Contains(EMethod.PostAllRemove)) || !userMethods.Contains(EMethod.PostRemove))
                {
                    _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                    return Redirect("/");
                }
                return View(response.Result);
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return RedirectToAction("Index");
        }
        [HttpPost("PhotoUpdate/{id}")]
        public async Task<IActionResult> PhotoUpdate(PostDto post)
        {
            if ((post.UserId != loginUserId && !userMethods.Contains(EMethod.PostAllUpdate)) || !userMethods.Contains(EMethod.PostUpdate))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var response = await _postService.ChangeMedia(post);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return View();
        }


        [HttpGet("ChangeIsAir/{id}")]
        public async Task<IActionResult> ChangeIsAir(long id)
        {
            var response = await _postService.Get(id);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                if ((response?.Result?.UserId != loginUserId && !userMethods.Contains(EMethod.PostAllRemove)) || !userMethods.Contains(EMethod.PostRemove))
                {
                    _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                    return Redirect("/");
                }
                response.Result.IsAir = !response.Result.IsAir;
                await Update(_mapper.Map<PostDto>(response.Result));
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return RedirectToAction("Index");
        }


        [HttpGet("Files/{postId}")]
        public async Task<IActionResult> PostMedias(long postId, [FromQuery] int? page)
        {
            var response = await _postMediaService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.PostMediaFilter>
            {
                ContentCount = 15,
                PageCount = page ?? 0,
                Filter = new Dtos.Filters.PostMediaFilter
                {
                    PostId = postId,
                }
            });

            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(response.Result);
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return RedirectToAction("Index");
        }

        [HttpGet("DownloadedUsers/{postMediaId}")]
        public async Task<IActionResult> DownloadedUsers(long postMediaId, [FromQuery] int? page)
        {
            var response = await _postMediaDownloadService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.PostMediaDownloadFilter>
            {
                ContentCount = 25,
                PageCount = page ?? 0,
                Filter = new Dtos.Filters.PostMediaDownloadFilter
                {
                    PostMediaId = postMediaId,
                }
            });

            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(response.Result);
            }
            var message = string.Join(Environment.NewLine, response.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return RedirectToAction("Index");
        }




    }
}
