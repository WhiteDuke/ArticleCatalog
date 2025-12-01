using System.Threading.Tasks;
using ArticleCatalog.Domain.Requests;
using ArticleCatalog.Dto;

namespace ArticleCatalog.Service.Services;

public interface IArticleCatalogService
{
    public Task CreateArticleAsync(CreateArticleRequest request);

    public Task<ArticleDto> UpdateArticleAsync(UpdateArticleRequest request);

    public Task<ArticleDto> GetArticleByIdAsync(int id);

    public Task<ArticleDto[]> GetArticlesAsync(int pageNumber, int pageSize);

    public Task<ArticleDto[]> GetArticlesOfSectionAsync(int sectionId);

    public Task<SectionDto[]> GetSections();
}