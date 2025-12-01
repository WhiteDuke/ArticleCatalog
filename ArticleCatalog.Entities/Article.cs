using System;
using System.Collections.Generic;

namespace ArticleCatalog.Entities;

/// <summary>
/// Сущность "Статья"
/// </summary>
public class Article
{
    /// <summary>
    /// Идентификатор статьи
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Заголовок статьи
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// Дата создания статьи
    /// </summary>
    public DateTime CreatedDate {get; set;}
    
    /// <summary>
    /// Дата редактирования статьи
    /// </summary>
    public DateTime? UpdatedDate {get; set;}

    /// <summary>
    /// Список связей между статьёй и тэгами
    /// </summary>
    public List<ArticleTag> ArticleTags { get; set; } = [];
    
    /// <summary>
    /// Список связей между разделоми и статьёй
    /// </summary>
    public List<SectionArticle> SectionArticles { get; set; } = [];
}