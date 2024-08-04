using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace GowBoard.Hubs
{
    public class NotificationHub : Hub
    {
        public override Task OnConnected()
        {
            string memberId = Context.QueryString["memberId"];
            if (!string.IsNullOrEmpty(memberId))
            {
                Groups.Add(Context.ConnectionId, memberId);
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string memberId = Context.QueryString["memberId"];
            if (!string.IsNullOrEmpty(memberId))
            {
                Groups.Remove(Context.ConnectionId, memberId);
            }
            return base.OnDisconnected(stopCalled);
        }

        public void AssociateUser(string memberId)
        {
            Groups.Add(Context.ConnectionId, memberId);
        }

    }
}