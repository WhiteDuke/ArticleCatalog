using System.Collections.Generic;
using System.Threading.Tasks;
using ArticleCatalog.Domain.Entities;
using ArticleCatalog.Domain.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ArticleCatalog.Service.Controllers;

[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Article>>> GetArticles()
    {
        await Task.Delay(1000);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetArticleById(int id)
    {
        await Task.Delay(1000);
        return Ok();
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateArticle(CreateArticleRequest article)
    {
        await Task.Delay(1000);
        return Ok();
    }
}