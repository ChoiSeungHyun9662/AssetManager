param(
    [string]$AssetRoot = (Join-Path $PSScriptRoot "..\Assets\resource_tokens"),
    [int[]]$Sizes = @(512, 256, 128, 64, 32),
    [double]$IconScale = 0.51,
    [string]$MainFill = "#8C2F3A",
    [string]$ShadowFill = "#4A141B",
    [switch]$VerifyOnly
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Drawing

$basePath = Join-Path $AssetRoot "base\deal_base.png"
$finalDir = Join-Path $AssetRoot "final"
$comparisonPath = Join-Path $finalDir "deal_comparison_v2.png"

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

function Set-HighQualityGraphics {
    param([System.Drawing.Graphics]$Graphics)

    $Graphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceOver
    $Graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $Graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $Graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $Graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
}

function New-StarPath {
    param(
        [float]$CenterX,
        [float]$CenterY,
        [float]$OuterRadius,
        [float]$InnerRadius
    )

    $points = New-Object 'System.Drawing.PointF[]' 10
    $startAngle = -90.0

    for ($i = 0; $i -lt 10; $i++) {
        $radius = if (($i % 2) -eq 0) { $OuterRadius } else { $InnerRadius }
        $angle = ($startAngle + ($i * 36.0)) * [Math]::PI / 180.0
        $points[$i] = [System.Drawing.PointF]::new(
            [float]($CenterX + ([Math]::Cos($angle) * $radius)),
            [float]($CenterY + ([Math]::Sin($angle) * $radius))
        )
    }

    $path = [System.Drawing.Drawing2D.GraphicsPath]::new()
    $path.AddPolygon($points)
    return $path
}

function Draw-FilledDealIcon {
    param(
        [System.Drawing.Graphics]$Graphics,
        [int]$Size,
        [System.Drawing.Color]$Fill,
        [System.Drawing.Color]$Shadow
    )

    $center = [float](($Size - 1) / 2.0)
    $iconWidth = [float]($Size * $IconScale)
    $outerRadius = [float]($iconWidth / 2.0)
    $ringWidth = [float][Math]::Max(2.0, $Size * 0.034)
    $shadowOffset = [float][Math]::Max(1.0, $Size * 0.006)
    $starOuter = [float]($outerRadius * 0.70)
    $starInner = [float]($starOuter * 0.46)

    $fillBrush = [System.Drawing.SolidBrush]::new($Fill)
    $shadowBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(70, $Shadow.R, $Shadow.G, $Shadow.B))
    $ringPen = [System.Drawing.Pen]::new($Fill, $ringWidth)
    $shadowPen = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(62, $Shadow.R, $Shadow.G, $Shadow.B), $ringWidth)

    try {
        $ringRect = [System.Drawing.RectangleF]::new($center - $outerRadius, $center - $outerRadius, $outerRadius * 2, $outerRadius * 2)
        $shadowRingRect = [System.Drawing.RectangleF]::new($ringRect.X + $shadowOffset, $ringRect.Y + $shadowOffset, $ringRect.Width, $ringRect.Height)
        $Graphics.DrawEllipse($shadowPen, $shadowRingRect)
        $Graphics.DrawEllipse($ringPen, $ringRect)

        $shadowStar = New-StarPath -CenterX ($center + $shadowOffset) -CenterY ($center + $shadowOffset) -OuterRadius $starOuter -InnerRadius $starInner
        $star = New-StarPath -CenterX $center -CenterY $center -OuterRadius $starOuter -InnerRadius $starInner
        try {
            $Graphics.FillPath($shadowBrush, $shadowStar)
            $Graphics.FillPath($fillBrush, $star)
        }
        finally {
            $star.Dispose()
            $shadowStar.Dispose()
        }
    }
    finally {
        $shadowPen.Dispose()
        $ringPen.Dispose()
        $shadowBrush.Dispose()
        $fillBrush.Dispose()
    }
}

