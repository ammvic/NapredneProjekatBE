using AutoMapper;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PhotoStudio.Application.Services
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> RegisterAsync(string firstName, string lastName, string email, string password, string phoneNumber, string address)
        {
            var verificationCode = GenerateVerificationCode();
            await SendVerificationEmailAsync(email, verificationCode);

            // Registrovanje korisnika u bazi sa statusom neverifikovan
            return await _unitOfWork.Users.VerifyAndRegisterUserAsync(firstName, lastName, email, password, phoneNumber, address);
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            return await _unitOfWork.Users.LoginAsync(email, password);
        }

        public async Task<Album> AccessAlbumByCodeAsync(string albumCode)
        {
            return await _unitOfWork.Users.AccessAlbumByCodeAsync(albumCode);
        }

        public async Task<bool> DownloadMediaAsync(int userId, string albumCode, int mediaId)
        {
            return await _unitOfWork.Users.DownloadMediaAsync(userId, albumCode, mediaId);
        }

        private string GenerateVerificationCode()
        {
            var randomNumber = new byte[3];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomNumber);
            }
            return BitConverter.ToString(randomNumber).Replace("-", "").Substring(0, 6);
        }

        private async Task SendVerificationEmailAsync(string email, string verificationCode)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("your-email@example.com"),
                Subject = "Your Verification Code",
                Body = $"Your verification code is: <b>{verificationCode}</b>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            using (var smtpClient = new SmtpClient("smtp.example.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new System.Net.NetworkCredential("username", "password");
                smtpClient.EnableSsl = true;
                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        public async Task<bool> VerifyUserAsync(string email, string verificationCode)
        {
            return await _unitOfWork.Users.VerifyUserAsync(email, verificationCode);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _unitOfWork.Users.GetUserByEmailAsync(email);
        }

        public async Task<User> FindByNameAsync(string firstName, string lastName)
        {
            return await _unitOfWork.Users.FindByNameAsync(firstName, lastName);
        }
    }
}
