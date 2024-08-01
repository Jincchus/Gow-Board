using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace GowBoard.Hubs
{
    public class NotificationHub : Hub
    {
        // 알람 보내기
        public async Task SendNotification(string memberId, string message)
        {
            await Clients.User(memberId).receiveNotification(message);
        }
    }
}