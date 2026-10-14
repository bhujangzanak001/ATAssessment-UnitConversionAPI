namespace UnitConversionAPI;

/// <summary>
/// Represents a conversion request sent to the API.
/// The client provides the source value, unit pair, and the category to resolve the conversion.
/// </summary>
public class ConversionRequest
{
    public double Value { get; set; }
    public string FromUnit { get; set; } = string.Empty;
    public string ToUnit { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // e.g., Length, Temperature, Weight
}

/// <summary>
/// Represents the result of a conversion operation.
/// </summary>
public class ConversionResponse
{
    public double OriginalValue { get; set; }
    public string FromUnit { get; set; } = string.Empty;
    public string ToUnit { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public double ConvertedValue { get; set; }
}

/// <summary>
/// Root configuration object for the entire conversion catalog.
/// It contains one dictionary per category, such as Length, Weight, Temperature, and Time.
/// </summary>
public class UnitConversionConfiguration
{
    public Dictionary<string, UnitCategoryConfiguration> Categories { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

/// <summary>
/// Represents the definition of a single conversion category.
/// A category may expose units, explicit conversion rules, and a fallback formula.
/// </summary>
public class UnitCategoryConfiguration
{
    public Dictionary<string, double> Units { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> Conversions { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string? Formula { get; set; }
}

/// <summary>
/// Request payload for creating or updating a category through the API.
/// </summary>
public class AddCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public UnitCategoryConfiguration Configuration { get; set; } = new();
}

/// <summary>
/// Request payload for creating or updating a unit definition inside an existing category.
/// </summary>
public class AddUnitRequest
{
    public string Name { get; set; } = string.Empty;
    public double Factor { get; set; }
}


