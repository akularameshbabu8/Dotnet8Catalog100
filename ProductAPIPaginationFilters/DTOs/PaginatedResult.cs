namespace ProductAPIPaginationFilters.DTOs
{
    public record PaginatedResult<T>(IEnumerable<T> Items, int TotalCount, int TotalPages, int CurrentPage, int PageSize);

}
