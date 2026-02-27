using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;

namespace DataAccess.Helpers;

public class GeoHelper
{
    private static readonly Geometry nyBoundary;

    private const string Path = "gz_2010_us_040_00_500k.json";

    static GeoHelper()
    {
        var serializer = GeoJsonSerializer.Create();
        using (var streamReader = new StreamReader(Path))
        using (var jsonReader = new JsonTextReader(streamReader))
        {
            var featureCollection = serializer.Deserialize<FeatureCollection>(jsonReader);
            
            var nyFeature = featureCollection.FirstOrDefault(f => 
                f.Attributes.GetNames().Any(n => n.Equals("NAME", StringComparison.OrdinalIgnoreCase) 
                                                 && f.Attributes[n]?.ToString() == "New York"));

            nyBoundary = nyFeature.Geometry;
        }
    }

    public static bool IsInNewYork(double lon, double lat)
    {
        var point = new Point(lon, lat);
        return nyBoundary.Contains(point);
    }
}