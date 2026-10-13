using Core.Abstraction.Exceptions;
using Core.CQRS.Commands;
using Product.Abstraction.Responses.Products;
using Product.Application.Products.CreateProduct;
using Product.Core.Exceptions;
using Product.Infrastructure.Repositories;
using Entity = Product.Domain.Entities;

namespace Product.Application.Products;

public sealed class CreateProductCommandHandler(IProductRepository productRepository)
    : CommandHandler<CreateProductCommand, ProductDto>
{
    public override async Task<ProductDto> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var skuExists = await productRepository.ExistsAsync(x => x.Sku == command.Sku, cancellationToken);
        BusinessException.ThrowIf(skuExists, ProductLocalizations.Errors.Business.SkuAlreadyExists);

        var product = new Entity.Product(command.Name, command.Sku, command.Description, command.Price, command.Stock);

        await productRepository.AddAsync(product, cancellationToken);

        return product.ToMapResponse();
    }
}
