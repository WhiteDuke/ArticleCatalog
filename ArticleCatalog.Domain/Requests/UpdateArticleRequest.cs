namespace ArticleCatalog.Domain.Requests;

public sealed class UpdateArticleRequest : CreateArticleRequest
{
    public int Id { get; set; }
}