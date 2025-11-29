using System;

namespace ArticleCatalog.Domain.Dto;

public sealed class ArticleDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? UpdatedDate { get; set; }

    public string[] Tags { get; set; }
}