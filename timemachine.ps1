<#
.SYNOPSIS
    Build and launch Llyn, either from the current source or a versioned snapshot.
.DESCRIPTION
    With no version, the current working-tree source (src/Llyn.UIShell) is
    published into an isolated temp workspace and installed into the selected
    current destination (run/ by default). It can also mirror the complete
    publish output into snapshots/<version>/ before launching.

    With a version, snapshots/<version>/Llyn-V<version>.zip is extracted into
    an isolated temp workspace, published there, and the complete win-x64 publish
    output is installed directly into snapshots/<version>/ and launched.

    The checked-out source tree is never used as build output.
.PARAMETER Version
    Historical major.minor.revision to build. Omit it to use the current working tree.
.PARAMETER CurrentDestination
    Destination for a current-source build: run (default) or build. Ignored for
    historical versions, which are installed under snapshots/<version>/.
.PARAMETER MirrorCurrentSnapshot
    Also copy a current-source publish into snapshots/<current-version>/.
.PARAMETER NoLaunch
    Build and install without starting Llyn.
.PARAMETER Help
    Display this help and exit without building or launching. The alias -? is supported.
.EXAMPLE
    .\timemachine.ps1
    Build the current source into run/ and launch it.
.EXAMPLE
    .\timemachine.ps1 -NoLaunch
    Build the current source without launching it.
.EXAMPLE
    .\timemachine.ps1 2.9.9991
    Build and launch a historical snapshot.
.EXAMPLE
    .\timemachine.ps1 2.9.9991 -NoLaunch
    Build a historical snapshot without launching it.
#>
#requires -Version 5.1
[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidatePattern('^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$')]
    [string]$Version,

    [ValidateSet('build', 'run')]
    [string]$CurrentDestination = 'run',

    [switch]$MirrorCurrentSnapshot,

    [switch]$NoLaunch,

    [Alias('?')]
    [switch]$Help
)

