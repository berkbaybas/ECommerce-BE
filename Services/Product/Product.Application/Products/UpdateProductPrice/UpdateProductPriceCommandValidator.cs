using Core.Validation.Validators;
using FluentValidation;
using Product.Core.Exceptions;

namespace Product.Application.Products;

public sealed class UpdateProductPriceCommandValidator : BaseValidator<UpdateProductPriceCommand>
{
    public UpdateProductPriceCommandValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage(ProductLocalizations.Errors.Validation.PriceNegative);
    }
}
