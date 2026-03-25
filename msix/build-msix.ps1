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

# 从 lumina-glass.svg 生成 MSIX 所需资产图
# 方形图标：直接渲染 SVG；宽图/启动屏：SVG 居中放置在对应尺寸画布上
$svgIcon   = Resolve-Path "Assets\lumina-glass.svg"

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

# 方形图标（直接从 SVG 导出）
Export-Svg $svgIcon "$pkgDir\Assets\StoreLogo.png"         50  50
Export-Svg $svgIcon "$pkgDir\Assets\Square44x44Logo.png"   44  44
Export-Svg $svgIcon "$pkgDir\Assets\Square150x150Logo.png" 150 150

# 宽图标 310x150：SVG 居中在深色背景上
$tmpWide = [IO.Path]::GetTempFileName() + ".svg"
@"
<svg xmlns="http://www.w3.org/2000/svg" width="310" height="150">
  <rect width="310" height="150" fill="#0c1445"/>
  <image href="$($svgIcon -replace '\\','/')"
         x="80" y="0" width="150" height="150"/>
</svg>
"@ | Set-Content $tmpWide -Encoding UTF8
Export-Svg $tmpWide "$pkgDir\Assets\Wide310x150Logo.png" 310 150
Remove-Item $tmpWide

# 启动屏 620x300：SVG 居中在深色背景上
$tmpSplash = [IO.Path]::GetTempFileName() + ".svg"
@"
<svg xmlns="http://www.w3.org/2000/svg" width="620" height="300">
  <rect width="620" height="300" fill="#0c1445"/>
  <image href="$($svgIcon -replace '\\','/')"
         x="185" y="25" width="250" height="250"/>
</svg>
"@ | Set-Content $tmpSplash -Encoding UTF8
Export-Svg $tmpSplash "$pkgDir\Assets\SplashScreen.png" 620 300
Remove-Item $tmpSplash

Write-Host "Assets generated from lumina-glass.svg"

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
