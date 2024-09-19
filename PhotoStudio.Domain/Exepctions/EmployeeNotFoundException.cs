using PhotoStudio.Domain.Models;
using System;

namespace PhotoStudio.Domain.Exceptions
{
    public class EmployeeNotFoundException : NotFoundException
    {
        public EmployeeNotFoundException(int employeeId)
            : base($"Employee with id '{employeeId}' not found.")
        {
        }
    }
}
