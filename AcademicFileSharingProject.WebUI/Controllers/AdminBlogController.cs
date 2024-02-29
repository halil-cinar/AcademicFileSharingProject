using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using AcademicFileSharingProject.Entities.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Reflection.Metadata;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    [Route("/Admin/Blog")]
    public class AdminBlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly List<EMethod> userMethods;
        private readonly IToastNotification _toastNotification;
        private readonly IAccountService _accountService;
        private readonly long? loginUserId = null;


        public AdminBlogController(IBlogService blogService, IToastNotification toastNotification, IMapper mapper, IHttpContextAccessor contextAccessor, IAccountService accountService)
        {
            _blogService = blogService;
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
                        var roleResult = _accountService.GetUserRoleMethods(result.Result.Result.UserId);
                        roleResult.Wait();
                        if (roleResult.Result.ResultStatus == Dtos.Enums.ResultStatus.Success)
                        {
                            userMethods = roleResult.Result.Result;
                            ViewBag.CanUpdate = !(!userMethods.Contains(EMethod.BlogUpdate) || !userMethods.Contains(EMethod.BlogAllUpdate));
                            ViewBag.CanDelete = !(!userMethods.Contains(EMethod.BlogRemove) || !userMethods.Contains(EMethod.BlogAllRemove));

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

            if (!userMethods.Contains(EMethod.BlogAllList) || !userMethods.Contains(EMethod.BlogList))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _blogService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.BlogFilter>
            {
                ContentCount = 10,
                PageCount = 0,
                Filter = new Dtos.Filters.BlogFilter
                {
                    UserId = (!userMethods.Contains(EMethod.BlogAllList)) ? loginUserId : null
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(long id)
        {
            if (!userMethods.Contains(EMethod.BlogDetail))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _blogService.Get(id);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(result.Result);
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();


        }


        [HttpGet("Update/{id}")]
        public async Task<IActionResult> Update(long id)
        {

            var result = await _blogService.Get(id);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                if ((result?.Result?.UserId != loginUserId && !userMethods.Contains(EMethod.BlogAllRemove)) || !userMethods.Contains(EMethod.BlogRemove))
                {
                    _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                    return Redirect("/");
                }
                return View(result.Result);
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();


        }

        [HttpPost("Update/{id}")]
        public async Task<IActionResult> Update(BlogDto blog)
        {
            if ((blog.UserId != loginUserId && !userMethods.Contains(EMethod.BlogAllUpdate)) || !userMethods.Contains(EMethod.BlogUpdate))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _blogService.Update(blog);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();


        }


        [HttpGet("Add")]
        public async Task<IActionResult> Add()
        {
            if (!userMethods.Contains(EMethod.BlogAdd))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            return View();
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(BlogDto blog)
        {
            if (!userMethods.Contains(EMethod.BlogAdd))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            blog.UserId = (long)loginUserId;

            var result = await _blogService.Add(blog);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View(blog);


        }


        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {

            if (((await _blogService.Get(id))?.Result?.UserId != loginUserId && !userMethods.Contains(EMethod.BlogAllRemove)) || !userMethods.Contains(EMethod.BlogRemove))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }

            var result = await _blogService.Delete(id);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return RedirectToAction("Index");
        }


        [HttpGet("ChangePhoto/{id}")]
        public async Task<IActionResult> ChangePhoto(long id)
        {
            var result = await _blogService.Get(id);


            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                if ((result.Result.UserId != loginUserId && !userMethods.Contains(EMethod.BlogAllUpdate)) || !userMethods.Contains(EMethod.BlogUpdate))
                {
                    _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                    return Redirect("/");
                }
                return View(new BlogDto
                {
                    Id = id,

                });
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();


        }

        [HttpPost("ChangePhoto/{id}")]
        public async Task<IActionResult> ChangePhoto(BlogDto blog)
        {
            if ((blog.UserId != loginUserId && !userMethods.Contains(EMethod.BlogAllUpdate)) || !userMethods.Contains(EMethod.BlogUpdate))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _blogService.ChangePhoto(blog);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View(blog);


        }

        [HttpGet("ChangeIsAir/{id}")]
        public async Task<IActionResult> ChangeIsAir(long id)
        {

            var result = await _blogService.Get(id);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                if ((result.Result.UserId != loginUserId && !userMethods.Contains(EMethod.BlogAllUpdate)) || !userMethods.Contains(EMethod.BlogUpdate))
                {
                    _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                    return Redirect("/");
                }
                result.Result.IsAir = !result.Result.IsAir;
                await Update(_mapper.Map<BlogDto>(result.Result));
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return RedirectToAction("Index");
        }


    }
}
