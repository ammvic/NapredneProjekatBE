using MediatR;
using PhotoStudio.API.Mediator.User.Command;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.User.Handler
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, UserDTO>
    {
        private readonly IUserRepository _userRepository;

        public LoginUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDTO> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.LoginAsync(request.Email, request.Password);

            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Credits = user.Credits,
                IsVerified = user.IsVerified
            };
        }
    }
}
