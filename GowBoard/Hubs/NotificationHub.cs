using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace GowBoard.Hubs
{
    public class NotificationHub : Hub
    {
        // 클라이언트가 호출할 수 있는 메서드
        public async Task receiveNotification( string message)
        {
            // 클라이언트가 호출할 메서드
            // 클라이언트가 호출할 때는 이 메서드가 필요합니다.
            await Clients.Caller.receiveNotification(message);
        }
    }
}