using System.Text.Json.Serialization;

namespace DataAccess.MessagesModels.PlacesModels;

public class MBResponsePlaces
{
    [JsonPropertyName("type")] public string type { get; set; }
    [JsonPropertyName("query")] public double[] query { get; set; }
    [JsonPropertyName("features")] public List<Feature> features { get; set; }
}

public class Feature
{
    [JsonPropertyName("place_name")] public string PlaceName { get; set; }

    [JsonPropertyName("context")] public List<Context> context { get; set; }
}

public class Context
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("text")] public string Text { get; set; }
}