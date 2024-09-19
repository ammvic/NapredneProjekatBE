using MediatR;
using PhotoStudio.Application.DTOs;

namespace PhotoStudio.API.Mediator.User.Command
{
    public class LoginUserCommand : IRequest<UserDTO>
    {
        public string Email { get; }
        public string Password { get; }

        public LoginUserCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
