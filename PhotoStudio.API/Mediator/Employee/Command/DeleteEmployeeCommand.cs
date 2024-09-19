using MediatR;

namespace PhotoStudio.API.Mediator.Employee.Command
{
    public class DeleteEmployeeCommand : IRequest<bool>
    {
        public int Id { get; }

        public DeleteEmployeeCommand(int id)
        {
            Id = id;
        }
    }
}
