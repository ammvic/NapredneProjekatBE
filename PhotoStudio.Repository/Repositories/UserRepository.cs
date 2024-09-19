using PhotoStudio.Domain.Models;
using PhotoStudio.Domain.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using PhotoStudio.Infrastructure;

namespace PhotoStudio.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PhotoStudioContext _context;

        public UserRepository(PhotoStudioContext context)
        {
            _context = context;
        }

        // Slanje verifikacionog koda korisniku putem emaila
        public async Task SendVerificationCodeAsync(string email)
        {
            var verificationCode = GenerateVerificationCode();
            await SendVerificationEmailAsync(email, verificationCode);
        }

        // Registracija i verifikacija korisnika
        public async Task<bool> VerifyAndRegisterUserAsync(string firstName, string lastName, string email, string password, string phoneNumber, string address)
        {
            var verificationCode = GenerateVerificationCode();
            await SendVerificationEmailAsync(email, verificationCode);

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                var hashedPassword = builder.ToString();

                // Dodaj korisnika sa statusom neverifikovan
                var user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Password = hashedPassword,
                    PhoneNumber = phoneNumber,
                    Address = address,
                    VerificationCode = verificationCode,
                    IsVerified = false
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return true; 
            }
        }

        // Verifikacija korisnika pomoću koda
        public async Task<bool> VerifyUserAsync(string email, string verificationCode)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return false;
            }

            // Proverite da li je verifikacioni kod ispravan
            if (user.VerificationCode == verificationCode)
            {
                user.IsVerified = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }


        // Logovanje korisnika
        public async Task<User> LoginAsync(string email, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password && u.IsVerified);
        }

        // Pristupanje albumu pomoću koda
        public async Task<Album> AccessAlbumByCodeAsync(string albumCode)
        {
            return await _context.Albums
                .Include(a => a.Media)
                .FirstOrDefaultAsync(a => a.Code == albumCode);
        }

        // Preuzimanje medija iz albuma
        public async Task<bool> DownloadMediaAsync(int userId, string albumCode, int mediaId)
        {
            var album = await _context.Albums
                .Include(a => a.Media)
                .FirstOrDefaultAsync(a => a.Code == albumCode);

            var media = album?.Media.FirstOrDefault(m => m.Id == mediaId);

            if (album == null || media == null)
                return false;

            int mediaCost = media.Cost;

            if (!await HasSufficientCreditAsync(userId, mediaCost))
                return false;

            var user = await _context.Users.FindAsync(userId);
            user.Credits -= mediaCost;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        // Proveravanje kredita korisnika
        private async Task<bool> HasSufficientCreditAsync(int userId, int mediaCost)
        {
            var user = await _context.Users.FindAsync(userId);
            return user != null && user.Credits >= mediaCost;
        }

        // Generisanje verifikacionog koda
        private string GenerateVerificationCode()
        {
            var randomNumber = new byte[3];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomNumber);
            }
            return BitConverter.ToString(randomNumber).Replace("-", "").Substring(0, 6);
        }

        // Slanje verifikacionog koda putem emaila
        private async Task SendVerificationEmailAsync(string email, string verificationCode)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("roomrover11@gmail.com"),
                Subject = "Your Verification Code",
                Body = $"<p>Your verification code is: <b>{verificationCode}</b></p>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            using (var smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new System.Net.NetworkCredential("roomrover11@gmail.com", "tjejukunknytofih");
                smtpClient.EnableSsl = true;
                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> FindByNameAsync(string firstName, string lastName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.FirstName == firstName && u.LastName == lastName);
        }

        public async Task<IEnumerable<Booking>> GetBookingsForUserAsync(int userId)
        {
            return await _context.Bookings
                                 .Where(b => b.UserId == userId)
                                 .Select(b => new Booking
                                 {
                                     DateTime = b.DateTime
                                 })
                                 .ToListAsync();
        }

        public async Task<int> GetUserCreditsAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user != null ? user.Credits : 0;
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }
        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

    }
}
