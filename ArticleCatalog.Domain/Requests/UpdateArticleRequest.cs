namespace ArticleCatalog.Domain.Requests;

public sealed class UpdateArticleRequest
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public string[] Tags { get; set; }
}