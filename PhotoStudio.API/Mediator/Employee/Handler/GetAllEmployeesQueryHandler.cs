using MediatR;
using PhotoStudio.API.Mediator.Employee.Query;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.Employee.Handler
{
    public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, IEnumerable<EmployeeDTO>>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetAllEmployeesQueryHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<EmployeeDTO>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();

            return employees.Select(employee => new EmployeeDTO
            {
                Id = employee.Id,
                JMBG = employee.JMBG,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Role = employee.Role
            });
        }
    }
}
