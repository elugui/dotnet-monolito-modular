using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonolitoModular.Slices.Cadastrados.Estruturas.Domain;
using MonolitoModular.Slices.Cadastrados.Estruturas.Features.CreateEstrutura;
using MonolitoModular.Slices.Cadastrados.Estruturas.Features.UpdateEstrutura;
using MonolitoModular.Slices.Cadastrados.Estruturas.Features.DeleteEstrutura;
using MonolitoModular.Slices.Cadastrados.Estruturas.Features.GetEstrutura;
using MonolitoModular.Slices.Cadastrados.Estruturas.Features.ListEstruturas;

namespace MonolitoModular.Host.Controllers
{
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
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateEstruturaCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Delete(Guid codigo)
        {
            var result = await _mediator.Send(new DeleteEstruturaCommand { Codigo = codigo });
            return Ok(result);
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> Get(Guid codigo)
        {
            var result = await _mediator.Send(new GetEstruturaQuery { Codigo = codigo });
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var result = await _mediator.Send(new ListEstruturasQuery());
            return Ok(result);
        }
    }
}
