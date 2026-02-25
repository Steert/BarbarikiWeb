using System.Globalization;
using BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestClient = Rest.RestClient;

namespace WebApi.Controllers;

public record OrderRequest(double longitude, double latitude, double subtotal);

[ApiController]
[Route("orders")]
public class DeliveryController(IDeliveryService deliveryService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] OrderRequest request)
    {
        await deliveryService.CreateAsync(request.longitude, request.latitude, request.subtotal);
        return NoContent();
    }

    [HttpPost("import")]
    public async Task<IActionResult> import(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Empty file");

        if (!file.FileName.EndsWith(".csv"))
            return BadRequest("Invalid file format");
        
        using Stream stream = file.OpenReadStream();
        await deliveryService.ImportAsync(stream);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var collection = await deliveryService.GetAllAsync();
        return Ok(collection);
    }
    
    [HttpGet("tax")]
    public async Task<string> GetAddress(double lat, double lng, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
    
        client.DefaultRequestHeaders.Add("User-Agent", "BarbarikiWeb_App_v1.0");

        string url = string.Format(CultureInfo.InvariantCulture, 
            "https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={0}&lon={1}&zoom=18", 
            lat, lng);

        try 
        {
            var response = await client.GetAsync(url);
        
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<OsmResponse>();
                return result?.DisplayName ?? "Адрес не найден";
            }
        }
        catch (Exception ex)
        {
            return $"Ошибка запроса: {ex.Message}";
        }

        return "Сервис OSM недоступен";

    }
}
public class OsmResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("display_name")]
    public string DisplayName { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("address")]
    public Dictionary<string, string> Address { get; set; }
}