using Core.Abstraction.Exceptions;
using Product.Abstraction.Enums;
using Product.Core.Exceptions;

namespace Product.Domain.Entities;

public sealed partial class Product
{
    public Product(string name, string sku, string? description, decimal price, int stock)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(name, ProductLocalizations.Errors.Validation.NameEmpty);
        BusinessException.ThrowIfNullOrWhiteSpace(sku, ProductLocalizations.Errors.Validation.SkuEmpty);
        BusinessException.ThrowIf(price < 0, ProductLocalizations.Errors.Validation.PriceNegative);
        BusinessException.ThrowIf(stock < 0, ProductLocalizations.Errors.Validation.StockNegative);

        this.Name = name;
        this.Sku = sku;
        this.Description = description;
        this.Price = price;
        this.Stock = stock;
        this.Status = stock > 0 ? ProductStatus.Active : ProductStatus.OutOfStock;
    }

    public void UpdateDetails(string name, string? description)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(name, ProductLocalizations.Errors.Validation.NameEmpty);

        this.Name = name;
        this.Description = description;
        this.MarkUpdated();
    }

    public void UpdatePrice(decimal price)
    {
        BusinessException.ThrowIf(price < 0, ProductLocalizations.Errors.Validation.PriceNegative);

        this.Price = price;
        this.MarkUpdated();
    }

    public void UpdateStock(int stock)
    {
        BusinessException.ThrowIf(stock < 0, ProductLocalizations.Errors.Validation.StockNegative);

        this.Stock = stock;
        this.Status = stock > 0 && this.Status != ProductStatus.Discontinued
            ? ProductStatus.Active
            : stock == 0 && this.Status != ProductStatus.Discontinued
                ? ProductStatus.OutOfStock
                : this.Status;
        this.MarkUpdated();
    }

    public void Discontinue()
    {
        BusinessException.ThrowIf(this.Status == ProductStatus.Discontinued, ProductLocalizations.Errors.Business.AlreadyDiscontinued);

        this.Status = ProductStatus.Discontinued;
        this.MarkUpdated();
    }
}
