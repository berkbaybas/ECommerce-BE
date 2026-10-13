using Core.Validation.Validators;
using FluentValidation;
using Product.Core.Exceptions;

namespace Product.Application.Products;

public sealed class UpdateProductStockCommandValidator : BaseValidator<UpdateProductStockCommand>
{
    public UpdateProductStockCommandValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithMessage(ProductLocalizations.Errors.Validation.StockNegative);
    }
}
