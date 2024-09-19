using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;
using PhotoStudio.Domain.Models;

namespace PhotoStudio.GraphDB.Services
{
    public class AlbumsGraphDbContext
    {
        private readonly IGraphClient _client;

        public AlbumsGraphDbContext(IGraphClient client)
        {
            _client = client;
        }

        // Dodavanje albuma
        public async Task<Album> CreateAlbumAsync(Album album)
        {
            await _client.Cypher
                .Create("(album:Album {newAlbum})")
                .WithParam("newAlbum", album)
                .ExecuteWithoutResultsAsync();
            return album;
        }

        // Dodavanje medija u album
        public async Task AddMediaToAlbumAsync(Media media)
        {
            await _client.Cypher
                .Match("(album:Album)")
                .Where((Album album) => album.Id == media.AlbumId)
                .Create("(media:Media {newMedia})-[:BELONGS_TO]->(album)")
                .WithParam("newMedia", media)
                .ExecuteWithoutResultsAsync();
        }

        // Dohvatanje zaposlenog prema ID-u
        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            var result = await _client.Cypher
                .Match("(employee:Employee)")
                .Where((Employee employee) => employee.Id == employeeId)
                .Return(employee => employee.As<Employee>())
                .ResultsAsync;

            return result.FirstOrDefault(); 
        }

        // Dohvatanje albuma prema kodu
        public async Task<Album> GetAlbumByCodeAsync(string code)
        {
            var result = await _client.Cypher
                .Match("(album:Album)")
                .Where((Album album) => album.Code == code)
                .Return(album => album.As<Album>())
                .ResultsAsync;

            return result.FirstOrDefault(); 
        }

        // Dohvatanje javnih albuma
        public async Task<IEnumerable<Album>> GetPublicAlbumsAsync()
        {
            return await _client.Cypher
                .Match("(album:Album)")
                .Where((Album album) => album.IsPublic)
                .Return(album => album.As<Album>())
                .ResultsAsync;
        }

        // Dodavanje albuma
        public async Task AddAlbumAsync(Album album)
        {
            await _client.Cypher
                .Create("(album:Album {newAlbum})")
                .WithParam("newAlbum", album)
                .ExecuteWithoutResultsAsync();
        }
    }
}
