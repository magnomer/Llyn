<#
.SYNOPSIS
Audits that every draft record property is named on every side that carries a draft, or waived by name.

.DESCRIPTION
Reads the project configuration from auditdraft.json next to this script, then:
  1. Lists the record sources through git and parses them with Roslyn, keeping the positional
     parameters of every listed draft record type.
  2. For each side (portrait, markup, exemplar), joins the side's sources and looks every
     property up as a whole word. A property in the shared waiver is skipped outright.
  3. Reports:
       Unnamed properties  a property no source of the side names and no waiver of the side covers
       Stale waivers       a side waiver the side names anyway, or one naming no property
       Unmatched settings  a listed type that is no record under the include, or a shared waiver
                           that is no property of a draft record

Files come from git: tracked and untracked files, never ignored ones, as in the convention
tests. The console follows scripts\report.md. No report file is written, since the hit list is
the whole result.
The parse runs in a small helper compiled once against Roslyn 4.14.0, the version the convention
tests pin, and cached under the temp folder. Git and the .NET SDK are required.
under the temp folder, so nothing is restored from a feed. Git and the .NET SDK are required.

This script carries no project-specific value of its own. Everything a project chooses - the
record include, the types, the waivers and the sides - lives in auditdraft.json.

auditdraft.json shape:
  {
    "generation": 15,
    "project": "Llyn",
    "records": {
      "include": ["src/Llyn.Core/Lexicon/*.cs"],
      "types": ["LCardDraft"],
      "waiver": ["LCardDraftId"]
    },
    "sides": [
      { "name": "portrait", "include": ["src/Llyn.Core/Portrait/*.cs"], "waiver": [] }
    ]
  }

.PARAMETER Root
Project root to audit. Defaults to the directory containing this script's parent.

.PARAMETER ConfigPath
Path to the JSON configuration. Defaults to auditdraft.json next to this script.

.PARAMETER Help
Display this help and exit. The alias -? is supported.

.EXAMPLE
auditdraft
Audit the current checkout.
#>
#requires -Version 5.1
# AUDITDRAFT GENERATION 15 - auditdraft.ps1.
# A generation is not a revision count. It names functionality, not edits, so editing one of these
# files is never on its own a reason to raise it. Raise it only when the audited outcome changes.
# A generation names the set of checks the audit applies. Two projects on the same generation audit
# the same things and their reports compare directly, whatever else differs between the files. A check
# added, removed, or changed in what it reports is a new generation; wording, plumbing, and refactoring
# leave it alone. Every audit script shares one generation number with the convention-test settings,
# and each refuses a configuration written at another generation.
# Generation 12: the first generation of this audit. It reports the four facts of the convention test.
# Generation 13: nothing this audit reports changes; the number rises with the UI audit, which
# counts every surface markup line that hooks logic into the markup and every surface member
# that is not a constructor.
# Generation 14: nothing this audit reports changes; the number rises with the UI audit, which
# also counts command parameters, member paths and literal tags in surface markup as hooks.
# Generation 15: nothing this audit reports changes; the number rises with the name audit, which
# counts prefix rings, and the UI audit, which counts pack URIs, scaffold types and contract IDs.
[CmdletBinding()]
param(
    [string]$Root,
    [string]$ConfigPath,
    [Alias('?')]
    [switch]$Help
)

if ($Help) {
    @'
NAME
    auditdraft.ps1

SYNOPSIS
    Audit that every positional property of every draft record is named by
    the portrait, the markup and the exemplar, or waived by name.

SYNTAX
    auditdraft [-Root <path>] [-ConfigPath <path>] [-Help]

CONFIGURATION
    All project-specific values live in auditdraft.json next to the script:
    the record include and types, the shared waiver, and each side with its
    include and its own waiver.

RESULT
    Unnamed properties  A property a side neither names nor waives.
    Stale waivers       A side waiver the side names, or one naming no property.
    Unmatched settings  A type that is no record, or a shared waiver naming no property.

    The exit code is 1 when any Result gate is above 0 and 0 otherwise.

OPTIONS
    -Root <path>
        Project root to audit. Defaults to the parent of the script folder.

    -ConfigPath <path>
        JSON configuration file. Defaults to .\auditdraft.json.

    -Help, -?
        Display this help and exit without running the audit.

EXAMPLES
    auditdraft
        Audit the current checkout.
'@ | Write-Host
    exit 0
}

