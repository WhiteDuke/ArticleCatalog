using System.Linq;
using ArticleCatalog.Domain;
using ArticleCatalog.Domain.Requests;
using FluentValidation;

namespace ArticleCatalog.Service.Validation;

public sealed class CreateArticleRequestValidator : AbstractValidator<CreateArticleRequest>
{
    public CreateArticleRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x != null)
            .WithMessage("Отсутствуют данные");

        RuleFor(x => x.Tags)
            .Must(x => x.Distinct().Count() <= GlobalConstants.ArticleTagsMaxCount)
            .When(x => x?.Tags != null)
            .WithMessage($"У статьи может быть не больше {GlobalConstants.ArticleTagsMaxCount} уникальных тэгов");

        RuleFor(x => x.Title)
            .Must(x => !string.IsNullOrEmpty(x))
            .When(x => x != null)
            .WithMessage("У статьи должно быть название");

        RuleFor(x => x.Title)
            .Must(x => x.Length <= GlobalConstants.ArticleTitleLength)
            .When(x => x != null && !string.IsNullOrWhiteSpace(x.Title))
            .WithMessage($"Название статьи должно быть не больше {GlobalConstants.ArticleTitleLength} символов");
    }
}