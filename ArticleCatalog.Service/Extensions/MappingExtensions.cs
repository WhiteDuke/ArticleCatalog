using System.Linq;
using ArticleCatalog.Domain.Dto;
using ArticleCatalog.Domain.Entities;

namespace ArticleCatalog.Service.Extensions;

public static class MappingExtensions
{
    public static ArticleDto MapArticleToArticleDto(this Article article)
    {
        if (article == null)
        {
            return null;
        }

        return new ArticleDto
        {
            Id = article.Id,
            CreatedDate = article.CreatedDate,
            UpdatedDate = article.UpdatedDate,
            Title = article.Title,
            Tags = article.ArticleTags == null || article.ArticleTags.Count == 0 
                ? []
                : article.ArticleTags.OrderBy(x => x.Order).Select(x => x.Tag.Name).ToArray()
        };
    }
}