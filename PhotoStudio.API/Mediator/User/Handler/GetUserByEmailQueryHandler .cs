using MediatR;
using PhotoStudio.API.Mediator.User.Query;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.User.Handler
{
    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDTO>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByEmailQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDTO> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

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
