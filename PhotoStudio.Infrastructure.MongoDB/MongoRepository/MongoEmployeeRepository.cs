using MongoDB.Driver;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoStudio.Infrastructure.MongoDB.MongoRepository
{
    public class MongoEmployeeRepository : IEmployeeRepository
    {
        private readonly IMongoCollection<Employee> _employeesCollection;

        public MongoEmployeeRepository(IMongoDatabase database)
        {
            _employeesCollection = database.GetCollection<Employee>("employees");
        }

        // Dobijanje zaposlenog po ID-u
        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _employeesCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        // Dobijanje zaposlenog po JMBG-u
        public async Task<Employee> GetEmployeeByJMBGAsync(string jmbg)
        {
            return await _employeesCollection.Find(e => e.JMBG == jmbg).FirstOrDefaultAsync();
        }

        // Dodavanje novog zaposlenog
        public async Task AddEmployeeAsync(Employee employee)
        {
            await _employeesCollection.InsertOneAsync(employee);
        }

        // Brisanje zaposlenog po ID-u
        public async Task DeleteEmployeeAsync(int id)
        {
            await _employeesCollection.DeleteOneAsync(e => e.Id == id);
        }

        // Dobijanje svih zaposlenih
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _employeesCollection.Find(_ => true).ToListAsync();
        }

        // Dobijanje svih rezervacija zaposlenog
        public async Task<IEnumerable<EmployeeBooking>> GetEmployeeBookingsAsync(int employeeId)
        {
            // Pretpostavimo da imate kolekciju rezervacija koje se odnose na zaposlene
            var bookingsCollection = _employeesCollection.Database.GetCollection<EmployeeBooking>("employeeBookings");

            // Filtriramo rezervacije na osnovu ID-a zaposlenog
            return await bookingsCollection.Find(b => b.EmployeeId == employeeId).ToListAsync();
        }

        public async Task<Employee> GetEmployeeByEmailAsync(string email)
        {
            var filter = Builders<Employee>.Filter.Eq(e => e.Email, email);
            return await _employeesCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
