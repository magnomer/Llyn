<#
.SYNOPSIS
Checks explicitly announced module boundaries and reports advisory call-cohesion candidates.

.DESCRIPTION
AuditModule binds tracked C# with Roslyn. For every module announced in AuditModule.json it
reports outside references that bypass the configured entrance and contracts, entrance members
that expose an unlisted module type, and cycles between module entrances or contracts. Counts
are checked against exact ceilings; a missing ceiling means zero, and a written ceiling above
its count is stale. With enforced false, all findings remain visible but gate nothing.

Graft supplies only advisory call-cohesion candidates. The script first runs `graft check .
--json`; missing, stale, invalid, or incomplete graph data disables suggestions without changing
Roslyn findings, ceilings, exit status, or convention-test parity. Candidates are relational
review evidence, never semantic module or ownership proof.

Reports are written as AuditModule-<version>.md and .html. The page contains filterable findings,
declared modules, freshness, and advisory candidates. Git, the .NET SDK, and a built solution are
required for authoritative binding. Graft is optional.

.PARAMETER Root
Project root. Defaults to the parent of the scripts folder.

.PARAMETER ReportDirectory
Overrides report.directory.

.PARAMETER Top
Overrides console.top.

.PARAMETER Open
Opens the Markdown report.

.PARAMETER NoOpen
Writes reports without opening the HTML page.

.PARAMETER NoPause
Disables paging. Accepted for Audit-family and Report compatibility.

.PARAMETER Help
Displays command help.
#>
#requires -Version 5.1
# AUDITMODULE - AUDIT GENERATION 27.
[CmdletBinding()]
param(
    [string]$Root,
    [string]$ReportDirectory,
    [int]$Top = 0,
    [switch]$Open,
    [switch]$NoOpen,
    [switch]$NoPause,
    [Alias('?')]
    [switch]$Help
)

if ($Help) {
    @'
NAME
    AuditModule.ps1

SYNOPSIS
    Check announced module boundaries and show advisory call-cohesion candidates.

SYNTAX
    auditmodule [-Root <path>] [-ReportDirectory <path>] [-Top <n>] [-Open] [-NoOpen] [-NoPause] [-Help]

OPTIONS
    -Root <path>             Project root; defaults to the parent of scripts.
    -ReportDirectory <path>  Overrides the configured report directory.
    -Top <n>                 Overrides the configured console row limit.
    -Open                    Opens the Markdown report.
    -NoOpen                  Does not open the HTML page.
    -NoPause                 Disables paging.
    -Help, -?                Shows this help.
'@ | Write-Host
    exit 0
}

$script:AuditGeneration = 27
$script:Utf8 = New-Object System.Text.UTF8Encoding($false)
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
Write-Host "AUDITMODULE - AUDIT GENERATION $script:AuditGeneration" -ForegroundColor Blue

function Get-AuditProperty {
    param($Node, [string]$Name, [string]$Owner)

    $property = @($Node.PSObject.Properties | Where-Object { $_.Name -ceq $Name })
    if ($property.Count -ne 1) { throw "The module-audit configuration must contain exactly one '$Name' key in $Owner." }
    return $property[0].Value
}

function Read-AuditConfig {
    param([string]$Path)

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { throw "The audit configuration was not found: $Path" }
    try { $config = Get-Content -LiteralPath $Path -Raw -Encoding UTF8 | ConvertFrom-Json }
    catch { throw "The audit configuration is not valid JSON: $Path`n$($_.Exception.Message)" }
    foreach ($name in @('generation', 'project', 'enforced', 'helper', 'source', 'modules', 'suggestions', 'ceilings', 'console', 'report')) {
        [void](Get-AuditProperty $config $name 'root')
    }
    if ([int]$config.generation -ne $script:AuditGeneration) {
        throw "The module-audit configuration is generation $($config.generation), but this tooling is generation $script:AuditGeneration."
    }
    return $config
}

