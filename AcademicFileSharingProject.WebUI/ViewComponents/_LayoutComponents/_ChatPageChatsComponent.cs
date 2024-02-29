using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.ViewComponents._LayoutComponents
{
    public class _ChatPageChatsComponent : ViewComponent
    {
        private readonly IChatService _chatService;
        private readonly IChatUserService _chatUserService;
        private readonly IUserService _userService;
        private readonly IToastNotification _toastNotification;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly long loginUserId = 1;
        public _ChatPageChatsComponent(IChatService chatService, IChatUserService chatUserService, IUserService userService, IToastNotification toastNotification, IHttpContextAccessor httpContextAccessor)
        {
            _chatService = chatService;
            _chatUserService = chatUserService;
            _userService = userService;
            _toastNotification = toastNotification;
            _httpContextAccessor = httpContextAccessor;
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

            _httpContextAccessor?.HttpContext?.Response?.Redirect("/");

            return View();
        }
    }
}
