using System.Text.Json;

namespace DataAccess.Helpers;

public class TaxRateHelper
{
    private const string Path = "taxrates.json";

    public static List<TaxRateData> taxRates;

    static TaxRateHelper()
    {
        string jsonContent = File.ReadAllText(Path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var rateList = JsonSerializer.Deserialize<List<TaxRateData>>(jsonContent, options);
        taxRates = rateList;
    }
}

public record TaxRateData(
    string County,
    double Rate
);