using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Core.ExtensionsMethods;
using AcademicFileSharingProject.Core.WebUI;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.Hubs
{
    public class NotificationHub : Hub, INotificationHub
    {
       
        public async Task SendNotification(long userId,string message)
        {
            await Clients.Clients(ExtensionsMethods.CalculateMD5Hash("GroupId="+userId)).SendAsync("ReceiveNotification", message);
        }

    }
}
