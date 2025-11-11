using ModularMonolith.Modules.Customers.Application.DTOs;
using ModularMonolith.Modules.Customers.Domain.Repositories;
using ModularMonolith.Shared.Application.Queries;

namespace ModularMonolith.Modules.Customers.Application.Queries;

public class GetAllCustomersQueryHandler : IQueryHandler<GetAllCustomersQuery, IEnumerable<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetAllCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);

        return customers.Select(c => new CustomerDto(
            c.Id,
            c.Name,
            c.Email.Value,
            c.PhoneNumber,
            c.CreatedAt,
            c.UpdatedAt,
            c.IsActive
        ));
    }
}
