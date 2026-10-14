using Microsoft.AspNetCore.Mvc;

namespace UnitConversionAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitConversionController : ControllerBase
{
    private readonly IUnitConversionService _unitConversionService;

    public UnitConversionController(IUnitConversionService unitConversionService)
    {
        _unitConversionService = unitConversionService ?? throw new ArgumentNullException(nameof(unitConversionService));
    }

    [HttpGet("convert")]
    public ActionResult<ConversionResponse> Convert(
        [FromQuery] double value,
        [FromQuery] string category,
        [FromQuery] string fromUnit,
        [FromQuery] string toUnit)
    {
        try
        {
            var response = _unitConversionService.Convert(value, category, fromUnit, toUnit);
            return Ok(new ConversionResponse
            {
                OriginalValue = value,
                Category = category,
                FromUnit = fromUnit,
                ToUnit = toUnit,
                ConvertedValue = response
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("convert")]
    public ActionResult<ConversionResponse> Convert([FromBody] ConversionRequest request)
    {
        try
        {
            return Ok(_unitConversionService.Convert(request));
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
