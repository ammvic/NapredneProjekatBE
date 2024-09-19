using MongoDB.Driver;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoStudio.Infrastructure.MongoDB.MongoRepository
{
    public class MongoAlbumRepository : IAlbumRepository
    {
        private readonly IMongoCollection<Album> _albumsCollection;
        private readonly IMongoCollection<Media> _mediaCollection;
        private readonly IMongoCollection<Employee> _employeesCollection;

        public MongoAlbumRepository(IMongoDatabase database)
        {
            _albumsCollection = database.GetCollection<Album>("albums");
            _mediaCollection = database.GetCollection<Media>("media");
            _employeesCollection = database.GetCollection<Employee>("employees");
        }

        // Kreiranje novog albuma
        public async Task<Album> CreateAlbumAsync(Album album)
        {
            await _albumsCollection.InsertOneAsync(album);
            return album;
        }

        // Dodavanje medija u album
        public async Task AddMediaToAlbumAsync(Media media)
        {
           
            var filter = Builders<Album>.Filter.Eq(a => a.Id, media.AlbumId);
            var update = Builders<Album>.Update.Push(a => a.Media, media);
            await _albumsCollection.UpdateOneAsync(filter, update);
        }

        // Pretraživanje zaposlenih po ID-u
        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            return await _employeesCollection.Find(e => e.Id == employeeId).FirstOrDefaultAsync();
        }

        // Pretraživanje albuma po kodu
        public async Task<Album> GetAlbumByCodeAsync(string code)
        {
            return await _albumsCollection.Find(a => a.Code == code).FirstOrDefaultAsync();
        }

        // Dohvatanje svih javnih albuma (koji imaju dizajn)
        public async Task<IEnumerable<Album>> GetPublicAlbumsAsync()
        {
            
            return await _albumsCollection.Find(a => a.IsPublic).ToListAsync();
        }

        // Dodavanje albuma (ako već nije kreiran)
        public async Task AddAlbumAsync(Album album)
        {
            // Pretpostavljamo da album već postoji, samo ga ažuriramo ako je potrebno
            var existingAlbum = await _albumsCollection.Find(a => a.Id == album.Id).FirstOrDefaultAsync();
            if (existingAlbum == null)
            {
                await CreateAlbumAsync(album);
            }
            else
            {
                var filter = Builders<Album>.Filter.Eq(a => a.Id, album.Id);
                await _albumsCollection.ReplaceOneAsync(filter, album);
            }
        }
    }
}
