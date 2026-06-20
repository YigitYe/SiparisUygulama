using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SiparisUygulama.Models
{
    public class YemekSiparisDBContextFactory : IDesignTimeDbContextFactory<YemekSiparisDBContext>
    {
        public YemekSiparisDBContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<YemekSiparisDBContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new YemekSiparisDBContext(optionsBuilder.Options);
        }
    }
}
