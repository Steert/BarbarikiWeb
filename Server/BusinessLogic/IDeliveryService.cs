using DataAccess;

namespace BusinessLogic;

public interface IDeliveryService
{
    Task CreateAsync(double longitude, double latitude, double subtotal, CancellationToken cancellationToken = default);
    
    Task ImportAsync(Stream reader, CancellationToken cancellationToken = default);
    
    Task<PagedResponse<Delivery>> GetByPage(int pageNumber, int pageSize,CancellationToken cancellationToken = default);
}