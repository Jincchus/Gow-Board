using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace GowBoard.Hubs
{
    public class NotificationHub : Hub
    {
        public static void SendNotificationToUser(string recipientMemberId, string message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.Group(recipientMemberId).receiveNotification(message, recipientMemberId);
        }
    }
}