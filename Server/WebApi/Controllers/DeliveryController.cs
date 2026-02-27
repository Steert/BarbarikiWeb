using BusinessLogic;
using DataAccess.Helpers;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

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
    public async Task<ActionResult<PagedResponse<Delivery>>> GetOrders(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize > 100) pageSize = 100;

        var result = await deliveryService.GetByPage(pageNumber, pageSize, ct);
        return Ok(result);
    }

    [HttpGet("Tax")]
    public async Task<bool> GetTax(double longitude, double latitude)
    {
        bool okay = false;
        JurisdictionLookupService.GetJurisdiction(longitude, latitude, out okay);
        return okay;
    }
}