# Under Windows PowerShell 5.1 an advanced script evaluates a parameter default before
# $PSScriptRoot is available to it, so the defaults are resolved here.
if ([string]::IsNullOrWhiteSpace($Root)) {
    $Root = Split-Path -Parent $PSScriptRoot
}
if ([string]::IsNullOrWhiteSpace($ConfigPath)) {
    $ConfigPath = Join-Path $PSScriptRoot 'auditdraft.json'
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [Console]::OutputEncoding

$script:AuditGeneration = 15
$script:TargetFramework = 'net10.0'
$script:Invariant = [System.Globalization.CultureInfo]::InvariantCulture

Write-Host "AUDITDRAFT GENERATION $script:AuditGeneration" -ForegroundColor Blue

$script:HelperProject = @'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>{TARGET_FRAMEWORK}</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RestoreIgnoreFailedSources>true</RestoreIgnoreFailedSources>
    <NuGetAudit>false</NuGetAudit>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.14.0" />
  </ItemGroup>
</Project>
'@

$script:HelperProgram = @'
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

if (args.Length != 2)
{
    Console.Error.WriteLine("usage: <request> <output>");
    return 2;
}

using JsonDocument request = JsonDocument.Parse(File.ReadAllText(args[0]));
string[] files = request.RootElement.GetProperty("files").EnumerateArray().Select(item => item.GetString() ?? "").ToArray();
HashSet<string> types = new(
    request.RootElement.GetProperty("types").EnumerateArray().Select(item => item.GetString() ?? ""),
    StringComparer.Ordinal);

CSharpParseOptions options = new(
    languageVersion: LanguageVersion.Preview,
    documentationMode: DocumentationMode.None,
    kind: SourceCodeKind.Regular);

List<string> order = [];
Dictionary<string, List<string>> records = new(StringComparer.Ordinal);
foreach (string path in files)
{
    SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(path), options, path);
    foreach (RecordDeclarationSyntax record in tree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>())
    {
        string type = record.Identifier.ValueText;
        if (!types.Contains(type) || record.ParameterList is null)
        {
            continue;
        }

        if (!records.ContainsKey(type))
        {
            order.Add(type);
        }

        records[type] = record.ParameterList.Parameters.Select(parameter => parameter.Identifier.ValueText).ToList();
    }
}

var result = order.Select(type => new { type, names = records[type] }).ToList();
File.WriteAllText(args[1], JsonSerializer.Serialize(result), new System.Text.UTF8Encoding(false));
return 0;
'@

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

function Read-AuditConfig {
    param([Parameter(Mandatory = $true)][string]$Path)

    $pathFull = [System.IO.Path]::GetFullPath($Path)
    if (-not (Test-Path -LiteralPath $pathFull -PathType Leaf)) {
        throw "The draft-audit configuration was not found: $pathFull"
    }

    try {
        $config = [System.IO.File]::ReadAllText($pathFull, [System.Text.UTF8Encoding]::new($false)) | ConvertFrom-Json
    }
    catch {
        throw "The draft-audit configuration is not valid JSON: $pathFull`n$($_.Exception.Message)"
    }

    $problems = [System.Collections.Generic.List[string]]::new()
    foreach ($key in @('generation', 'project', 'records.include', 'records.types', 'records.waiver', 'sides')) {
        if ($null -eq (Get-ConfigNode -Document $config -Key $key)) {
            $problems.Add("missing key '$key'")
        }
    }

    $index = 0
    $sideNodes = if ($config.PSObject.Properties.Name -contains 'sides') { @($config.sides) } else { @() }
    foreach ($side in $sideNodes) {
        $index++
        if ($null -eq $side) { continue }
        foreach ($key in @('name', 'include', 'waiver')) {
            if ($null -eq (Get-ConfigNode -Document $side -Key $key)) {
                $problems.Add("side $index needs '$key'")
            }
        }
    }

    if ($problems.Count -gt 0) {
        throw "The draft-audit configuration is not valid: $pathFull`n  " + ($problems -join "`n  ")
    }

    if ([int]$config.generation -ne $script:AuditGeneration) {
        throw "The draft-audit configuration is generation $($config.generation) but this tooling is generation $script:AuditGeneration.`nA generation names the set of checks applied, so the two must match: $pathFull"
    }

    return $config
}

