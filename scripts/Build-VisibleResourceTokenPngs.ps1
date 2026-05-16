param(
    [string]$AssetRoot = (Join-Path $PSScriptRoot "..\Assets\resource_tokens"),
    [int[]]$Sizes = @(1024, 512, 256, 128, 64, 32),
    [double]$IconScale = 0.54,
    [string]$PrimaryIconColor = "#F3EBDD",
    [string]$SecondaryIconColor = "#E8DECC",
    [double]$PngDilationRadiusRatio = 0.006,
    [double]$ShadowAlpha = 0.18,
    [switch]$VerifyOnly
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Drawing

$resources = @("cash", "research", "credit", "commodity", "deal")
$baseDir = Join-Path $AssetRoot "base_normalized"
$baseFallbackDir = Join-Path $AssetRoot "base"
$iconDir = Join-Path $AssetRoot "icons"
$rawDir = Join-Path $AssetRoot "final_raw"
$previousFinalDir = Join-Path $AssetRoot "final"
$outputDir = Join-Path $AssetRoot "final_visible"
$contactSheetPath = Join-Path $outputDir "contact_sheet_before_after.png"
$largestSize = ($Sizes | Measure-Object -Maximum).Maximum

function New-DirectoryIfMissing {
    param([string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        New-Item -ItemType Directory -Path $Path | Out-Null
    }
}

function Convert-HexColor {
    param([string]$Hex)

    $value = $Hex.TrimStart("#")
    if ($value.Length -ne 6) {
        throw "Expected a 6-digit hex color, got $Hex"
    }

    return [System.Drawing.Color]::FromArgb(
        [Convert]::ToInt32($value.Substring(0, 2), 16),
        [Convert]::ToInt32($value.Substring(2, 2), 16),
        [Convert]::ToInt32($value.Substring(4, 2), 16)
    )
}

function Get-AlphaBounds {
    param([System.Drawing.Bitmap]$Bitmap)

    $minX = $Bitmap.Width
    $minY = $Bitmap.Height
    $maxX = -1
    $maxY = -1

    for ($y = 0; $y -lt $Bitmap.Height; $y++) {
        for ($x = 0; $x -lt $Bitmap.Width; $x++) {
            if ($Bitmap.GetPixel($x, $y).A -gt 8) {
                if ($x -lt $minX) { $minX = $x }
                if ($y -lt $minY) { $minY = $y }
                if ($x -gt $maxX) { $maxX = $x }
                if ($y -gt $maxY) { $maxY = $y }
            }
        }
    }

    if ($maxX -lt 0) {
        throw "Image has no visible alpha pixels."
    }

    return [System.Drawing.Rectangle]::new($minX, $minY, $maxX - $minX + 1, $maxY - $minY + 1)
}

function New-IconSilhouette {
    param(
        [System.Drawing.Bitmap]$Source,
        [System.Drawing.Rectangle]$Bounds,
        [System.Drawing.Color]$Color
    )

    $silhouette = [System.Drawing.Bitmap]::new($Bounds.Width, $Bounds.Height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)

    for ($y = 0; $y -lt $Bounds.Height; $y++) {
        for ($x = 0; $x -lt $Bounds.Width; $x++) {
            $alpha = $Source.GetPixel($Bounds.X + $x, $Bounds.Y + $y).A
            $silhouette.SetPixel($x, $y, [System.Drawing.Color]::FromArgb($alpha, $Color.R, $Color.G, $Color.B))
        }
    }

    return $silhouette
}

function New-ShadowSilhouette {
    param(
        [System.Drawing.Bitmap]$Icon,
        [double]$AlphaScale
    )

    $shadow = [System.Drawing.Bitmap]::new($Icon.Width, $Icon.Height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)

    for ($y = 0; $y -lt $Icon.Height; $y++) {
        for ($x = 0; $x -lt $Icon.Width; $x++) {
            $alpha = [int][Math]::Round($Icon.GetPixel($x, $y).A * $AlphaScale)
            $shadow.SetPixel($x, $y, [System.Drawing.Color]::FromArgb($alpha, 0, 0, 0))
        }
    }

    return $shadow
}

function Remove-NearWhiteBackground {
    param([System.Drawing.Bitmap]$Bitmap)

    $hasTransparentPixel = $false
    for ($y = 0; $y -lt $Bitmap.Height -and -not $hasTransparentPixel; $y++) {
        for ($x = 0; $x -lt $Bitmap.Width; $x++) {
            if ($Bitmap.GetPixel($x, $y).A -lt 255) {
                $hasTransparentPixel = $true
                break
            }
        }
    }

    if ($hasTransparentPixel) {
        return
    }

    for ($y = 0; $y -lt $Bitmap.Height; $y++) {
        for ($x = 0; $x -lt $Bitmap.Width; $x++) {
            $pixel = $Bitmap.GetPixel($x, $y)
            if ($pixel.R -ge 245 -and $pixel.G -ge 245 -and $pixel.B -ge 235) {
                $Bitmap.SetPixel($x, $y, [System.Drawing.Color]::FromArgb(0, $pixel.R, $pixel.G, $pixel.B))
            }
        }
    }
}

function Resize-Png {
    param(
        [string]$InputPath,
        [string]$OutputPath,
        [int]$Size
    )

    $source = $null
    $canvas = $null
    $graphics = $null

    try {
        $source = [System.Drawing.Bitmap]::new($InputPath)
        $canvas = [System.Drawing.Bitmap]::new($Size, $Size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $graphics = [System.Drawing.Graphics]::FromImage($canvas)
        Set-HighQualityGraphics -Graphics $graphics
        $graphics.Clear([System.Drawing.Color]::Transparent)
        $graphics.DrawImage($source, 0, 0, $Size, $Size)
        $canvas.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    }
    finally {
        if ($graphics -ne $null) { $graphics.Dispose() }
        if ($canvas -ne $null) { $canvas.Dispose() }
        if ($source -ne $null) { $source.Dispose() }
    }
}

function Set-HighQualityGraphics {
    param([System.Drawing.Graphics]$Graphics)

    $Graphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceOver
    $Graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $Graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $Graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $Graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
}

function Get-BasePath {
    param([string]$Resource)

    $normalizedPath = Join-Path $baseDir "${Resource}_base.png"
    if (Test-Path -LiteralPath $normalizedPath) {
        return $normalizedPath
    }

    $fallbackPath = Join-Path $baseFallbackDir "${Resource}_base.png"
    if (Test-Path -LiteralPath $fallbackPath) {
        return $fallbackPath
    }

    return $null
}

function Convert-SvgIconToPng {
    param(
        [string]$SvgPath,
        [string]$PngPath
    )

    $magick = Get-Command "magick" -ErrorAction SilentlyContinue
    if ($magick -ne $null) {
        & $magick.Source "-background" "none" "-resize" "2048x2048" $SvgPath $PngPath
        if ($LASTEXITCODE -eq 0 -and (Test-Path -LiteralPath $PngPath)) {
            return $PngPath
        }
    }

    $rsvg = Get-Command "rsvg-convert" -ErrorAction SilentlyContinue
    if ($rsvg -ne $null) {
        & $rsvg.Source "-w" "2048" "-h" "2048" "-o" $PngPath $SvgPath
        if ($LASTEXITCODE -eq 0 -and (Test-Path -LiteralPath $PngPath)) {
            return $PngPath
        }
    }

    throw "SVG icon found, but neither ImageMagick 'magick' nor 'rsvg-convert' is available to rasterize it: $SvgPath"
}

function Get-IconPath {
    param([string]$Resource)

    $svgPath = Join-Path $iconDir "${Resource}_icon.svg"
    if (Test-Path -LiteralPath $svgPath) {
        $tempPath = Join-Path ([System.IO.Path]::GetTempPath()) "asset-manager-${Resource}-icon.png"
        return Convert-SvgIconToPng -SvgPath $svgPath -PngPath $tempPath
    }

    $pngPath = Join-Path $iconDir "${Resource}_icon.png"
    if (Test-Path -LiteralPath $pngPath) {
        return $pngPath
    }

    return $null
}

function Compose-FromBaseAndIcon {
    param(
        [string]$Resource,
        [int]$Size,
        [System.Drawing.Color]$IconColor
    )

    $basePath = Get-BasePath -Resource $Resource
    $iconPath = Get-IconPath -Resource $Resource
    if ($basePath -eq $null -or $iconPath -eq $null) {
        return $false
    }

    $outputPath = Join-Path $outputDir "${Resource}_token_${Size}.png"
    $baseImage = $null
    $iconImage = $null
    $iconSilhouette = $null
    $shadowSilhouette = $null
    $canvas = $null
    $graphics = $null

    try {
        $baseImage = [System.Drawing.Bitmap]::new($basePath)
        $iconImage = [System.Drawing.Bitmap]::new($iconPath)

        if ($baseImage.Width -ne $baseImage.Height) {
            throw "Base image must be square: $basePath is $($baseImage.Width)x$($baseImage.Height)"
        }

        Remove-NearWhiteBackground -Bitmap $baseImage

        $bounds = Get-AlphaBounds -Bitmap $iconImage
        $iconSilhouette = New-IconSilhouette -Source $iconImage -Bounds $bounds -Color $IconColor
        $shadowSilhouette = New-ShadowSilhouette -Icon $iconSilhouette -AlphaScale $ShadowAlpha

        $targetIconWidth = [int][Math]::Round($Size * $IconScale)
        $targetIconHeight = [int][Math]::Round($targetIconWidth * ($iconSilhouette.Height / $iconSilhouette.Width))
        $iconX = [int][Math]::Round(($Size - $targetIconWidth) / 2)
        $iconY = [int][Math]::Round(($Size - $targetIconHeight) / 2)
        $shadowOffset = [Math]::Max(1, [int][Math]::Round($Size * 0.002))
        $weightRadius = [Math]::Max(1, [int][Math]::Round($Size * $PngDilationRadiusRatio))

        $canvas = [System.Drawing.Bitmap]::new($Size, $Size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $graphics = [System.Drawing.Graphics]::FromImage($canvas)
        Set-HighQualityGraphics -Graphics $graphics
        $graphics.Clear([System.Drawing.Color]::Transparent)
        $graphics.DrawImage($baseImage, 0, 0, $Size, $Size)
        $graphics.DrawImage($shadowSilhouette, $iconX + $shadowOffset, $iconY + $shadowOffset, $targetIconWidth, $targetIconHeight)

        for ($dy = -$weightRadius; $dy -le $weightRadius; $dy++) {
            for ($dx = -$weightRadius; $dx -le $weightRadius; $dx++) {
                if ($dx -eq 0 -and $dy -eq 0) {
                    continue
                }
                if (($dx * $dx + $dy * $dy) -gt ($weightRadius * $weightRadius)) {
                    continue
                }

                $graphics.DrawImage($iconSilhouette, $iconX + $dx, $iconY + $dy, $targetIconWidth, $targetIconHeight)
            }
        }

        $graphics.DrawImage($iconSilhouette, $iconX, $iconY, $targetIconWidth, $targetIconHeight)
        $canvas.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    }
    finally {
        if ($graphics -ne $null) { $graphics.Dispose() }
        if ($canvas -ne $null) { $canvas.Dispose() }
        if ($shadowSilhouette -ne $null) { $shadowSilhouette.Dispose() }
        if ($iconSilhouette -ne $null) { $iconSilhouette.Dispose() }
        if ($iconImage -ne $null) { $iconImage.Dispose() }
        if ($baseImage -ne $null) { $baseImage.Dispose() }
    }

    return $true
}

function Compose-FromRawToken {
    param(
        [string]$Resource,
        [int]$Size,
        [System.Drawing.Color]$IconColor
    )

    $rawPath = Join-Path $rawDir "${Resource}_token.png"
    if (-not (Test-Path -LiteralPath $rawPath)) {
        return $false
    }

    $outputPath = Join-Path $outputDir "${Resource}_token_${Size}.png"
    $source = $null
    $canvas = $null
    $graphics = $null

    try {
        $source = [System.Drawing.Bitmap]::new($rawPath)
        $canvas = [System.Drawing.Bitmap]::new($source.Width, $source.Height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)

        for ($y = 0; $y -lt $source.Height; $y++) {
            for ($x = 0; $x -lt $source.Width; $x++) {
                $pixel = $source.GetPixel($x, $y)
                $isLikelyIcon = (
                    $pixel.A -gt 32 -and
                    $x -gt ($source.Width * 0.18) -and $x -lt ($source.Width * 0.82) -and
                    $y -gt ($source.Height * 0.18) -and $y -lt ($source.Height * 0.82) -and
                    $pixel.R -lt 70 -and $pixel.G -lt 70 -and $pixel.B -lt 70
                )

                if ($isLikelyIcon) {
                    $canvas.SetPixel($x, $y, [System.Drawing.Color]::FromArgb($pixel.A, $IconColor.R, $IconColor.G, $IconColor.B))
                }
                else {
                    $canvas.SetPixel($x, $y, $pixel)
                }
            }
        }

        if ($Size -eq $source.Width) {
            $canvas.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        }
        else {
            $tempPath = Join-Path ([System.IO.Path]::GetTempPath()) "asset-manager-${Resource}-raw-visible.png"
            $canvas.Save($tempPath, [System.Drawing.Imaging.ImageFormat]::Png)
            Resize-Png -InputPath $tempPath -OutputPath $outputPath -Size $Size
            Remove-Item -LiteralPath $tempPath -Force
        }
    }
    finally {
        if ($graphics -ne $null) { $graphics.Dispose() }
        if ($canvas -ne $null) { $canvas.Dispose() }
        if ($source -ne $null) { $source.Dispose() }
    }

    return $true
}

function Build-VisibleToken {
    param(
        [string]$Resource,
        [int]$Size,
        [System.Drawing.Color]$IconColor
    )

    if (Compose-FromBaseAndIcon -Resource $Resource -Size $Size -IconColor $IconColor) {
        return
    }

    if (Compose-FromRawToken -Resource $Resource -Size $Size -IconColor $IconColor) {
        return
    }

    throw "No supported input found for $Resource. Expected base/icon assets or final_raw token image."
}

function Test-VisibleOutput {
    param(
        [string]$Resource,
        [int]$Size,
        [System.Drawing.Color]$IconColor
    )

    $outputPath = Join-Path $outputDir "${Resource}_token_${Size}.png"
    if (-not (Test-Path -LiteralPath $outputPath)) {
        throw "Missing output: $outputPath"
    }

    $image = $null
    try {
        $image = [System.Drawing.Bitmap]::new($outputPath)

        if ($image.Width -ne $Size -or $image.Height -ne $Size) {
            throw "Output has wrong size: $outputPath is $($image.Width)x$($image.Height), expected ${Size}x${Size}"
        }
        if ($image.Width -ne $image.Height) {
            throw "Output is not square: $outputPath"
        }

        $hasTransparentPixel = $false
        $brightIconPixels = 0
        $minX = $image.Width
        $minY = $image.Height
        $maxX = -1
        $maxY = -1

        for ($y = 0; $y -lt $image.Height; $y++) {
            for ($x = 0; $x -lt $image.Width; $x++) {
                $pixel = $image.GetPixel($x, $y)
                if ($pixel.A -lt 255) {
                    $hasTransparentPixel = $true
                }

                $isBrightIcon = (
                    $pixel.A -gt 80 -and
                    [Math]::Abs($pixel.R - $IconColor.R) -le 12 -and
                    [Math]::Abs($pixel.G - $IconColor.G) -le 12 -and
                    [Math]::Abs($pixel.B - $IconColor.B) -le 12
                )

                if ($isBrightIcon) {
                    $brightIconPixels++
                    if ($x -lt $minX) { $minX = $x }
                    if ($y -lt $minY) { $minY = $y }
                    if ($x -gt $maxX) { $maxX = $x }
                    if ($y -gt $maxY) { $maxY = $y }
                }

            }
        }

        if (-not $hasTransparentPixel) {
            throw "Output has no transparent pixels: $outputPath"
        }
        if ($brightIconPixels -le [Math]::Max(4, [int]($Size * $Size * 0.004))) {
            throw "Output does not contain enough bright icon-colored pixels: $outputPath"
        }
        $centerX = ($minX + $maxX) / 2.0
        $centerY = ($minY + $maxY) / 2.0
        $tolerance = [Math]::Max(2, $Size * 0.15)
        if ([Math]::Abs($centerX - (($Size - 1) / 2.0)) -gt $tolerance -or [Math]::Abs($centerY - (($Size - 1) / 2.0)) -gt $tolerance) {
            throw "Icon color bounds are not centered in $outputPath"
        }
    }
    finally {
        if ($image -ne $null) { $image.Dispose() }
    }
}

function Get-BeforeImagePath {
    param([string]$Resource)

    $rawPath = Join-Path $rawDir "${Resource}_token.png"
    if (Test-Path -LiteralPath $rawPath) {
        return $rawPath
    }

    $previousPath = Join-Path $previousFinalDir "${Resource}_512.png"
    if (Test-Path -LiteralPath $previousPath) {
        return $previousPath
    }

    $basePath = Get-BasePath -Resource $Resource
    if ($basePath -ne $null) {
        return $basePath
    }

    return $null
}

function New-ContactSheet {
    $tile = 192
    $labelHeight = 34
    $headerHeight = 40
    $gap = 16
    $pairGap = 22
    $width = ($resources.Count * (($tile * 2) + $pairGap)) + (($resources.Count + 1) * $gap)
    $height = $headerHeight + $labelHeight + $tile + $labelHeight + $gap
    $font = $null
    $smallFont = $null
    $sheet = $null
    $graphics = $null
    $brush = $null
    $mutedBrush = $null

    try {
        $sheet = [System.Drawing.Bitmap]::new($width, $height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $graphics = [System.Drawing.Graphics]::FromImage($sheet)
        Set-HighQualityGraphics -Graphics $graphics
        $graphics.Clear([System.Drawing.Color]::FromArgb(255, 28, 28, 28))
        $font = [System.Drawing.Font]::new("Segoe UI", 12, [System.Drawing.FontStyle]::Regular)
        $smallFont = [System.Drawing.Font]::new("Segoe UI", 10, [System.Drawing.FontStyle]::Regular)
        $brush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255, 243, 235, 221))
        $mutedBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255, 184, 174, 154))

        $format = [System.Drawing.StringFormat]::new()
        $format.Alignment = [System.Drawing.StringAlignment]::Center
        $format.LineAlignment = [System.Drawing.StringAlignment]::Center

        $graphics.DrawString("Resource token visibility: before / after", $font, $brush, [System.Drawing.RectangleF]::new(0, 0, $width, $headerHeight), $format)

        for ($i = 0; $i -lt $resources.Count; $i++) {
            $resource = $resources[$i]
            $x = $gap + ($i * (($tile * 2) + $pairGap + $gap))
            $beforePath = Get-BeforeImagePath -Resource $resource
            $afterPath = Join-Path $outputDir "${resource}_token_${largestSize}.png"

            $graphics.DrawString($resource, $smallFont, $mutedBrush, [System.Drawing.RectangleF]::new($x, $headerHeight, ($tile * 2) + $pairGap, $labelHeight), $format)
            $graphics.DrawString("before", $smallFont, $mutedBrush, [System.Drawing.RectangleF]::new($x, $headerHeight + $labelHeight + $tile, $tile, $labelHeight), $format)
            $graphics.DrawString("after", $smallFont, $brush, [System.Drawing.RectangleF]::new($x + $tile + $pairGap, $headerHeight + $labelHeight + $tile, $tile, $labelHeight), $format)

            if ($beforePath -ne $null) {
                $before = [System.Drawing.Bitmap]::new($beforePath)
                try {
                    $graphics.DrawImage($before, $x, $headerHeight + $labelHeight, $tile, $tile)
                }
                finally {
                    $before.Dispose()
                }
            }

            $after = [System.Drawing.Bitmap]::new($afterPath)
            try {
                $graphics.DrawImage($after, $x + $tile + $pairGap, $headerHeight + $labelHeight, $tile, $tile)
            }
            finally {
                $after.Dispose()
            }
        }

        $sheet.Save($contactSheetPath, [System.Drawing.Imaging.ImageFormat]::Png)
    }
    finally {
        if ($mutedBrush -ne $null) { $mutedBrush.Dispose() }
        if ($brush -ne $null) { $brush.Dispose() }
        if ($smallFont -ne $null) { $smallFont.Dispose() }
        if ($font -ne $null) { $font.Dispose() }
        if ($graphics -ne $null) { $graphics.Dispose() }
        if ($sheet -ne $null) { $sheet.Dispose() }
    }
}

New-DirectoryIfMissing -Path $outputDir
$iconColor = Convert-HexColor -Hex $PrimaryIconColor
$fallbackIconColor = Convert-HexColor -Hex $SecondaryIconColor

if (-not $VerifyOnly) {
    foreach ($resource in $resources) {
        foreach ($size in $Sizes) {
            Build-VisibleToken -Resource $resource -Size $size -IconColor $iconColor
            Write-Host "Wrote $(Join-Path $outputDir "${resource}_token_${size}.png")"
        }
    }

    New-ContactSheet
    Write-Host "Wrote $contactSheetPath"
}

foreach ($resource in $resources) {
    foreach ($size in $Sizes) {
        Test-VisibleOutput -Resource $resource -Size $size -IconColor $iconColor
    }
}

if (-not (Test-Path -LiteralPath $contactSheetPath)) {
    throw "Missing contact sheet: $contactSheetPath"
}

Write-Host "Verified $($resources.Count * $Sizes.Count) visible resource token PNGs in $outputDir"
