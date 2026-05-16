param(
    [string]$AssetRoot = (Join-Path $PSScriptRoot "..\Assets\resource_tokens"),
    [int[]]$Sizes = @(512, 256, 128, 64, 32),
    [double]$IconScale = 0.46,
    [switch]$VerifyOnly
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Drawing

$resources = @("cash", "research", "credit", "commodity", "deal")
$baseDir = Join-Path $AssetRoot "base"
$iconDir = Join-Path $AssetRoot "icons"
$finalDir = Join-Path $AssetRoot "final"

function New-DirectoryIfMissing {
    param([string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        New-Item -ItemType Directory -Path $Path | Out-Null
    }
}

function Get-AlphaBounds {
    param([System.Drawing.Bitmap]$Bitmap)

    $minX = $Bitmap.Width
    $minY = $Bitmap.Height
    $maxX = -1
    $maxY = -1

    for ($y = 0; $y -lt $Bitmap.Height; $y++) {
        for ($x = 0; $x -lt $Bitmap.Width; $x++) {
            if ($Bitmap.GetPixel($x, $y).A -gt 0) {
                if ($x -lt $minX) { $minX = $x }
                if ($y -lt $minY) { $minY = $y }
                if ($x -gt $maxX) { $maxX = $x }
                if ($y -gt $maxY) { $maxY = $y }
            }
        }
    }

    if ($maxX -lt 0) {
        throw "Icon has no visible alpha pixels."
    }

    return [System.Drawing.Rectangle]::new($minX, $minY, $maxX - $minX + 1, $maxY - $minY + 1)
}

function New-BlackSilhouette {
    param(
        [System.Drawing.Bitmap]$Source,
        [System.Drawing.Rectangle]$Bounds
    )

    $silhouette = [System.Drawing.Bitmap]::new($Bounds.Width, $Bounds.Height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)

    for ($y = 0; $y -lt $Bounds.Height; $y++) {
        for ($x = 0; $x -lt $Bounds.Width; $x++) {
            $sourcePixel = $Source.GetPixel($Bounds.X + $x, $Bounds.Y + $y)
            $silhouette.SetPixel($x, $y, [System.Drawing.Color]::FromArgb($sourcePixel.A, 8, 8, 8))
        }
    }

    return $silhouette
}

function Compose-ResourceToken {
    param(
        [string]$Resource,
        [int]$Size
    )

    $basePath = Join-Path $baseDir "${Resource}_base.png"
    $iconPath = Join-Path $iconDir "${Resource}_icon.png"
    $outputPath = Join-Path $finalDir "${Resource}_${Size}.png"

    if (-not (Test-Path -LiteralPath $basePath)) {
        throw "Missing base image: $basePath"
    }
    if (-not (Test-Path -LiteralPath $iconPath)) {
        throw "Missing icon image: $iconPath"
    }

    $baseImage = $null
    $iconImage = $null
    $silhouette = $null
    $canvas = $null
    $graphics = $null

    try {
        $baseImage = [System.Drawing.Bitmap]::new($basePath)
        $iconImage = [System.Drawing.Bitmap]::new($iconPath)

        if ($baseImage.Width -ne $baseImage.Height) {
            throw "Base image must be square: $basePath is $($baseImage.Width)x$($baseImage.Height)"
        }

        $bounds = Get-AlphaBounds -Bitmap $iconImage
        $silhouette = New-BlackSilhouette -Source $iconImage -Bounds $bounds
        $targetIconWidth = [int][Math]::Round($Size * $IconScale)
        $targetIconHeight = [int][Math]::Round($targetIconWidth * ($silhouette.Height / $silhouette.Width))
        $iconX = [int][Math]::Round(($Size - $targetIconWidth) / 2)
        $iconY = [int][Math]::Round(($Size - $targetIconHeight) / 2)

        $canvas = [System.Drawing.Bitmap]::new($Size, $Size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $graphics = [System.Drawing.Graphics]::FromImage($canvas)
        $graphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceOver
        $graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
        $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
        $graphics.Clear([System.Drawing.Color]::Transparent)

        $graphics.DrawImage($baseImage, 0, 0, $Size, $Size)
        $graphics.DrawImage($silhouette, $iconX, $iconY, $targetIconWidth, $targetIconHeight)

        $canvas.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    }
    finally {
        if ($graphics -ne $null) { $graphics.Dispose() }
        if ($canvas -ne $null) { $canvas.Dispose() }
        if ($silhouette -ne $null) { $silhouette.Dispose() }
        if ($iconImage -ne $null) { $iconImage.Dispose() }
        if ($baseImage -ne $null) { $baseImage.Dispose() }
    }

    return $outputPath
}

function Test-ResourceTokenOutput {
    param(
        [string]$Resource,
        [int]$Size
    )

    $outputPath = Join-Path $finalDir "${Resource}_${Size}.png"
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
        for ($y = 0; $y -lt $image.Height -and -not $hasTransparentPixel; $y++) {
            for ($x = 0; $x -lt $image.Width; $x++) {
                if ($image.GetPixel($x, $y).A -lt 255) {
                    $hasTransparentPixel = $true
                    break
                }
            }
        }

        if (-not $hasTransparentPixel) {
            throw "Output has no transparent pixels: $outputPath"
        }
    }
    finally {
        if ($image -ne $null) { $image.Dispose() }
    }
}

New-DirectoryIfMissing -Path $finalDir

if (-not $VerifyOnly) {
    foreach ($resource in $resources) {
        foreach ($size in $Sizes) {
            $outputPath = Compose-ResourceToken -Resource $resource -Size $size
            Write-Host "Wrote $outputPath"
        }
    }
}

foreach ($resource in $resources) {
    foreach ($size in $Sizes) {
        Test-ResourceTokenOutput -Resource $resource -Size $size
    }
}

Write-Host "Verified $($resources.Count * $Sizes.Count) resource token PNGs in $finalDir"
