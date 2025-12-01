using System;
using System.Threading.Tasks;
using ArticleCatalog.Domain.Dto;
using ArticleCatalog.Service.Exceptions;
using ArticleCatalog.Service.Services;
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