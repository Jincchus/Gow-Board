using GowBoard.Hubs;
using GowBoard.Models.Service.Interface;
using Microsoft.AspNet.SignalR;

namespace GowBoard.Models.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext _hubContext;

        public NotificationService(IHubContext hubContext)
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        }

        public void SendNotification(string memberId, string message)
        {
            _hubContext.Clients.User(memberId).receiveNotification(message);
        }
    }
}