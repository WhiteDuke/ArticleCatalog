using System;

namespace ArticleCatalog.Domain.Entities;

public class Article
{
    public int Id { get; set; }

    public string Title { get; set; }
    
    public DateTimeOffset CreatedDate {get; set;}
    
    public DateTimeOffset? UpdatedDate {get; set;}

    public ArticleTag[] ArticleTags { get; set; }
    
    public SectionArticle[] SectionArticles { get; set; }
}