using Microsoft.AspNetCore.Mvc;

namespace UnitConversionAPI.Controllers;

/// <summary>
/// HTTP controller for conversion execution and configuration management.
/// This layer exposes the public API endpoints used by clients and external tooling.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UnitConversionController : ControllerBase
{
    private readonly IUnitConversionService _unitConversionService;

    /// <summary>
    /// Creates a controller instance and injects the conversion service.
    /// </summary>
    public UnitConversionController(IUnitConversionService unitConversionService)
    {
        _unitConversionService = unitConversionService ?? throw new ArgumentNullException(nameof(unitConversionService));
    }

    /// <summary>
    /// Retrieves the current conversion catalog as configured at runtime.
    /// </summary>
    [HttpGet("config")]
    public ActionResult<UnitConversionConfiguration> GetConfiguration()
    {
        return Ok(_unitConversionService.GetConfiguration());
    }

    /// <summary>
    /// Adds or creates a category definition.
    /// </summary>
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

    /// <summary>
    /// Updates an existing category definition.
    /// </summary>
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

    /// <summary>
    /// Removes a category definition and its associated units.
    /// </summary>
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

    /// <summary>
    /// Adds a unit definition to an existing category.
    /// </summary>
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

    /// <summary>
    /// Updates the configuration for a given unit within a category.
    /// </summary>
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

    /// <summary>
    /// Deletes a unit definition from a category.
    /// </summary>
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

    /// <summary>
    /// Converts a numeric value using query-string parameters.
    /// This overload is useful for simple browser-based or manual testing.
    /// </summary>
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

    /// <summary>
    /// Converts a value using a typed request object.
    /// This is the main JSON-based contract for API clients.
    /// </summary>
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
