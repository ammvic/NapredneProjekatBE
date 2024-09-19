using PhotoStudio.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoStudio.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> FindByNameAsync(string firstName, string lastName);
        Task<User> LoginAsync(string email, string password);
        Task<Album> AccessAlbumByCodeAsync(string albumCode);
        Task<bool> DownloadMediaAsync(int userId, string albumCode, int mediaId);
        Task<bool> VerifyAndRegisterUserAsync(string firstName, string lastName, string email, string password, string phoneNumber, string address);
        Task<bool> VerifyUserAsync(string email, string verificationCode);
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<Booking>> GetBookingsForUserAsync(int userId);
        Task<int> GetUserCreditsAsync(int userId);
        Task<User> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(User user);
    }
}
