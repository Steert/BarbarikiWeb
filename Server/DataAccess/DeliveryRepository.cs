using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DataAccess;

public class PagedResponse<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    public PagedResponse(List<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

internal class DeliveryRepository(AppContext context) : IDeliveryRepository
{
    public async Task CreateAsync(Delivery delivery, CancellationToken cancellationToken)
    {
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

    public async Task<PagedResponse<Delivery>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var totalCount = await context.Deliveries.CountAsync(cancellationToken);
        
        var items = await context.Deliveries
            .AsNoTracking()
            .OrderByDescending(d => d.timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<Delivery>(items, totalCount, pageNumber, pageSize);
    }
}