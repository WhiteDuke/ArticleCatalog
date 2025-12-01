namespace ArticleCatalog.Entities;

/// <summary>
/// Сущность связи между разделом и статьями
/// </summary>
public class SectionArticle
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Идентификатор раздела
    /// </summary>
    public int SectionId { get; set; }
    
    /// <summary>
    /// Сущность "Раздел"
    /// </summary>
    public Section Section { get; set; }
    
    /// <summary>
    /// Идентификатор статьи
    /// </summary>
    public int ArticleId { get; set; }
    
    /// <summary>
    /// Сущность "Статья"
    /// </summary>
    public Article Article { get; set; }
}