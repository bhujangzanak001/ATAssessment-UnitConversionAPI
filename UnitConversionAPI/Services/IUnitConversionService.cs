namespace UnitConversionAPI;

public interface IUnitConversionService
{
    ConversionResponse Convert(ConversionRequest request);
    double Convert(double value, string category, string fromUnit, string toUnit);
    UnitConversionConfiguration GetConfiguration();
    void AddOrUpdateCategory(string categoryName, UnitCategoryConfiguration configuration);
    void DeleteCategory(string categoryName);
    void AddOrUpdateUnit(string categoryName, string unitName, double factor);
    void DeleteUnit(string categoryName, string unitName);
    void SaveConfiguration();
}
