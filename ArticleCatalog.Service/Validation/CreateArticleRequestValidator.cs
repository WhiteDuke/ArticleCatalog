using System.Linq;
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
            .Must(x => x.Distinct().Count() <= 256)
            .When(x => x?.Tags != null)
            .WithMessage("У статьи может быть не больше 256 уникальных тэгов");

        RuleFor(x => x.Title)
            .Must(x => !string.IsNullOrEmpty(x))
            .When(x => x != null)
            .WithMessage("У статьи должно быть название");

        RuleFor(x => x.Title)
            .Must(x => x.Length <= 256)
            .When(x => x != null && !string.IsNullOrWhiteSpace(x.Title))
            .WithMessage("Название статьи должно быть не больше 256 символов");
    }
}