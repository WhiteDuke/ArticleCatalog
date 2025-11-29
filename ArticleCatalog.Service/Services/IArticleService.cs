using System.Threading.Tasks;
using ArticleCatalog.Domain.Data;
using ArticleCatalog.Domain.Requests;

namespace ArticleCatalog.Service.Services;

public interface IArticleService
{
    public Task CreateArticleAsync(CreateArticleRequest request);

    public Task<ArticleData> GetArticleByIdAsync(int id);

    public Task<ArticleData[]> GetArticlesAsync(int pageNumber, int pageSize);
}