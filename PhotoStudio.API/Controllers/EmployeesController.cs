using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PhotoStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly IAlbumRepository _albumRepository;
        private readonly IConfiguration _configuration;

        public EmployeesController(IEmployeeRepository employeeRepository, IConfiguration configuration, IMediaRepository mediaRepository, IAlbumRepository albumRepository)
        {
            _employeeRepository = employeeRepository;
            _mediaRepository = mediaRepository;
            _albumRepository = albumRepository;
            _configuration = configuration;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            return Ok(employees);
        }

        // GET: api/employees/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee1(int id)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<Employee>> AddEmployee([FromBody] AddEmployeeDto employeeDto)
        {
            // Kreiranje novog zaposlenog na osnovu prosleđenih podataka
            var employee = new Employee
            {
                JMBG = employeeDto.JMBG,
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                PasswordHash = employeeDto.Password, // Lozinka se čuva direktno
                Role = (Role)employeeDto.Role // Mapiranje integer role na enum Role
            };

            // Dodavanje zaposlenog u repozitorijum
            await _employeeRepository.AddEmployeeAsync(employee);

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Employee not found." });
            }

            await _employeeRepository.DeleteEmployeeAsync(id);

            // Return a success message
            return Ok(new { message = "Employee successfully deleted." });
        }


        // POST: api/employees/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var employee = await _employeeRepository.GetEmployeeByEmailAsync(request.Email);

            if (employee == null || employee.PasswordHash != request.Password) // Direktna provera lozinke
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = GenerateJwtToken(employee);
            return Ok(new { Token = token, Role = employee.Role.ToString(), employee.Id });
        }


        private string GenerateJwtToken(Employee employee)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Email, employee.Email),
            new Claim(ClaimTypes.Name, employee.FirstName + " " + employee.LastName),
            new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new Claim(ClaimTypes.Role, employee.Role.ToString()) // Konvertuj enum u string
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            // Dodato: Preuzimanje predstojećih booking-a za zaposlenog koristeći EmployeeBooking
            var employeeBookings = await _employeeRepository.GetEmployeeBookingsAsync(id);
            var upcomingBookings = employeeBookings.Select(eb => eb.Booking).ToList();

            employee.EmployeeBookings = employeeBookings.ToList(); // Povezivanje sa zaposlenim

            return Ok(new
            {
                Employee = employee,
                UpcomingBookings = upcomingBookings
            });
        }

    }
    public class AddEmployeeDto
    {
        public string JMBG { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; } // Integer koji mapira na Role enum
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
