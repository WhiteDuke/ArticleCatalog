using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticleCatalog.DataAccess;
using ArticleCatalog.Domain.Dto;
using ArticleCatalog.Domain.Entities;
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
        var tagNames = new HashSet<string>(request.Tags.Select(x => x.Trim()));

        var existingTags = await _dbContext.Tags
            .Where(t => tagNames.Contains(t.Name))
            .ToListAsync();

        var existingTagDict = existingTags.ToDictionary(t => t.Name);

        var article = new Article
        {
            CreatedDate = DateTime.UtcNow,
            Title = request.Title,
            ArticleTags = []
        };

        // Обрабатываем теги
        for (var i = 0; i < request.Tags.Length; i++)
        {
            var tagName = request.Tags[i];
            if (existingTagDict.TryGetValue(tagName, out var existingTag))
            {
                article.ArticleTags.Add(new ArticleTag { Tag = existingTag, Order = i});
            }
            else
            {
                var newTag = new Tag { Name = tagName };
                _dbContext.Tags.Add(newTag);
                article.ArticleTags.Add(new ArticleTag { Tag = newTag, Order = i});
            }
        }

        _dbContext.Articles.Add(article);
        await _dbContext.SaveChangesAsync();
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

        var tagNames = new HashSet<string>(request.Tags.Select(x => x.Trim()));

        var existingTags = await _dbContext.Tags
            .Where(t => tagNames.Contains(t.Name))
            .ToListAsync();

        var existingTagDict = existingTags.ToDictionary(t => t.Name);

        // Обрабатываем теги
        for (var i = 0; i < request.Tags.Length; i++)
        {
            var tagName = request.Tags[i];

            if (existingTagDict.TryGetValue(tagName, out var existingTag))
            {
                article.ArticleTags.Add(new ArticleTag { Tag = existingTag, Order = i});
            }
            else
            {
                var newTag = new Tag { Name = tagName };
                _dbContext.Tags.Add(newTag);
                article.ArticleTags.Add(new ArticleTag { Tag = newTag, Order = i});
            }
        }

        article.Title = request.Title;
        article.UpdatedDate = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

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