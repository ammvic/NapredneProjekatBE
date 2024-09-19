using MediatR;
using PhotoStudio.API.Mediator.Album.Query;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.Album.Handler
{
    public class GetAlbumByCodeQueryHandler : IRequestHandler<GetAlbumByCodeQuery, AlbumDTO>
    {
        private readonly IAlbumRepository _albumRepository;

        public GetAlbumByCodeQueryHandler(IAlbumRepository albumRepository)
        {
            _albumRepository = albumRepository;
        }

        public async Task<AlbumDTO> Handle(GetAlbumByCodeQuery request, CancellationToken cancellationToken)
        {
            var album = await _albumRepository.GetAlbumByCodeAsync(request.Code);

            if (album == null)
            {
                return null; 
            }

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
