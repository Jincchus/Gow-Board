using GowBoard.Models.DTO.ResponseDTO;

namespace GowBoard.Models.Service.Interface
{
    public interface INotificationService
    {
        void NotifyUser(ResBoardDetailDTO resBoardDetailDTO);
    }
}
