namespace Catalog.API.Products.GetProducts;

public record GetProductsCommand () : IQuery<GetProductsResult>;

public record GetProductsResult(
    IEnumerable<Product> Products
);

internal class GetProductsQueryHandler(IDocumentSession session) : IQueryHandler<GetProductsCommand, GetProductsResult>
{
    public async Task<GetProductsResult> Handle(GetProductsCommand query, CancellationToken cancellationToken)
    {
        var products = await session.Query<Product>().ToListAsync(cancellationToken);

        return new GetProductsResult(products);
    }
}
