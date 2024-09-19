using MediatR;
using PhotoStudio.API.Mediator.User.Command;
using PhotoStudio.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.User.Handler
{
    public class DownloadMediaCommandHandler : IRequestHandler<DownloadMediaCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public DownloadMediaCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(DownloadMediaCommand request, CancellationToken cancellationToken)
        {
            return await _userRepository.DownloadMediaAsync(request.UserId, request.AlbumCode, request.MediaId);
        }
    }
}
