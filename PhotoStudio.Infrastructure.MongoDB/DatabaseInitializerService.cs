using Microsoft.Extensions.Hosting;
using PhotoStudio.Infrastructure.MongoDB;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoStudio.Infrastructure.MongoDB
{
    public class DatabaseInitializerService : IHostedService
    {
        private readonly MongoService _mongoService;

        public DatabaseInitializerService(MongoService mongoService)
        {
            _mongoService = mongoService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _mongoService.InitializeDatabaseAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
