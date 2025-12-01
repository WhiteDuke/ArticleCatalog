using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticleCatalog.DataAccess;
using ArticleCatalog.Domain.Exceptions;
using ArticleCatalog.Domain.Extensions;
using ArticleCatalog.Domain.Helpers;
using ArticleCatalog.Domain.Requests;
using ArticleCatalog.Dto;
using ArticleCatalog.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArticleCatalog.Domain.Services;

public sealed class ArticleCatalogService : IArticleCatalogService
{
    private readonly ArticleCatalogDbContext _dbContext;

    public ArticleCatalogService(ArticleCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateArticleAsync(CreateArticleRequest request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var uniqueTags = GetUniqueTags(request.Tags); 
            
            var article = new Article
            {
                CreatedDate = DateTime.UtcNow,
                Title = request.Title,
                ArticleTags = []
            };

            article = await AddTagsToArticleAsync(article, uniqueTags);

            _dbContext.Articles.Add(article);
            await _dbContext.SaveChangesAsync();

            await AddArticleToSectionAsync(article, uniqueTags);

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

            // Тэги статьи, до редактирования
            var savedArticleTagIds = article.ArticleTags.Select(x => x.TagId).ToArray();
            // Разделы, к которым относится статья по совпадению тегов до редактирования
            var presentSections = await _dbContext.Sections
                .Include(s => s.SectionArticles)
                .Include(s => s.SectionTags)
                .Where(section =>
                    section.SectionTags.Count == savedArticleTagIds.Length &&
                    savedArticleTagIds.All(tagId => section.SectionTags.Any(st => st.TagId == tagId))
                ).ToListAsync();

            if (presentSections.Count != 0)
            {
                presentSections.ForEach(s => s.SectionArticles.Clear());
            }

            if (article.ArticleTags.Count != 0)
            {
                article.ArticleTags.Clear();
            }

            var uniqueTags = GetUniqueTags(request.Tags);
            article = await AddTagsToArticleAsync(article, uniqueTags);
            article.Title = request.Title;
            article.UpdatedDate = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();

            await AddArticleToSectionAsync(article, uniqueTags);

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

    private static string[] GetUniqueTags(IEnumerable<string> notUniqueTags)
    {
        // чтобы обеспечить уникальность тэгов для статьи, но сохранить порядок тэгов
        // ["тег 1", "тег 2", "тег 3", "тег 1", "тег 4"] - "тег 1" в позиции 3 будет отброшен
        var tagsHashSet = new HashSet<string>();
        var uniqueTags = notUniqueTags.Where(x => tagsHashSet.Add(x.Trim())).Select(t => t).ToArray();

        return uniqueTags;
    }

    private async Task<Article> AddTagsToArticleAsync(Article article, string[] uniqueTags)
    {
        var existingTags = await _dbContext.Tags
            .Where(t => uniqueTags.Contains(t.Name))
            .ToListAsync();

        var existingTagDict = existingTags.ToDictionary(t => t.Name, t => t);

        for (var i = 0; i < uniqueTags.Length; i++)
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

        return article;
    }

    private async Task AddArticleToSectionAsync(Article article, ICollection<string> uniqueTags)
    {
        var articleId = article.Id;

        var articleTagIds = article.ArticleTags.Select(x => x.TagId).ToArray();
        var matchingSections = await _dbContext.Sections
            .Include(s => s.SectionTags)
            .Where(section => 
                section.SectionTags.Count == articleTagIds.Length &&
                articleTagIds.All(tagId => section.SectionTags.Any(st => st.TagId == tagId))
            ).ToListAsync();

        if (matchingSections.Count != 0)
        {
            foreach (var matchingSection in matchingSections)
            {
                _dbContext.SectionArticles.Add(new SectionArticle
                {
                    ArticleId = articleId,
                    SectionId = matchingSection.Id
                });    
            }
        }
        else
        {
            var sectionTitle = SectionHelper.GetSectionTitle(uniqueTags);

            var newSection = new Section
            {
                Title = sectionTitle,
                SectionTags = article.ArticleTags.Select(x => new SectionTag()
                {
                    TagId = x.TagId
                }).ToList(), 
                SectionArticles =
                [
                    new SectionArticle
                    {
                        ArticleId = articleId
                    }
                ]
            };

            _dbContext.Sections.Add(newSection);
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

        var articles = await query.OrderByDescending(x => x.UpdatedDate ?? x.CreatedDate)
            .Skip(pageNumber).Take(pageSize)
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

    public async Task<ArticleDto[]> GetArticlesOfSectionAsync(int sectionId)
    {
        var section = await _dbContext.Sections.Where(s => s.Id == sectionId).FirstOrDefaultAsync();

        if (section == null)
        {
            throw new EntityNotFoundException("Раздел не найден по идентификатору");
        }

        var sectionArticles = await _dbContext.SectionArticles
            .Where(sa => sa.SectionId == sectionId).AsNoTracking()
            .Include(s => s.Article)
            .ThenInclude(s => s.ArticleTags)
            .ThenInclude(at => at.Tag)
            .AsNoTracking()
            .Select(x => x.Article)
            .OrderByDescending(x => x.UpdatedDate ?? x.CreatedDate)
            .ToArrayAsync();

        var articles = sectionArticles.Select(x => x.MapArticleToArticleDto()).ToArray();

        return articles;
    }

    public async Task<SectionDto[]> GetSections()
    {
        var sections = await _dbContext.Sections.AsNoTracking()
            .Include(s => s.SectionArticles)
            .OrderByDescending(s => s.SectionArticles.Count).ToArrayAsync();

        return sections.Select(s => s.MapSectionToSectionDto()).ToArray();
    }
}