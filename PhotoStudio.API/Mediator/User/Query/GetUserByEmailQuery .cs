using MediatR;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Models;

namespace PhotoStudio.API.Mediator.User.Query
{
    public class GetUserByEmailQuery : IRequest<UserDTO>
    {
        public string Email { get; }

        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }
    }
}
