using MediatR;
using PhotoStudio.Application.DTOs;

namespace PhotoStudio.API.Mediator.User.Command
{
    public class RegisterUserCommand : IRequest<UserDTO>
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Password { get; }
        public string PhoneNumber { get; }
        public string Address { get; }
        public string VerificationCode { get; }

        public RegisterUserCommand(string firstName, string lastName, string email, string password, string phoneNumber, string address, string verificationCode)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            PhoneNumber = phoneNumber;
            Address = address;
            VerificationCode = verificationCode;
        }
    }
}
