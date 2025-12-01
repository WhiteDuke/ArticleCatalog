using System;
using System.Threading.Tasks;
using ArticleCatalog.Domain.Exceptions;
using ArticleCatalog.Domain.Services;
using ArticleCatalog.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ArticleCatalog.Service.Controllers;

[Route("api/[controller]")]
public class SectionsController : ControllerBase
{
    private readonly IArticleCatalogService _articleCatalogService;

    public SectionsController(IArticleCatalogService articleCatalogService)
    {
        _articleCatalogService = articleCatalogService;
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
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return Problem("Не удалось получить список статей для раздела. Попробуйте позднее.");
        }
    }
}