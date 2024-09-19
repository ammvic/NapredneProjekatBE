using MediatR;

namespace PhotoStudio.API.Mediator.User.Command
{
    public class VerifyUserCommand : IRequest<bool>
    {
        public string Email { get; }
        public string VerificationCode { get; }

        public VerifyUserCommand(string email, string verificationCode)
        {
            Email = email;
            VerificationCode = verificationCode;
        }
    }
}
