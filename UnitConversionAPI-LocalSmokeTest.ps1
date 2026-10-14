$ErrorActionPreference = 'Stop'

$BaseUrl = 'http://localhost:5002'

function Invoke-JsonRequest {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Method,

        [Parameter(Mandatory = $true)]
        [string]$Uri,

        [object]$Body = $null,

        [hashtable]$Headers = @{}
    )

    $requestHeaders = @{
        'Content-Type' = 'application/json'
    }

    foreach ($key in $Headers.Keys) {
        $requestHeaders[$key] = $Headers[$key]
    }

    if ($null -eq $Body) {
        return Invoke-RestMethod -Method $Method -Uri $Uri -Headers $requestHeaders
    }

    $jsonBody = $Body | ConvertTo-Json -Depth 10
    return Invoke-RestMethod -Method $Method -Uri $Uri -Headers $requestHeaders -Body $jsonBody
}

Write-Host '=== GET config ==='
$config = Invoke-JsonRequest -Method 'GET' -Uri "$BaseUrl/api/unitconversion/config"
$config | ConvertTo-Json -Depth 10

Write-Host "`n=== GET conversion: 10 meters to feet ==="
$lengthConversion = Invoke-JsonRequest -Method 'GET' -Uri "$BaseUrl/api/unitconversion/convert?category=Length&fromUnit=meter&toUnit=foot&value=10"
$lengthConversion | ConvertTo-Json -Depth 10

Write-Host "`n=== POST conversion: 25 Celsius to Fahrenheit ==="
$postBody = @{
    value = 25
    category = 'Temperature'
    fromUnit = 'celsius'
    toUnit = 'fahrenheit'
}
$postConversion = Invoke-JsonRequest -Method 'POST' -Uri "$BaseUrl/api/unitconversion/convert" -Body $postBody
$postConversion | ConvertTo-Json -Depth 10

Write-Host "`n=== POST create category: DemoCategory ==="
$createCategoryBody = @{
    name = 'DemoCategory'
    configuration = @{
        units = @{
            meter = 1
            foot = 0.3048
        }
    }
}
$createCategory = Invoke-JsonRequest -Method 'POST' -Uri "$BaseUrl/api/unitconversion/config/categories" -Body $createCategoryBody
$createCategory | ConvertTo-Json -Depth 10

Write-Host "`n=== POST add unit: yard to DemoCategory ==="
$addUnitBody = @{
    name = 'yard'
    factor = 0.9144
}
$addUnit = Invoke-JsonRequest -Method 'POST' -Uri "$BaseUrl/api/unitconversion/config/categories/DemoCategory/units" -Body $addUnitBody
$addUnit | ConvertTo-Json -Depth 10

Write-Host "`n=== DELETE DemoCategory ==="
$deleteCategory = Invoke-JsonRequest -Method 'DELETE' -Uri "$BaseUrl/api/unitconversion/config/categories/DemoCategory"
$deleteCategory | ConvertTo-Json -Depth 10

Write-Host "`n=== Final config ==="
$finalConfig = Invoke-JsonRequest -Method 'GET' -Uri "$BaseUrl/api/unitconversion/config"
$finalConfig | ConvertTo-Json -Depth 10

Write-Host "`n=== Smoke test completed successfully ==="
