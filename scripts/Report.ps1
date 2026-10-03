<#
.SYNOPSIS
Writes every analysis document for the repository afresh: the audits, the method map and the statistics.

.DESCRIPTION
Reads the configuration from Report.json next to this script, then performs these actions:
  1. Deletes every older page of the configured steps directly inside the report folder, so
     the run leaves only fresh output. A step owns the files whose name starts with its script
     name and a dash, or with each prefix in its clear list. The folder holds the reports of
     every script family, so a file of another name is never touched, and subfolders are never
     deleted, moved or written. -Keep skips this.
  2. Runs each step in order, one at a time, with -NoOpen and -NoPause passed to every script
     that declares them. A step names one script, or a family whose every script runs in
     ordinal order. A missing or throwing script is recorded as failed and the run continues.
  3. Prints one result row per script, with its exit code and the pages it left.

The pages are the documents uploaded to GitHub with each version.

.PARAMETER ConfigPath
Path to the JSON configuration. Defaults to Report.json next to this script.

.PARAMETER Keep
Keep the older pages instead of deleting them before the run.

.PARAMETER Help
Display this help and exit. The alias -? is supported.

.EXAMPLE
report
Delete the older pages, then write every audit, the method map and the statistics afresh.

.EXAMPLE
report -Keep
Write every page afresh and keep the older ones.
#>
#requires -Version 5.1
# REPORT - REPORT GENERATION 1.
# A generation is not a revision count. It names functionality, not edits, so editing this file is
# never on its own a reason to raise it. Raise it only when the written documents change.
# Generation 1: the steps are the audit run, the method map and the statistics family; each step's
# older pages are deleted first, and each script writes its own pages into the shared report folder.
[CmdletBinding()]
param(
    [string]$ConfigPath = '',
    [switch]$Keep,
    [Alias('?')]
    [switch]$Help
)

if ($Help) {
    @'
NAME
    Report.ps1

SYNOPSIS
    Write every analysis document afresh: the audits, the method map and the statistics.

SYNTAX
    report [-ConfigPath <path>] [-Keep] [-Help]

CONFIGURATION
    All project-specific values live in Report.json next to the script:
    the report directory and the steps, in order. A step names one script
    with "script", or every script of a family with "family". A family
    script is one whose header reads "# <NAME> - <FAMILY> GENERATION <n>.".
    A step's optional "clear" list adds file-name prefixes it owns.

RUN
    First every older page of the steps directly inside the report folder
    is deleted, unless -Keep is given. A step owns the files named after
    its script and a dash, and those starting with a prefix in its clear
    list. Any other file and every subfolder stays. Then each script runs
    in order, with -NoOpen and -NoPause passed to every script that
    declares them. A missing or throwing script is recorded as failed and
    the run continues.

RESULT
    One row per script. Count is its exit code, so 0 passes. Meaning names
    the pages it left in the report folder and its duration.
    The run exits with 1 when any script failed or threw, else 0.

OPTIONS
    -ConfigPath <path>
        Path to the JSON configuration. Defaults to Report.json.

    -Keep
        Keep the older pages instead of deleting them before the run.

    -Help, -?
        Display this help and exit.

EXAMPLES
    report
        Delete the older pages, then write every page afresh.

    report -Keep
        Write every page afresh and keep the older ones.
'@ | Write-Host
    exit 0
}

Write-Host 'REPORT - REPORT GENERATION 1' -ForegroundColor Blue

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [Console]::OutputEncoding

$script:ReportGeneration = 1

function Get-ConfigNode {
    param(
        [Parameter(Mandatory = $true)]$Document,
        [Parameter(Mandatory = $true)][string]$Key
    )

    $node = $Document
    foreach ($segment in ($Key -split '\.')) {
        if ($null -eq $node -or -not ($node.PSObject.Properties.Name -contains $segment)) {
            return $null
        }
        $node = $node.$segment
    }

    return , $node
}

