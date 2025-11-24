using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonolitoModular.Slices.Cadastrados.Estruturas.Features.CreateEstrutura;

namespace MonolitoModular.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstruturasController : ControllerBase
{
    private readonly IMediator _mediator;

    public EstruturasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEstruturaCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        // Implementar consulta se necessário
        return Ok();
    }
}