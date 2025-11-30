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
    private readonly ArticleCatalogDbContext _dbContext;

    public ArticleService(ArticleCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateArticleAsync(CreateArticleRequest request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            // чтобы обеспечить уникальность тэгов для статьи, но сохранить порядок тэгов
            // ["тег 1", "тег 2", "тег 3", "тег 1", "тег 4"] - "тег 1" в позиции 3 будет отброшен
            var tagsHashSet = new HashSet<string>();
            var uniqueTags = request.Tags.Where(x => tagsHashSet.Add(x.Trim())).Select(t => t.Trim()).ToList();

            var existingTags = await _dbContext.Tags
                .Where(t => tagsHashSet.Contains(t.Name))
                .ToListAsync();

            var existingTagDict = existingTags.ToDictionary(t => t.Name, t => t);

            var article = new Article
            {
                CreatedDate = DateTime.UtcNow,
                Title = request.Title,
                ArticleTags = []
            };

            for (var i = 0; i < uniqueTags.Count; i++)
            {
                var order = i + 1;
                var tagName = uniqueTags[i];

                if (existingTagDict.TryGetValue(tagName, out var existingTag))
                {
                    article.ArticleTags.Add(new ArticleTag { Tag = existingTag, Order = order });
                }
                else
                {
                    var newTag = new Tag { Name = tagName };
                    _dbContext.Tags.Add(newTag);
                    article.ArticleTags.Add(new ArticleTag { Tag = newTag, Order = order });
                }
            }

            _dbContext.Articles.Add(article);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ArticleDto> UpdateArticleAsync(UpdateArticleRequest request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var article = await _dbContext.Articles
                .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
                .Where(a => a.Id == request.Id)
                .FirstOrDefaultAsync();

            if (article == null)
            {
                throw new EntityNotFoundException("Статья не найдена по идентификатору");
            }

            article.ArticleTags.Clear();

            var tagsHashSet = new HashSet<string>();
            var uniqueTags = request.Tags.Where(x => tagsHashSet.Add(x.Trim())).Select(t => t.Trim()).ToList();

            var existingTags = await _dbContext.Tags
                .Where(t => tagsHashSet.Contains(t.Name))
                .ToListAsync();

            var existingTagDict = existingTags.ToDictionary(t => t.Name, t => t);

            for (var i = 0; i < uniqueTags.Count; i++)
            {
                var tagName = uniqueTags[i];
                var order = i + 1;

                if (existingTagDict.TryGetValue(tagName, out var existingTag))
                {
                    article.ArticleTags.Add(new ArticleTag { Tag = existingTag, Order = order });
                }
                else
                {
                    var newTag = new Tag { Name = tagName };
                    _dbContext.Tags.Add(newTag);
                    article.ArticleTags.Add(new ArticleTag { Tag = newTag, Order = order });
                }
            }

            article.Title = request.Title;
            article.UpdatedDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return article.MapArticleToArticleDto();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ArticleDto> GetArticleByIdAsync(int id)
    {
        var article = await _dbContext.Articles.Include(x => x.ArticleTags)
            .ThenInclude(x => x.Tag)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return article?.MapArticleToArticleDto();
    }

    public async Task<ArticleDto[]> GetArticlesAsync(int pageNumber, int pageSize)
    {
        if (pageSize == 0)
        {
            pageSize = GlobalConstants.DefaultPageSize;
        }

        var query = _dbContext.Articles.Include(x => x.ArticleTags)
            .ThenInclude(x => x.Tag)
            .AsNoTracking();

        var articles = await query.Skip(pageNumber).Take(pageSize)
            .OrderByDescending(x => x.UpdatedDate ?? x.CreatedDate)
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