if ($Help) {
    @'
NAME
    timemachine.ps1

SYNOPSIS
    Build and optionally launch Llyn from current or historical source.

SYNTAX
    .\timemachine.ps1 [<version>] [-CurrentDestination <build|run>]
        [-MirrorCurrentSnapshot] [-NoLaunch] [-Help]

OPTIONS
    <version>
        Historical major.minor.revision to build. Omit for current source.

    -CurrentDestination <build|run>
        Current-source destination. Defaults to run.

    -MirrorCurrentSnapshot
        Also copy a current-source publish into its versioned snapshot.

    -NoLaunch
        Build and install without starting Llyn.

    -Help, -?
        Display this help and exit without building or launching.

EXAMPLES
    .\timemachine.ps1
        Build current source into run/ and launch it.

    .\timemachine.ps1 -NoLaunch
        Build current source without launching it.

    .\timemachine.ps1 2.9.9991
        Build and launch a historical snapshot.

    .\timemachine.ps1 2.9.9991 -NoLaunch
        Build a historical snapshot without launching it.
'@ | Write-Host
    exit 0
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$root = [System.IO.Path]::GetFullPath($PSScriptRoot)
$runtime = 'win-x64'
$snapshotsRoot = Join-Path $root 'snapshots'
$versionPattern = '^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$'

$useCurrentSource = [string]::IsNullOrEmpty($Version)

if ($useCurrentSource) {
    $versionFile = Join-Path $root 'version.json'
    if (-not (Test-Path -LiteralPath $versionFile -PathType Leaf)) {
        throw "version.json was not found: $versionFile"
    }

    $versionData = Get-Content -LiteralPath $versionFile -Raw | ConvertFrom-Json
    if (-not ($versionData.PSObject.Properties.Name -contains 'current-version')) {
        throw 'version.json must contain a current-version property.'
    }

    $Version = [string]$versionData.'current-version'
    if ($Version -notmatch $versionPattern) {
        throw "Invalid version format in version.json: $Version"
    }
}

$tempRoot = Join-Path $root 'temp'
$workDir = Join-Path $tempRoot ("timemachine-$Version-" + [Guid]::NewGuid().ToString('N'))
$sourceDir = Join-Path $workDir 'source'
$publishDir = Join-Path $workDir 'publish'

if ($useCurrentSource) {
    $destinationDir = Join-Path $root $CurrentDestination
}
else {
    $destinationDir = Join-Path $snapshotsRoot $Version
}

$executableName = if ($useCurrentSource) { 'Llyn.exe' } else { 'Llyn.Windows.exe' }
$destinationExe = Join-Path $destinationDir $executableName
$stagingParent = if ($useCurrentSource) { $root } else { $snapshotsRoot }
$stagingDir = Join-Path $stagingParent ('.' + $Version + '.staging-' + [Guid]::NewGuid().ToString('N'))
$snapshotStagingDir = $null

if (-not $useCurrentSource) {
    $versionDir = Join-Path $snapshotsRoot $Version
    $sourceArchive = Join-Path $versionDir "Llyn-V$Version.zip"
    if (-not (Test-Path -LiteralPath $sourceArchive -PathType Leaf)) {
        throw "Source snapshot was not found: $sourceArchive"
    }
}

New-Item -ItemType Directory -Path $workDir -Force | Out-Null

try {
    if ($useCurrentSource) {
        $project = Join-Path $root 'src\Llyn.UIShell\Llyn.UIShell.csproj'
        if (-not (Test-Path -LiteralPath $project -PathType Leaf)) {
            throw "Llyn project was not found: $project"
        }

        Write-Host "Building current source: $Version" -ForegroundColor Cyan
    }
    else {
        New-Item -ItemType Directory -Path $sourceDir -Force | Out-Null

        Write-Host "Extracting source snapshot: $sourceArchive" -ForegroundColor Cyan
        Expand-Archive -LiteralPath $sourceArchive -DestinationPath $sourceDir

        $projects = @(
            Get-ChildItem -LiteralPath $sourceDir -Include 'Llyn.UIShell.csproj', 'Llyn.Windows.csproj' -File -Recurse |
                Where-Object { $_.Directory.Name -in 'Llyn.UIShell', 'Llyn.Windows' }
        )

        if ($projects.Count -eq 0) {
            throw "A Llyn UI project was not found in source snapshot: $sourceArchive"
        }

        if ($projects.Count -gt 1) {
            $projectList = ($projects.FullName | ForEach-Object { "  $_" }) -join [Environment]::NewLine
            throw "More than one Llyn UI project was found in the source snapshot:$([Environment]::NewLine)$projectList"
        }

        $project = $projects[0].FullName
        $executableName = [System.IO.Path]::GetFileNameWithoutExtension($project) + '.exe'
        $destinationExe = Join-Path $destinationDir $executableName
        $sourceRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $project))
        Write-Host "Historical source: $sourceRoot" -ForegroundColor Cyan
    }

    New-Item -ItemType Directory -Path $publishDir | Out-Null

    & dotnet publish $project `
        --configuration Release `
        --runtime $runtime `
        --self-contained true `
        --output $publishDir `
        -p:PublishSingleFile=true `
        -p:IncludeNativeLibrariesForSelfExtract=true `
        -p:DebugType=none `
        -p:DebugSymbols=false

    $publishExitCode = $LASTEXITCODE
    if ($publishExitCode -ne 0) {
        throw "dotnet publish failed with exit code $publishExitCode."
    }

    $publishedExe = Join-Path $publishDir $executableName
    if (-not (Test-Path -LiteralPath $publishedExe -PathType Leaf)) {
        throw "Publish reported success but $executableName is missing: $publishedExe"
    }

    # Stage the whole publish output beside its final destination. This keeps
    # the executable and every version-matched dependency together.
    Move-Item -LiteralPath $publishDir -Destination $stagingDir

    # Keep the historical source archive alongside the installed snapshot.
    if (-not $useCurrentSource) {
        Copy-Item -LiteralPath $sourceArchive -Destination $stagingDir
    }

    if (Test-Path -LiteralPath $destinationDir) {
        Remove-Item -LiteralPath $destinationDir -Recurse -Force
    }

    Move-Item -LiteralPath $stagingDir -Destination $destinationDir

    if (-not (Test-Path -LiteralPath $destinationExe -PathType Leaf)) {
        throw "$executableName is missing from the installed build: $destinationExe"
    }

    Write-Host "Installed: $destinationDir" -ForegroundColor Green

    if ($useCurrentSource -and $MirrorCurrentSnapshot) {
        New-Item -ItemType Directory -Path $snapshotsRoot -Force | Out-Null

        $snapshotDir = Join-Path $snapshotsRoot $Version
        $snapshotStagingDir = Join-Path $snapshotsRoot ('.' + $Version + '.staging-' + [Guid]::NewGuid().ToString('N'))
        New-Item -ItemType Directory -Path $snapshotStagingDir | Out-Null

        # Mirror the complete publish payload so the executable and all of its
        # version-matched dependencies stay together. Preserve a source archive
        # that already belongs to this version.
        Get-ChildItem -LiteralPath $destinationDir -Force | ForEach-Object {
            Copy-Item -LiteralPath $_.FullName -Destination $snapshotStagingDir -Recurse -Force
        }

        $sourceArchive = Join-Path $snapshotDir "Llyn-V$Version.zip"
        if (Test-Path -LiteralPath $sourceArchive -PathType Leaf) {
            Copy-Item -LiteralPath $sourceArchive -Destination $snapshotStagingDir
        }

        if (Test-Path -LiteralPath $snapshotDir) {
            Remove-Item -LiteralPath $snapshotDir -Recurse -Force
        }

        Move-Item -LiteralPath $snapshotStagingDir -Destination $snapshotDir
        $snapshotStagingDir = $null

        Write-Host "Snapshot: $snapshotDir" -ForegroundColor Green
    }

    if (-not $NoLaunch) {
        $process = Start-Process `
            -FilePath $destinationExe `
            -WorkingDirectory $destinationDir `
            -PassThru

        Write-Host "Launched Llyn $Version (PID $($process.Id))." -ForegroundColor Green
    }
}
finally {
    if ($null -ne $snapshotStagingDir -and (Test-Path -LiteralPath $snapshotStagingDir)) {
        Remove-Item -LiteralPath $snapshotStagingDir -Recurse -Force -ErrorAction SilentlyContinue
    }

    if (Test-Path -LiteralPath $stagingDir) {
        Remove-Item -LiteralPath $stagingDir -Recurse -Force -ErrorAction SilentlyContinue
    }

    if (Test-Path -LiteralPath $workDir) {
        Remove-Item -LiteralPath $workDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}
