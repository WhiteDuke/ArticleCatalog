using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ArticleCatalog.DataAccess;

/// <summary>
/// Вызывается механизмом EF Core при генерации миграций.
/// При создании контекста получаем для него строку подключения из appsettings проекта ArticleCatalog.Service
/// </summary>
public class ArticleCatalogDbContextFactory : IDesignTimeDbContextFactory<ArticleCatalogDbContext>
{
    public ArticleCatalogDbContext CreateDbContext(string[] args)
    {
        var webServiceProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ArticleCatalog.Service");
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(webServiceProjectPath)
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        
        var optionsBuilder = new DbContextOptionsBuilder<ArticleCatalogDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ArticleCatalogDbContext(optionsBuilder.Options);
    }
}