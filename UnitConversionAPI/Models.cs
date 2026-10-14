namespace UnitConversionAPI;

public class ConversionRequest
{
    public double Value { get; set; }
    public string FromUnit { get; set; } = string.Empty;
    public string ToUnit { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // e.g., Length, Temperature, Weight
}

public class ConversionResponse
{
    public double OriginalValue { get; set; }
    public string FromUnit { get; set; } = string.Empty;
    public string ToUnit { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public double ConvertedValue { get; set; }
}

public class UnitConversionConfiguration
{
    public Dictionary<string, UnitCategoryConfiguration> Categories { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public class UnitCategoryConfiguration
{
    public Dictionary<string, double> Units { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> Conversions { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string? Formula { get; set; }
}


