namespace UnitConversionAPI;

/// <summary>
/// Defines the conversion service contract used by the controller layer.
/// The service is responsible for performing unit conversion and maintaining the
/// runtime configuration that drives the supported categories and formulas.
/// </summary>
public interface IUnitConversionService
{
    /// <summary>
    /// Converts a request object that contains category and unit details.
    /// </summary>
    ConversionResponse Convert(ConversionRequest request);

    /// <summary>
    /// Converts a numeric value between two units within the specified category.
    /// </summary>
    double Convert(double value, string category, string fromUnit, string toUnit);

    /// <summary>
    /// Returns the current in-memory configuration used by the API.
    /// </summary>
    UnitConversionConfiguration GetConfiguration();

    /// <summary>
    /// Adds or updates a category definition in the conversion catalog.
    /// </summary>
    void AddOrUpdateCategory(string categoryName, UnitCategoryConfiguration configuration);

    /// <summary>
    /// Deletes a category and persists the updated configuration.
    /// </summary>
    void DeleteCategory(string categoryName);

    /// <summary>
    /// Adds or updates a unit factor under a category.
    /// </summary>
    void AddOrUpdateUnit(string categoryName, string unitName, double factor);

    /// <summary>
    /// Removes a unit from a configured category.
    /// </summary>
    void DeleteUnit(string categoryName, string unitName);

    /// <summary>
    /// Persists the current configuration back to the JSON file.
    /// </summary>
    void SaveConfiguration();
}
