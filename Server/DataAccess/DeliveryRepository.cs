using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using DataAccess.Helpers;
using DataAccess.MessagesModels;

namespace DataAccess;

internal class DeliveryRepository(AppContext context) : IDeliveryRepository
{
    public async Task CreateAsync(double longitude, double latitude, double subtotal, CancellationToken cancellationToken)
    {
        var delivery = new Delivery
        {
            longitude = longitude,
            latitude = latitude,
            timestamp = DateTime.UtcNow,
            subtotal = subtotal,
        };
        await context.Deliveries.AddAsync(delivery, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
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
        
        int batchSize = 15;
        var batch = new List<Task<(bool IsSkip, Delivery Data)>>();
        
        int counter = 0;
        while (await csv.ReadAsync())
        {
            counter++;
            Console.WriteLine(counter);
            try
            {
                double longitude = csv.GetField<double>("longitude");
                double latitude = csv.GetField<double>("latitude");
                double subtotal = csv.GetField<double>("subtotal");

                string rawTime = csv.GetField("timestamp");
                DateTime timestamp = DateTime.TryParse(rawTime, CultureInfo.InvariantCulture, out var parsed)
                    ? DateTime.SpecifyKind(parsed, DateTimeKind.Utc)
                    : DateTime.UtcNow;
                
                batch.Add(ValidateAndReturnData(new Delivery(){longitude = longitude, latitude = latitude, subtotal = subtotal, timestamp = timestamp}));
                
                if (batch.Count >= batchSize)
                {
                    await ProcessBatch(batch, writer, cancellationToken);
                    batch.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error, line :{csv.Context.Parser.Row}: {ex.Message}");
                throw;
            }
        }
        
        if (batch.Count > 0)
        {
            await ProcessBatch(batch, writer, cancellationToken);
        }
        
        await writer.CompleteAsync(cancellationToken);
    }

    public async Task<List<Delivery>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Deliveries.ToListAsync();
    }
    private static async Task<(bool IsSkip, Delivery Data)> ValidateAndReturnData(Delivery data)
    {
        bool isSkip = await AddressHelper.ValidateLocation(data.longitude, data.latitude);
        return (isSkip, data);
    }

    private static async Task ProcessBatch(List<Task<(bool IsSkip, Delivery Data)>> batch, NpgsqlBinaryImporter writer, CancellationToken ct)
    {
        var results = await Task.WhenAll(batch);

        foreach (var (isSkip, data) in results)
        {
            if (isSkip) continue;

            await writer.StartRowAsync(ct);
            await writer.WriteAsync(data.longitude, NpgsqlTypes.NpgsqlDbType.Double, ct);
            await writer.WriteAsync(data.latitude, NpgsqlTypes.NpgsqlDbType.Double, ct);
            await writer.WriteAsync(data.subtotal, NpgsqlTypes.NpgsqlDbType.Double, ct);
            await writer.WriteAsync(data.timestamp, NpgsqlTypes.NpgsqlDbType.TimestampTz, ct);
        }
    }
}