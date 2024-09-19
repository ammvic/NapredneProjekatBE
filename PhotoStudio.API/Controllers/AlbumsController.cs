using Microsoft.AspNetCore.Mvc;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Exceptions;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PhotoStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly string _photoStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");


        public AlbumsController(
            IAlbumRepository albumRepository,
            IMediaRepository mediaRepository,
            IUserRepository userRepository,
            IBookingRepository bookingRepository)
        {
            _albumRepository = albumRepository;
            _mediaRepository = mediaRepository;
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
        }

        // 1. Upload fotografija
        [HttpPost("uploadPhotos")]
        public async Task<IActionResult> UploadPhotos([FromForm] IFormFileCollection photos, int albumId, int employeeId)
        {
            if (photos == null || photos.Count == 0)
            {
                return BadRequest("Please select photos to upload.");
            }

            // Putanja do direktorijuma gde će se fajlovi čuvati
            var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadFolderPath))
            {
                Directory.CreateDirectory(uploadFolderPath);
            }

            // Logika za čuvanje fotografija
            foreach (var photo in photos)
            {
                if (photo.Length > 0)
                {
                    // Kreiramo jedinstveno ime za fajl kako bismo izbegli konflikte
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(photo.FileName);

                    var filePath = Path.Combine(uploadFolderPath, uniqueFileName);

                    // Čuvanje fajla na disk
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(fileStream);
                    }

                    // Kreiranje relativne putanje za frontend (npr. /uploads/ime_fajla)
                    var relativeFilePath = $"/uploads/{uniqueFileName}";

                    // Kreiranje novog medija sa relativnom URL putanjom
                    var media = new Media
                    {
                        FilePath = relativeFilePath, // Ovde koristimo relativnu URL putanju
                        MediaType = MediaType.Photo,
                        UploadedAt = DateTime.Now,
                        EmployeeId = employeeId,
                        AlbumId = albumId,
                        Cost = 100 // Ovde postavite cenu medija po potrebi
                    };

                    // Čuvanje medija u bazi
                    await _mediaRepository.AddMediaAsync(media);
                }
            }

            return Ok("Photos uploaded successfully.");
        }


        // 2. Kreiranje novog albuma sa imenom i prezimenom korisnika
        [HttpPost("createAlbum")]
        public async Task<IActionResult> CreateAlbum([FromBody] AlbumDTO albumDto)
        {
            if (albumDto == null)
            {
                return BadRequest("Album data is null.");
            }

            // Pronađi rezervaciju na osnovu sessionId-a
            var booking = await _bookingRepository.GetBookingByIdAsync(albumDto.SessionId);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Pronađi korisnika na osnovu rezervacije
            var user = await _userRepository.GetByIdAsync(booking.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            try
            {
                // Kreiraj novi album
                var album = new Album
                {
                    Name = $"{user.FirstName} {user.LastName}'s Album",
                    Code = GenerateAccessCode(),
                    IsPublic = albumDto.IsPublic,
                    EmployeeId = albumDto.EmployeeId,
                    UserId = user.Id,
                    CreatedAt = DateTime.Now
                };

                var createdAlbum = await _albumRepository.CreateAlbumAsync(album);

                // Pošalji korisniku email sa kodom za pristup
                await SendVerificationEmailAsync(user.Email, createdAlbum.Code);

                Console.WriteLine($"Album created successfully with code: {createdAlbum.Code}");

                // Promenite vraćanje odgovora da dodate više informacija ako je potrebno
                return CreatedAtAction(nameof(GetAlbumByCode), new { code = createdAlbum.Code }, new { Id = createdAlbum.Id, Code = createdAlbum.Code });

            }
            catch (Exception ex)
            {
                // Dodavanje logike za hvatanje greške i vraćanje korisniku
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // Preuzimanje albuma pomoću koda
        [HttpGet("{code}")]
        public async Task<IActionResult> GetAlbumByCode(string code)
        {
            var album = await _albumRepository.GetAlbumByCodeAsync(code);
            if (album == null)
            {
                return NotFound();
            }

            return Ok(album);
        }
        // Preuzimanje albuma sa svim fotografijama pomoću koda
        [HttpGet("{code}/photos")]
        public async Task<IActionResult> GetAlbumWithPhotosByCode(string code)
        {
            var album = await _albumRepository.GetAlbumByCodeAsync(code);
            if (album == null)
            {
                return NotFound("Album not found.");
            }

            var mediaItems = await _mediaRepository.GetMediaByAlbumIdAsync(album.Id);
            var albumWithPhotos = new
            {
                Album = album,
                Photos = mediaItems.Where(m => m.MediaType == MediaType.Photo).Select(m => new
                {
                    m.Id,
                    FilePath = $"{Request.Scheme}://{Request.Host}/uploads/{Path.GetFileName(m.FilePath)}", // Generiše punu putanju
                    m.UploadedAt,
                    m.Cost
                }).ToList()
            };


            return Ok(albumWithPhotos);
        }



        private string GenerateAccessCode()
        {
            return Guid.NewGuid().ToString(); // Generiši jedinstveni kod
        }

        private async Task SendVerificationEmailAsync(string email, string accessCode)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("roomrover11@gmail.com", "tjejukunknytofih"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("roomrover11@gmail.com"),
                Subject = "Your Album Access Code",
                Body = $"Your new album has been created. The access code is: {accessCode}",
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);

        }
        [HttpPost("downloadPhoto")]
        public async Task<IActionResult> DownloadPhoto([FromBody] DownloadPhotoRequest request)
        {
            // Proverite da li je korisnik prijavljen
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Proverite da li korisnik ima dovoljno kredita
            var photo = await _mediaRepository.GetMediaByIdAsync(request.PhotoId);
            if (photo == null)
            {
                return NotFound("Photo not found.");
            }

            if (user.Credits < photo.Cost)
            {
                return BadRequest("Insufficient credits.");
            }

            // Oduzmite kredite korisniku
            user.Credits -= photo.Cost;
            await _userRepository.UpdateUserAsync(user);

            // Prikupite putanju do slike
            var photoPath = Path.Combine(_photoStoragePath, Path.GetFileName(photo.FilePath));

            // Proverite da li fajl postoji
            if (!System.IO.File.Exists(photoPath))
            {
                return NotFound("Photo file not found.");
            }

            var fileStream = new FileStream(photoPath, FileMode.Open, FileAccess.Read);
            var contentType = "image/png"; // Prilagodite MIME tip prema vrsti slike, ako je potrebno
            var fileName = Path.GetFileName(photoPath);

            return File(fileStream, contentType, fileName);
        }


    }
    public class DownloadPhotoRequest
    {
        public int UserId { get; set; }
        public int PhotoId { get; set; }
    }

    public class AlbumDTO
    {
        public int SessionId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsPublic { get; set; }
    }
}
