namespace GowBoard.Models.Service.Interface
{
    public interface INotificationService
    {
        void NotifyUser(string memberId, string message);
    }
}
