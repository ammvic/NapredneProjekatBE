using MediatR;
using PhotoStudio.API.Mediator.Employee.Query;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.Employee.Handler
{
    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDTO>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<EmployeeDTO> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(request.Id);

            if (employee == null)
            {
                return null; 
            }

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
