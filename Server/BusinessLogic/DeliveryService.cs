using System.Globalization;
using DataAccess;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using AppContext = DataAccess.AppContext;

namespace BusinessLogic;

internal class DeliveryService(IDeliveryRepository deliveryRepository, AppContext context) : IDeliveryService
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
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            IgnoreBlankLines = true
        };

        using var connection = (NpgsqlConnection)context.Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken);

        using var writer = connection.BeginBinaryImport(
            "COPY \"Deliveries\" (longitude, latitude, subtotal, timestamp) FROM STDIN (FORMAT BINARY)");

        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, config);

        if (await csv.ReadAsync())
        {
            csv.ReadHeader();
        }

        while (await csv.ReadAsync())
        {
            try
            {
                double longitude = csv.GetField<double>("longitude");
                double latitude = csv.GetField<double>("latitude");
                double subtotal = csv.GetField<double>("subtotal");

                string rawTime = csv.GetField("timestamp");
                DateTime timestamp = DateTime.TryParse(rawTime, CultureInfo.InvariantCulture, out var parsed)
                    ? DateTime.SpecifyKind(parsed, DateTimeKind.Utc)
                    : DateTime.UtcNow;

                await writer.StartRowAsync(cancellationToken);
                await writer.WriteAsync(longitude, NpgsqlTypes.NpgsqlDbType.Double, cancellationToken);
                await writer.WriteAsync(latitude, NpgsqlTypes.NpgsqlDbType.Double, cancellationToken);
                await writer.WriteAsync(subtotal, NpgsqlTypes.NpgsqlDbType.Double, cancellationToken);
                await writer.WriteAsync(timestamp, NpgsqlTypes.NpgsqlDbType.TimestampTz, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error, line :{csv.Context.Parser.Row}: {ex.Message}");
                throw;
            }
        }

        await writer.CompleteAsync(cancellationToken);
    }

    public async Task<List<Delivery>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await deliveryRepository.GetAllAsync(cancellationToken);
    }
}