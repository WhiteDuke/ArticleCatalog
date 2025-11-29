using System;
using System.Linq;
using System.Threading.Tasks;
using ArticleCatalog.DataAccess;
using ArticleCatalog.Domain.Dto;
using ArticleCatalog.Domain.Requests;
using ArticleCatalog.Service.Exceptions;
using ArticleCatalog.Service.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ArticleCatalog.Service.Services;

public sealed class ArticleService : IArticleService
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

    public async Task<ArticleDto> UpdateArticleAsync(UpdateArticleRequest request)
    {
        var article = await _dbContext.Articles
            .Include(a => a.ArticleTags)
            .ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync();

        if (article == null)
        {
            throw new EntityNotFoundException("Статья не найдена по идентификатору");
        }

        article.Title = request.Title;
        article.UpdatedDate = DateTimeOffset.Now;

        await Task.CompletedTask;

        return article.MapArticleToArticleDto();
    }

    public async Task<ArticleDto> GetArticleByIdAsync(int id)
    {
        var article = await _dbContext.Articles.Include(x => x.ArticleTags)
            .ThenInclude(x => x.Tag)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return article?.MapArticleToArticleDto();
    }

    public async Task<ArticleDto[]> GetArticlesAsync(int pageNumber = DefaultPageNumber, int pageSize = DefaultPageSize)
    {
        var query = _dbContext.Articles.Include(x => x.ArticleTags)
            .ThenInclude(x => x.Tag)
            .AsNoTracking();

        var articles = await query.Skip(pageNumber - 1).Take(pageSize)
            .Select(x => new ArticleDto()
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