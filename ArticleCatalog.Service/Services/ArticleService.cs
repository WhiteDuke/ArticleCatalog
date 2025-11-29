using System.Linq;
using System.Threading.Tasks;
using ArticleCatalog.DataAccess;
using ArticleCatalog.Domain.Data;
using ArticleCatalog.Domain.Requests;
using Microsoft.EntityFrameworkCore;

namespace ArticleCatalog.Service.Services;

public class ArticleService : IArticleService
{
    private const int DefaultPageNumber = 0;
    private const int DefaultPageSize = 50;

    private readonly ArticleCatalogDbContext _dbContext;

    public ArticleService(ArticleCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateArticleAsync(CreateArticleRequest request)
    {
        // TODO: проверить и сохранить теги
        
        await Task.CompletedTask;
    }

    public async Task<ArticleData> GetArticleByIdAsync(int id)
    {
        var article = await _dbContext.Articles.Include(x => x.ArticleTags)
            .ThenInclude(x => x.Tag)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (article == null)
        {
            return null;
        }

        var articleData = new ArticleData
        {
            Id = article.Id,
            CreatedDate = article.CreatedDate,
            UpdatedDate = article.UpdatedDate,
            Tags = article.ArticleTags.OrderBy(x => x.Order).Select(x => x.Tag.Name).ToArray(),
            Title = article.Title
        };

        return articleData;
    }

    public async Task<ArticleData[]> GetArticlesAsync(int pageNumber = DefaultPageNumber, int pageSize = DefaultPageSize)
    {
        var query = _dbContext.Articles.Include(x => x.ArticleTags)
            .ThenInclude(x => x.Tag)
            .AsNoTracking();

        var articles = await query.Skip(pageNumber - 1).Take(pageSize)
            .Select(x => new ArticleData()
            {
                Id = x.Id,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                Tags = x.ArticleTags.Select(t => t.Tag.Name).ToArray(),
                Title = x.Title
            }).ToArrayAsync();

        return articles;
    }
}