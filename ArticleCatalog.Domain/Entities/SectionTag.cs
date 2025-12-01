namespace ArticleCatalog.Domain.Entities;

/// <summary>
/// Сущность связи между разделом и тэгом
/// </summary>
public class SectionTag
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
    /// Идентификатор тэга
    /// </summary>
    public int TagId { get; set; }
    
    /// <summary>
    /// Сущность "Тэг"
    /// </summary>
    public Tag Tag { get; set; }
}