function Build-DealToken {
    param([int]$Size)

    if (-not (Test-Path -LiteralPath $basePath)) {
        throw "Missing deal base image: $basePath"
    }

    $outputPath = Join-Path $finalDir "deal_${Size}_v2.png"
    $fill = Convert-HexColor -Hex $MainFill
    $shadow = Convert-HexColor -Hex $ShadowFill
    $base = $null
    $canvas = $null
    $graphics = $null

    try {
        $base = [System.Drawing.Bitmap]::new($basePath)
        $canvas = [System.Drawing.Bitmap]::new($Size, $Size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $graphics = [System.Drawing.Graphics]::FromImage($canvas)
        Set-HighQualityGraphics -Graphics $graphics
        $graphics.Clear([System.Drawing.Color]::Transparent)
        $graphics.DrawImage($base, 0, 0, $Size, $Size)
        Draw-FilledDealIcon -Graphics $graphics -Size $Size -Fill $fill -Shadow $shadow
        $canvas.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    }
    finally {
        if ($graphics -ne $null) { $graphics.Dispose() }
        if ($canvas -ne $null) { $canvas.Dispose() }
        if ($base -ne $null) { $base.Dispose() }
    }

    return $outputPath
}

function Test-DealToken {
    param([int]$Size)

    $path = Join-Path $finalDir "deal_${Size}_v2.png"
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Missing output: $path"
    }

    $fill = Convert-HexColor -Hex $MainFill
    $image = $null

    try {
        $image = [System.Drawing.Bitmap]::new($path)
        if ($image.Width -ne $Size -or $image.Height -ne $Size) {
            throw "Output has wrong size: $path is $($image.Width)x$($image.Height), expected ${Size}x${Size}"
        }
        if ($image.Width -ne $image.Height) {
            throw "Output is not square: $path"
        }

        $hasTransparentPixel = $false
        $burgundyPixels = 0
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

                $isIconPixel = (
                    $pixel.A -gt 90 -and
                    [Math]::Abs($pixel.R - $fill.R) -le 18 -and
                    [Math]::Abs($pixel.G - $fill.G) -le 18 -and
                    [Math]::Abs($pixel.B - $fill.B) -le 18
                )

                if ($isIconPixel) {
                    $burgundyPixels++
                    if ($x -lt $minX) { $minX = $x }
                    if ($y -lt $minY) { $minY = $y }
                    if ($x -gt $maxX) { $maxX = $x }
                    if ($y -gt $maxY) { $maxY = $y }
                }
            }
        }

        if (-not $hasTransparentPixel) {
            throw "Transparency is not preserved: $path"
        }
        if ($burgundyPixels -le [Math]::Max(8, [int]($Size * $Size * 0.018))) {
            throw "Icon does not contain enough burgundy fill: $path"
        }

        $centerX = ($minX + $maxX) / 2.0
        $centerY = ($minY + $maxY) / 2.0
        $tolerance = [Math]::Max(1.5, $Size * 0.025)
        if ([Math]::Abs($centerX - (($Size - 1) / 2.0)) -gt $tolerance -or [Math]::Abs($centerY - (($Size - 1) / 2.0)) -gt $tolerance) {
            throw "Icon is not centered: $path"
        }
    }
    finally {
        if ($image -ne $null) { $image.Dispose() }
    }
}

function New-ComparisonPreview {
    $tile = 256
    $gap = 24
    $labelHeight = 34
    $width = ($tile * 2) + ($gap * 3)
    $height = $tile + $labelHeight + ($gap * 2)
    $originalPath = Join-Path $finalDir "deal_512.png"
    $updatedPath = Join-Path $finalDir "deal_512_v2.png"

    $sheet = $null
    $graphics = $null
    $font = $null
    $brush = $null
    $mutedBrush = $null
    $original = $null
    $updated = $null

    try {
        $sheet = [System.Drawing.Bitmap]::new($width, $height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $graphics = [System.Drawing.Graphics]::FromImage($sheet)
        Set-HighQualityGraphics -Graphics $graphics
        $graphics.Clear([System.Drawing.Color]::FromArgb(255, 28, 24, 22))

        $font = [System.Drawing.Font]::new("Segoe UI", 12, [System.Drawing.FontStyle]::Regular)
        $brush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255, 243, 235, 221))
        $mutedBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255, 190, 174, 150))
        $format = [System.Drawing.StringFormat]::new()
        $format.Alignment = [System.Drawing.StringAlignment]::Center
        $format.LineAlignment = [System.Drawing.StringAlignment]::Center

        $original = [System.Drawing.Bitmap]::new($originalPath)
        $updated = [System.Drawing.Bitmap]::new($updatedPath)

        $graphics.DrawImage($original, $gap, $gap, $tile, $tile)
        $graphics.DrawImage($updated, ($gap * 2) + $tile, $gap, $tile, $tile)
        $graphics.DrawString("original", $font, $mutedBrush, [System.Drawing.RectangleF]::new($gap, $gap + $tile, $tile, $labelHeight), $format)
        $graphics.DrawString("updated", $font, $brush, [System.Drawing.RectangleF]::new(($gap * 2) + $tile, $gap + $tile, $tile, $labelHeight), $format)

        $sheet.Save($comparisonPath, [System.Drawing.Imaging.ImageFormat]::Png)
    }
    finally {
        if ($updated -ne $null) { $updated.Dispose() }
        if ($original -ne $null) { $original.Dispose() }
        if ($mutedBrush -ne $null) { $mutedBrush.Dispose() }
        if ($brush -ne $null) { $brush.Dispose() }
        if ($font -ne $null) { $font.Dispose() }
        if ($graphics -ne $null) { $graphics.Dispose() }
        if ($sheet -ne $null) { $sheet.Dispose() }
    }
}

if (-not $VerifyOnly) {
    foreach ($size in $Sizes) {
        $output = Build-DealToken -Size $size
        Write-Host "Wrote $output"
    }

    New-ComparisonPreview
    Write-Host "Wrote $comparisonPath"
}

foreach ($size in $Sizes) {
    Test-DealToken -Size $size
}

if (-not (Test-Path -LiteralPath $comparisonPath)) {
    throw "Missing comparison preview: $comparisonPath"
}

Write-Host "Verified $($Sizes.Count) deal token v2 PNGs in $finalDir"
