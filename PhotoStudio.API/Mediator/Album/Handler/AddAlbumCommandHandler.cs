using MediatR;
using PhotoStudio.API.Mediator.Album.Command;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.Album.Handler
{
    public class AddAlbumCommandHandler : IRequestHandler<AddAlbumCommand, AlbumDTO>
    {
        private readonly IAlbumRepository _albumRepository;

        public AddAlbumCommandHandler(IAlbumRepository albumRepository)
        {
            _albumRepository = albumRepository;
        }

        public async Task<AlbumDTO> Handle(AddAlbumCommand request, CancellationToken cancellationToken)
        {
            var album = new PhotoStudio.Domain.Models.Album
            {
                Name = request.Name,
                Code = request.Code,
                IsPublic = request.IsPublic,
                EmployeeId = request.EmployeeId,
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow,
                Media = new List<Media>()
            };

            await _albumRepository.AddAlbumAsync(album);

            return new AlbumDTO
            {
                Id = album.Id,
                Name = album.Name,
                Code = album.Code,
                IsPublic = album.IsPublic,
                EmployeeId = album.EmployeeId,
                UserId = album.UserId
            };
        }
    }
}
