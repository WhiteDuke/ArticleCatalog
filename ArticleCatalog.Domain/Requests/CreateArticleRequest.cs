namespace ArticleCatalog.Domain.Requests;

/// <summary>
/// Запрос на создание статьи
/// </summary>
public class CreateArticleRequest
{
    /// <summary>
    /// Заголовок статьи
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Набор тэгов статьи (уникальный, не более 256шт.)
    /// </summary>
    public string[] Tags { get; set; } = [];
}