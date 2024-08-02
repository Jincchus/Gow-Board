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

        public void SendNotification(string memberId, string message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.User(memberId).receiveNotification(message);
        }

    }
}