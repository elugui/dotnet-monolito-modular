using MonolitoModular.Slices.Users.Domain;
using MonolitoModular.Slices.Users.Infrastructure;

namespace MonolitoModular.Slices.Users.Features.CreateUser;

public record CreateUserCommand(string Name, string Email) : IRequest<Guid>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly UsersDbContext _context;

    public CreateUserCommandHandler(UsersDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Name = request.Name,
            Email = request.Email
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
