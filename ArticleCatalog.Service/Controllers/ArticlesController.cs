using System;
using System.Threading.Tasks;
using ArticleCatalog.Domain;
using ArticleCatalog.Domain.Exceptions;
using ArticleCatalog.Domain.Requests;
using ArticleCatalog.Domain.Services;
using ArticleCatalog.Dto;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace ArticleCatalog.Service.Controllers;

[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleCatalogService _articleCatalogService;
    private readonly IValidator<CreateArticleRequest> _createArticleRequestValidator;
    private readonly IValidator<UpdateArticleRequest> _updateArticleRequestValidator;
    private readonly ILogger<ArticlesController> _logger;

    public ArticlesController(
        IArticleCatalogService articleCatalogService,
        IValidator<CreateArticleRequest> createArticleRequestValidator,
        IValidator<UpdateArticleRequest> updateArticleRequestValidator,
        ILogger<ArticlesController> logger)
    {
        _articleCatalogService = articleCatalogService;
        _createArticleRequestValidator = createArticleRequestValidator;
        _updateArticleRequestValidator = updateArticleRequestValidator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ArticleDto[]>> GetArticles(
        [FromQuery] int pageNumber = 0,
        [FromQuery] int pageSize = GlobalConstants.DefaultPageSize)
    {
        var articles = await _articleCatalogService.GetArticlesAsync(pageNumber, pageSize);
        return Ok(articles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ArticleDto>> GetArticleById([FromRoute] int id)
    {
        var article = await _articleCatalogService.GetArticleByIdAsync(id);

        if (article != null)
        {
            return Ok(article);
        }

        _logger.LogWarning("Не найдена статья по идентификатору {Id}", id);
        return NotFound();
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateArticle([FromBody] CreateArticleRequest createArticleRequest)
    {
        // ReSharper disable once MethodHasAsyncOverload
        var validationResult = _createArticleRequestValidator.Validate(createArticleRequest);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Результат валидации запроса на создание статьи: {ValidationResult}", validationResult.ToString());
            return BadRequest(validationResult.ToString());
        }

        try
        {
            await _articleCatalogService.CreateArticleAsync(createArticleRequest);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось создать статью");
            return Problem("Не удалось сохранить статью. Попробуйте позднее.");
        }
    }

    [HttpPatch("update")]
    public async Task<ActionResult<ArticleDto>> UpdateArticle([FromBody] UpdateArticleRequest updateArticleRequest)
    {
        // ReSharper disable once MethodHasAsyncOverload
        var validationResult = _updateArticleRequestValidator.Validate(updateArticleRequest);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Результат валидации запроса на обновление статьи: {ValidationResult}", validationResult.ToString());
            return BadRequest(validationResult.ToString());
        }

        try
        {
            var updatedArticle = await _articleCatalogService.UpdateArticleAsync(updateArticleRequest);

            return Ok(updatedArticle);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Не найдена статья по идентификатору");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка при редактировании статьи");
            return Problem("Не удалось отредактировать статью. Попробуйте позднее.");
        }
    }
}