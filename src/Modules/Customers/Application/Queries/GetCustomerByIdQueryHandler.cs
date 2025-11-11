using ModularMonolith.Modules.Customers.Application.DTOs;
using ModularMonolith.Modules.Customers.Domain.Repositories;
using ModularMonolith.Shared.Application.Queries;

namespace ModularMonolith.Modules.Customers.Application.Queries;

public class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        if (customer == null)
        {
            return null;
        }

        return new CustomerDto(
            customer.Id,
            customer.Name,
            customer.Email.Value,
            customer.PhoneNumber,
            customer.CreatedAt,
            customer.UpdatedAt,
            customer.IsActive
        );
    }
}
