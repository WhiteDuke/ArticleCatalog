using System.Collections.Generic;

namespace ArticleCatalog.Domain.Entities;

/// <summary>
/// Модель "Тэг статьи"
/// </summary>
public class Tag
{
    /// <summary>
    /// Идентификатор тэга
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Название тэга
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Список связей между тэгом и статьями
    /// </summary>
    public List<ArticleTag> ArticleTags { get; set; } = [];
}