namespace ArticleCatalog.Domain.Entities;

public class SectionArticle
{
    public int Id { get; set; }
    
    public int SectionId { get; set; }
    
    public Section Section { get; set; }
    
    public int ArticleId { get; set; }
    
    public Article Article { get; set; }
}