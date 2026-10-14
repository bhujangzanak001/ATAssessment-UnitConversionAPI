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

    [HttpGet("config")]
    public ActionResult<UnitConversionConfiguration> GetConfiguration()
    {
        return Ok(_unitConversionService.GetConfiguration());
    }

    [HttpPost("config/categories")]
    public ActionResult<UnitConversionConfiguration> AddOrUpdateCategory([FromBody] AddCategoryRequest request)
    {
        try
        {
            _unitConversionService.AddOrUpdateCategory(request.Name, request.Configuration);
            return Ok(_unitConversionService.GetConfiguration());
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("config/categories/{categoryName}")]
    public ActionResult<UnitConversionConfiguration> UpdateCategory(string categoryName, [FromBody] UnitCategoryConfiguration configuration)
    {
        try
        {
            _unitConversionService.AddOrUpdateCategory(categoryName, configuration);
            return Ok(_unitConversionService.GetConfiguration());
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("config/categories/{categoryName}")]
    public ActionResult<UnitConversionConfiguration> DeleteCategory(string categoryName)
    {
        try
        {
            _unitConversionService.DeleteCategory(categoryName);
            return Ok(_unitConversionService.GetConfiguration());
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("config/categories/{categoryName}/units")]
    public ActionResult<UnitConversionConfiguration> AddOrUpdateUnit(string categoryName, [FromBody] AddUnitRequest request)
    {
        try
        {
            _unitConversionService.AddOrUpdateUnit(categoryName, request.Name, request.Factor);
            return Ok(_unitConversionService.GetConfiguration());
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("config/categories/{categoryName}/units/{unitName}")]
    public ActionResult<UnitConversionConfiguration> UpdateUnit(string categoryName, string unitName, [FromBody] AddUnitRequest request)
    {
        try
        {
            _unitConversionService.AddOrUpdateUnit(categoryName, unitName, request.Factor);
            return Ok(_unitConversionService.GetConfiguration());
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("config/categories/{categoryName}/units/{unitName}")]
    public ActionResult<UnitConversionConfiguration> DeleteUnit(string categoryName, string unitName)
    {
        try
        {
            _unitConversionService.DeleteUnit(categoryName, unitName);
            return Ok(_unitConversionService.GetConfiguration());
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
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
