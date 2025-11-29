using System;

namespace ArticleCatalog.Domain.Data;

public class ArticleData
{
    public int Id { get; set; }

    public string Title { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? UpdatedDate { get; set; }

    public string[] Tags { get; set; }
}