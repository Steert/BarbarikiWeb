using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public interface IDeliveryRepository
{
    Task CreateAsync(Delivery delivery, CancellationToken cancellationToken);
    
    Task ImportAsync(Stream stream, CancellationToken cancellationToken);
    
    Task<List<Delivery>> GetAllAsync(CancellationToken cancellationToken);
}