using System.Collections.Generic;

namespace ArticleCatalog.Entities;

/// <summary>
/// Сущность "Раздел"
/// </summary>
public class Section
{
    /// <summary>
    /// Идентификатор раздела
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Заголовок раздела
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Список связей между разделом и статьями
    /// </summary>
    public List<SectionArticle> SectionArticles { get; set; } = [];

    /// <summary>
    /// Список связей между разделом и тэгами
    /// </summary>
    public List<SectionTag> SectionTags { get; set; } = [];
}