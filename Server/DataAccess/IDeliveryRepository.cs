using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public interface IDeliveryRepository
{
    Task CreateAsync(double longitude, double latitude, double subtotal, CancellationToken cancellationToken);
    
    Task ImportAsync(Stream stream, CancellationToken cancellationToken);

    Task<PagedResponse<Delivery>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
}