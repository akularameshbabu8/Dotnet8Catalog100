using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductAPIPaginationFilters.CQRS.Queries;
using ProductAPIPaginationFilters.DTOs;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedResult<Product>>
{
    private readonly AppDbContext _context;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    public GetProductsQueryHandler(AppDbContext context, ILogger<GetProductsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResult<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching products with pagination and filters");

            if (request.Page < 1 || request.PageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than zero.");

            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(request.ProductCode))
                query = query.Where(p => p.Code.Contains(request.ProductCode));

            var totalCount = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            var items = await query.Skip((request.Page - 1) * request.PageSize)
                                   .Take(request.PageSize)
                                   .ToListAsync(cancellationToken);

            return new PaginatedResult<Product>(items, totalCount, totalPages, request.Page, request.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products.");
            throw;
        }
    }
}
