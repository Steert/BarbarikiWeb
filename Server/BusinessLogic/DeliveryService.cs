using DataAccess;


namespace BusinessLogic;

internal class DeliveryService(IDeliveryRepository deliveryRepository) : IDeliveryService
{
    public async Task CreateAsync(double longitude, double latitude, double subtotal,
        CancellationToken cancellationToken = default)
    {
        var delivery = new Delivery
        {
            longitude = longitude,
            latitude = latitude,
            timestamp = DateTime.UtcNow,
            subtotal = subtotal,
        };
        await deliveryRepository.CreateAsync(delivery, cancellationToken);
    }

    public async Task ImportAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        await deliveryRepository.ImportAsync(stream, cancellationToken);
    }

    public async Task<PagedResponse<Delivery>> GetByPage(int pageNumber, int pageSize,CancellationToken cancellationToken = default)
    {
        return await deliveryRepository.GetPagedAsync(pageNumber, pageSize, cancellationToken);
    }
}