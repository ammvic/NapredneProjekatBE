using AutoMapper;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Exceptions;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoStudio.Application.Services
{
    public class EmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Employee> LoginAsync(string jmbg, string firstName, string lastName)
        {
            var employee = await _unitOfWork.Employees.GetEmployeeByJMBGAsync(jmbg);
            if (employee != null && employee.FirstName == firstName && employee.LastName == lastName)
            {
                return employee;
            }

            throw new NotFoundException("Employee not found or credentials are incorrect.");
        }

        public async Task AddEmployeeAsync(Employee admin, Employee newEmployee)
        {
            if (admin.Role != Role.Administrator)
            {
                throw new PhotoStudio.Domain.Exceptions.UnauthorizedAccessException("Only administrators can add employees.");
            }

            await _unitOfWork.Employees.AddEmployeeAsync(newEmployee);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteEmployeeAsync(Employee admin, int employeeId)
        {
            if (admin.Role != Role.Administrator)
            {
                throw new PhotoStudio.Domain.Exceptions.UnauthorizedAccessException("Only administrators can remove employees.");
            }

            await _unitOfWork.Employees.DeleteEmployeeAsync(employeeId);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _unitOfWork.Employees.GetAllEmployeesAsync();
        }

        public async Task<IEnumerable<EmployeeBooking>> GetEmployeeBookingsAsync(int employeeId)
        {
            return await _unitOfWork.Employees.GetEmployeeBookingsAsync(employeeId);
        }
    }
}