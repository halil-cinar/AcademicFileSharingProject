using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.Hubs
{
    public class MessageHub : Hub
    {

        private readonly IChatService _chatService;
        private readonly IChatUserService _chatUserService;
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly IToastNotification _toastNotification;

        public MessageHub(IToastNotification toastNotification, IUserService userService, IMessageService messageService, IChatUserService chatUserService, IChatService chatService)
        {
            _toastNotification = toastNotification;
            _userService = userService;
            _messageService = messageService;
            _chatUserService = chatUserService;
            _chatService = chatService;
        }

        private readonly long loginUserId = 1;

        public async Task SendMessage(MessageDto message)
        {
            message.SenderUserId =(long) loginUserId;
            var result = await _messageService.Add(message);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                await Clients.All.SendAsync("ReceiveMessage", message);
            }

        }
    }
}