function Resolve-ProjectRoot {
    param([Parameter(Mandatory = $true)][string]$Path)

    $item = Get-Item -LiteralPath $Path -ErrorAction SilentlyContinue
    if ($null -eq $item -or -not $item.PSIsContainer) {
        throw "The project root is not a directory: $Path"
    }

    return $item.FullName.TrimEnd([char[]]@('\', '/'))
}

function ConvertTo-ArgumentText {
    param([Parameter(Mandatory = $true)][AllowEmptyString()][string]$Value)

    if ($Value.Length -eq 0 -or $Value -match '[\s"]') {
        return '"' + (($Value -replace '(\\*)"', '$1$1\"') -replace '(\\+)$', '$1$1') + '"'
    }

    return $Value
}

function Get-GitFiles {
    # The same enumeration as the convention tests: git lists tracked and untracked files matching
    # the pathspecs without case, files gone from disk drop out, and the full paths sort ordinally
    # without case.
    param(
        [Parameter(Mandatory = $true)][string]$ProjectRoot,
        [Parameter(Mandatory = $true)][string[]]$Include
    )

    $arguments = @('-c', 'core.quotePath=false', 'ls-files', '--cached', '--others', '--exclude-standard', '--') +
        @($Include | ForEach-Object { ':(icase)' + $_ })
    $info = New-Object System.Diagnostics.ProcessStartInfo
    $info.FileName = 'git'
    $info.WorkingDirectory = $ProjectRoot
    $info.RedirectStandardOutput = $true
    $info.RedirectStandardError = $true
    $info.UseShellExecute = $false
    $info.StandardOutputEncoding = [System.Text.UTF8Encoding]::new($false)
    $info.StandardErrorEncoding = [System.Text.UTF8Encoding]::new($false)
    $info.Arguments = (@($arguments | ForEach-Object { ConvertTo-ArgumentText $_ })) -join ' '

    $process = [System.Diagnostics.Process]::Start($info)
    $errorTask = $process.StandardError.ReadToEndAsync()
    $output = $process.StandardOutput.ReadToEnd()
    $errorText = $errorTask.GetAwaiter().GetResult()
    $process.WaitForExit()
    if ($process.ExitCode -ne 0) {
        throw "Git failed to enumerate source files.`n$errorText"
    }

    $separator = [System.IO.Path]::DirectorySeparatorChar
    $seen = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    $files = [System.Collections.Generic.List[string]]::new()
    foreach ($line in $output.Split("`n")) {
        $relative = $line.Trim()
        if ($relative.Length -eq 0) { continue }
        $full = [System.IO.Path]::Combine($ProjectRoot, $relative.Replace('/', $separator))
        if (-not $seen.Add($full) -or -not [System.IO.File]::Exists($full)) { continue }
        $files.Add($full)
    }

    $sorted = $files.ToArray()
    [System.Array]::Sort($sorted, [System.StringComparer]::OrdinalIgnoreCase)
    return , $sorted
}

function Get-HelperCacheFolder {
    param(
        [Parameter(Mandatory = $true)][string]$ProjectName,
        [Parameter(Mandatory = $true)][string]$SdkVersion
    )

    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        $seed = $script:HelperProject + "`n" + $script:HelperProgram + "`n" + $script:TargetFramework + "`n" + $SdkVersion
        $digest = $sha.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($seed))
    }
    finally {
        $sha.Dispose()
    }

    $hash = ([System.BitConverter]::ToString($digest) -replace '-', '').Substring(0, 16).ToLowerInvariant()
    return Join-Path ([System.IO.Path]::GetTempPath()) ($ProjectName + '-AuditDraft' + [System.IO.Path]::DirectorySeparatorChar + $hash)
}

