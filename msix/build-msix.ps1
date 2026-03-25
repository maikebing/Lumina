# 构建 MSIX 包
# 依赖: Windows SDK (makeappx.exe, signtool.exe) 在 PATH 中
# 用法: .\build-msix.ps1 [-CertThumbprint <thumbprint>]
param(
    [string]$PublishDir  = "..\publish",
    [string]$OutputDir   = "dist",
    [string]$Version     = "0.1.0.0",
    [string]$CertThumbprint = ""
)

$ErrorActionPreference = "Stop"

# 准备打包目录
$pkgDir = "$OutputDir\pkg"
if (Test-Path $pkgDir) { Remove-Item $pkgDir -Recurse -Force }
New-Item -ItemType Directory -Path $pkgDir | Out-Null
New-Item -ItemType Directory -Path "$pkgDir\Assets" | Out-Null

# 拷贝可执行文件
Copy-Item "$PublishDir\Lumina.App.exe" $pkgDir
if (Test-Path "$PublishDir\lang") {
    Copy-Item "$PublishDir\lang" "$pkgDir\lang" -Recurse
}

# 拷贝 manifest 和占位资产
Copy-Item "Package.appxmanifest" $pkgDir

# 生成占位资产图（如果真实图片不存在）
$assetSizes = @{
    "StoreLogo.png"         = "50x50"
    "Square44x44Logo.png"   = "44x44"
    "Square150x150Logo.png" = "150x150"
    "Wide310x150Logo.png"   = "310x150"
    "SplashScreen.png"      = "620x300"
}
$realAssetsDir = "Assets"
foreach ($asset in $assetSizes.Keys) {
    $src = Join-Path $realAssetsDir $asset
    if (Test-Path $src) {
        Copy-Item $src "$pkgDir\Assets\$asset"
    } else {
        Write-Warning "Asset not found: $src — copy a real PNG before submitting to Store."
        # 创建 1x1 占位 PNG（仅用于本地测试）
        $bytes = [byte[]](0x89,0x50,0x4E,0x47,0x0D,0x0A,0x1A,0x0A,0x00,0x00,0x00,0x0D,0x49,0x48,0x44,0x52,
                          0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x01,0x08,0x02,0x00,0x00,0x00,0x90,0x77,0x53,
                          0xDE,0x00,0x00,0x00,0x0C,0x49,0x44,0x41,0x54,0x08,0xD7,0x63,0xF8,0xCF,0xC0,0x00,
                          0x00,0x00,0x02,0x00,0x01,0xE2,0x21,0xBC,0x33,0x00,0x00,0x00,0x00,0x49,0x45,0x4E,
                          0x44,0xAE,0x42,0x60,0x82)
        [IO.File]::WriteAllBytes("$pkgDir\Assets\$asset", $bytes)
    }
}

# 构建 MSIX
if (-not (Test-Path $OutputDir)) { New-Item -ItemType Directory -Path $OutputDir | Out-Null }
$msixPath = "$OutputDir\Lumina-$($Version.TrimEnd('.0').TrimEnd('.'))-x64.msix"
makeappx pack /d $pkgDir /p $msixPath /nv
Write-Host "MSIX created: $msixPath"

# 签名（可选，仅当提供了证书指纹时）
if ($CertThumbprint) {
    signtool sign /sha1 $CertThumbprint /fd SHA256 /tr http://timestamp.digicert.com /td SHA256 $msixPath
    Write-Host "Signed: $msixPath"
} else {
    Write-Warning "No certificate thumbprint provided — MSIX is unsigned. Use -CertThumbprint to sign."
}
