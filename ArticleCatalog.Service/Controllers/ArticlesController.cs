using System;
using System.Threading.Tasks;
using ArticleCatalog.Domain.Dto;
using ArticleCatalog.Domain.Requests;
using ArticleCatalog.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArticleCatalog.Service.Controllers;

[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<ActionResult<ArticleDto[]>> GetArticles([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        var articles = await _articleService.GetArticlesAsync(pageNumber, pageSize);
        return Ok(articles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ArticleDto>> GetArticleById(int id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        return Ok(article);
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateArticle(CreateArticleRequest createArticleRequest)
    {
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
}