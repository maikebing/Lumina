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

# 拷贝 manifest
Copy-Item "Package.appxmanifest" $pkgDir

# 从专用 SVG 源文件生成 MSIX 所需资产图
$svgSmall  = Resolve-Path "Assets\lumina-small.svg"   # 44/50px 极简图标
$svgIcon   = Resolve-Path "Assets\lumina-glass.svg"   # 150px 标准图标
$svgWide   = Resolve-Path "Assets\lumina-wide.svg"    # 310x150 宽图标
$svgSplash = Resolve-Path "Assets\lumina-splash.svg"  # 620x300 启动屏

# 检测 Inkscape
$inkscape = Get-Command inkscape -ErrorAction SilentlyContinue
if (-not $inkscape) {
    $inkscape = Get-Command "C:\Program Files\Inkscape\bin\inkscape.exe" -ErrorAction SilentlyContinue
}
if (-not $inkscape) {
    throw "未找到 Inkscape，请安装 Inkscape 并确保其在 PATH 中，或手动提供 Assets PNG 文件。"
}
$ink = $inkscape.Source

function Export-Svg {
    param([string]$Svg, [string]$Out, [int]$W, [int]$H)
    & $ink --export-type=png --export-filename="$Out" -w $W -h $H "$Svg" 2>$null
    if (-not (Test-Path $Out)) { throw "Inkscape 导出失败: $Out" }
}

# 小尺寸图标（极简版，细节清晰）
Export-Svg $svgSmall "$pkgDir\Assets\Square44x44Logo.png" 44  44
Export-Svg $svgSmall "$pkgDir\Assets\StoreLogo.png"       50  50

# 标准方形图标
Export-Svg $svgIcon  "$pkgDir\Assets\Square150x150Logo.png" 150 150

# 宽图标（图标+文字横向布局）
Export-Svg $svgWide  "$pkgDir\Assets\Wide310x150Logo.png"   310 150

# 启动屏（完整品牌画面）
Export-Svg $svgSplash "$pkgDir\Assets\SplashScreen.png"     620 300

Write-Host "Assets generated from SVG sources"

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
