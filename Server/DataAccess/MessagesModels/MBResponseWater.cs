using System.Text.Json.Serialization;

namespace DataAccess.MessagesModels.WaterModels;

public class MBResponseWater
{
    [JsonPropertyName("features")] public List<FeaturesWater> features { get; set; }
}

public class FeaturesWater
{
    [JsonPropertyName("properties")] public PropertiesWater properties { get; set; } = new();
}

public class PropertiesWater
{
    [JsonPropertyName("tilequery")] public TilequeryWater tilequery { get; set; } = new();
}

public class TilequeryWater
{
    [JsonPropertyName("layer")] public string layer { get; set; }
}