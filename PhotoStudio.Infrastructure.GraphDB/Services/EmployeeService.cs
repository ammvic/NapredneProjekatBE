using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;
using PhotoStudio.Domain.Models;

namespace PhotoStudio.GraphDB.Services
{
    public class EmployeesGraphDbContext
    {
        private readonly IGraphClient _client;

        public EmployeesGraphDbContext(IGraphClient client)
        {
            _client = client;
        }

        // Dohvatanje zaposlenog prema ID-u
        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            var result = await _client.Cypher
                .Match("(employee:Employee)")
                .Where((Employee employee) => employee.Id == id)
                .Return(employee => employee.As<Employee>())
                .ResultsAsync;

            return result.FirstOrDefault();
        }

        // Dohvatanje zaposlenog prema JMBG
        public async Task<Employee> GetEmployeeByJMBGAsync(string jmbg)
        {
            var result = await _client.Cypher
                .Match("(employee:Employee)")
                .Where((Employee employee) => employee.JMBG == jmbg)
                .Return(employee => employee.As<Employee>())
                .ResultsAsync;

            return result.FirstOrDefault();
        }

        // Dodavanje zaposlenog
        public async Task AddEmployeeAsync(Employee employee)
        {
            await _client.Cypher
                .Create("(employee:Employee {newEmployee})")
                .WithParam("newEmployee", employee)
                .ExecuteWithoutResultsAsync();
        }

        // Brisanje zaposlenog prema ID-u
        public async Task DeleteEmployeeAsync(int id)
        {
            await _client.Cypher
                .Match("(employee:Employee)")
                .Where((Employee employee) => employee.Id == id)
                .Delete("employee")
                .ExecuteWithoutResultsAsync();
        }

        // Dohvatanje svih zaposlenih
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            var result = await _client.Cypher
                .Match("(employee:Employee)")
                .Return(employee => employee.As<Employee>())
                .ResultsAsync;

            return result;
        }
    }
}
