using MediatR;
using PhotoStudio.Application.DTOs;
using System.Collections.Generic;

namespace PhotoStudio.API.Mediator.Employee.Query
{
    public class GetAllEmployeesQuery : IRequest<IEnumerable<EmployeeDTO>>
    {
    }
}
