using System.Globalization;
using System.Net.Http.Json;
using DataAccess.MessagesModels.PlacesModels;
using DataAccess.MessagesModels.WaterModels;

namespace DataAccess.Helpers;

public class AddressHelper
{
    private static string mapboxToken = Environment.GetEnvironmentVariable("API_WATER_TOKEN");
    private static readonly HttpClient client = new HttpClient();

    static AddressHelper()
    {
        client.DefaultRequestHeaders.Add("User-Agent", "BarbarikiApp/1.0");
        
        client.Timeout = TimeSpan.FromSeconds(5);
    }

    public static async Task<bool> ValidateLocation(double longitude, double latitude)
    {
        
        if (longitude < -79.9 || longitude > -71.7 || latitude < 40.4 || latitude > 45.1)
        {
            return true; 
        }
        
        client.DefaultRequestHeaders.Add("User-Agent", "BarbarikiApp/1.0");
        
        string lon = longitude.ToString(CultureInfo.InvariantCulture);
        string lat = latitude.ToString(CultureInfo.InvariantCulture);
    
        string PlacesUrl =
            $"https://api.mapbox.com/geocoding/v5/mapbox.places/{lon},{lat}.json?access_token={mapboxToken}";
        string WaterUrl =
            $"https://api.mapbox.com/v4/mapbox.mapbox-streets-v8/tilequery/{lon},{lat}.json?access_token={mapboxToken}&limit=10";

        var dataPlaces = await client.GetFromJsonAsync<MBResponsePlaces>(PlacesUrl);
        var dataWater = await client.GetFromJsonAsync<MBResponseWater>(WaterUrl);

        if (dataWater == null || dataPlaces == null || dataPlaces.features.Count == 0)
        {
            return true;
        }

        bool isWater = false;

        if (dataWater.features.Count != 0)
        {
            foreach (var item in dataWater.features)
            {
                if (item.properties.tilequery.layer == "water")
                {
                    isWater = true;
                }
            }
        }

        var region = dataPlaces.features[0].context.Find(x => x.Id.Contains("region"));
        
        if (region == null)
        {
            return true;
        }
        
        return region.Text != "New York" || isWater;
    }
}