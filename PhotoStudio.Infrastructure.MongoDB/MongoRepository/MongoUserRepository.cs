using MongoDB.Bson;
using MongoDB.Driver;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PhotoStudio.Infrastructure.MongoDB.MongoRepository
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Album> _albumsCollection;
        private readonly IMongoCollection<Media> _mediaCollection;
        private readonly IMongoCollection<Booking> _bookingsCollection;

        public MongoUserRepository(IMongoDatabase database)
        {
            _usersCollection = database.GetCollection<User>("users");
            _albumsCollection = database.GetCollection<Album>("albums");
            _mediaCollection = database.GetCollection<Media>("media");
            _bookingsCollection = database.GetCollection<Booking>("bookings");
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

            var hashedPassword = HashPassword(password);

            // Kreiranje korisnika u bazi sa statusom neverifikovan
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

            // Dodavanje korisnika u bazu
            await _usersCollection.InsertOneAsync(user);

            return true;
        }

        // Verifikacija korisnika pomoću koda
        public async Task<bool> VerifyUserAsync(string email, string verificationCode)
        {
            var user = await _usersCollection.Find(u => u.Email == email && u.VerificationCode == verificationCode).FirstOrDefaultAsync();

            if (user == null)
            {
                return false;
            }

            user.IsVerified = true;
            user.VerificationCode = null; // Očisti verifikacioni kod nakon uspešne verifikacije

            // Ažuriranje korisnika u bazi
            await _usersCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
            return true;
        }

        // Logovanje korisnika
        public async Task<User> LoginAsync(string email, string password)
        {
            var hashedPassword = HashPassword(password);
            return await _usersCollection.Find(u => u.Email == email && u.Password == hashedPassword && u.IsVerified).FirstOrDefaultAsync();
        }

        // Pristupanje albumu pomoću koda
        public async Task<Album> AccessAlbumByCodeAsync(string albumCode)
        {
            return await _albumsCollection.Find(a => a.Code == albumCode).FirstOrDefaultAsync();
        }

        // Preuzimanje medija iz albuma
        public async Task<bool> DownloadMediaAsync(int userId, string albumCode, int mediaId)
        {
            var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            var media = await _mediaCollection.Find(m => m.AlbumId == mediaId && m.Id == mediaId).FirstOrDefaultAsync();

            if (user == null || media == null || user.Credits < media.Cost)
            {
                return false;
            }

            user.Credits -= media.Cost;
            await _usersCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
            return true;
        }

        // Proveravanje kredita korisnika
        private async Task<bool> HasSufficientCreditAsync(int userId, int mediaCost)
        {
            var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
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

        // Heširanje lozinke
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
                smtpClient.Credentials = new NetworkCredential("roomrover11@gmail.com", "tjejukunknytofih");
                smtpClient.EnableSsl = true;
                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> FindByNameAsync(string firstName, string lastName)
        {
            return await _usersCollection.Find(u => u.FirstName == firstName && u.LastName == lastName).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsForUserAsync(int userId)
        {
            var bookingsCollection = _usersCollection.Database.GetCollection<Booking>("bookings");
            return await bookingsCollection.Find(b => b.UserId == userId).ToListAsync();
        }
        public async Task<int> GetUserCreditsAsync(int userId)
        {
            var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            return user != null ? user.Credits : 0;
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
        }
        public async Task<User> GetByIdAsync(int id)
        {
            return await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
        }
        public async Task UpdateUserAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<User>.Update.Set(u => u.Credits, user.Credits);

            var result = await _usersCollection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                throw new KeyNotFoundException($"User with ID {user.Id} not found.");
            }
        }

    }
}
