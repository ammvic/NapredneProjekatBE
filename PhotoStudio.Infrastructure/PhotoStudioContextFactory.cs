using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PhotoStudio.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PhotoStudioContext>
    {
        public PhotoStudioContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../PhotoStudio.API"))  
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<PhotoStudioContext>();
            var connectionString = configuration.GetConnectionString("PhotoStudioContext");

            builder.UseSqlServer(connectionString);

            return new PhotoStudioContext(builder.Options);
        }
    }
}
