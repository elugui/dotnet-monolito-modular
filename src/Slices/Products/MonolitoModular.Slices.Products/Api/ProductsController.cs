using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonolitoModular.Slices.Products.Features.CreateProductWithUserValidation;
using MonolitoModular.Slices.Products.Features.GetProduct;
using MonolitoModular.Slices.Products.Features.ListProducts;

namespace MonolitoModular.Slices.Products.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _mediator.Send(new ListProductsQuery());
            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _mediator.Send(new GetProductQuery(id));
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductWithUserValidationRequest request)
        {
            var productId = await _mediator.Send(
                new CreateProductWithUserValidationCommand(
                    request.Name,
                    request.Description,
                    request.Price,
                    request.Stock,
                    request.CreatedByUserId
                )
            );
            return CreatedAtAction(nameof(GetById), new { id = productId }, new { id = productId });
        }
    }

    public record CreateProductWithUserValidationRequest(
        string Name,
        string Description,
        decimal Price,
        int Stock,
        string CreatedByUserId
    );
}