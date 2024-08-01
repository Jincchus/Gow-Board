namespace GowBoard.Models.Service.Interface
{
    public interface INotificationService
    {
        void SendNotification(string memberId, string message);
    }
}
