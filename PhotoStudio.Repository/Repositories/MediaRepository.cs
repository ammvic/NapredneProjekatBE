using PhotoStudio.Domain.Models;
using PhotoStudio.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhotoStudio.Infrastructure;

namespace PhotoStudio.Repository.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        private readonly PhotoStudioContext _context;

        public MediaRepository(PhotoStudioContext context)
        {
            _context = context;
        }

        public async Task AddMediaAsync(Media media)
        {
            _context.Media.Add(media);
            await _context.SaveChangesAsync();
        }

        public async Task<Media> GetMediaByIdAsync(int id)
        {
            return await _context.Media
                .Include(m => m.Employee)
                .Include(m => m.Album)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Media>> GetMediaByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.Media
                .Where(m => ids.Contains(m.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<Media>> GetMediaByAlbumIdAsync(int albumId)
        {
            return await _context.Media
                .Where(m => m.AlbumId == albumId)
                .Include(m => m.Employee)
                .ToListAsync();
        }

        public async Task<IEnumerable<Media>> GetAllMediaAsync()
        {
            return await _context.Media
                .Include(m => m.Employee)
                .Include(m => m.Album)
                .ToListAsync();
        }

        public async Task UpdateMediaAsync(Media media)
        {
            _context.Media.Update(media);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMediaAsync(int id)
        {
            var media = await _context.Media.FindAsync(id);
            if (media != null)
            {
                _context.Media.Remove(media);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Media>> GetMediaWithEmployeeByAlbumIdAsync(int albumId)
        {
            return await _context.Media
                .Where(m => m.AlbumId == albumId)
                .Include(m => m.Employee)
                .ToListAsync();
        }
    }
}
