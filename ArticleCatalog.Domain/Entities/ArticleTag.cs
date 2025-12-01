namespace ArticleCatalog.Domain.Entities;

/// <summary>
/// Сущность связи между сущностями "Статья" и "Тэг"
/// </summary>
public class ArticleTag
{
    /// <summary>
    /// Идентификатор записи
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Порядок тэга
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Идентификатор статьи
    /// </summary>
    public int ArticleId { get; set; }
    
    /// <summary>
    /// Сущность "Статья"
    /// </summary>
    public Article Article { get; set; }

    /// <summary>
    /// Идентификатор тэга
    /// </summary>
    public int TagId { get; set; }

    /// <summary>
    /// Сущность "Тэг"
    /// </summary>
    public Tag Tag { get; set; }
}