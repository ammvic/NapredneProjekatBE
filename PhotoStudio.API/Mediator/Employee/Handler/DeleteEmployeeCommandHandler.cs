using MediatR;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.API.Mediator.Employee.Command;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.API.Mediator.Employee.Handler
{
    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, bool>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public DeleteEmployeeCommandHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            await _employeeRepository.DeleteEmployeeAsync(request.Id);
            return true;
        }
    }
}
