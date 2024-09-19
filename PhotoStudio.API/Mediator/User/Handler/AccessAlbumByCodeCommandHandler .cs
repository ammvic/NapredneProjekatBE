using MediatR;
using PhotoStudio.API.Mediator.User.Command;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.User.Handler
{
    public class AccessAlbumByCodeCommandHandler : IRequestHandler<AccessAlbumByCodeCommand, AlbumDTO>
    {
        private readonly IUserRepository _userRepository;

        public AccessAlbumByCodeCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<AlbumDTO> Handle(AccessAlbumByCodeCommand request, CancellationToken cancellationToken)
        {
            var album = await _userRepository.AccessAlbumByCodeAsync(request.AlbumCode);

            return new AlbumDTO
            {
                Id = album.Id,
                Name = album.Name,
                Code = album.Code
                
            };
        }
    }
}
