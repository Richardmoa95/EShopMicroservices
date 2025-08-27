namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommad(
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price
) : ICommand<CreateProductResult>;

public record CreateProductResult(
    Guid Id
);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommad>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.");
        RuleFor(x => x.Category).NotEmpty().WithMessage("At least one category is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Product description is required.");
        RuleFor(x => x.ImageFile).NotEmpty().WithMessage("Product image file is required.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Product price must be greater than zero.");
    }
}

internal class CreateProductCommandHandler
    (IDocumentSession session, ILogger<CreateProductCommandHandler> logger)
    : ICommandHandler<CreateProductCommad, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommad request, CancellationToken cancellationToken)
    {
        logger.LogInformation("CreateProductCommandHandler.Handle Called with {@request}", request);

        // create Product entity from command object
        var product = new Product
        {
            Name = request.Name,
            Category = request.Category,
            Description = request.Description,
            ImageFile = request.ImageFile,
            Price = request.Price
        };

        // save to database
        session.Store(product);
        await session.SaveChangesAsync(cancellationToken);

        // return result
        return new CreateProductResult(product.Id);
    }
}
