[CmdletBinding()]
param(
    [ValidateSet("EditMode", "PlayMode", "QuitOnly")]
    [string] $Mode = "EditMode",

    [string] $AssemblyNames,

    [string] $UnityExe
)

$ErrorActionPreference = "Stop"

# In Codex, launch this script with escalated execution; Unity exits early in the default sandbox.
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$projectPath = Join-Path $repoRoot "Asset Manager"
$versionFile = Join-Path $projectPath "ProjectSettings\ProjectVersion.txt"
$outputDir = Join-Path $repoRoot ".scratch\test-results"

if (-not (Test-Path -LiteralPath $versionFile)) {
    throw "Unity project version file was not found: $versionFile"
}

$versionLine = Select-String -Path $versionFile -Pattern "^m_EditorVersion:\s*(.+)$" | Select-Object -First 1
if (-not $versionLine) {
    throw "Could not read Unity editor version from: $versionFile"
}

$editorVersion = $versionLine.Matches[0].Groups[1].Value.Trim()
if (-not $UnityExe) {
    if ($env:UNITY_EXE) {
        $UnityExe = $env:UNITY_EXE
    } else {
        $UnityExe = Join-Path $env:ProgramFiles "Unity\Hub\Editor\$editorVersion\Editor\Unity.exe"
    }
}

if (-not (Test-Path -LiteralPath $UnityExe)) {
    throw "Unity executable was not found: $UnityExe"
}

New-Item -ItemType Directory -Force -Path $outputDir | Out-Null

$stamp = Get-Date -Format "yyyyMMdd-HHmmss"
$logPath = Join-Path $outputDir "unity-$($Mode.ToLowerInvariant())-$stamp.log"
$unityArgs = @(
    "-batchmode",
    "-projectPath", $projectPath
)

if ($Mode -eq "QuitOnly") {
    $unityArgs += "-quit"
} else {
    if (-not $AssemblyNames) {
        $AssemblyNames = if ($Mode -eq "EditMode") {
            "AssetManager.Tests.EditMode"
        } else {
            "AssetManager.Tests.PlayMode"
        }
    }

    $resultsPath = Join-Path $outputDir "$($Mode.ToLowerInvariant())-$stamp-results.xml"
    $unityArgs += @(
        "-runTests",
        "-testPlatform", $Mode,
        "-assemblyNames", $AssemblyNames,
        "-testResults", $resultsPath
    )

    if ($Mode -eq "EditMode") {
        $unityArgs += "-runSynchronously"
    }
}

$unityArgs += @("-logFile", $logPath)

function ConvertTo-UnityCommandLineArgument {
    param([string] $Value)

    if ($Value -eq "") {
        return '""'
    }

    if ($Value -notmatch '[\s"]') {
        return $Value
    }

    return '"' + ($Value -replace '"', '\"') + '"'
}

$argumentString = ($unityArgs | ForEach-Object { ConvertTo-UnityCommandLineArgument $_ }) -join " "

Write-Host "Unity: $UnityExe"
Write-Host "Mode: $Mode"
Write-Host "Log: $logPath"
if ($Mode -ne "QuitOnly") {
    Write-Host "Results: $resultsPath"
}

$process = Start-Process -FilePath $UnityExe -ArgumentList $argumentString -Wait -PassThru -WindowStyle Hidden
exit $process.ExitCode
