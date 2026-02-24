using DataAccess;

namespace BusinessLogic;

public interface IDeliveryService
{
    Task CreateAsync(double longitude, double latitude, double subtotal, CancellationToken cancellationToken = default);
    
    Task ImportAsync(Stream reader, CancellationToken cancellationToken = default);
    
    Task<List<Delivery>> GetAllAsync(CancellationToken cancellationToken = default);
}