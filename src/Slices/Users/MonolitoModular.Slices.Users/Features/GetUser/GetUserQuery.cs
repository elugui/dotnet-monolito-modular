using MonolitoModular.Slices.Users.Domain;
using MonolitoModular.Slices.Users.Infrastructure;

namespace MonolitoModular.Slices.Users.Features.GetUser;

public record GetUserQuery(Guid Id) : IRequest<User?>;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User?>
{
    private readonly UsersDbContext _context;

    public GetUserQueryHandler(UsersDbContext context)
    {
        _context = context;
    }

    public async Task<User?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users.FindAsync(new object[] { request.Id }, cancellationToken);
    }
}
