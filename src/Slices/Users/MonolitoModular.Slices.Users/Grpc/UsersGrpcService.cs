using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace MonolitoModular.Slices.Users.Grpc;

/// <summary>
/// gRPC service for Users slice - Enables inter-slice communication
/// </summary>
public class UsersGrpcService : UsersService.UsersServiceBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersGrpcService> _logger;

    public UsersGrpcService(IMediator mediator, ILogger<UsersGrpcService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get a user by ID
    /// </summary>
    public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("gRPC GetUser called for ID: {UserId}", request.Id);

            if (!Guid.TryParse(request.Id, out var userId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid user ID format"));
            }

            var user = await _mediator.Send(new Features.GetUser.GetUserQuery(userId), context.CancellationToken);

            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"User {request.Id} not found"));
            }

            return new GetUserResponse
            {
                User = MapToDto(user)
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while getting user"));
        }
    }

    /// <summary>
    /// Get a user by email address
    /// </summary>
    public override async Task<GetUserResponse> GetUserByEmail(GetUserByEmailRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("gRPC GetUserByEmail called");

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email is required"));
            }

            var users = await _mediator.Send(new Features.ListUsers.ListUsersQuery(), context.CancellationToken);
            var user = users.FirstOrDefault(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"User with email {request.Email} not found"));
            }

            return new GetUserResponse
            {
                User = MapToDto(user)
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email");
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while getting user by email"));
        }
    }

    /// <summary>
    /// Check if a user exists by ID
    /// </summary>
    public override async Task<UserExistsResponse> UserExists(UserExistsRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("gRPC UserExists called for ID: {UserId}", request.Id);

            if (!Guid.TryParse(request.Id, out var userId))
            {
                return new UserExistsResponse { Exists = false };
            }

            var user = await _mediator.Send(new Features.GetUser.GetUserQuery(userId), context.CancellationToken);

            return new UserExistsResponse
            {
                Exists = user is not null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user exists {UserId}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while checking user existence"));
        }
    }

    /// <summary>
    /// Validate if a user is valid (exists and is active)
    /// </summary>
    public override async Task<ValidateUserResponse> ValidateUser(ValidateUserRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("gRPC ValidateUser called for ID: {UserId}", request.Id);

            if (!Guid.TryParse(request.Id, out var userId))
            {
                return new ValidateUserResponse
                {
                    IsValid = false,
                    Reason = "Invalid user ID format"
                };
            }

            var user = await _mediator.Send(new Features.GetUser.GetUserQuery(userId), context.CancellationToken);

            if (user is null)
            {
                return new ValidateUserResponse
                {
                    IsValid = false,
                    Reason = "User not found"
                };
            }

            if (!user.IsActive)
            {
                return new ValidateUserResponse
                {
                    IsValid = false,
                    Reason = "User is not active"
                };
            }

            return new ValidateUserResponse
            {
                IsValid = true,
                Reason = string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user {UserId}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while validating user"));
        }
    }

    /// <summary>
    /// List users with pagination
    /// </summary>
    public override async Task<ListUsersResponse> ListUsers(ListUsersRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("gRPC ListUsers called - Page: {Page}, Size: {Size}", request.PageNumber, request.PageSize);

            var users = await _mediator.Send(new Features.ListUsers.ListUsersQuery(), context.CancellationToken);

            // Apply filters
            if (request.ActiveOnly)
            {
                users = users.Where(u => u.IsActive).ToList();
            }

            // Apply pagination
            var pageSize = request.PageSize > 0 ? request.PageSize : 10;
            var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
            var totalCount = users.Count;

            var pagedUsers = users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto)
                .ToList();

            var response = new ListUsersResponse
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            response.Users.AddRange(pagedUsers);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing users");
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while listing users"));
        }
    }

    /// <summary>
    /// Map domain entity to gRPC DTO
    /// </summary>
    private static UserDto MapToDto(Domain.User user)
    {
        return new UserDto
        {
            Id = user.Id.ToString(),
            Name = user.Name,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt.ToString("O") // ISO 8601 format
        };
    }
}
