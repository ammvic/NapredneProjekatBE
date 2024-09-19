using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Neo4jClient;
using PhotoStudio.Domain.Models;

namespace PhotoStudio.GraphDB.Services
{
    public class UsersGraphDbContext
    {
        private readonly IGraphClient _client;
        private readonly IConfiguration _configuration;

        public UsersGraphDbContext(IGraphClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        private SmtpClient CreateSmtpClient()
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            return new SmtpClient
            {
                Host = smtpSettings["Host"],
                Port = int.Parse(smtpSettings["Port"]),
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = true
            };
        }

        private async Task SendVerificationEmailAsync(string toEmail, string verificationCode)
        {
            var smtpClient = CreateSmtpClient();
            var fromAddress = _configuration["SmtpSettings:From"];
            var subject = "Verify Your Email";
            var body = $"Please verify your email by using this verification code: {verificationCode}";

            var mailMessage = new MailMessage(fromAddress, toEmail)
            {
                Subject = subject,
                Body = body
            };

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log error (omitted for brevity)
                throw new InvalidOperationException("Error sending email", ex);
            }
        }

        // Registracija novog korisnika
        public async Task<User> RegisterAsync(string firstName, string lastName, string email, string password, string phoneNumber, string address, string verificationCode)
        {
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password, 
                PhoneNumber = phoneNumber,
                Address = address,
                Credits = 0,
                VerificationCode = verificationCode,
                IsVerified = false
            };

            await _client.Cypher
                .Create("(user:User {newUser})")
                .WithParam("newUser", user)
                .ExecuteWithoutResultsAsync(); // Nema povratne vrednosti

            return user;
        }

        // Logovanje korisnika
        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await _client.Cypher
                .Match("(user:User)")
                .Where((User user) => user.Email == email && user.Password == password)
                .Return(user => user.As<User>())
                .ResultsAsync; 

            return user.FirstOrDefault(); // Vraća korisnika ili null ako ne postoji
        }

        // Pristupanje albumu pomoću koda
        public async Task<Album> AccessAlbumByCodeAsync(string albumCode)
        {
            var album = await _client.Cypher
                .Match("(album:Album)")
                .Where((Album album) => album.Code == albumCode)
                .Return(album => album.As<Album>())
                .ResultsAsync; 

            return album.FirstOrDefault();
        }

        // Preuzimanje medija iz albuma
        public async Task<bool> DownloadMediaAsync(int userId, string albumCode, int mediaId)
        {
            var media = await _client.Cypher
                .Match("(album:Album)-[:CONTAINS]->(media:Media)")
                .Where((Album album) => album.Code == albumCode)
                .AndWhere((Media media) => media.Id == mediaId)
                .Return(media => media.As<Media>())
                .ResultsAsync;

            return media.Any();
        }

        // Registracija korisnika sa verifikacijom
        public async Task<string> RegisterWithVerificationAsync(string firstName, string lastName, string email, string password, string phoneNumber, string address)
        {
            string verificationCode = Guid.NewGuid().ToString(); // Generiše jedinstveni verifikacioni kod

            await RegisterAsync(firstName, lastName, email, password, phoneNumber, address, verificationCode);

            await SendVerificationEmailAsync(email, verificationCode);

            return verificationCode;
        }

        // Verifikacija korisnika
        public async Task<bool> VerifyUserAsync(string email, string verificationCode)
        {
            var result = await _client.Cypher
                .Match("(user:User)")
                .Where((User user) => user.Email == email && user.VerificationCode == verificationCode)
                .Set("user.IsVerified = true")
                .Return(user => user.As<User>())
                .ResultsAsync; 

            return result.Any(); // Vraća true ako je korisnik uspešno verifikovan
        }

        // Pronalaženje korisnika po email adresi
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _client.Cypher
                .Match("(user:User)")
                .Where((User user) => user.Email == email)
                .Return(user => user.As<User>())
                .ResultsAsync;

            return user.FirstOrDefault();
        }
    }
}
