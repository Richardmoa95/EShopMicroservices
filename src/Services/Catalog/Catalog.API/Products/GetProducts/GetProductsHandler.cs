namespace Catalog.API.Products.GetProducts;

public record GetProductsCommand () : IQuery<GetProductsResult>;

public record GetProductsResult(
    IEnumerable<Product> Products
);

internal class GetProductsQueryHandler(IDocumentSession session, ILogger<GetProductsQueryHandler> logger) : IQueryHandler<GetProductsCommand, GetProductsResult>
{
    public async Task<GetProductsResult> Handle(GetProductsCommand query, CancellationToken cancellationToken)
    {
        logger.LogInformation("GetProductsQueryHandler.Handle called with {@Query}", query);

        var products = await session.Query<Product>().ToListAsync(cancellationToken);

        return new GetProductsResult(products);
    }
}
