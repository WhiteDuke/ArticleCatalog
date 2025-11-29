namespace ArticleCatalog.Domain.Entities;

public class Section
{
    public int Id { get; set; }

    public string Title { get; set; }

    public ArticleTag[] ArticleTags { get; set; }
    
    public SectionArticle[] SectionArticles { get; set; }
}