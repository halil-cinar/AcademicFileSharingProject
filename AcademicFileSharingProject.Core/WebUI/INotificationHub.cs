using Microsoft.AspNetCore.SignalR;

namespace AcademicFileSharingProject.Core.WebUI
{
    public abstract class INotificationHub:Hub
    {
        public abstract Task SendNotification(long userId, string message);

    }
}
