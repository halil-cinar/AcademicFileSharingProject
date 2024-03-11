using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using AcademicFileSharingProject.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.ViewComponents._LayoutComponents
{
    public class _ChatPageChatsComponent : ViewComponent
    {
        private readonly IChatService _chatService;
        private readonly IChatUserService _chatUserService;
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        private readonly IToastNotification _toastNotification;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly long? loginUserId = null;
        public _ChatPageChatsComponent(IChatService chatService, IChatUserService chatUserService, IUserService userService, IToastNotification toastNotification, IHttpContextAccessor httpContextAccessor, IAccountService accountService)
        {
            _chatService = chatService;
            _chatUserService = chatUserService;
            _userService = userService;
            _toastNotification = toastNotification;
            _contextAccessor = httpContextAccessor;
            _accountService = accountService;
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
        public async Task<IViewComponentResult> InvokeAsync()
        {

            var result = await _chatService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.ChatFilter>
            {
                ContentCount = 10,
                PageCount = 0,
                Filter = new Dtos.Filters.ChatFilter
                {
                    UserId = loginUserId,
                }
            }
            );
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)

            {
                ViewBag.Chats = result.Result;
                return View();
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            _contextAccessor?.HttpContext?.Response?.Redirect("/");

            return View();
        }
    }
}
