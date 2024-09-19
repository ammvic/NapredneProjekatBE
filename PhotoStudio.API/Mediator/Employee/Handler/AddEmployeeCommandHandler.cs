using MediatR;
using PhotoStudio.API.Mediator.Employee.Command;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.Employee.Handler
{
    public class AddEmployeeCommandHandler : IRequestHandler<AddEmployeeCommand, EmployeeDTO>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public AddEmployeeCommandHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<EmployeeDTO> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = new PhotoStudio.Domain.Models.Employee
            {
                JMBG = request.JMBG,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = request.PasswordHash,
                Role = request.Role
            };

            await _employeeRepository.AddEmployeeAsync(employee);

            return new EmployeeDTO
            {
                Id = employee.Id,
                JMBG = employee.JMBG,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Role = employee.Role
            };
        }
    }
}
