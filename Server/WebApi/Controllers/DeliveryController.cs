using BusinessLogic;
using Microsoft.AspNetCore.Mvc;

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
}