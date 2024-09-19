using MediatR;
using PhotoStudio.API.Mediator.User.Command;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.User.Handler
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDTO>
    {
        private readonly IUserRepository _userRepository;

        public RegisterUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDTO> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Pozivanje metode za registraciju sa verifikacijom
            var isRegistered = await _userRepository.VerifyAndRegisterUserAsync(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password,
                request.PhoneNumber,
                request.Address
            );

            if (!isRegistered)
            {
                // Ako registracija nije uspela, vraćamo null ili možete obraditi grešku na odgovarajući način.
                return null;
            }

            // Pretpostavka da se korisnik registruje, a zatim vraća user objekat sa svim informacijama.
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
                IsVerified = user.IsVerified // Status verifikacije
            };
        }
    }
}