function Resolve-AuditRoot {
    param([string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) { $Path = Split-Path -Parent $PSScriptRoot }
    $item = Get-Item -LiteralPath $Path -ErrorAction Stop
    if (-not $item.PSIsContainer) { throw "The project root is not a directory: $Path" }
    return $item.FullName.TrimEnd([char[]]@([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar))
}

function Read-AuditVersion {
    param([string]$ProjectRoot, $Config)

    $path = Join-Path $ProjectRoot ([string]$Config.report.versionFile)
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "Version file was not found: $path" }
    try { $data = Get-Content -LiteralPath $path -Raw -Encoding UTF8 | ConvertFrom-Json }
    catch { throw "Version file is not valid JSON: $path`n$($_.Exception.Message)" }
    $value = [string]$data.([string]$Config.report.versionKey)
    if ([string]::IsNullOrWhiteSpace($value)) { return 'unknown' }
    return $value
}

function Read-GraftFreshness {
    param([string]$ProjectRoot)

    $graft = Get-Command graft -ErrorAction SilentlyContinue
    if ($null -eq $graft) { return [pscustomobject]@{ Fresh = $false; Text = 'unavailable: graft executable not found' } }
    $nativePreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    $output = & $graft.Source check $ProjectRoot --json 2>&1
    $exitCode = $LASTEXITCODE
    $ErrorActionPreference = $nativePreference
    if ($exitCode -ne 0) { return [pscustomobject]@{ Fresh = $false; Text = "unavailable/incomplete: graft check exited $exitCode" } }
    try { $status = ($output -join [Environment]::NewLine) | ConvertFrom-Json }
    catch { return [pscustomobject]@{ Fresh = $false; Text = 'unavailable/incomplete: graft check returned invalid JSON' } }
    if ($null -eq $status -or -not ($status.PSObject.Properties.Name -contains 'graph')) {
        return [pscustomobject]@{ Fresh = $false; Text = 'unavailable/incomplete: graft check response has invalid schema' }
    }
    $graph = $status.graph
    $required = @('ok', 'added', 'removed', 'changed', 'stale')
    $valid = $null -ne $graph -and @($required | Where-Object { -not ($graph.PSObject.Properties.Name -contains $_) }).Count -eq 0
    $valid = $valid -and $graph.ok -is [bool] -and $graph.ok
    $valid = $valid -and @($required[1..4] | Where-Object { $graph.$_ -isnot [System.Array] }).Count -eq 0
    $fresh = $valid -and @($graph.added).Count -eq 0 -and @($graph.removed).Count -eq 0 -and @($graph.changed).Count -eq 0 -and @($graph.stale).Count -eq 0
    if (-not $fresh) { return [pscustomobject]@{ Fresh = $false; Text = 'unavailable/incomplete: wiring graph is missing or stale' } }
    return [pscustomobject]@{ Fresh = $true; Text = 'fresh wiring schema 1' }
}

function Get-AuditHelper {
    param([string]$Dotnet, [string]$Framework, [string]$Project)

    $nativePreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    $sdkOutput = & $Dotnet --version 2>&1
    $ErrorActionPreference = $nativePreference
    if ($LASTEXITCODE -ne 0) { throw "The .NET SDK version could not be read.`n$($sdkOutput -join [Environment]::NewLine)" }
    $moduleSource = [System.IO.File]::ReadAllText((Join-Path $PSScriptRoot 'AuditModule.cs'))
    $binderSource = [System.IO.File]::ReadAllText((Join-Path $PSScriptRoot 'AuditBinder.cs'))
    $projectText = @'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>{FRAMEWORK}</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RestoreIgnoreFailedSources>true</RestoreIgnoreFailedSources>
    <NuGetAudit>false</NuGetAudit>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.14.0" />
  </ItemGroup>
</Project>
'@.Replace('{FRAMEWORK}', $Framework)
    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        $seed = $projectText + "`n" + $moduleSource + "`n" + $binderSource + "`n" + ([string]($sdkOutput | Select-Object -First 1)).Trim()
        $digest = $sha.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($seed))
    }
    finally { $sha.Dispose() }
    $hash = ([System.BitConverter]::ToString($digest) -replace '-', '').Substring(0, 16).ToLowerInvariant()
    $folder = Join-Path (Join-Path ([System.IO.Path]::GetTempPath()) ($Project + '-AuditModule')) $hash
    $binary = Join-Path (Join-Path $folder 'bin') 'AuditModule.Helper.dll'
    if (Test-Path -LiteralPath $binary -PathType Leaf) { return $binary }
    $helper = Join-Path $folder 'helper'
    [System.IO.Directory]::CreateDirectory($helper) | Out-Null
    [System.IO.File]::WriteAllText((Join-Path $helper 'AuditModule.Helper.csproj'), $projectText, $script:Utf8)
    [System.IO.File]::WriteAllText((Join-Path $helper 'Program.cs'), $moduleSource, $script:Utf8)
    [System.IO.File]::WriteAllText((Join-Path $helper 'AuditBinder.cs'), $binderSource, $script:Utf8)
    $nativePreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    $buildOutput = & $Dotnet build (Join-Path $helper 'AuditModule.Helper.csproj') --configuration Release --nologo --verbosity quiet --output (Join-Path $folder 'bin') 2>&1
    $ErrorActionPreference = $nativePreference
    if ($LASTEXITCODE -ne 0) { throw "The module binder could not be built.`n$($buildOutput -join [Environment]::NewLine)" }
    return $binary
}

