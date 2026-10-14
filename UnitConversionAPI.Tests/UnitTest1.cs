using UnitConversionAPI;

namespace UnitConversionAPI.Tests;

public class UnitConversionServiceTests
{
    private static UnitConversionService CreateService()
    {
        var config = new UnitConversionConfiguration
        {
            Categories = new Dictionary<string, UnitCategoryConfiguration>
            {
                ["Length"] = new()
                {
                    Units = new Dictionary<string, double>
                    {
                        ["meter"] = 1.0,
                        ["feet"] = 0.3048
                    }
                },
                ["Temperature"] = new()
                {
                    Units = new Dictionary<string, double>
                    {
                        ["celsius"] = 1.0,
                        ["fahrenheit"] = 0.5555555555555556
                    },
                    Formula = "(value * 9 / 5) + 32"
                },
                ["Weight"] = new()
                {
                    Units = new Dictionary<string, double>
                    {
                        ["kg"] = 1.0,
                        ["pound"] = 0.45359237
                    }
                }
            }
        };

        return new UnitConversionService(config);
    }

    [Fact]
    public void Convert_WhenLengthUnitsAreProvided_ReturnsConvertedValue()
    {
        var service = CreateService();

        var result = service.Convert(10, "Length", "meter", "feet");

        Assert.Equal(32.80839895013123, result, 12);
    }

    [Fact]
    public void Convert_WhenTemperatureUnitsAreProvided_ReturnsConvertedValue()
    {
        var service = CreateService();

        var result = service.Convert(25, "Temperature", "celsius", "fahrenheit");

        Assert.Equal(77, result, 10);
    }

    [Fact]
    public void Convert_WhenWeightUnitsAreProvided_ReturnsConvertedValue()
    {
        var service = CreateService();

        var result = service.Convert(5, "Weight", "kg", "pound");

        Assert.Equal(11.023113109, result, 9);
    }

    [Fact]
    public void Convert_WhenUnitIsNotConfigured_ThrowsArgumentException()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() => service.Convert(1, "Length", "yard", "meter"));
    }
}
