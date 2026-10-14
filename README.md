# Unit Conversion API

This project implements a configurable unit conversion API using ASP.NET Core 10. It allows callers to convert numeric values between measurement units across multiple categories such as length, temperature, weight, area, volume, time, speed, pressure, energy, power, and data.

## Solution overview

The application follows a clean layered design:

- Controller layer handles HTTP requests
- Service layer contains conversion logic
- Configuration file drives supported categories and units
- Dependency injection keeps the app extensible and testable
- Runtime config updates are persisted to the JSON catalog without requiring code changes

This design fits the interview requirement to support an arbitrary number of units and categories without changing application code for each new conversion.

## Project structure

```text
InterviewAT/
├── README.md
├── UnitConversionAPI-LocalSmokeTest.ps1
├── UnitConversionAPI/
│   ├── ConversionConfigs.json
│   ├── Program.cs
│   ├── Models.cs
│   ├── UnitConversionAPI.csproj
│   ├── Controllers/
│   ├── Services/
│   └── wwwroot/swagger/
├── UnitConversionAPI.Tests/
│   └── UnitConversionServiceTests.cs
└── .gitignore
```

## Supported categories

The conversion catalog is defined in [UnitConversionAPI/ConversionConfigs.json](UnitConversionAPI/ConversionConfigs.json). The current configuration includes categories such as:

- Length
- Temperature
- Weight
- Area
- Volume
- Time
- Speed
- Pressure
- Energy
- Power
- Data

## API contract

### Conversion endpoints

```http
GET /api/unitconversion/convert?category=Length&fromUnit=meter&toUnit=feet&value=10
```

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

### Configuration management endpoints

```http
GET /api/unitconversion/config
```

```http
POST /api/unitconversion/config/categories
Content-Type: application/json

{
  "name": "Area",
  "configuration": {
    "units": {
      "square_meter": 1,
      "square_foot": 0.09290304
    }
  }
}
```

```http
POST /api/unitconversion/config/categories/Area/units
Content-Type: application/json

{
  "name": "square_yard",
  "factor": 0.83612736
}
```

```http
DELETE /api/unitconversion/config/categories/Area
```

```http
DELETE /api/unitconversion/config/categories/Area/units/square_yard
```

## Local run instructions

From the repo root, start the API with:

```bash
dotnet run --project UnitConversionAPI --urls http://localhost:5002
```

The app will start locally at:

```text
http://localhost:5002
```

Swagger UI is enabled in Development mode, so you can open:

```text
http://localhost:5002/swagger/
```

to test the endpoints through a browser-based UI.

## Smoke test script

A PowerShell smoke-test script is included at the repo root:

- [UnitConversionAPI-LocalSmokeTest.ps1](UnitConversionAPI-LocalSmokeTest.ps1)

You can run it with:

```powershell
powershell -ExecutionPolicy Bypass -File .\UnitConversionAPI-LocalSmokeTest.ps1
```

It exercises the conversion and config endpoints to validate the API is running correctly.

## Validation performed

The following conversion checks were executed against the running local API and returned the expected values:

```text
Length meter -> foot: 32.80839895013123
Length kilometer -> mile: 3.1068559611866697
Temperature celsius -> fahrenheit: 77
Temperature fahrenheit -> celsius: 25
Weight kg -> pound: 4.409245243697551
Weight pound -> kg: 1.9999998894606503
Area square_meter -> square_foot: 107.63910416709722
Volume liter -> gallon_us: 1.3208602617907421
Time hour -> minute: 120
Speed mile_per_hour -> meter_per_second: 26.8224
Pressure bar -> psi: 29.007547546043366
Energy kilojoule -> calorie: 239.0057361376673
Power kilowatt -> watt: 3000
Data megabyte -> kilobyte: 2048
```

These results confirm that the API’s conversion engine is working for all supported categories in the current configuration.

## Test status

The unit tests are implemented in [UnitConversionAPI.Tests/UnitConversionServiceTests.cs](UnitConversionAPI.Tests/UnitConversionServiceTests.cs). They validate core conversion behavior for length, temperature, and weight scenarios.

Validation command:

```bash
dotnet test UnitConversionAPI.Tests\UnitConversionAPI.Tests.csproj --nologo
```

Current result: 5 passed, 0 failed.

## Design decisions and tradeoffs

### 1. Configuration-first design
The conversion rules are loaded from JSON instead of hardcoding them in C#. This keeps the solution flexible and makes it easy to add new units without a code deployment.

Tradeoff: this is extremely flexible, but it depends on valid configuration data being present and consistent.

### 2. Interface-based service layer
A dedicated service interface and implementation are used for conversion logic. This makes the application easier to test and easier to extend.

Tradeoff: a bit more structure than a single controller method, but much cleaner for larger systems.

### 3. Formula support for non-linear conversions
Temperature conversions, such as Celsius/Fahrenheit, are handled with configurable formulas. This is more expressive than a simple unit multiplier and supports cases where the relationship is not a direct ratio.

Tradeoff: formula parsing is intentionally simple and safe for interview/demo scenarios, not a full expression engine.

### 4. Local-first development setup
The app is built for local execution and uses a simple JSON-driven configuration rather than a database or external service.

Tradeoff: it is perfect for a coding interview and local demos, but not yet optimized for enterprise-scale multi-tenant configuration management.

## Notes

This implementation is intentionally lightweight and pragmatic: it satisfies the conversion challenge while staying easy to explain, run locally, and extend in a real interview or demo scenario.
