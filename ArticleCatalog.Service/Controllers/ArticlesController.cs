using System;
using System.Threading.Tasks;
using ArticleCatalog.Domain.Dto;
using ArticleCatalog.Domain.Requests;
using ArticleCatalog.Service.Exceptions;
using ArticleCatalog.Service.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ArticleCatalog.Service.Controllers;

[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;
    private readonly IValidator<CreateArticleRequest> _createArticleRequestValidator;
    private readonly IValidator<UpdateArticleRequest> _updateArticleRequestValidator;

    public ArticlesController(
        IArticleService articleService,
        IValidator<CreateArticleRequest> createArticleRequestValidator,
        IValidator<UpdateArticleRequest> updateArticleRequestValidator)
    {
        _articleService = articleService;
        _createArticleRequestValidator = createArticleRequestValidator;
        _updateArticleRequestValidator = updateArticleRequestValidator;
    }

    [HttpGet]
    public async Task<ActionResult<ArticleDto[]>> GetArticles(
        [FromQuery] int pageNumber = 0,
        [FromQuery] int pageSize = GlobalConstants.DefaultPageSize)
    {
        var articles = await _articleService.GetArticlesAsync(pageNumber, pageSize);
        return Ok(articles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ArticleDto>> GetArticleById([FromRoute] int id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        return Ok(article);
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateArticle([FromBody] CreateArticleRequest createArticleRequest)
    {
        // ReSharper disable once MethodHasAsyncOverload
        var validationResult = _createArticleRequestValidator.Validate(createArticleRequest);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToString());
        }
        
        try
        {
            await _articleService.CreateArticleAsync(createArticleRequest);

            return Ok();
        }
        catch (Exception)
        {
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
            return BadRequest(validationResult.ToString());
        }

        try
        {
            var updatedArticle = await _articleService.UpdateArticleAsync(updateArticleRequest);

            return Ok(updatedArticle);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return Problem("Не удалось отредактировать статью. Попробуйте позднее.");
        }
    }
}