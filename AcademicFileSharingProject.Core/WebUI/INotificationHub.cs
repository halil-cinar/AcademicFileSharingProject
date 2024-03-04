using Microsoft.AspNetCore.SignalR;

namespace AcademicFileSharingProject.Core.WebUI
{
    public interface INotificationHub
    {
        public Task SendNotification(long userId, string message);

    }
}
