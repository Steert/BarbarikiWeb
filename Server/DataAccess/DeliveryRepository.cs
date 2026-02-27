using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using DataAccess.Helpers;
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
    public async Task CreateAsync(double longitude, double latitude, double subtotal,
        CancellationToken cancellationToken)
    {
        bool isSpecial;

        double composite_tax_rate = JurisdictionLookupService.GetJurisdiction(longitude, latitude, out isSpecial);
        double special_rate = 0;
        double county_rate = 0;

        if (isSpecial)
        {
            special_rate = 0.00375;
        }

        county_rate = composite_tax_rate - special_rate - 0.04;
        double tax_amount = subtotal * composite_tax_rate;
        double total_amount = subtotal + tax_amount;
        
        var delivery = new Delivery
        {
            longitude = longitude,
            latitude = latitude,
            timestamp = DateTime.UtcNow,
            subtotal = subtotal,
            composite_tax_rate = composite_tax_rate,
            county_rate = county_rate,
            special_rates =  special_rate,
            tax_amount = tax_amount,
            total_amount = total_amount,
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
            "COPY \"Deliveries\" (longitude, latitude, subtotal, timestamp, composite_tax_rate, state_rate, county_rate, special_rates, tax_amount, total_amount) FROM STDIN (FORMAT BINARY)");

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
                
                if (!GeoHelper.IsInNewYork(longitude, latitude))
                {
                    continue;
                }

                bool isSpecial;

                double composite_tax_rate = JurisdictionLookupService.GetJurisdiction(longitude, latitude, out isSpecial);
                double special_rate = 0;
                double county_rate = 0;

                if (isSpecial)
                {
                    special_rate = 0.00375;
                }

                county_rate = composite_tax_rate - special_rate - 0.04;
                double tax_amount = subtotal * composite_tax_rate;
                double total_amount = subtotal + tax_amount;
                
                await writer.StartRowAsync();
                await writer.WriteAsync(longitude, NpgsqlTypes.NpgsqlDbType.Double);
                await writer.WriteAsync(latitude, NpgsqlTypes.NpgsqlDbType.Double);
                await writer.WriteAsync(subtotal, NpgsqlTypes.NpgsqlDbType.Double);
                await writer.WriteAsync(timestamp, NpgsqlTypes.NpgsqlDbType.TimestampTz);
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