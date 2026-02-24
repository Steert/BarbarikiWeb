using Microsoft.EntityFrameworkCore;

namespace DataAccess;

internal class DeliveryRepository(AppContext context) : IDeliveryRepository
{
    public async Task CreateAsync(Delivery delivery, CancellationToken cancellationToken)
    {
        await context.Deliveries.AddAsync(delivery, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Delivery>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Deliveries.ToListAsync();
    }
}