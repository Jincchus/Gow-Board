using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace GowBoard.Hubs
{
    public class NotificationHub : Hub
    {
        public override Task OnConnected()
        {
            var memberId = Context.QueryString["MemberId"];
            if (!string.IsNullOrEmpty(memberId))
            {
                Groups.Add(Context.ConnectionId, memberId);
            }
            else
            {
                Groups.Add(Context.ConnectionId, "AnonymousGroup");
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var memberId = Context.User.Identity.Name;
            if (!string.IsNullOrEmpty(memberId))
            {
                Groups.Remove(Context.ConnectionId, memberId);
            }
            return base.OnDisconnected(stopCalled);
        }

        public static void SendNotificationToUser(string memberId, string message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.Group(memberId).receiveNotification(message, memberId);
        }
    }
}