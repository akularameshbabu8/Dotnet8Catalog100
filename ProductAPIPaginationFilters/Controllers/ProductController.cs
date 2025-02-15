using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductAPIPaginationFilters.CQRS.Queries;
using ProductAPIPaginationFilters.Middlewares;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? productCode = null)
    {
        _logger.LogInformation("Fetching products via CQRS");

        try
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(new ErrorResponse { Message = "Page and pageSize must be greater than zero." });

            var result = await _mediator.Send(new GetProductsQuery(page, pageSize, productCode));

            if (!result.Items.Any())
                return NotFound(new ErrorResponse { Message = "No products found with the given filters." });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products.");
            return StatusCode(500, new ErrorResponse { Message = "Internal Server Error", Details = ex.Message });
        }
    }
}
