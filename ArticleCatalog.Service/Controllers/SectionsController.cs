using System;
using System.Threading.Tasks;
using ArticleCatalog.Domain.Exceptions;
using ArticleCatalog.Domain.Services;
using ArticleCatalog.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace ArticleCatalog.Service.Controllers;

[Route("api/[controller]")]
public class SectionsController : ControllerBase
{
    private readonly IArticleCatalogService _articleCatalogService;
    private readonly ILogger<SectionsController> _logger;

    public SectionsController(IArticleCatalogService articleCatalogService, ILogger<SectionsController> logger)
    {
        _articleCatalogService = articleCatalogService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> GetSections()
    {
        var sections = await _articleCatalogService.GetSections();

        return Ok(sections);
    }

    [HttpGet("{sectionId}/articles")]
    public async Task<ActionResult<ArticleDto[]>> GetArticlesOfSection([FromRoute] int sectionId)
    {
        try
        {
            var articles = await _articleCatalogService.GetArticlesOfSectionAsync(sectionId);
            return Ok(articles);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Не найден раздел по идентификатору {Id}", sectionId);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка при получении списка статей для раздела");
            return Problem("Не удалось получить список статей для раздела. Попробуйте позднее.");
        }
    }
}