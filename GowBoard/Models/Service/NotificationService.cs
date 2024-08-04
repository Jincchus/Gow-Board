using GowBoard.Hubs;
using GowBoard.Models.Service.Interface;
using Microsoft.AspNet.SignalR;
using System;

namespace GowBoard.Models.Service
{
    public class NotificationService : INotificationService
    {
        private readonly Lazy<IHubContext> _hubContext;

        public NotificationService()
        {
            _hubContext = new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<NotificationHub>());
        }

        public void NotifyUser(string memberId, string message)
        {
            var context = _hubContext.Value;
            context.Clients.Group(memberId).receiveNotification(memberId, message);
        }

    }
}