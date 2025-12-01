using System;

namespace ArticleCatalog.Domain.Dto;

/// <summary>
/// Статья
/// </summary>
public sealed class ArticleDto
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
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Дата редактирования статьи
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Набор тэгов (уникальный, не более 256 шт.)
    /// </summary>
    public string[] Tags { get; set; }
}