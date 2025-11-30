namespace ArticleCatalog.Domain.Requests;

public class CreateArticleRequest
{
    public string Title { get; set; }
    
    public string[] Tags { get; set; }
}