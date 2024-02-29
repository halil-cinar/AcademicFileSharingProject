using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using AcademicFileSharingProject.Dtos.Result;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NToastNotify;
using NuGet.Protocol;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    [Route("/chat")]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IChatUserService _chatUserService;
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly IToastNotification _toastNotification;
        private readonly long loginUserId = 1;

        public ChatController(IToastNotification toastNotification, IUserService userService, IChatService chatService, IMessageService messageService)
        {
            _toastNotification = toastNotification;
            _userService = userService;
            _chatService = chatService;
            _messageService = messageService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewBag.loginUserId = loginUserId;

            return View();
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> Index(long id)
        {
            var messageResult = await _messageService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.MessageFilter>
            {

                ContentCount = 10,
                PageCount = 0,
                Filter = new Dtos.Filters.MessageFilter
                {
                    ChatId = id,
                }
            });
            if (messageResult.ResultStatus==Dtos.Enums.ResultStatus.Success)
            {
                ViewBag.loginUserId=loginUserId;
                ViewBag.Messages = messageResult.Result;
            }
            else
            {
                var message2 = string.Join(Environment.NewLine, messageResult.ErrorMessages.Select(x => x.Message).ToList());
                _toastNotification.AddErrorToastMessage(message2);

            }
            var result = await _chatService.Get(id);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                ViewBag.loginUserId=loginUserId;
                return View(result.Result);
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);


            return View();

        }

        [HttpGet("search/user")]
        public async Task<List<UserListDto>> SearchUser([FromQuery] string query)
        {
            var result = await _userService.SearchUser(query);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return result.Result;
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);


            return null;
        }

        [HttpGet("addDirectChat/{userId:long}")]
        public async Task<IActionResult> AddDirectChat(long userId)
        {
            var userList = new List<long>();
            userList.Add(userId);
            userList.Add(loginUserId);
            var result = await _chatService.Add(new Dtos.AddOrUpdateDtos.ChatDto
            {
                Title = "",
                ChatType = Entities.Enums.EChatType.Private,
                UserIds = userList,
                
            });
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return Redirect("/chat/"+result.Result.Id);
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);


            return Redirect("/chat");
        }

        [HttpPost("send")]
        public async Task<bool> SendMessage([FromBody]string messageDto)
        {
            var message = JsonConvert.DeserializeObject<MessageDto>(messageDto);
            message.SenderUserId=(long) loginUserId;
            var result = await _messageService.Add(message);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return true;
            }
            //var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            //_toastNotification.AddErrorToastMessage(message);


            return false;
        }
    }
}
