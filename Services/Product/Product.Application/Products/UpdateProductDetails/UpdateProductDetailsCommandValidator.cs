using Core.Validation.Validators;
using FluentValidation;
using Product.Core.Exceptions;

namespace Product.Application.Products;

public sealed class UpdateProductDetailsCommandValidator : BaseValidator<UpdateProductDetailsCommand>
{
    public UpdateProductDetailsCommandValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.Name).NotEmpty().WithMessage(ProductLocalizations.Errors.Validation.NameEmpty);
    }
}
