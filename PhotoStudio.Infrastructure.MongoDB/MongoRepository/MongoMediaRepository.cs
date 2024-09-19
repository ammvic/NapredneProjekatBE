using MongoDB.Driver;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoStudio.Infrastructure.MongoDB.MongoRepository
{
    public class MongoMediaRepository : IMediaRepository
    {
        private readonly IMongoCollection<Media> _mediaCollection;
        private readonly IMongoCollection<Employee> _employeesCollection;
        private readonly IMongoCollection<Album> _albumsCollection;

        public MongoMediaRepository(IMongoDatabase database)
        {
            _mediaCollection = database.GetCollection<Media>("media");
            _employeesCollection = database.GetCollection<Employee>("employees");
            _albumsCollection = database.GetCollection<Album>("albums");
        }

        // Dodavanje novog medija
        public async Task AddMediaAsync(Media media)
        {
            await _mediaCollection.InsertOneAsync(media);
        }

        // Dobijanje medija po ID-u
        public async Task<Media> GetMediaByIdAsync(int id)
        {
            return await _mediaCollection.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

        // Dobijanje medija po listi ID-eva
        public async Task<IEnumerable<Media>> GetMediaByIdsAsync(IEnumerable<int> ids)
        {
            return await _mediaCollection.Find(m => ids.Contains(m.Id)).ToListAsync();
        }

        // Dobijanje medija po ID-u albuma
        public async Task<IEnumerable<Media>> GetMediaByAlbumIdAsync(int albumId)
        {
            return await _mediaCollection.Find(m => m.AlbumId == albumId).ToListAsync();
        }

        // Dobijanje svih medija
        public async Task<IEnumerable<Media>> GetAllMediaAsync()
        {
            return await _mediaCollection.Find(_ => true).ToListAsync();
        }

        // Ažuriranje medija
        public async Task UpdateMediaAsync(Media media)
        {
            await _mediaCollection.ReplaceOneAsync(m => m.Id == media.Id, media);
        }

        // Brisanje medija po ID-u
        public async Task DeleteMediaAsync(int id)
        {
            await _mediaCollection.DeleteOneAsync(m => m.Id == id);
        }

        // Dobijanje medija sa zaposlenim po ID-u albuma
        public async Task<IEnumerable<Media>> GetMediaWithEmployeeByAlbumIdAsync(int albumId)
        {
            var mediaList = await _mediaCollection.Find(m => m.AlbumId == albumId).ToListAsync();

            // Opcionalno: možete proširiti da uključite informacije o zaposlenima
            foreach (var media in mediaList)
            {
                media.Employee = await _employeesCollection.Find(e => e.Id == media.EmployeeId).FirstOrDefaultAsync();
            }

            return mediaList;
        }
    }
}
