using MediatR;
using PhotoStudio.Application.DTOs;

namespace PhotoStudio.API.Mediator.Album.Command
{
    public class AddAlbumCommand : IRequest<AlbumDTO>
    {
        public string Name { get; }
        public string Code { get; }
        public bool IsPublic { get; }
        public int EmployeeId { get; }
        public int? UserId { get; }

        public AddAlbumCommand(string name, string code, bool isPublic, int employeeId, int? userId)
        {
            Name = name;
            Code = code;
            IsPublic = isPublic;
            EmployeeId = employeeId;
            UserId = userId;
        }
    }
}
