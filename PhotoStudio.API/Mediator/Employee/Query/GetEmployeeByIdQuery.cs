using MediatR;
using PhotoStudio.Application.DTOs;

namespace PhotoStudio.API.Mediator.Employee.Query
{
    public class GetEmployeeByIdQuery : IRequest<EmployeeDTO>
    {
        public int Id { get; }

        public GetEmployeeByIdQuery(int id)
        {
            Id = id;
        }
    }
}
