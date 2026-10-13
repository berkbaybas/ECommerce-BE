using Core.Validation.Validators;
using FluentValidation;
using Product.Core.Exceptions;

namespace Product.Application.Products;

public sealed class CreateProductCommandValidator : BaseValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        this.RuleFor(x => x.Name).NotEmpty().WithMessage(ProductLocalizations.Errors.Validation.NameEmpty);
        this.RuleFor(x => x.Sku).NotEmpty().WithMessage(ProductLocalizations.Errors.Validation.SkuEmpty);
        this.RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage(ProductLocalizations.Errors.Validation.PriceNegative);
        this.RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithMessage(ProductLocalizations.Errors.Validation.StockNegative);
    }
}
