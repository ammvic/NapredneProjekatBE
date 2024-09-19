using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public BookingsController(IBookingRepository bookingRepository, IEmployeeRepository employeeRepository, IPaymentRepository paymentRepository)
        {
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _employeeRepository = employeeRepository;
        }

        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (booking == null)
            {
                return NotFound($"Booking with ID {bookingId} not found.");
            }

            await _bookingRepository.DeleteAsync(bookingId);
            return NoContent();
        }



        // POST: api/Bookings/Create
        [HttpPost("Create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid booking details.");
            }

            try
            {
                var booking = new Booking
                {
                    ServiceType = request.PhotographyType,
                    Location = request.Location,
                    DateTime = DateTime.Parse(request.DateTime),
                    IsAdditionalShootingIncluded = request.VideoRecording,
                    UserId = request.User.Id,
                    UserFirstName = request.User.FirstName,
                    UserLastName = request.User.LastName,
                    Status = "Pending"
                };

                // Provera dostupnosti termina
                bool isAvailable = await _bookingRepository.IsDateTimeAvailableAsync(booking);
                if (!isAvailable)
                {
                    return BadRequest("Selected date and time are not available.");
                }

                // Kreiranje rezervacije
                var (createdBooking, photographer, videographer) = await _bookingRepository.AddAsync(booking);

                // Generisanje ponude i cene
                var (offer, price) = await _bookingRepository.GenerateOfferAsync(createdBooking);

                // Ažuriranje rezervacije sa ponudom i cenom
                createdBooking.AdvanceAmount = price * 0.3m; // Primer: 30% avansa
                createdBooking.Status = "Confirmed";
                await _bookingRepository.UpdateAsync(createdBooking);

                // Simulacija plaćanja
                var paymentSimulated = await _paymentRepository.SimulatePayment(createdBooking.Id, createdBooking.AdvanceAmount);
                if (!paymentSimulated)
                {
                    return BadRequest("Payment simulation failed. Insufficient advance amount or booking not found.");
                }

                // Priprema informacija o zaposlenima
                var employees = createdBooking.EmployeeBookings.Select(eb => new EmployeeInfoDTO
                {
                    Id = eb.EmployeeId,
                    FirstName = eb.Employee.FirstName,
                    LastName = eb.Employee.LastName,
                    Role = eb.Role.ToString()
                }).ToList();

                // Formiranje odgovora
                var response = new
                {
                    Booking = new
                    {
                        createdBooking.Id,
                        createdBooking.ServiceType,
                        createdBooking.Location,
                        createdBooking.DateTime,
                        createdBooking.Status,
                        createdBooking.AdvanceAmount,
                        User = new
                        {
                            createdBooking.UserId,
                            createdBooking.UserFirstName,
                            createdBooking.UserLastName
                        }
                    },
                    Offer = offer,
                    TotalPrice = price,
                    Employees = employees
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Logovanje greške (ako je implementirano logovanje)
                // _logger.LogError(ex, "Error creating booking");

                return StatusCode(500, $"An error occurred while creating the booking: {ex.Message}");
            }
        }


        [HttpPost("CheckAvailability")]
        public async Task<IActionResult> CheckAvailability([FromBody] BookingRequest request)
        {
            var booking = new Booking
            {
                ServiceType = request.PhotographyType,
                Location = request.Location,
                DateTime = DateTime.Parse(request.DateTime), // Pretvara datum i vreme u DateTime
                IsAdditionalShootingIncluded = request.VideoRecording
            };

            bool isAvailable = await _bookingRepository.IsDateTimeAvailableAsync(booking);

            // Definiši osnovne cene za različite tipove usluga
            var basePrices = new Dictionary<string, int>
            {
                { "private", 1000 },
                { "graduation", 150 },
                { "wedding", 500 },
                { "event", 300 }
            };

            // Definiši dodatne cene za video snimanje
            var additionalServicePrices = new Dictionary<string, int>
            {
                { "videoRecording", 200 }
            };

            // Definiši ponude za različite tipove usluga
            var offers = new Dictionary<string, string>
            {
                { "private", "30% off" },
                { "graduation", "15% off" },
                { "wedding", "20% off" },
                { "event", "5% off" }
            };

            // Postavi osnovnu cenu i ponudu na osnovu tipa usluge
            var photographyType = request.PhotographyType.ToLower(); // Proveri da li ovo ispravno funkcioniše.
            var basePrice = basePrices.ContainsKey(request.PhotographyType) ? basePrices[request.PhotographyType] :0;
            var offer = offers.ContainsKey(request.PhotographyType) ? offers[request.PhotographyType] : "Trenutno ova rezervacija nije dostupna";

            // Ako je dodatno snimanje uključeno, dodaj dodatnu cenu
            if (request.VideoRecording && additionalServicePrices.ContainsKey("videoRecording"))
            {
                basePrice += additionalServicePrices["videoRecording"];
                offer += " + Free Video Editing";
            }

            return Ok(new { available = isAvailable, price = basePrice, offer = offer });
        }

        // GET: api/Bookings/GenerateOffer
        [HttpGet("GenerateOffer")]
        public async Task<IActionResult> GenerateOffer([FromQuery] Booking booking)
        {
            var (offer, price) = await _bookingRepository.GenerateOfferAsync(booking);
            return Ok(new { Offer = offer, Price = price });
        }

        // POST: api/Bookings/Confirm
        [HttpPost("Confirm")]
        public async Task<IActionResult> ConfirmBooking([FromBody] ConfirmBookingRequest request)
        {
            var result = await _paymentRepository.SimulatePayment(request.BookingId, request.AdvancePaymentAmount);

            if (!result)
            {
                return BadRequest("Unable to confirm booking or insufficient advance payment.");
            }

            // Ako je plaćanje simulirano uspešno, možemo potvrditi rezervaciju
            var confirmResult = await _bookingRepository.ConfirmBookingAsync(request.BookingId, request.AdvancePaymentAmount);

            if (!confirmResult)
            {
                return BadRequest("Unable to confirm booking.");
            }

            return Ok("Booking confirmed successfully.");
        }
    }

    public class BookingRequest
    {
        public string PhotographyType { get; set; }
        public string Location { get; set; }
        public string DateTime { get; set; }
        public bool Album { get; set; }
        public bool VideoRecording { get; set; }
        public UserInfoDTO User { get; set; }
    }

    public class ConfirmBookingRequest
    {
        public int BookingId { get; set; }
        public decimal AdvancePaymentAmount { get; set; }
    }

}
