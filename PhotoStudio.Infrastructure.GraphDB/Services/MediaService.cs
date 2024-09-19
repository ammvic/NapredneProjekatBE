using Neo4jClient;
using Neo4jClient.Cypher;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhotoStudio.GraphDB.Services
{
    public class MediaService
    {
        private readonly IGraphClient _graphClient;
        private readonly HttpClient _httpClient;

        public MediaService(IGraphClient graphClient, HttpClient httpClient)
        {
           
            _graphClient = graphClient;
            _httpClient = httpClient;
        }

        public async Task AddMediaAsync(int mediaId, string fileName, string filePath, string fileType, DateTime uploadDate, int albumId)
        {
            await _graphClient.Cypher
                .Match("(album:Album {Id: $albumId})")
                .Create("(media:Media {Id: $id, FileName: $fileName, FilePath: $filePath, FileType: $fileType, UploadDate: $uploadDate})-[:PART_OF]->(album)")
                .WithParam("albumId", albumId)
                .WithParam("id", mediaId)
                .WithParam("fileName", fileName)
                .WithParam("filePath", filePath)
                .WithParam("fileType", fileType)
                .WithParam("uploadDate", uploadDate)
                .ExecuteWithoutResultsAsync();
        }

        public async Task RemoveMediaAsync(int mediaId)
        {
            await _graphClient.Cypher
                .Match("(media:Media {Id: $id})")
                .DetachDelete("media")
                .WithParam("id", mediaId)
                .ExecuteWithoutResultsAsync();
        }

        public async Task UpdateMediaAsync(int mediaId, string fileName, string filePath, string fileType)
        {
            await _graphClient.Cypher
                .Match("(media:Media {Id: $id})")
                .Set("media.FileName = $fileName, media.FilePath = $filePath, media.FileType = $fileType")
                .WithParam("id", mediaId)
                .WithParam("fileName", fileName)
                .WithParam("filePath", filePath)
                .WithParam("fileType", fileType)
                .ExecuteWithoutResultsAsync();
        }

        public async Task<dynamic> GetMediaByIdAsync(int mediaId)
        {
            var query = new
            {
                statements = new[]
                {
                new
                {
                    statement = $"MATCH (media:Media {{Id: {mediaId}}}) RETURN media"
                }
            }
            };

            var response = await _httpClient.PostAsJsonAsync("http://localhost:7474/db/neo4j/tx/commit", query);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(content);

            // Assume that the data is in result.results[0].data
            var data = result?.results[0]?.data;

            return data?.FirstOrDefault();
        }
    }

}
