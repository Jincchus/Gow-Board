using GowBoard.Hubs;
using GowBoard.Models.DTO.ResponseDTO;
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

        public void NotifyUser(ResBoardDetailDTO resBoardDetailDTO)
        {
            var context = _hubContext.Value;
            var memberId = resBoardDetailDTO.Writer.MemberId;
            var title = resBoardDetailDTO.Title;
            var contentId = resBoardDetailDTO.BoardContentId;
            context.Clients.Group(memberId).receiveNotification(memberId, title, contentId);
        }

    }
}