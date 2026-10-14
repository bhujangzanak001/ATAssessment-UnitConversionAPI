namespace UnitConversionAPI;

public interface IUnitConversionService
{
    ConversionResponse Convert(ConversionRequest request);
    double Convert(double value, string category, string fromUnit, string toUnit);
}
