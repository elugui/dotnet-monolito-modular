using ModularMonolith.Modules.Customers.Application.DTOs;
using ModularMonolith.Modules.Customers.Domain.Entities;
using ModularMonolith.Modules.Customers.Domain.Repositories;
using ModularMonolith.Shared.Application.Commands;
using ModularMonolith.Shared.Application.Interfaces;

namespace ModularMonolith.Modules.Customers.Application.Commands;

public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Check if customer with email already exists
        var existingCustomer = await _customerRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingCustomer != null)
        {
            throw new InvalidOperationException($"Customer with email {request.Email} already exists");
        }

        var customer = Customer.Create(request.Name, request.Email, request.PhoneNumber);

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
