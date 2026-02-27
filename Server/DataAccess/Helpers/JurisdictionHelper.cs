using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Prepared;
using NetTopologySuite.IO;


namespace DataAccess.Helpers;

public class JurisdictionLookupService
{
    private static readonly List<(IPreparedGeometry Boundary, TaxRateData Data)> jurisdictions;
    private const string Path = "NYC.geojson";

    static JurisdictionLookupService()
    {
        var reader = new GeoJsonReader();
        var featureCollection = reader.Read<FeatureCollection>(File.ReadAllText(Path));

        jurisdictions = new List<(IPreparedGeometry, TaxRateData)>();

        foreach (var feature in featureCollection)
        {
            string name = feature.Attributes["name"]?.ToString();
            var rateData =
                TaxRateHelper.taxRates.FirstOrDefault(r => r.County.Contains(name, StringComparison.OrdinalIgnoreCase));
            if (rateData != null)
            {
                var prepared = PreparedGeometryFactory.Prepare(feature.Geometry);
                jurisdictions.Add((prepared, rateData));
            }
        }
    }

    public static double GetJurisdiction(double lon, double lat, out bool isSpecial)
    {
        var point = new Point(lon, lat);
        isSpecial = jurisdictions.FirstOrDefault(j => j.Boundary.Contains(point)).Data.County.Contains("*");
        return jurisdictions.FirstOrDefault(j => j.Boundary.Contains(point)).Data.Rate;
    }
}