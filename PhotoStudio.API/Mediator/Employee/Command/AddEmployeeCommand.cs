using MediatR;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Models;

namespace PhotoStudio.API.Mediator.Employee.Command
{
    public class AddEmployeeCommand : IRequest<EmployeeDTO>
    {
        public string JMBG { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string PasswordHash { get; }
        public Role Role { get; }

        public AddEmployeeCommand(string jmbg, string firstName, string lastName, string passwordHash, Role role)
        {
            JMBG = jmbg;
            FirstName = firstName;
            LastName = lastName;
            PasswordHash = passwordHash;
            Role = role;
        }
    }
}
