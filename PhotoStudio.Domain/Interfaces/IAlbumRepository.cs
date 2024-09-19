using PhotoStudio.Domain.Models;
using System.Threading.Tasks;

namespace PhotoStudio.Domain.Interfaces
{
    public interface IAlbumRepository
    {
        Task<Album> CreateAlbumAsync(Album album);
        Task AddMediaToAlbumAsync(Media media);
        Task<Employee> GetEmployeeByIdAsync(int employeeId);
        Task<Album> GetAlbumByCodeAsync(string code);
        Task<IEnumerable<Album>> GetPublicAlbumsAsync(); // Za albume sa dizajnom
        Task AddAlbumAsync(Album album);

    }
}
