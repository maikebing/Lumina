param(
    [string]$PublishDir = "..\publish",
    [string]$OutputDir = "dist",
    [string]$Version = "0.1.0.0",
    [string]$CertThumbprint = ""
)

$ErrorActionPreference = "Stop"

# Prepare the staging directory used by makeappx.
$pkgDir = Join-Path $OutputDir "pkg"
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}
if (Test-Path $pkgDir) {
    Remove-Item $pkgDir -Recurse -Force
}

New-Item -ItemType Directory -Path $pkgDir | Out-Null
New-Item -ItemType Directory -Path (Join-Path $pkgDir "Assets") | Out-Null

# Copy the published app payload.
Copy-Item (Join-Path $PublishDir "Lumina.App.exe") $pkgDir
if (Test-Path (Join-Path $PublishDir "lang")) {
    Copy-Item (Join-Path $PublishDir "lang") (Join-Path $pkgDir "lang") -Recurse
}

# Include repository licensing notices in the packaged app payload.
Copy-Item "..\LICENSE" (Join-Path $pkgDir "LICENSE.txt")
Copy-Item "..\THIRD_PARTY_NOTICES.md" $pkgDir

# Copy the manifest template.
Copy-Item "Package.appxmanifest" $pkgDir

# Generate the required PNG assets from the SVG sources.
$svgSmall = Resolve-Path "Assets\lumina-small.svg"
$svgIcon = Resolve-Path "Assets\lumina-glass.svg"
$svgWide = Resolve-Path "Assets\lumina-wide.svg"
$svgSplash = Resolve-Path "Assets\lumina-splash.svg"

$inkscape = Get-Command "inkscape" -ErrorAction SilentlyContinue
if (-not $inkscape) {
    $inkscape = Get-Command "C:\Program Files\Inkscape\bin\inkscape.exe" -ErrorAction SilentlyContinue
}

if (-not $inkscape) {
    throw "Inkscape was not found. Install Inkscape or provide the generated PNG assets manually."
}

$ink = $inkscape.Source

function Export-Svg {
    param(
        [string]$Svg,
        [string]$Out,
        [int]$W,
        [int]$H
    )

    & $ink --export-type=png --export-filename="$Out" -w $W -h $H "$Svg" 2>$null
    if (-not (Test-Path $Out)) {
        throw "Inkscape export failed: $Out"
    }
}

Export-Svg $svgSmall (Join-Path $pkgDir "Assets\Square44x44Logo.png") 44 44
Export-Svg $svgSmall (Join-Path $pkgDir "Assets\StoreLogo.png") 50 50
Export-Svg $svgIcon (Join-Path $pkgDir "Assets\Square150x150Logo.png") 150 150
Export-Svg $svgWide (Join-Path $pkgDir "Assets\Wide310x150Logo.png") 310 150
Export-Svg $svgSplash (Join-Path $pkgDir "Assets\SplashScreen.png") 620 300

Write-Host "Assets generated from SVG sources"

# Build the MSIX package.
$trimmedVersion = $Version.TrimEnd(".0").TrimEnd(".")
$msixPath = Join-Path $OutputDir "Lumina-$trimmedVersion-x64.msix"
makeappx pack /d $pkgDir /p $msixPath /nv
Write-Host "MSIX created: $msixPath"

if ($CertThumbprint) {
    signtool sign /sha1 $CertThumbprint /fd SHA256 /tr http://timestamp.digicert.com /td SHA256 $msixPath
    Write-Host "Signed: $msixPath"
}
else {
    Write-Warning "No certificate thumbprint provided - MSIX is unsigned. Use -CertThumbprint to sign."
}
