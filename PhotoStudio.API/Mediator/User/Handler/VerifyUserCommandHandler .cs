using MediatR;
using PhotoStudio.API.Mediator.User.Command;
using PhotoStudio.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.User.Handler
{
    public class VerifyUserCommandHandler : IRequestHandler<VerifyUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public VerifyUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
        {
            return await _userRepository.VerifyUserAsync(request.Email, request.VerificationCode);
        }
    }
}
