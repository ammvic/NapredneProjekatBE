using PhotoStudio.Domain.Models;
using PhotoStudio.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhotoStudio.Infrastructure;
using PhotoStudio.Domain.Exceptions;

namespace PhotoStudio.Repository.Repositories
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly PhotoStudioContext _context;

        public AlbumRepository(PhotoStudioContext context)
        {
            _context = context;
        }

        // Metoda za kreiranje novog albuma
        public async Task<Album> CreateAlbumAsync(Album album)
        {
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
            return album;
        }

        // Metoda za preuzimanje albuma po kodu
        public async Task<Album> GetAlbumByCodeAsync(string code)
        {
            return await _context.Albums
                .Include(a => a.Media)
                .FirstOrDefaultAsync(a => a.Code == code);
        }

        // Metoda za preuzimanje javnih albuma
        public async Task<IEnumerable<Album>> GetPublicAlbumsAsync()
        {
            return await _context.Albums
                .Where(a => a.IsPublic)
                .Include(a => a.Media)
                .ToListAsync();
        }

        // Metoda za dodavanje medija u album
        public async Task AddMediaToAlbumAsync(Media media)
        {
            var album = await _context.Albums
                .Include(a => a.Media)
                .FirstOrDefaultAsync(a => a.Id == media.AlbumId);

            if (album == null)
            {
                throw new NotFoundException("Album not found.");
            }

            album.Media.Add(media);
            await _context.SaveChangesAsync();
        }

        // Metoda za preuzimanje medija iz albuma preko koda albuma
        public async Task<IEnumerable<Media>> GetMediaByAlbumCodeAsync(string code)
        {
            var album = await _context.Albums
                .Include(a => a.Media)
                .FirstOrDefaultAsync(a => a.Code == code);

            if (album == null)
            {
                throw new NotFoundException("Album not found.");
            }

            return album.Media;
        }

        // Metoda za preuzimanje zaposlenog po ID-u
        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);

            if (employee == null)
            {
                throw new NotFoundException("Employee not found.");
            }

            return employee;
        }

        // Metoda za dodavanje albuma
        public async Task AddAlbumAsync(Album album)
        {
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
        }
    }
}
