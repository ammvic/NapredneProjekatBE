using PhotoStudio.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoStudio.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<Employee> GetEmployeeByJMBGAsync(string jmbg);
        Task AddEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
        Task<Employee> GetEmployeeByEmailAsync(string email);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<IEnumerable<EmployeeBooking>> GetEmployeeBookingsAsync(int employeeId);
    }
}
