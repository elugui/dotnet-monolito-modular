using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonolitoModular.Slices.Users.Features.CreateUser;
using MonolitoModular.Slices.Users.Features.GetUser;
using MonolitoModular.Slices.Users.Features.ListUsers;

namespace MonolitoModular.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _mediator.Send(new ListUsersQuery());
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _mediator.Send(new GetUserQuery(id));
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var userId = await _mediator.Send(new CreateUserCommand(request.Name, request.Email));
        return CreatedAtAction(nameof(GetById), new { id = userId }, new { id = userId });
    }
}

public record CreateUserRequest(string Name, string Email);
