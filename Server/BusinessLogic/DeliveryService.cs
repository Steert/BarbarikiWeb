using DataAccess;


namespace BusinessLogic;

internal class DeliveryService(IDeliveryRepository deliveryRepository) : IDeliveryService
{
    public async Task CreateAsync(double longitude, double latitude, double subtotal,
        CancellationToken cancellationToken = default)
    {
        await deliveryRepository.CreateAsync(longitude, latitude, subtotal, cancellationToken);
    }

    public async Task ImportAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        await deliveryRepository.ImportAsync(stream, cancellationToken);
    }

    public async Task<List<Delivery>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await deliveryRepository.GetAllAsync(cancellationToken);
    }
}