using MediatR;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Models;

namespace PhotoStudio.API.Mediator.User.Command
{
    public class AccessAlbumByCodeCommand : IRequest<AlbumDTO>
    {
        public string AlbumCode { get; }

        public AccessAlbumByCodeCommand(string albumCode)
        {
            AlbumCode = albumCode;
        }
    }
}
