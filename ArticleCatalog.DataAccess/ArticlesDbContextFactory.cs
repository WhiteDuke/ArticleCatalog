using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ArticleCatalog.DataAccess;

public class ArticlesDbContextFactory : IDesignTimeDbContextFactory<ArticlesDbContext>
{
    public ArticlesDbContext CreateDbContext(string[] args)
    {
        var webServiceProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ArticleCatalog.Service");
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(webServiceProjectPath)
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        
        var optionsBuilder = new DbContextOptionsBuilder<ArticlesDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ArticlesDbContext(optionsBuilder.Options);
    }
}