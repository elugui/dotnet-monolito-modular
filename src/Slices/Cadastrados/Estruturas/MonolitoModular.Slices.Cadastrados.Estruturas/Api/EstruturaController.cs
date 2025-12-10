using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonolitoModular.Slices.Cadastrados.Estruturas.Features.CreateEstrutura;
using MonolitoModular.Slices.Cadastrados.Estruturas.Features.GetEstrutura;
using MonolitoModular.Slices.Cadastrados.Estruturas.Features.ListEstruturas;
using MonolitoModular.Slices.Cadastrados.Estruturas.Features.UpdateEstrutura;
using MonolitoModular.Slices.Cadastrados.Estruturas.Features.DeleteEstrutura;

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


    /// <summary>
    /// Cria uma nova Estrutura.
    /// </summary>
    /// <param name="command">Dados para criação da estrutura.</param>
    /// <returns>Id da estrutura criada.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEstruturaCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }


    /// <summary>
    /// Obtém uma Estrutura pelo Id.
    /// </summary>
    /// <param name="id">Id da estrutura.</param>
    /// <returns>Dados da estrutura ou 404 se não encontrada.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var estrutura = await _mediator.Send(new GetEstruturaQuery { Codigo = id });
        if (estrutura == null) return NotFound();
        return Ok(estrutura);
    }


    /// <summary>
    /// Lista todas as Estruturas.
    /// </summary>
    /// <returns>Lista de estruturas.</returns>
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var estruturas = await _mediator.Send(new ListEstruturasQuery());
        return Ok(estruturas);
    }


    /// <summary>
    /// Atualiza uma Estrutura existente.
    /// </summary>
    /// <param name="id">Id da estrutura.</param>
    /// <param name="command">Dados para atualização.</param>
    /// <returns>Resultado da operação.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEstruturaCommand command)
    {
        if (id != command.Codigo)
            return BadRequest("Id do recurso e do corpo não conferem.");
        var result = await _mediator.Send(command);
        return Ok(result);
    }


    /// <summary>
    /// Remove uma Estrutura pelo Id.
    /// </summary>
    /// <param name="id">Id da estrutura.</param>
    /// <returns>Resultado da operação.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteEstruturaCommand { Codigo = id });
        return Ok(result);
    }
}