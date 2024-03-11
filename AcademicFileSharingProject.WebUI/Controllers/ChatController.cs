using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using AcademicFileSharingProject.Dtos.Result;
using AcademicFileSharingProject.Entities.Enums;
using AcademicFileSharingProject.WebUI.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly IUserDeviceService _userDeviceService;
        private readonly long? loginUserId = null;
        private readonly UserListDto loginUser = null;
        private readonly IAccountService _accountService;
        private readonly List<EMethod> userMethods;
        private readonly IHttpContextAccessor _contextAccessor;

        public ChatController(IToastNotification toastNotification, IUserService userService, IChatService chatService, IMessageService messageService, IHubContext<SignalRHub> hubContext, IUserDeviceService userDeviceService, IAccountService accountService, IHttpContextAccessor contextAccessor, IChatUserService chatUserService)
        {
            _toastNotification = toastNotification;
            _userService = userService;
            _chatService = chatService;
            _messageService = messageService;
            _hubContext = hubContext;
            _userDeviceService = userDeviceService;
            _accountService = accountService;
            _contextAccessor = contextAccessor;


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
            _chatUserService = chatUserService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            if (loginUserId == null)
            {
                _toastNotification.AddInfoToastMessage("Bu alana giriş yapmak için lütfen giriş yapınız");
                return Redirect("/");
            }
            ViewBag.loginUserId = loginUserId;

            return View();
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> Index(long id)
        {
            if (loginUserId == null)
            {
                _toastNotification.AddInfoToastMessage("Bu alana giriş yapmak için lütfen giriş yapınız");
                return Redirect("/");
            }
            var messageResult = await _messageService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.MessageFilter>
            {

                ContentCount = 50,
                PageCount = 0,
                Filter = new Dtos.Filters.MessageFilter
                {
                    ChatId = id,
                }
            });
            if (messageResult.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                ViewBag.loginUserId = loginUserId;
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
                if (!result.Result.ChatUsers.Select(x => x.UserId).ToList().Contains((long)loginUserId))
                {
                    _toastNotification.AddInfoToastMessage("Giriş yetkiniz bulunmamaktadır");
                    return Redirect("/");
                }


                ViewBag.loginUserId = loginUserId;
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
            if (loginUserId == null)
            {
                _toastNotification.AddInfoToastMessage("Bu alana giriş yapmak için lütfen giriş yapınız");
                return Redirect("/");
            }
            var userList = new List<long>();
            userList.Add(userId);
            userList.Add((long)loginUserId);
            var result = await _chatService.Add(new Dtos.AddOrUpdateDtos.ChatDto
            {
                Title = "",
                ChatType = Entities.Enums.EChatType.Private,
                UserIds = userList,

            });
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return Redirect("/chat/" + result.Result.Id);
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);


            return Redirect("/chat");
        }

        [HttpPost("send")]
        public async Task<bool> SendMessage([FromBody] MessageDto messageDto)
        {
            if (loginUserId == null)
            {
                _toastNotification.AddInfoToastMessage("Bu alana giriş yapmak için lütfen giriş yapınız");
                return false;
            }
            messageDto.SenderUserId = (long)loginUserId;
            var result = await _messageService.Add(messageDto);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                var userResult = await _chatUserService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.ChatUserFilter>
                {
                    ContentCount = int.MaxValue,
                    PageCount = 0,
                    Filter = new Dtos.Filters.ChatUserFilter
                    {
                        ChatId = messageDto.ChatId,
                    }
                });
                if (userResult.ResultStatus == Dtos.Enums.ResultStatus.Success)
                {
                    var userList = userResult.Result.Values.Select(x => x.UserId).ToList();

                    var deviceResult = await _userDeviceService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.UserDeviceFilter>
                    {
                        ContentCount = int.MaxValue,
                        PageCount = 0,
                        Filter = new Dtos.Filters.UserDeviceFilter
                        {
                            UserIds = userList
                        }
                    });
                    if (deviceResult.ResultStatus == Dtos.Enums.ResultStatus.Success)
                    {
                        var conIds = deviceResult.Result.Values.Select(x => x.ConnectionId).ToList();
                        await _hubContext.Clients.Clients(conIds).SendAsync("ReceiveMessage", messageDto);
                    }
                }

                return true;
            }
            //var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            //_toastNotification.AddErrorToastMessage(message);


            return false;
        }
    }
}