function Read-ReportConfig {
    param([Parameter(Mandatory = $true)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "The report configuration was not found: $Path"
    }

    try {
        $config = Get-Content -LiteralPath $Path -Raw -Encoding UTF8 | ConvertFrom-Json
    }
    catch {
        throw "The report configuration is not valid JSON: $Path`n$($_.Exception.Message)"
    }

    foreach ($key in @('generation', 'report.directory', 'steps')) {
        if ($null -eq (Get-ConfigNode -Document $config -Key $key)) {
            throw "The report configuration has no key '$key': $Path"
        }
    }
    if ([int]$config.generation -ne $script:ReportGeneration) {
        throw "The report configuration is generation $($config.generation); this script is generation $script:ReportGeneration."
    }

    return $config
}

function Get-OrdinalKey {
    # Sort-Object compares text by culture, which Windows PowerShell 5.1 and pwsh 7 order differently.
    # Uppercase hexadecimal UTF-16 code units compare alike under every culture, so this key sorts ordinally.
    param([string]$Text)
    return [System.BitConverter]::ToString([System.Text.Encoding]::BigEndianUnicode.GetBytes($Text)).Replace('-', '')
}

function Resolve-ReportFolder {
    # The folder must be a direct child of the repository root, so clearing it can never reach
    # anything else.
    param(
        [Parameter(Mandatory = $true)][string]$Root,
        [Parameter(Mandatory = $true)][string]$Name
    )

    $nameTrimmed = $Name.Trim().TrimEnd('\', '/')
    if ($nameTrimmed.Length -eq 0 -or $nameTrimmed -eq '.' -or $nameTrimmed -eq '..' -or $nameTrimmed.IndexOfAny([char[]]@('\', '/', ':')) -ge 0) {
        throw "The report folder must be a plain folder name directly below the repository root, but it is: $Name"
    }

    return Join-Path $Root $nameTrimmed
}

function Get-FamilyScripts {
    # Every script whose header names the family, in ordinal order.
    param(
        [Parameter(Mandatory = $true)][string]$Folder,
        [Parameter(Mandatory = $true)][string]$Family
    )

    $pattern = '(?m)^# [A-Z0-9.]+ - ' + [regex]::Escape($Family.ToUpperInvariant()) + ' GENERATION \d+\.\r?$'
    $names = foreach ($file in Get-ChildItem -LiteralPath $Folder -Filter '*.ps1' -File) {
        if ($file.Name -like '*.config.ps1') { continue }
        if ([regex]::IsMatch([System.IO.File]::ReadAllText($file.FullName), $pattern)) { $file.BaseName }
    }

    return @($names | Sort-Object -Property @{ Expression = { Get-OrdinalKey $_ } })
}

function Get-StepScripts {
    # Expands the configured steps into one entry per script, each with the page prefixes it owns.
    param(
        [Parameter(Mandatory = $true)]$Config,
        [Parameter(Mandatory = $true)][string]$Folder
    )

    $entries = [System.Collections.Generic.List[object]]::new()
    foreach ($step in @($Config.steps)) {
        $single = [string](Get-ConfigNode -Document $step -Key 'script')
        $family = [string](Get-ConfigNode -Document $step -Key 'family')
        if (($single.Length -gt 0) -eq ($family.Length -gt 0)) {
            throw 'Every report step names either "script" or "family", and only one of them.'
        }

        $clear = @(@(Get-ConfigNode -Document $step -Key 'clear') | Where-Object { $null -ne $_ } | ForEach-Object { [string]$_ })
        $names = @(if ($single.Length -gt 0) { $single } else { Get-FamilyScripts -Folder $Folder -Family $family })
        if ($names.Count -eq 0) {
            throw "The report family $family has no script in $Folder."
        }

        foreach ($name in $names) {
            $entries.Add([pscustomobject]@{ Name = $name; Prefixes = @(@($name + '-') + $clear) })
        }
    }

    return $entries.ToArray()
}

function Test-OwnedPage {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string[]]$Prefixes
    )

    return @($Prefixes | Where-Object { $Name.StartsWith($_, [System.StringComparison]::OrdinalIgnoreCase) }).Count -gt 0
}

function Get-OwnedPages {
    # The files directly inside the folder that a step owns. Subfolders are never listed.
    param(
        [Parameter(Mandatory = $true)][string]$Folder,
        [Parameter(Mandatory = $true)][string[]]$Prefixes
    )

    if (-not [System.IO.Directory]::Exists($Folder)) { return @() }
    return @([System.IO.Directory]::GetFiles($Folder) | Where-Object { Test-OwnedPage -Name ([System.IO.Path]::GetFileName($_)) -Prefixes $Prefixes })
}

function Invoke-Step {
    # Runs one script live, so its own console shows as it goes, and records how it ended.
    param(
        [Parameter(Mandatory = $true)]$Entry,
        [Parameter(Mandatory = $true)][string]$Root,
        [Parameter(Mandatory = $true)][string]$Folder
    )

    $path = Join-Path (Join-Path $Root 'scripts') ($Entry.Name + '.ps1')
    $result = [ordered]@{ Name = $Entry.Name; Exit = 1; Pages = 0; Seconds = [double]0; Problem = '' }
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        $result.Problem = 'missing script'
        return [pscustomobject]$result
    }

    $quiet = @{}
    $parameters = (Get-Command -Name $path).Parameters
    if ($parameters.ContainsKey('NoOpen')) { $quiet['NoOpen'] = $true }
    if ($parameters.ContainsKey('NoPause')) { $quiet['NoPause'] = $true }

    Write-Host ''
    Write-Host "Running $($Entry.Name)..." -ForegroundColor DarkGray
    Set-Location -LiteralPath $Root
    $watch = [System.Diagnostics.Stopwatch]::StartNew()
    $global:LASTEXITCODE = 0
    try {
        # A native stderr line must never end the run, so every script runs under Continue.
        $ErrorActionPreference = 'Continue'
        & $path @quiet
        $result.Exit = [int]$global:LASTEXITCODE
    }
    catch {
        $result.Problem = 'threw: ' + $_.Exception.Message
    }
    finally {
        $ErrorActionPreference = 'Stop'
    }
    $watch.Stop()
    Set-Location -LiteralPath $Root

    $result.Seconds = $watch.Elapsed.TotalSeconds
    $result.Pages = @(Get-OwnedPages -Folder $Folder -Prefixes $Entry.Prefixes).Count
    return [pscustomobject]$result
}

$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
if ([string]::IsNullOrWhiteSpace($ConfigPath)) {
    $ConfigPath = Join-Path $PSScriptRoot 'Report.json'
}
$config = Read-ReportConfig -Path ([System.IO.Path]::GetFullPath($ConfigPath))
$reportFolder = Resolve-ReportFolder -Root $repoRoot -Name ([string]$config.report.directory)
$entries = @(Get-StepScripts -Config $config -Folder $PSScriptRoot)

Write-Host ('Scanned: {0} scripts ({1}) for {2}' -f $entries.Count, (($entries | ForEach-Object { $_.Name }) -join ', '), $reportFolder) -ForegroundColor DarkGray

$deleted = 0
if (-not $Keep) {
    $owned = @($entries | ForEach-Object { $_.Prefixes } | ForEach-Object { $_ })
    foreach ($page in @(Get-OwnedPages -Folder $reportFolder -Prefixes $owned)) {
        [System.IO.File]::Delete($page)
        $deleted++
    }
}
[System.IO.Directory]::CreateDirectory($reportFolder) | Out-Null
Write-Host ('Deleted: {0} older pages' -f $deleted) -ForegroundColor DarkGray

$results = [System.Collections.Generic.List[object]]::new()
foreach ($entry in $entries) {
    $results.Add((Invoke-Step -Entry $entry -Root $repoRoot -Folder $reportFolder))
}
Set-Location -LiteralPath $repoRoot

# Result: one row per script, in the audit console grammar.
$rows = @($results | ForEach-Object {
        $failed = $_.Exit -ne 0 -or $_.Problem.Length -gt 0
        $meaning = if ($_.Problem.Length -gt 0) { $_.Problem } else { '{0} pages in {1:N0} s' -f $_.Pages, $_.Seconds }
        [pscustomobject]@{ Status = $(if ($failed) { 'FAIL' } else { 'OK' }); Count = $(if ($_.Problem.Length -gt 0) { 1 } else { $_.Exit }); Gate = $_.Name; Meaning = $meaning }
    })
$statusWidth = [Math]::Max(6, (@($rows | ForEach-Object { $_.Status.Length }) | Measure-Object -Maximum).Maximum)
$countWidth = [Math]::Max(5, (@($rows | ForEach-Object { ([string]$_.Count).Length }) | Measure-Object -Maximum).Maximum)
$gateWidth = [Math]::Max(4, (@($rows | ForEach-Object { $_.Gate.Length }) | Measure-Object -Maximum).Maximum)
$meaningWidth = [Math]::Max(7, (@($rows | ForEach-Object { $_.Meaning.Length }) | Measure-Object -Maximum).Maximum)

Write-Host ''
Write-Host 'Result' -ForegroundColor Blue
Write-Host '------' -ForegroundColor DarkGray
Write-Host ('{0}  {1}  {2}  {3}' -f 'Status'.PadRight($statusWidth), 'Count'.PadLeft($countWidth), 'Gate'.PadRight($gateWidth), 'Meaning') -ForegroundColor Cyan
Write-Host ('{0}  {1}  {2}  {3}' -f ('-' * $statusWidth), ('-' * $countWidth), ('-' * $gateWidth), ('-' * $meaningWidth)) -ForegroundColor Cyan
foreach ($row in $rows) {
    $colour = if ($row.Status -eq 'OK') { 'Green' } else { 'Red' }
    Write-Host $row.Status.PadRight($statusWidth) -ForegroundColor $colour -NoNewline
    Write-Host ('  {0}  {1}  {2}' -f ([string]$row.Count).PadLeft($countWidth), $row.Gate.PadRight($gateWidth), $row.Meaning)
}

$failing = @($rows | Where-Object { $_.Status -eq 'FAIL' })
Write-Host ''
if ($failing.Count -eq 0) {
    Write-Host ('PASS: all {0} gates at 0.' -f $rows.Count) -ForegroundColor Green
    Write-Host ''
    Write-Host "Report: $reportFolder"
    exit 0
}

Write-Host ('FAIL: {0} of {1} gates above 0. See {2}.' -f $failing.Count, $rows.Count, (($failing | ForEach-Object { $_.Gate }) -join ', ')) -ForegroundColor Red
Write-Host ''
Write-Host "Report: $reportFolder"
exit 1
