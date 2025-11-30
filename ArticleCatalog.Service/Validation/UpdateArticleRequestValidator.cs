using ArticleCatalog.Domain.Requests;
using FluentValidation;

namespace ArticleCatalog.Service.Validation;

public sealed class UpdateArticleRequestValidator : AbstractValidator<UpdateArticleRequest>
{
    public UpdateArticleRequestValidator()
    {
        RuleFor(x => x.Id)
            .Must(id => id != 0)
            .WithMessage("Отсутствует идентификатор статьи");

        RuleFor(x => x)
            .SetValidator(new CreateArticleRequestValidator());
    }
}