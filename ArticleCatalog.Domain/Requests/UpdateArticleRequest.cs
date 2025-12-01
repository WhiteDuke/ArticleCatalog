namespace ArticleCatalog.Domain.Requests;

/// <summary>
/// Запрос на обновление статьи
/// </summary>
public sealed class UpdateArticleRequest : CreateArticleRequest
{
    /// <summary>
    /// Идентификатор статьи
    /// </summary>
    public int Id { get; set; }
}