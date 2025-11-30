using ArticleCatalog.Domain.Requests;
using FluentValidation;

namespace ArticleCatalog.Service.Validation;

public sealed class UpdateArticleRequestValidator : AbstractValidator<UpdateArticleRequest>
{
    public UpdateArticleRequestValidator()
    {
        RuleFor(x => x)
            .SetValidator(new CreateArticleRequestValidator());

        RuleFor(x => x.Id)
            .Must(id => id != 0)
            .When(x => x != null)
            .WithMessage("Отсутствует идентификатор статьи");
    }
}