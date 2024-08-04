using GowBoard.Hubs;
using GowBoard.Models.Service.Interface;

namespace GowBoard.Models.Service
{
    public class NotificationService : INotificationService
    {
        public void NotifyUser(string memberId, string message)
        {
            NotificationHub.SendNotificationToUser(memberId, message);
        }

    }
}