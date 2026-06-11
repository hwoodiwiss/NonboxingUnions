#! /usr/bin/env pwsh
#Requires -Version 7.0
#Requires -PSEdition Core

param (
    [string] $Configuration = "Release",
    [string] $OutputPath = (Join-Path $PSScriptRoot "artifacts"),
    [switch] $SkipTests = $false
)

$testProjectPaths = @(
    "tests/NonboxingUnion.Tests/NonboxingUnion.Tests.csproj",
    "tests/NonboxingUnion.Generator.IncrementalTests/NonboxingUnion.Generator.IncrementalTests.csproj"
)

$packageProjectPaths = @(
    "src/NonboxingUnion/NonboxingUnion.csproj"
)

if (-not $SkipTests) {
    foreach ($testProjectPath in $testProjectPaths) {
        dotnet test $testProjectPath --configuration $Configuration

        if ($LASTEXITCODE -ne 0) {
            throw "dotnet test failed with exit code $LASTEXITCODE"
        }
    }
}

foreach ($packageProjectPath in $packageProjectPaths) {
    dotnet pack $packageProjectPath --configuration $Configuration

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet pack failed with exit code $LASTEXITCODE"
    }
}
