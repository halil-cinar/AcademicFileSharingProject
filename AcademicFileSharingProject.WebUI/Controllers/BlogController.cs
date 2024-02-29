using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.DataAccess.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    [Route("Blog")]
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly IBlogCommentService _blogCommentService;
        private readonly IToastNotification _toastNotification;
        private readonly long loginUserID=1;
        public BlogController(IToastNotification toastNotification, IBlogService blogService, IBlogCommentService blogCommentService)
        {
            _toastNotification = toastNotification;
            _blogService = blogService;
            _blogCommentService = blogCommentService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var result = await _blogService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.BlogFilter>
            {
                ContentCount = 10,
                PageCount = 0,
                Filter = new Dtos.Filters.BlogFilter
                {
                    IsAir= true,
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

        [HttpGet("{id:long}")]
        public async Task<IActionResult> Detail(long id)
        {
            var result = await _blogService.Get(id);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                if (result.Result.IsAir == false)
                {
                    _toastNotification.AddErrorToastMessage("İlgili post yayından kaldırıldığı için görüntüleyemezsiniz.");
                    return RedirectToAction("Index");
                }
                return View(result.Result);
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return RedirectToAction("Index");
        }

        [HttpGet("AddComment")]
        public async Task<IActionResult> AddComment(BlogCommentDto blogComment)
        {
            blogComment.SenderUserId =(long) loginUserID;
            var result = await _blogCommentService.Add(blogComment);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return Redirect("/blog/" + blogComment.BlogId);
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return Redirect("/blog/" + blogComment.BlogId);
        }

    }
}
