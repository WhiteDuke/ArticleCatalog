using System.Threading.Tasks;
using ArticleCatalog.Domain.Dto;
using ArticleCatalog.Domain.Requests;

namespace ArticleCatalog.Service.Services;

public interface IArticleService
{
    public Task CreateArticleAsync(CreateArticleRequest request);

    public Task<ArticleDto> UpdateArticleAsync(UpdateArticleRequest request);

    public Task<ArticleDto> GetArticleByIdAsync(int id);

    public Task<ArticleDto[]> GetArticlesAsync(int pageNumber, int pageSize);
}