function Invoke-AuditHelper {
    # Compiles the parse helper once per text and SDK, then hands it the files and types and
    # reads back the records in the order the test's dictionary holds them.
    param(
        [Parameter(Mandatory = $true)][string]$ProjectName,
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][string[]]$Files,
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][string[]]$Types
    )

    $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($null -eq $dotnet) {
        throw 'The .NET SDK is required, but dotnet was not found on PATH.'
    }

    $previousNoLogo = $env:DOTNET_NOLOGO
    $previousTelemetry = $env:DOTNET_CLI_TELEMETRY_OPTOUT
    $env:DOTNET_NOLOGO = '1'
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
    $temporaryFolder = Join-Path ([System.IO.Path]::GetTempPath()) ($ProjectName + '-AuditDraft-' + [Guid]::NewGuid().ToString('N'))
    [System.IO.Directory]::CreateDirectory($temporaryFolder) | Out-Null
    $nativePreference = $ErrorActionPreference

    try {
        $ErrorActionPreference = 'Continue'
        $sdkOutput = & $dotnet.Source --version 2>&1
        $sdkExit = $LASTEXITCODE
        $ErrorActionPreference = $nativePreference
        if ($sdkExit -ne 0) {
            throw "The .NET SDK version could not be read.`n$($sdkOutput -join [Environment]::NewLine)"
        }

        $sdkVersion = ([string]($sdkOutput | Select-Object -First 1)).Trim()
        $cacheFolder = Get-HelperCacheFolder -ProjectName $ProjectName -SdkVersion $sdkVersion
        $binaryPath = Join-Path (Join-Path $cacheFolder 'bin') 'AuditDraft.dll'
        if (-not (Test-Path -LiteralPath $binaryPath -PathType Leaf)) {
            Write-Host 'Compiling the draft parser once for this SDK...'
            $cacheParent = Split-Path -Parent $cacheFolder
            if (Test-Path -LiteralPath $cacheParent) {
                Get-ChildItem -LiteralPath $cacheParent -Directory |
                    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
            }

            $helperFolder = Join-Path $cacheFolder 'helper'
            [System.IO.Directory]::CreateDirectory($helperFolder) | Out-Null
            $encoding = [System.Text.UTF8Encoding]::new($false)
            $projectPath = Join-Path $helperFolder 'AuditDraft.csproj'
            [System.IO.File]::WriteAllText($projectPath, $script:HelperProject.Replace('{TARGET_FRAMEWORK}', $script:TargetFramework), $encoding)
            [System.IO.File]::WriteAllText((Join-Path $helperFolder 'Program.cs'), $script:HelperProgram, $encoding)
            $buildArguments = @('build', $projectPath, '--configuration', 'Release', '--nologo', '--verbosity', 'quiet',
                '--output', (Join-Path $cacheFolder 'bin'))
            $ErrorActionPreference = 'Continue'
            $buildOutput = & $dotnet.Source @buildArguments 2>&1
            $buildExit = $LASTEXITCODE
            $ErrorActionPreference = $nativePreference
            if ($buildExit -ne 0) {
                Remove-Item -LiteralPath $cacheFolder -Recurse -Force -ErrorAction SilentlyContinue
                throw "The draft parser could not be built.`n$($buildOutput -join [Environment]::NewLine)"
            }
        }

        $requestPath = Join-Path $temporaryFolder 'request.json'
        $outputPath = Join-Path $temporaryFolder 'records.json'
        $request = [ordered]@{ files = @($Files); types = @($Types) }
        [System.IO.File]::WriteAllText($requestPath, (ConvertTo-Json -InputObject $request -Depth 4), [System.Text.UTF8Encoding]::new($false))

        $ErrorActionPreference = 'Continue'
        $helperOutput = & $dotnet.Source $binaryPath $requestPath $outputPath 2>&1
        $helperExit = $LASTEXITCODE
        $ErrorActionPreference = $nativePreference
        if ($helperExit -ne 0) {
            throw "The draft parser failed.`n$($helperOutput -join [Environment]::NewLine)"
        }

        $json = [System.IO.File]::ReadAllText($outputPath, [System.Text.UTF8Encoding]::new($false))
        $parsed = ConvertFrom-Json -InputObject $json
        return , [object[]]@($parsed | Where-Object { $null -ne $_ })
    }
    finally {
        $ErrorActionPreference = $nativePreference
        $env:DOTNET_NOLOGO = $previousNoLogo
        $env:DOTNET_CLI_TELEMETRY_OPTOUT = $previousTelemetry
        if (Test-Path -LiteralPath $temporaryFolder) {
            Remove-Item -LiteralPath $temporaryFolder -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
}

function Test-NameUse {
    param([Parameter(Mandatory = $true)][AllowEmptyString()][string]$Text, [Parameter(Mandatory = $true)][string]$Name)

    return [regex]::IsMatch($Text, '\b' + [regex]::Escape($Name) + '\b', [System.Text.RegularExpressions.RegexOptions]::CultureInvariant)
}

function Write-SectionTitle {
    param([Parameter(Mandatory = $true)][string]$Text)

    Write-Host ''
    Write-Host $Text -ForegroundColor Blue
    Write-Host ('-' * $Text.Length) -ForegroundColor DarkGray
}

function Write-ResultTable {
    param([Parameter(Mandatory = $true)][object[]]$Rows)

    Write-SectionTitle 'Result'
    $statusWidth = 6
    $countWidth = [Math]::Max(5, ($Rows | ForEach-Object { (Format-Integer $_.Count).Length } | Measure-Object -Maximum).Maximum)
    $gateWidth = [Math]::Max(4, ($Rows | ForEach-Object { $_.Gate.Length } | Measure-Object -Maximum).Maximum)
    $meaningWidth = [Math]::Max(7, ($Rows | ForEach-Object { $_.Meaning.Length } | Measure-Object -Maximum).Maximum)
    Write-Host ('{0}  {1}  {2}  Meaning' -f 'Status'.PadRight($statusWidth), 'Count'.PadLeft($countWidth), 'Gate'.PadRight($gateWidth)) -ForegroundColor Cyan
    Write-Host (@(('-' * $statusWidth), ('-' * $countWidth), ('-' * $gateWidth), ('-' * $meaningWidth)) -join '  ') -ForegroundColor Cyan
    foreach ($row in $Rows) {
        $failing = $row.Count -gt 0
        $status = if ($failing) { 'FAIL' } else { 'OK' }
        $text = '{0}  {1}  {2}  {3}' -f $status.PadRight($statusWidth), (Format-Integer $row.Count).PadLeft($countWidth), $row.Gate.PadRight($gateWidth), $row.Meaning
        Write-Host $status -ForegroundColor $(if ($failing) { 'Red' } else { 'Green' }) -NoNewline
        Write-Host $text.Substring($status.Length)
    }

    $failed = @($Rows | Where-Object { $_.Count -gt 0 })
    Write-Host ''
    if ($failed.Count -eq 0) {
        Write-Host ('PASS: all {0} gates at 0.' -f $Rows.Count) -ForegroundColor Green
    }
    else {
        $sections = ($failed | ForEach-Object { '"' + $_.Section + '"' }) -join ', '
        Write-Host ('FAIL: {0} of {1} gates above 0. See {2}.' -f $failed.Count, $Rows.Count, $sections) -ForegroundColor Red
    }
}

function Format-Integer {
    param([long]$Value)

    return $Value.ToString('N0', $script:Invariant)
}

function Get-TextList {
    param($Node)

    return , [string[]]@(@($Node) | Where-Object { $null -ne $_ } | ForEach-Object { [string]$_ })
}

$projectRoot = Resolve-ProjectRoot -Path $Root
$config = Read-AuditConfig -Path $ConfigPath

$recordInclude = Get-TextList $config.records.include
$types = Get-TextList $config.records.types
$sharedWaiver = Get-TextList $config.records.waiver
$sides = @($config.sides)

$recordFiles = Get-GitFiles -ProjectRoot $projectRoot -Include $recordInclude
$records = Invoke-AuditHelper -ProjectName ([string]$config.project) -Files $recordFiles -Types $types

$recordNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
$properties = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
foreach ($record in $records) {
    [void]$recordNames.Add([string]$record.type)
    foreach ($name in (Get-TextList $record.names)) { [void]$properties.Add($name) }
}

$unnamed = [System.Collections.Generic.List[string]]::new()
$stale = [System.Collections.Generic.List[string]]::new()
$unmatched = [System.Collections.Generic.List[string]]::new()
$sideFiles = 0

if ($types.Length -gt 0) {
    $shared = [System.Collections.Generic.HashSet[string]]::new($sharedWaiver, [System.StringComparer]::Ordinal)
    foreach ($side in $sides) {
        $sideName = [string]$side.name
        $include = Get-TextList $side.include
        $waiver = Get-TextList $side.waiver
        $files = Get-GitFiles -ProjectRoot $projectRoot -Include $include
        if ($files.Length -eq 0) {
            throw "No tracked file matches $($include -join ' ')."
        }
        $sideFiles += $files.Length
        $text = [string]::Join("`n", [string[]]@($files | ForEach-Object { [System.IO.File]::ReadAllText($_) }))
        $waived = [System.Collections.Generic.HashSet[string]]::new($waiver, [System.StringComparer]::Ordinal)
        foreach ($record in $records) {
            foreach ($name in (Get-TextList $record.names)) {
                if ($shared.Contains($name)) { continue }
                $named = Test-NameUse -Text $text -Name $name
                if ($waived.Contains($name)) {
                    if ($named) { $stale.Add("${sideName}: $name is waived for the $sideName but the $sideName names it") }
                }
                elseif (-not $named) {
                    $unnamed.Add("${sideName}: $($record.type).$name")
                }
            }
        }
        foreach ($name in $waiver) {
            if (-not $properties.Contains($name)) {
                $stale.Add("${sideName}: $name is waived for the $sideName but is no property of a draft record")
            }
        }
    }
}

foreach ($type in $types) {
    if (-not $recordNames.Contains($type)) {
        $unmatched.Add("$type is no record under $($recordInclude -join ' ')")
    }
}
foreach ($name in $sharedWaiver) {
    if (-not $properties.Contains($name)) {
        $unmatched.Add("$name is waived but is no property of a draft record")
    }
}

Write-Host ('Scanned: {0} record files, {1} draft records, {2} side files' -f (Format-Integer $recordFiles.Length), (Format-Integer $records.Count), (Format-Integer $sideFiles)) -ForegroundColor DarkGray

$kinds = @(
    @{ Label = 'Unnamed properties'; Hits = $unnamed },
    @{ Label = 'Stale waivers'; Hits = $stale },
    @{ Label = 'Unmatched settings'; Hits = $unmatched }
)

$meanings = @{
    'Unnamed properties' = 'properties a side neither names nor waives'
    'Stale waivers' = 'side waivers the side names, or naming no property'
    'Unmatched settings' = 'types that are no record, or dead shared waivers'
}
$total = 0
$gateRows = foreach ($kind in $kinds) {
    $total += $kind.Hits.Count
    [pscustomobject]@{ Gate = $kind.Label; Count = $kind.Hits.Count; Meaning = $meanings[$kind.Label]; Section = $kind.Label }
}
Write-ResultTable -Rows @($gateRows)

foreach ($kind in $kinds) {
    if ($kind.Hits.Count -eq 0) { continue }
    Write-SectionTitle ('{0} ({1})' -f $kind.Label, (Format-Integer $kind.Hits.Count))
    foreach ($hit in $kind.Hits) {
        Write-Host $hit
    }
}

if ($total -gt 0) {
    exit 1
}

exit 0
