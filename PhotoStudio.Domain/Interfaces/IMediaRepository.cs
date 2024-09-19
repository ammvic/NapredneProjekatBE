using PhotoStudio.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoStudio.Domain.Interfaces
{
    public interface IMediaRepository
    {
        Task AddMediaAsync(Media media);
        Task<Media> GetMediaByIdAsync(int id);
        Task<IEnumerable<Media>> GetMediaByIdsAsync(IEnumerable<int> ids);
        Task<IEnumerable<Media>> GetMediaByAlbumIdAsync(int albumId);
        Task<IEnumerable<Media>> GetAllMediaAsync();
        Task UpdateMediaAsync(Media media);
        Task DeleteMediaAsync(int id);
        Task<IEnumerable<Media>> GetMediaWithEmployeeByAlbumIdAsync(int albumId);
    }
}
