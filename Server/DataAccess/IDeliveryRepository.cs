using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public interface IDeliveryRepository
{
    Task CreateAsync(Delivery delivery, CancellationToken cancellationToken);
    
    Task ImportAsync(Stream stream, CancellationToken cancellationToken);

    Task<PagedResponse<Delivery>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
}