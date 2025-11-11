using MonolitoModular.Slices.Users.Domain;
using MonolitoModular.Slices.Users.Infrastructure;

namespace MonolitoModular.Slices.Users.Features.ListUsers;

public record ListUsersQuery : IRequest<List<User>>;

public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, List<User>>
{
    private readonly UsersDbContext _context;

    public ListUsersQueryHandler(UsersDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }
}
