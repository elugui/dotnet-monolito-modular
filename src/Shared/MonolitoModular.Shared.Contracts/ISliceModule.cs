namespace MonolitoModular.Shared.Contracts;

/// <summary>
/// Interface for slice module registration
/// </summary>
public interface ISliceModule
{
    /// <summary>
    /// Register services for dependency injection
    /// </summary>
    void RegisterServices(IServiceCollection services, IConfiguration configuration);
}
