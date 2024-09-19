using MediatR;

namespace PhotoStudio.API.Mediator.User.Command
{
    public class DownloadMediaCommand : IRequest<bool>
    {
        public int UserId { get; }
        public string AlbumCode { get; }
        public int MediaId { get; }

        public DownloadMediaCommand(int userId, string albumCode, int mediaId)
        {
            UserId = userId;
            AlbumCode = albumCode;
            MediaId = mediaId;
        }
    }
}
