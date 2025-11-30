using System;
using System.Collections.Generic;

namespace ArticleCatalog.Domain.Entities;

public class Article
{
    public int Id { get; set; }

    public string Title { get; set; }
    
    public DateTime CreatedDate {get; set;}
    
    public DateTime? UpdatedDate {get; set;}

    public List<ArticleTag> ArticleTags { get; set; } = [];
    
    public List<SectionArticle> SectionArticles { get; set; } = [];
}