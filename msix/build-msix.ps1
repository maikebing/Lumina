param(
    [string]$PublishDir = "..\publish",
    [string]$OutputDir = "dist",
    [string]$Version = "0.1.0.0",
    [string]$CertThumbprint = ""
)

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

function Resolve-ScriptPath {
    param([string]$Path)

    return [System.IO.Path]::GetFullPath((Join-Path $scriptDir $Path))
}

# Prepare the staging directory used by makeappx.
$publishDirPath = Resolve-ScriptPath $PublishDir
$outputDirPath = Resolve-ScriptPath $OutputDir
$pkgDir = Join-Path $outputDirPath "pkg"

if (-not (Test-Path $outputDirPath)) {
    New-Item -ItemType Directory -Path $outputDirPath | Out-Null
}
if (Test-Path $pkgDir) {
    Remove-Item $pkgDir -Recurse -Force
}

New-Item -ItemType Directory -Path $pkgDir | Out-Null
New-Item -ItemType Directory -Path (Join-Path $pkgDir "Assets") | Out-Null

# Copy the published app payload.
Copy-Item (Join-Path $publishDirPath "Lumina.App.exe") $pkgDir
if (Test-Path (Join-Path $publishDirPath "lang")) {
    Copy-Item (Join-Path $publishDirPath "lang") (Join-Path $pkgDir "lang") -Recurse
}

# Include repository licensing notices in the packaged app payload.
Copy-Item (Resolve-ScriptPath "..\LICENSE") (Join-Path $pkgDir "LICENSE.txt")
Copy-Item (Resolve-ScriptPath "..\THIRD_PARTY_NOTICES.md") $pkgDir

# Copy the manifest template.
Copy-Item (Resolve-ScriptPath "Package.appxmanifest") $pkgDir

# Generate the required PNG assets with the C# asset generator.
$generatorProject = Resolve-ScriptPath "GenAssets\GenAssets.csproj"
& dotnet run --project $generatorProject --configuration Release -- $pkgDir
if ($LASTEXITCODE -ne 0) {
    throw "C# asset generation failed."
}

Write-Host "Assets generated with GenAssets"

# Build the MSIX package.
$trimmedVersion = $Version.TrimEnd(".0").TrimEnd(".")
$msixPath = Join-Path $outputDirPath "Lumina-$trimmedVersion-x64.msix"
makeappx pack /d $pkgDir /p $msixPath /nv
Write-Host "MSIX created: $msixPath"

if ($CertThumbprint) {
    signtool sign /sha1 $CertThumbprint /fd SHA256 /tr http://timestamp.digicert.com /td SHA256 $msixPath
    Write-Host "Signed: $msixPath"
}
else {
    Write-Warning "No certificate thumbprint provided - MSIX is unsigned. Use -CertThumbprint to sign."
}