$projectRoot = Resolve-AuditRoot $Root
$configPath = Join-Path $PSScriptRoot 'AuditModule.json'
$config = Read-AuditConfig $configPath
$version = Read-AuditVersion $projectRoot $config
if ($Top -gt 0) { $config.console.top = $Top }
$reportFolder = if ([string]::IsNullOrWhiteSpace($ReportDirectory)) { [string]$config.report.directory } else { $ReportDirectory }
if (-not [System.IO.Path]::IsPathRooted($reportFolder)) { $reportFolder = Join-Path $projectRoot $reportFolder }
[System.IO.Directory]::CreateDirectory($reportFolder) | Out-Null
$reportPath = Join-Path $reportFolder ([string]$config.report.prefix + $version + '.md')
$pagePath = Join-Path $reportFolder ([string]$config.report.prefix + $version + '.html')
$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if ($null -eq $dotnet) { throw 'The .NET SDK is required, but dotnet was not found on PATH.' }
$freshness = Read-GraftFreshness $projectRoot
$binary = Get-AuditHelper $dotnet.Source ([string]$config.helper.framework) ([string]$config.project)
$temporary = Join-Path ([System.IO.Path]::GetTempPath()) (([string]$config.project) + '-AuditModule-' + [Guid]::NewGuid().ToString('N'))
[System.IO.Directory]::CreateDirectory($temporary) | Out-Null

try {
    $pageData = Join-Path $temporary 'page.json'
    $arguments = @($binary, $configPath, $projectRoot, $version, $reportPath, (Join-Path $PSScriptRoot 'AuditBinder.json'), $pageData,
        $(if ($freshness.Fresh) { 'fresh' } else { 'unavailable' }), [string]$freshness.Text)
    $nativePreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    $output = & $dotnet.Source @arguments 2>&1
    $ErrorActionPreference = $nativePreference
    $exitCode = $LASTEXITCODE
    if ($exitCode -notin @(0, 3)) { throw "The module audit failed.`n$($output -join [Environment]::NewLine)" }
    foreach ($line in $output) { Write-Host ([string]$line) }
    if (-not (Test-Path -LiteralPath $pageData -PathType Leaf)) { throw "The module audit wrote no page data: $pageData" }
    $template = [System.IO.File]::ReadAllText((Join-Path $PSScriptRoot 'AuditModule.html'), $script:Utf8)
    if (-not $template.Contains('/*__DATA__*/') -or -not $template.Contains('__TITLE__')) { throw 'The module page template is missing a required marker.' }
    $data = [System.IO.File]::ReadAllText($pageData, $script:Utf8)
    $title = [System.Net.WebUtility]::HtmlEncode([string]$config.project)
    [System.IO.File]::WriteAllText($pagePath, $template.Replace('__TITLE__', $title).Replace('/*__DATA__*/', $data), $script:Utf8)
    Write-Host "Report: $pagePath"
}
finally {
    if (Test-Path -LiteralPath $temporary) { Remove-Item -LiteralPath $temporary -Recurse -Force -ErrorAction SilentlyContinue }
}

if ($Open -and (Test-Path -LiteralPath $reportPath -PathType Leaf)) { Invoke-Item -LiteralPath $reportPath }
if (-not $NoOpen -and (Test-Path -LiteralPath $pagePath -PathType Leaf)) { Start-Process -FilePath $pagePath }
exit ([int]($exitCode -ne 0))

