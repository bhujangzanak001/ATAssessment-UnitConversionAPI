# Unit Conversion API

A configurable ASP.NET Core 10 Web API for converting values across measurement categories such as length, temperature, and weight/mass.

## Features
- REST API for conversion requests
- Configurable units and conversion formulas through JSON
- Support for multiple categories and units without code changes
- Dependency injection with a clean service interface and controller design

## Run locally

```bash
dotnet restore
cd UnitConversionAPI
dotnet run
```

The API will run on a local HTTP endpoint, typically:

```text
http://localhost:5000
```

## Example requests

### GET conversion

```http
GET /api/unitconversion/convert?category=Length&fromUnit=meter&toUnit=feet&value=10
```

### POST conversion

```http
POST /api/unitconversion/convert
Content-Type: application/json

{
  "value": 25,
  "category": "Temperature",
  "fromUnit": "celsius",
  "toUnit": "fahrenheit"
}
```

## Supported categories

The configuration file is in `UnitConversionAPI/ConversionConfigs.json` and supports custom categories and conversion rules.

Example categories included:
- Length
- Temperature
- Weight

## Notes

This API is intentionally designed so new categories and units can be added by updating the conversion config file instead of changing application code.
