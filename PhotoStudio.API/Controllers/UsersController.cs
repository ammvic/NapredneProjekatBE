using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using PhotoStudio.Application.DTOs;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PhotoStudio.Repository.Repositories;

namespace PhotoStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IConfiguration _configuration;

        public UsersController(IUserRepository userRepository, IConfiguration configuration, IBookingRepository bookingRepository)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _bookingRepository = bookingRepository;
        }

        // POST: api/Users/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (ModelState.IsValid)
            {
                // Proveri da li korisnik već postoji
                var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest("User with this email already exists.");
                }

                // Pokreni proces registracije i pošalji verifikacioni kod korisniku
                bool isRegistrationInitiated = await _userRepository.VerifyAndRegisterUserAsync(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.Password,
                    request.PhoneNumber,
                    request.Address
                );

                if (isRegistrationInitiated)
                {
                    return Ok(new { Message = "Verification code sent. Please verify your email to complete registration." });
                }
                else
                {
                    return BadRequest("Registration initiation failed.");
                }
            }
            return BadRequest(ModelState);
        }

        // POST: api/Users/Verify
        [HttpPost("Verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyRequest request)
        {
            // Proveri verifikacioni kod
            bool isVerified = await _userRepository.VerifyUserAsync(request.Email, request.VerificationCode);

            if (isVerified)
            {
                // U ovom trenutku, korisnik je sada verifikovan i registracija je kompletirana
                var user = await _userRepository.GetUserByEmailAsync(request.Email);

                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                return Ok("User verified and registration completed successfully.");
            }
            return BadRequest("Invalid verification code.");
        }

        // POST: api/Users/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null || !VerifyPassword(user.Password, request.Password))
            {
                return Unauthorized("Invalid email or password.");
            }

            if (!user.IsVerified)
            {
                return Unauthorized("User not verified.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPassword(string storedHash, string inputPassword)
        {
            string inputHash = HashPassword(inputPassword);
            return storedHash == inputHash;
        }


        // GET: api/Users/MyBookings
        [HttpGet("MyBookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            try
            {
                // Preuzimanje ID-a korisnika iz JWT tokena
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Preuzimanje rezervacija za korisnika
                var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);

                // Mapiranje rezervacija na DTO koji sadrži ID, datum i vreme
                var bookingDtos = bookings.Select(b => new
                {
                    Id = b.Id,              // Dodato vraćanje ID-ja rezervacije
                    DateTime = b.DateTime
                }).ToList();

                return Ok(new { bookings = bookingDtos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpGet("MyCredits")]
        public async Task<IActionResult> GetMyCredits()
        {
            try
            {
                // Preuzimanje ID-a korisnika iz JWT tokena
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Preuzimanje kredita za korisnika
                var credits = await _userRepository.GetUserCreditsAsync(userId);

                return Ok(new { Credits = credits });
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Preuzimanje ID-a korisnika iz JWT tokena
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Dohvat korisničkih informacija
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Kreiranje DTO za vraćanje informacija
                var userProfile = new UserDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    Credits = user.Credits
                };

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
     

    }
}
