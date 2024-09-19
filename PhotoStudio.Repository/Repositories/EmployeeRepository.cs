using PhotoStudio.Domain.Models;
using PhotoStudio.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhotoStudio.Infrastructure;

namespace PhotoStudio.Repository.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly PhotoStudioContext _context;

        public EmployeeRepository(PhotoStudioContext context)
        {
            _context = context;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task<Employee> GetEmployeeByJMBGAsync(string jmbg)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.JMBG == jmbg);
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await GetEmployeeByIdAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.ToListAsync();
        }
        public async Task<IEnumerable<EmployeeBooking>> GetEmployeeBookingsAsync(int employeeId)
        {
            
            return await _context.EmployeeBookings
                                 .Include(eb => eb.Booking)
                                 .Where(eb => eb.EmployeeId == employeeId)
                                 .ToListAsync();
        }
        public async Task<Employee> GetEmployeeByEmailAsync(string email)
        {
            return await _context.Employees
                                 .FirstOrDefaultAsync(e => e.Email == email);
        }
    }
}
