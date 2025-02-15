using MediatR;
using ProductAPIPaginationFilters.DTOs;

namespace ProductAPIPaginationFilters.CQRS.Queries
{
    public record GetProductsQuery(int Page, int PageSize, string? ProductCode) : IRequest<PaginatedResult<Product>>;

}
