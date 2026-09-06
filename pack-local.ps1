Write-Host "CKN.SDK Local NuGet Packaging Script" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan

$outputDir = ".\artifacts\packages"

if (Test-Path $outputDir) {
    Write-Host "Cleaning old artifacts..." -ForegroundColor Yellow
    Remove-Item -Path "$outputDir\*" -Force -Recurse
} else {
    New-Item -ItemType Directory -Force -Path $outputDir | Out-Null
}

Write-Host "Restoring dependencies..." -ForegroundColor Green
dotnet restore CKN.SDK.slnx

Write-Host "Building solution in Release mode..." -ForegroundColor Green
dotnet build CKN.SDK.slnx -c Release --no-restore

Write-Host "Running tests..." -ForegroundColor Green
dotnet test CKN.SDK.slnx -c Release --no-build

if ($LASTEXITCODE -ne 0) {
    Write-Host "Tests failed! Packaging aborted." -ForegroundColor Red
    exit 1
}

Write-Host "Packing projects to $outputDir..." -ForegroundColor Green
dotnet pack CKN.SDK.slnx -c Release --no-build -o $outputDir

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "Packaging completed successfully!" -ForegroundColor Green
Write-Host "You can find your .nupkg files in the artifacts/packages folder." -ForegroundColor Yellow
