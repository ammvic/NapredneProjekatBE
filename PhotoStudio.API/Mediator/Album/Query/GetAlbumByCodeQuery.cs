using MediatR;
using PhotoStudio.Application.DTOs;

namespace PhotoStudio.API.Mediator.Album.Query
{
    public class GetAlbumByCodeQuery : IRequest<AlbumDTO>
    {
        public string Code { get; }

        public GetAlbumByCodeQuery(string code)
        {
            Code = code;
        }
    }
}
