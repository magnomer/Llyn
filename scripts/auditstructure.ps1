<#
.SYNOPSIS
Audit the ring chain of the source and report every dependency that steps past its neighbour.

.DESCRIPTION
Reads the ring table from auditstructure.json, enumerates the tracked source with Git, binds
every file with Roslyn, and classifies every symbol one ring names from another ring. A ring may
reach the one ring inside it that the table names for it; a data type of any inner ring may be
carried across any depth; nothing outward is ever named. A using of a deeper ring counts as data,
and a method call on a deeper record counts as behaviour. Outside the rings, each ring names only
the framework namespaces the table allows it, and a pure ring never touches an ambient member
such as the clock, the environment or the file system.

The reports are written to the configured report folder as AuditStructure-{version}.md and
AuditStructureViolated-{version}.md. Git and the .NET SDK are required, and the solution must be
built, since the shared binder of auditbinder.cs reads the generated code and the host build output
and refuses to bind a tree with a compile error. No project source is modified.

This script carries no project-specific value of its own, so the file is identical in every
project at the same generation. Everything a project chooses - the rings, their reach, their
frame, the composition root, the ceilings, the exemptions - lives in auditstructure.json.

.PARAMETER Root
Project root to audit. Defaults to the directory containing this script's parent.

.PARAMETER Open
Open the violation report after the audit finishes.

.PARAMETER NoPause
Do not stop at each console page. Paging is also off when output or input is redirected.

.PARAMETER Help
Display this help and exit without running the audit. The alias -? is supported.

.EXAMPLE
auditstructure
Audit the current checkout.

.EXAMPLE
auditstructure -Root C:\path\to\project -Open
Audit a specific checkout and open the violation report.
#>
#requires -Version 5.1
# AUDITSTRUCTURE GENERATION 12 - auditstructure.ps1.
# A generation is not a revision count. It names functionality, not edits, so editing one of these
# files is never on its own a reason to raise it. Raise it only when the audited outcome changes.
# A generation names the set of checks the audit applies. Two projects on the same generation audit
# the same things and their reports compare directly, whatever else differs between the files. A check
# added, removed, or changed in what it reports is a new generation; wording, plumbing, and refactoring
# leave it alone. Every audit script shares one generation number with the convention-test settings,
# and each refuses a configuration written at another generation.
# Generation 10 binds the source with Roslyn and holds every ring to one reach: a ring names the
# behaviour of the single ring the table lets it reach, carries the data of any inner ring, and
# names nothing outward. A ring names only the framework namespaces its frame lists, and a pure
# ring never touches an ambient member. The composition root alone is free of the chain.
# A reach is a call, a construction, an implementation, a field or a base. A carry is a record, an
# enum, a struct or a delegate named in a signature or passed through. The distinction is the
# kind of the type named, read from the binder, not the position of the name in the line.
# A configuration is total: a missing key is an error, never a default, and an unknown key is an
# error rather than a silent no-op. A ceiling is the hit count a violating pair may hold; a count
# above fails, a ceiling above the count is stale and fails too, so a ceiling only walks down. An
# exemption clears a name only inside the file its row names, and a row that matched nothing is
# reported as stale and fails the run, never pruned.
# Generation 11: nothing the structure audit reports changes; the number rises with the truth audit,
# which checks that a deportment field reaches no request, keeps one writer, holds no logic and
# treats no engine data.
# Generation 12: the audit reports what the convention tests report. It binds through the shared
# binder of auditbinder.cs with no compile error, counts a ceiling in hits, counts a using of a
# deeper ring and a method call on a deeper record as a reach, walks using static names, and adds
# the expose, surface, stray, banned, floor and table checks.
[CmdletBinding()]
param(
    [string]$Root,
    [switch]$Open,
    [switch]$NoPause,
    [Alias('?')]
    [switch]$Help
)

if ([string]::IsNullOrWhiteSpace($Root)) {
    $Root = Split-Path -Parent $PSScriptRoot
}

if ($Help) {
    @'
NAME
    auditstructure.ps1

SYNOPSIS
    Audit the ring chain of the source and report every dependency that steps past its neighbour.

SYNTAX
    auditstructure [-Root <path>] [-Open] [-NoPause] [-Help]

OPTIONS
    -Root <path>
        Project root to audit. Defaults to the directory containing this script's parent.

    -Open
        Open the violation report after the audit finishes.

    -NoPause
        Do not stop at each console page for a key. Off by itself when output
        or input is redirected.

    -Help, -?
        Display this help and exit without running the audit.

OUTPUT
    The console follows scripts\report.md: counters, findings by check, pairs.
    <report folder>\AuditStructure-{version}.md
    <report folder>\AuditStructureViolated-{version}.md

EXIT STATUS
    0   Every violating pair sits at its ceiling, and no ceiling or exemption is stale.
    1   A pair is above its ceiling, a ceiling sits above its count, or an exemption matched nothing.

EXAMPLES
    auditstructure
        Audit the current checkout.

    auditstructure -Root C:\path\to\project -Open
        Audit a specific checkout and open the violation report.
'@ | Write-Host
    exit 0
}

# Under Windows PowerShell 5.1 an advanced script evaluates a parameter default before
# $PSScriptRoot is available to it, so -Root arrives empty there while pwsh 7 resolves it.
if ([string]::IsNullOrWhiteSpace($Root)) {
    $Root = Split-Path -Parent $PSScriptRoot
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$script:AuditGeneration = 12
$script:ConfigDocument = 'auditstructure.json'
$script:BinderSource = [System.IO.File]::ReadAllText((Join-Path $PSScriptRoot 'auditbinder.cs'))
$script:PathSeparators = [char[]]@([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar)

# Console paging. A page is one window of rows; the audit stops at each page boundary and waits
# for a key so the reader can inspect the output before it scrolls away. Any key shows the next
# page, Q shows the rest without stopping. Paging is off when -NoPause is given or when either
# stream is redirected, so a pipeline or a log file never blocks on a key.
$script:PageLimit = 0
$script:PageWidth = 0
$script:PageCount = 0
if (-not $NoPause) {
    try {
        if (-not [Console]::IsOutputRedirected -and -not [Console]::IsInputRedirected) {
            $script:PageLimit = [Math]::Max(0, $Host.UI.RawUI.WindowSize.Height - 2)
            $script:PageWidth = [Math]::Max(1, $Host.UI.RawUI.WindowSize.Width)
        }
    }
    catch {
        $script:PageLimit = 0
    }
}

function Get-OrdinalKey {
    # Sort-Object compares text by culture, which Windows PowerShell 5.1 and pwsh 7 order differently.
    # Uppercase hexadecimal UTF-16 code units compare alike under every culture, so this key sorts ordinally.
    param([string]$Text)
    return [System.BitConverter]::ToString([System.Text.Encoding]::BigEndianUnicode.GetBytes($Text)).Replace('-', '')
}

function Write-AuditLine {
    param(
        [Parameter(Position = 0)][AllowEmptyString()][string]$Text = '',
        [ConsoleColor]$ForegroundColor
    )

    if ($script:PageLimit -gt 0) {
        $rows = [Math]::Max(1, [Math]::Ceiling($Text.Length / [double]$script:PageWidth))
        if ($script:PageCount + $rows -gt $script:PageLimit -and $script:PageCount -gt 0) {
            $prompt = '-- More -- (any key: next page, Q: no more pauses)'
            Write-Host $prompt -ForegroundColor Yellow -NoNewline
            $key = [Console]::ReadKey($true)
            Write-Host ("`r" + (' ' * $prompt.Length) + "`r") -NoNewline
            if ($key.Key -eq [ConsoleKey]::Q) {
                $script:PageLimit = 0
            }
            $script:PageCount = 0
        }
        $script:PageCount += $rows
    }

    if ($PSBoundParameters.ContainsKey('ForegroundColor')) {
        Write-Host $Text -ForegroundColor $ForegroundColor
    }
    else {
        Write-Host $Text
    }
}

function Write-AuditSection {
    param([string]$Title)

    Write-AuditLine ''
    Write-AuditLine $Title
    Write-AuditLine ('-' * $Title.Length)
}

function Write-AuditTable {
    param([string[]]$Header, [object[]]$Rows)

    $widths = @(for ($column = 0; $column -lt $Header.Count; $column++) {
        $cells = @($Header[$column]) + @($Rows | ForEach-Object { [string]$_[$column] })
        ($cells | Measure-Object -Property Length -Maximum).Maximum
    })
    $numeric = @(for ($column = 0; $column -lt $Header.Count; $column++) {
        $Rows.Count -gt 0 -and @($Rows | Where-Object { [string]$_[$column] -notmatch '^(-|-?[\d,]+(\.\d+)?( %)?)$' }).Count -eq 0
    })
    $format = {
        param([string[]]$Cells)
        $parts = for ($column = 0; $column -lt $Cells.Count; $column++) {
            if ($numeric[$column]) { $Cells[$column].PadLeft($widths[$column]) } else { $Cells[$column].PadRight($widths[$column]) }
        }
        ($parts -join '  ').TrimEnd()
    }
    Write-AuditLine (& $format $Header)
    Write-AuditLine (($widths | ForEach-Object { '-' * $_ }) -join '  ')
    foreach ($row in $Rows) {
        Write-AuditLine (& $format ([string[]]$row))
    }
}

Write-AuditLine "AUDITSTRUCTURE GENERATION $script:AuditGeneration" -ForegroundColor Cyan

$script:ReportName = 'AuditStructure-{version}.md'
$script:ViolationName = 'AuditStructureViolated-{version}.md'
$script:VersionKey = 'current-version'

# Each entry is a path through the document and the kind of value that must be found there.
# 'string', 'int', 'string[]', 'ring[]' for the ring table, 'map[]' for an object whose every
# property is an array of strings, or 'map[int]' for an object whose every property is an integer.
$script:AuditSchema = [ordered]@{
    'generation'                = 'int'
    'project'                   = 'string'
    'rings'                     = 'ring[]'
    'root'                      = 'string[]'
    'ambient'                   = 'string[]'
    'checks.neighbour'          = 'string'
    'checks.reach'              = 'string'
    'checks.cross'              = 'string'
    'checks.carry'              = 'string'
    'checks.outward'            = 'string'
    'checks.frame'              = 'string'
    'checks.ambient'            = 'string'
    'checks.root'               = 'string'
    'checks.expose'             = 'string'
    'checks.surface'            = 'string'
    'checks.stray'              = 'string'
    'checks.banned'             = 'string'
    'checks.floor'              = 'string'
    'checks.table'              = 'string'
    'surface'                   = 'map[]'
    'stray'                     = 'map[]'
    'banned'                    = 'map[]'
    'floor'                     = 'map[int]'
    'ceilings'                  = 'map[int]'
    'exempt'                    = 'map[]'
    'sources.include'           = 'string[]'
    'sources.excludeSegments'   = 'string[]'
    'sources.excludeSuffixes'   = 'string[]'
    'sources.excludePrefixes'   = 'string[]'
    'report.directory'          = 'string'
    'report.versionFile'        = 'string'
}

$script:RingSchema = [ordered]@{
    'name'  = 'string'
    'path'  = 'string'
    'reach' = 'string[]'
    'frame' = 'string[]'
    'pure'  = 'bool'
    'cut'   = 'bool'
}

$script:Severities = @('violation', 'review', 'allow')

# The order is the order the checks are reported in, heaviest first.
$script:CheckOrder = @('outward', 'reach', 'cross', 'expose', 'surface', 'frame', 'ambient', 'stray', 'banned', 'floor', 'table', 'carry', 'neighbour', 'root')

$script:CheckTitles = @{
    'outward'   = 'Outer ring named from an inner ring'
    'reach'     = 'Behaviour reached past the neighbour ring'
    'cross'     = 'Name from below the cut inside a UI ring'
    'expose'    = 'Surface signature naming a type from below its neighbour'
    'surface'   = 'Neighbour name outside the surface the ring may reach'
    'stray'     = 'Type declared in a ring that may not hold it'
    'banned'    = 'Banned word inside a ring folder'
    'floor'     = 'Ring holding fewer source files than its floor'
    'table'     = 'Ring reach differing from its project references'
    'frame'     = 'Framework namespace outside the ring frame'
    'ambient'   = 'Ambient member touched from a pure ring'
    'carry'     = 'Data carried from a deeper ring'
    'neighbour' = 'Neighbour ring reached'
    'root'      = 'Composition root reach'
}

function Resolve-ProjectRoot {
    param([string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        throw 'The project root is empty.'
    }

    $item = Get-Item -LiteralPath $Path -ErrorAction SilentlyContinue
    if ($null -eq $item) {
        throw "The project root was not found: $Path"
    }

    if (-not $item.PSIsContainer) {
        throw "The project root is not a directory: $Path"
    }

    return $item.FullName.TrimEnd($script:PathSeparators)
}

function Join-AuditPath {
    # A configured path is written with forward slashes and is always relative to the project root.
    param(
        [Parameter(Mandatory = $true)][string]$ProjectRoot,
        [Parameter(Mandatory = $true)][string]$Relative
    )

    $native = $Relative.Replace('/', [System.IO.Path]::DirectorySeparatorChar).Replace('\', [System.IO.Path]::DirectorySeparatorChar)
    return Join-Path $ProjectRoot $native
}

function Get-AuditNode {
    param(
        [Parameter(Mandatory = $true)]$Document,
        [Parameter(Mandatory = $true)][string]$Key
    )

    $node = $Document
    foreach ($segment in $Key.Split('.')) {
        if ($null -eq $node -or -not ($node.PSObject.Properties.Name -contains $segment)) {
            return @{ Found = $false; Value = $null }
        }

        $node = $node.$segment
    }

    return @{ Found = $true; Value = $node }
}

function Get-AuditKeyPaths {
    # Every leaf path present in the document, so an unknown key is reported rather than ignored.
    param(
        [Parameter(Mandatory = $true)]$Node,
        [string]$Prefix = ''
    )

    $paths = New-Object 'System.Collections.Generic.List[string]'
    foreach ($property in $Node.PSObject.Properties) {
        $path = if ($Prefix.Length -eq 0) { $property.Name } else { $Prefix + '.' + $property.Name }

        $isBranch = $null -ne $property.Value -and
            $property.Value -is [System.Management.Automation.PSCustomObject] -and
            -not $script:AuditSchema.Contains($path)

        if ($isBranch) {
            foreach ($child in (Get-AuditKeyPaths -Node $property.Value -Prefix $path)) {
                [void]$paths.Add($child)
            }
        }
        else {
            [void]$paths.Add($path)
        }
    }

    return , $paths.ToArray()
}

function Test-StringArray {
    param($Value)

    if ($null -eq $Value -or -not ($Value -is [System.Array])) { return $false }
    foreach ($item in $Value) {
        if (-not ($item -is [string])) { return $false }
    }

    return $true
}

function Test-AuditValue {
    param(
        [Parameter(Mandatory = $true)][string]$Kind,
        $Value
    )

    switch ($Kind) {
        'string' { return $Value -is [string] -and -not [string]::IsNullOrWhiteSpace($Value) }
        'int' { return $Value -is [int] -or $Value -is [long] }
        'bool' { return $Value -is [bool] }
        'string[]' { return Test-StringArray -Value $Value }
        'ring[]' {
            if ($null -eq $Value -or -not ($Value -is [System.Array]) -or $Value.Count -eq 0) { return $false }
            foreach ($ring in $Value) {
                if ($null -eq $ring -or -not ($ring -is [System.Management.Automation.PSCustomObject])) { return $false }
                foreach ($key in $script:RingSchema.Keys) {
                    if (-not ($ring.PSObject.Properties.Name -contains $key)) { return $false }
                    if (-not (Test-AuditValue -Kind $script:RingSchema[$key] -Value $ring.$key)) { return $false }
                }

                foreach ($property in $ring.PSObject.Properties) {
                    if (-not $script:RingSchema.Contains($property.Name)) { return $false }
                }
            }

            return $true
        }
        'map[]' {
            if ($null -eq $Value -or -not ($Value -is [System.Management.Automation.PSCustomObject])) { return $false }
            foreach ($property in $Value.PSObject.Properties) {
                if (-not (Test-StringArray -Value $property.Value)) { return $false }
            }

            return $true
        }
        'map[int]' {
            if ($null -eq $Value -or -not ($Value -is [System.Management.Automation.PSCustomObject])) { return $false }
            foreach ($property in $Value.PSObject.Properties) {
                if (-not ($property.Value -is [int] -or $property.Value -is [long]) -or $property.Value -lt 0) { return $false }
            }

            return $true
        }
        default { throw "Unknown schema kind: $Kind" }
    }
}

function Read-AuditConfig {
    param([Parameter(Mandatory = $true)][string]$ProjectRoot)

    $configPath = Join-Path $PSScriptRoot $script:ConfigDocument
    if (-not (Test-Path -LiteralPath $configPath -PathType Leaf)) {
        throw "The structure configuration was not found: $configPath"
    }

    try {
        $config = Get-Content -LiteralPath $configPath -Raw -Encoding UTF8 | ConvertFrom-Json
    }
    catch {
        throw "The structure configuration is not valid JSON: $configPath`n$($_.Exception.Message)"
    }

    $problems = New-Object 'System.Collections.Generic.List[string]'

    foreach ($key in $script:AuditSchema.Keys) {
        $node = Get-AuditNode -Document $config -Key $key
        if (-not $node.Found) {
            [void]$problems.Add("missing key: $key")
            continue
        }

        if (-not (Test-AuditValue -Kind $script:AuditSchema[$key] -Value $node.Value)) {
            [void]$problems.Add("key '$key' must be $($script:AuditSchema[$key])")
        }
    }

    foreach ($path in (Get-AuditKeyPaths -Node $config)) {
        if (-not $script:AuditSchema.Contains($path)) {
            [void]$problems.Add("unknown key: $path")
        }
    }

    foreach ($check in $script:CheckOrder) {
        $node = Get-AuditNode -Document $config -Key "checks.$check"
        if ($node.Found -and $node.Value -is [string] -and -not ($script:Severities -contains $node.Value)) {
            [void]$problems.Add("key 'checks.$check' must be one of: " + ($script:Severities -join ', '))
        }
    }

    $ringsNode = Get-AuditNode -Document $config -Key 'rings'
    if ($ringsNode.Found -and (Test-AuditValue -Kind 'ring[]' -Value $ringsNode.Value)) {
        $names = @($ringsNode.Value | ForEach-Object { [string]$_.name })
        $duplicates = @($names | Group-Object | Where-Object { $_.Count -gt 1 } | ForEach-Object { $_.Name })
        foreach ($duplicate in $duplicates) {
            [void]$problems.Add("ring '$duplicate' is declared more than once")
        }

        foreach ($ring in $ringsNode.Value) {
            foreach ($target in $ring.reach) {
                if (-not ($names -contains $target)) {
                    [void]$problems.Add("ring '$($ring.name)' reaches an undeclared ring '$target'")
                }

                if ($target -eq $ring.name) {
                    [void]$problems.Add("ring '$($ring.name)' reaches itself")
                }
            }
        }

        $rootNode = Get-AuditNode -Document $config -Key 'root'
        if ($rootNode.Found -and (Test-AuditValue -Kind 'string[]' -Value $rootNode.Value)) {
            $ringPaths = @($ringsNode.Value | ForEach-Object { ([string]$_.path).Replace('\', '/').Trim('/') + '/' })
            foreach ($entry in $rootNode.Value) {
                $relative = ([string]$entry).Replace('\', '/').Trim('/')
                if (-not @($ringPaths | Where-Object { $relative.StartsWith($_, [StringComparison]::OrdinalIgnoreCase) })) {
                    [void]$problems.Add("root '$entry' sits inside no ring, so no check ever reads it")
                }
                elseif (-not (Test-Path -LiteralPath (Join-Path $ProjectRoot $relative) -PathType Leaf)) {
                    [void]$problems.Add("root '$entry' names no file")
                }
            }
        }
    }

    $ceilingsNode = Get-AuditNode -Document $config -Key 'ceilings'
    if ($ceilingsNode.Found -and (Test-AuditValue -Kind 'map[int]' -Value $ceilingsNode.Value)) {
        foreach ($property in $ceilingsNode.Value.PSObject.Properties) {
            if ($property.Name -notmatch '^(outward|reach|cross|expose|surface|frame|ambient|stray|banned|floor|table|carry|neighbour|root):[^>]+>.+$') {
                [void]$problems.Add("ceiling '$($property.Name)' must be written as check:Ring>Target")
            }
        }
    }

    if ($problems.Count -gt 0) {
        throw "The structure configuration is not valid: $configPath`n  " + ($problems -join "`n  ")
    }

    if ($config.generation -ne $script:AuditGeneration) {
        throw "The structure configuration is generation $($config.generation) but this tooling is generation $script:AuditGeneration.`nA generation names the set of checks applied, so the two must match: $configPath"
    }

    return $config
}

function Read-ProjectVersion {
    param(
        [Parameter(Mandatory = $true)][string]$ProjectRoot,
        [Parameter(Mandatory = $true)][string]$Relative
    )

    $versionPath = Join-AuditPath -ProjectRoot $ProjectRoot -Relative $Relative
    if (-not (Test-Path -LiteralPath $versionPath -PathType Leaf)) {
        throw "The version file was not found: $versionPath"
    }

    try {
        $versionData = Get-Content -LiteralPath $versionPath -Raw -Encoding UTF8 | ConvertFrom-Json
    }
    catch {
        throw "The version file is not valid JSON: $versionPath`n$($_.Exception.Message)"
    }

    $version = [string]$versionData.($script:VersionKey)
    if ($version -notmatch '^\d+\.\d+\.\d+$') {
        throw "The version must contain three numeric components: '$version'"
    }

    return $version
}

function Test-ExcludedRelativePath {
    param(
        [Parameter(Mandatory = $true)][string]$RelativePath,
        [Parameter(Mandatory = $true)]$Config
    )

    $segments = $RelativePath -split '[\\/]'
    foreach ($segment in $segments) {
        if ($Config.sources.excludeSegments -contains $segment) {
            return $true
        }
    }

    $fileName = $segments[$segments.Length - 1]

    foreach ($suffix in $Config.sources.excludeSuffixes) {
        if ($fileName.EndsWith($suffix, [System.StringComparison]::OrdinalIgnoreCase)) {
            return $true
        }
    }

    foreach ($prefix in $Config.sources.excludePrefixes) {
        if ($fileName.StartsWith($prefix, [System.StringComparison]::Ordinal)) {
            return $true
        }
    }

    return $false
}

function Get-ProjectSourceFiles {
    param(
        [Parameter(Mandatory = $true)][string]$ProjectRoot,
        [Parameter(Mandatory = $true)]$Config
    )

    $git = Get-Command git -ErrorAction SilentlyContinue
    if ($null -eq $git) {
        throw 'Git is required to enumerate project source files, but git was not found on PATH.'
    }

    if (-not (Test-Path -LiteralPath (Join-Path $ProjectRoot '.git'))) {
        throw "The project root is not a Git working tree: $ProjectRoot"
    }

    $lsArguments = @('-c', 'core.quotePath=false', '-C', $ProjectRoot,
        'ls-files', '--cached', '--others', '--exclude-standard', '--') + @($Config.sources.include)
    $nativePreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    $gitOutput = & $git.Source @lsArguments 2>&1
    $ErrorActionPreference = $nativePreference
    $gitExitCode = $LASTEXITCODE
    if ($gitExitCode -ne 0) {
        throw "Git could not enumerate source files.`n$($gitOutput -join [Environment]::NewLine)"
    }

    $files = New-Object 'System.Collections.Generic.List[string]'
    foreach ($entry in $gitOutput) {
        $relativePath = ([string]$entry).Trim().Replace('\', '/')
        if ([string]::IsNullOrWhiteSpace($relativePath)) {
            continue
        }

        if (Test-ExcludedRelativePath -RelativePath $relativePath -Config $Config) {
            continue
        }

        $fullPath = Join-AuditPath -ProjectRoot $ProjectRoot -Relative $relativePath
        if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf)) {
            continue
        }

        [void]$files.Add($relativePath)
    }

    return @($files.ToArray() | Sort-Object -Property @{ Expression = { Get-OrdinalKey $_ } } -Unique)
}

function Get-PathRing {
    # The ring whose folder holds the file, the longest folder winning when one nests in another.
    param(
        [Parameter(Mandatory = $true)][string]$RelativePath,
        [Parameter(Mandatory = $true)]$Config
    )

    $best = $null
    $bestLength = -1
    foreach ($ring in $Config.rings) {
        $prefix = ([string]$ring.path).Replace('\', '/').Trim('/')
        if ($prefix.Length -le $bestLength) {
            continue
        }

        if ($RelativePath.StartsWith($prefix + '/', [System.StringComparison]::OrdinalIgnoreCase)) {
            $best = [string]$ring.name
            $bestLength = $prefix.Length
        }
    }

    return $best
}

function Test-NamePattern {
    # An entry is an exact name or a name ending in '*', which covers a family such as 'LState*'.
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Pattern
    )

    if ($Pattern.EndsWith('*')) {
        $stem = $Pattern.Substring(0, $Pattern.Length - 1)
        return $stem.Length -gt 0 -and $Name.StartsWith($stem, [System.StringComparison]::Ordinal)
    }

    return $Name.Equals($Pattern, [System.StringComparison]::Ordinal)
}

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
    <Reference Include="Microsoft.CodeAnalysis">
      <HintPath>$(MSBuildSDKsPath)/../Roslyn/bincore/Microsoft.CodeAnalysis.dll</HintPath>
      <Private>true</Private>
    </Reference>
    <Reference Include="Microsoft.CodeAnalysis.CSharp">
      <HintPath>$(MSBuildSDKsPath)/../Roslyn/bincore/Microsoft.CodeAnalysis.CSharp.dll</HintPath>
      <Private>true</Private>
    </Reference>
  </ItemGroup>
</Project>
'@

$script:HelperProgram = @'
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

if (args.Length != 4)
{
    Console.Error.WriteLine("usage: <config> <root> <binder> <output>");
    return 2;
}

string configPath = args[0];
string projectRoot = args[1];
string binderPath = args[2];
string outputPath = args[3];

JsonElement config = JsonDocument.Parse(File.ReadAllText(configPath)).RootElement;
string project = config.GetProperty("project").GetString()!;
List<Ring> rings = config.GetProperty("rings").EnumerateArray()
    .Select(ring => new Ring(
        ring.GetProperty("name").GetString()!,
        ring.GetProperty("path").GetString()!.Replace('\\', '/').Trim('/') + "/",
        Strings(ring.GetProperty("reach")),
        Strings(ring.GetProperty("frame")),
        ring.GetProperty("pure").GetBoolean(),
        ring.GetProperty("cut").GetBoolean()))
    .ToList();
HashSet<string> roots = Strings(config.GetProperty("root"))
    .Select(item => item.Replace('\\', '/').Trim('/'))
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
string[] ambient = Strings(config.GetProperty("ambient"));
Dictionary<string, string[]> surfaces = Map(config.GetProperty("surface"));
Dictionary<string, string[]> strays = Map(config.GetProperty("stray"));
Dictionary<string, string[]> banned = Map(config.GetProperty("banned"));
Dictionary<string, int> floors = config.GetProperty("floor").EnumerateObject()
    .ToDictionary(item => item.Name, item => item.Value.GetInt32(), StringComparer.Ordinal);

Dictionary<string, HashSet<string>> inner = new(StringComparer.Ordinal);
foreach (Ring ring in rings)
{
    HashSet<string> seen = new(StringComparer.Ordinal);
    Queue<string> pending = new(ring.Reach);
    while (pending.Count > 0)
    {
        string name = pending.Dequeue();
        if (!seen.Add(name))
        {
            continue;
        }

        foreach (string deeper in rings.FirstOrDefault(candidate => candidate.Name == name)?.Reach ?? [])
        {
            pending.Enqueue(deeper);
        }
    }

    inner[ring.Name] = seen;
}

AuditBinder binder = AuditBinder.Bind(projectRoot, binderPath);
CSharpCompilation compilation = binder.Compilation;

System.Collections.Concurrent.ConcurrentDictionary<string, Ring?> ringOfPath = new(StringComparer.OrdinalIgnoreCase);
Ring? RingOf(string relative) => ringOfPath.GetOrAdd(relative, key => rings
    .Where(candidate => key.StartsWith(candidate.Path, StringComparison.OrdinalIgnoreCase))
    .OrderByDescending(candidate => candidate.Path.Length)
    .FirstOrDefault());

Ring? SpaceRing(string space) => rings
    .Where(candidate => space == candidate.Name || space.StartsWith(candidate.Name + ".", StringComparison.Ordinal))
    .MaxBy(candidate => candidate.Name.Length);

Ring? SourceRing(INamedTypeSymbol type)
{
    Location? source = type.Locations.FirstOrDefault(location => location.IsInSource);
    return source?.SourceTree is null ? null : RingOf(binder.Relative(source.SourceTree.FilePath));
}

static bool IsData(INamedTypeSymbol type) =>
    type.TypeKind is TypeKind.Enum or TypeKind.Struct or TypeKind.Delegate || type.IsRecord;

static INamedTypeSymbol? TypeOf(ISymbol symbol) => symbol switch
{
    IAliasSymbol alias => TypeOf(alias.Target),
    INamedTypeSymbol named => named.IsTupleType || named.IsAnonymousType ? null : named.OriginalDefinition,
    IArrayTypeSymbol array => TypeOf(array.ElementType),
    INamespaceSymbol or ITypeParameterSymbol or ILocalSymbol or IParameterSymbol => null,
    IRangeVariableSymbol or IDiscardSymbol or ILabelSymbol or IPreprocessingSymbol => null,
    _ => symbol.ContainingType?.OriginalDefinition,
};

static bool IsHeader(SimpleNameSyntax name)
{
    SyntaxNode? parent = name.Parent;
    if (parent is QualifiedNameSyntax qualified)
    {
        parent = qualified.Parent;
    }

    return parent is UsingDirectiveSyntax or NamespaceDeclarationSyntax or FileScopedNamespaceDeclarationSyntax;
}

static string NamespaceOf(INamespaceSymbol? space) =>
    space is null || space.IsGlobalNamespace ? "" : space.ToDisplayString();

string LineText(SyntaxTree tree, int line) => line <= 0 ? "" : tree.GetText().Lines[line - 1].ToString().Trim();

System.Collections.Concurrent.ConcurrentBag<List<Finding>> bags = [];
Parallel.ForEach(binder.Tracked, tree =>
{
    string relative = binder.Relative(tree.FilePath);
    Ring? ring = RingOf(relative);
    if (ring is null)
    {
        return;
    }

    List<Finding> findings = [];
    bool isRoot = roots.Contains(relative);
    SemanticModel model = compilation.GetSemanticModel(tree, true);
    HashSet<string> taken = new(StringComparer.Ordinal);

    void Add(int line, string check, string target, string name)
    {
        if (taken.Add($"{line}|{check}|{target}|{name}"))
        {
            findings.Add(new Finding(relative, line, ring.Name, check, target, name, LineText(tree, line)));
        }
    }

    void Chain(int line, Ring target, string name, bool data)
    {
        if (target.Name == ring.Name)
        {
            return;
        }

        string check = isRoot ? "root"
            : ring.Reach.Contains(target.Name) ? "neighbour"
            : !inner[ring.Name].Contains(target.Name) ? "outward"
            : ring.Cut && !target.Cut ? "cross"
            : data ? "carry"
            : "reach";
        Add(line, check, target.Name, name);
    }

    SyntaxNode rootNode = tree.GetRoot();
    foreach (UsingDirectiveSyntax directive in rootNode.DescendantNodes().OfType<UsingDirectiveSyntax>())
    {
        int line = directive.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        if (directive.Name is not null)
        {
            string space = directive.Name.ToString();
            if (!space.StartsWith(project + ".", StringComparison.Ordinal)
                && !ring.Frame.Contains("*")
                && !ring.Frame.Contains(space))
            {
                Add(line, "frame", space, space);
            }
        }

        if (directive.NamespaceOrType is { } named
            && model.GetSymbolInfo(named).Symbol is INamespaceSymbol spaceSymbol
            && SpaceRing(spaceSymbol.ToDisplayString()) is { } target
            && !ring.Reach.Contains(target.Name))
        {
            Chain(line, target, "using " + spaceSymbol.ToDisplayString(), true);
        }
    }

    foreach (SimpleNameSyntax name in rootNode.DescendantNodes().OfType<SimpleNameSyntax>())
    {
        bool header = IsHeader(name);
        bool held = name.FirstAncestorOrSelf<UsingDirectiveSyntax>()?.StaticKeyword.IsKind(SyntaxKind.StaticKeyword) == true;
        if (header && !held)
        {
            continue;
        }

        SymbolInfo info = model.GetSymbolInfo(name);
        ISymbol? symbol = info.Symbol ?? info.CandidateSymbols.FirstOrDefault();
        if (symbol is null || TypeOf(symbol) is not INamedTypeSymbol type)
        {
            continue;
        }

        int line = name.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        Location? source = type.Locations.FirstOrDefault(location => location.IsInSource);
        if (source is null)
        {
            if (header)
            {
                continue;
            }

            string space = NamespaceOf(type.ContainingNamespace);
            if (space.Length > 0 && !ring.Frame.Contains("*") && !ring.Frame.Contains(space))
            {
                Add(line, "frame", space, space);
            }

            if (ring.Pure)
            {
                string full = space.Length > 0 ? space + "." + type.Name : type.Name;
                if (symbol is not ITypeSymbol and not IAliasSymbol and not IMethodSymbol { MethodKind: MethodKind.Constructor })
                {
                    full += "." + symbol.Name;
                }

                string? pattern = ambient.FirstOrDefault(
                    candidate => full == candidate || full.StartsWith(candidate + ".", StringComparison.Ordinal));
                if (pattern is not null)
                {
                    Add(line, "ambient", pattern, pattern);
                }
            }

            continue;
        }

        Ring? targetRing = RingOf(binder.Relative(source.SourceTree!.FilePath));
        if (targetRing is null)
        {
            continue;
        }

        bool behaviour = symbol is IMethodSymbol { MethodKind: MethodKind.Ordinary or MethodKind.ReducedExtension };
        Chain(line, targetRing, type.Name, IsData(type) && !behaviour);
    }

    foreach (BaseTypeDeclarationSyntax declaration in rootNode.DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
    {
        string declared = declaration.Identifier.Text;
        foreach (string pattern in strays.GetValueOrDefault(ring.Name, []).Where(pattern => Regex.IsMatch(declared, pattern)))
        {
            int line = declaration.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            Add(line, "stray", ring.Name, declared);
        }
    }

    foreach ((string folder, string[] words) in banned)
    {
        if (!relative.StartsWith(folder.Trim('/') + "/", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        Microsoft.CodeAnalysis.Text.TextLineCollection lines = tree.GetText().Lines;
        for (int index = 0; index < lines.Count; index++)
        {
            string text = lines[index].ToString();
            foreach (string word in words.Where(word => text.Contains(word, StringComparison.Ordinal)))
            {
                Add(index + 1, "banned", ring.Name, word);
            }
        }
    }

    bags.Add(findings);
});

List<Finding> all = bags.SelectMany(bag => bag).ToList();

HashSet<string> exposed = new(StringComparer.Ordinal);
foreach ((string pair, string[] surface) in surfaces)
{
    string ringName = pair[..pair.IndexOf('>')];
    string neighbour = pair[(pair.IndexOf('>') + 1)..];
    if (rings.FirstOrDefault(candidate => candidate.Name == ringName) is not { Cut: true })
    {
        continue;
    }

    IEnumerable<ISymbol> members = surface
        .SelectMany(name => compilation.GetSymbolsWithName(name, SymbolFilter.Type))
        .OfType<INamedTypeSymbol>()
        .Where(type => SourceRing(type)?.Name == neighbour)
        .SelectMany(PublicOf)
        .Where(member => member.DeclaredAccessibility == Accessibility.Public && !member.IsImplicitlyDeclared);
    foreach (ISymbol member in members)
    {
        Location? location = member.Locations.FirstOrDefault(place => place.IsInSource);
        if (location is null)
        {
            continue;
        }

        string relative = binder.Relative(location.SourceTree!.FilePath);
        int line = location.GetLineSpan().StartLinePosition.Line + 1;
        foreach (INamedTypeSymbol type in SignatureOf(member))
        {
            if (SourceRing(type) is { } target && inner[neighbour].Contains(target.Name)
                && exposed.Add($"{relative}|{line}|{ringName}|{target.Name}|{type.Name}"))
            {
                all.Add(new Finding(relative, line, ringName, "expose", target.Name, type.Name,
                    LineText(location.SourceTree, line)));
            }
        }
    }
}

foreach (Finding finding in all.Where(finding => finding.Check == "neighbour").ToList())
{
    if (surfaces.TryGetValue($"{finding.Ring}>{finding.Target}", out string[]? surface)
        && !surface.Contains(finding.Name, StringComparer.Ordinal))
    {
        all.Add(finding with { Check = "surface" });
    }
}

Dictionary<string, int> files = binder.Tracked
    .Select(tree => RingOf(binder.Relative(tree.FilePath))?.Name)
    .Where(name => name is not null)
    .GroupBy(name => name!, StringComparer.Ordinal)
    .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);
foreach ((string ringName, int floor) in floors)
{
    int count = files.GetValueOrDefault(ringName);
    if (count < floor)
    {
        all.Add(new Finding("", 0, ringName, "floor", ringName, $"{count} source file(s), floor {floor}", ""));
    }
}

foreach (Ring ring in rings)
{
    string folder = ring.Path.TrimEnd('/');
    string projectFile = Path.Combine(projectRoot, folder.Replace('/', Path.DirectorySeparatorChar), Path.GetFileName(folder) + ".csproj");
    if (!File.Exists(projectFile))
    {
        all.Add(new Finding(folder, 0, ring.Name, "table", ring.Name, "no project file at the ring path", ""));
        continue;
    }

    string[] references = XDocument.Load(projectFile).Descendants()
        .Where(node => node.Name.LocalName == "ProjectReference")
        .Select(node => (string?)node.Attribute("Include") ?? "")
        .Where(include => include.Length > 0)
        .Select(include => Path.GetFileNameWithoutExtension(include.Replace('\\', '/').Split('/')[^1]))
        .Select(name => rings.FirstOrDefault(candidate => Path.GetFileName(candidate.Path.TrimEnd('/')) == name)?.Name ?? name)
        .Distinct(StringComparer.Ordinal)
        .Order(StringComparer.Ordinal)
        .ToArray();
    if (!references.SequenceEqual(ring.Reach.Order(StringComparer.Ordinal), StringComparer.Ordinal))
    {
        string relative = binder.Relative(projectFile);
        all.Add(new Finding(relative, 0, ring.Name, "table", ring.Name,
            $"reaches [{string.Join(", ", ring.Reach)}] but references [{string.Join(", ", references)}]", ""));
    }
}

all = all
    .OrderBy(finding => finding.Path, StringComparer.Ordinal)
    .ThenBy(finding => finding.Line)
    .ToList();

JsonSerializerOptions options = new() { WriteIndented = false };
File.WriteAllText(outputPath, JsonSerializer.Serialize(all, options));
return 0;

static IEnumerable<ISymbol> PublicOf(INamedTypeSymbol type)
{
    foreach (ISymbol member in type.GetMembers())
    {
        yield return member;
        if (member is INamedTypeSymbol { DeclaredAccessibility: Accessibility.Public } nested)
        {
            foreach (ISymbol deeper in PublicOf(nested))
            {
                yield return deeper;
            }
        }
    }
}

static IEnumerable<INamedTypeSymbol> SignatureOf(ISymbol member)
{
    IEnumerable<ITypeSymbol> types = member switch
    {
        IMethodSymbol { AssociatedSymbol: null } method =>
            method.Parameters.Select(parameter => parameter.Type).Prepend(method.ReturnType),
        IPropertySymbol property => property.Parameters.Select(parameter => parameter.Type).Prepend(property.Type),
        IEventSymbol handler => [handler.Type],
        IFieldSymbol field => [field.Type],
        INamedTypeSymbol nested => nested.BaseType is null ? nested.Interfaces : nested.Interfaces.Prepend(nested.BaseType),
        _ => [],
    };
    Stack<ITypeSymbol> pending = new(types);
    while (pending.Count > 0)
    {
        ITypeSymbol type = pending.Pop();
        if (type is IArrayTypeSymbol array)
        {
            pending.Push(array.ElementType);
        }
        else if (type is INamedTypeSymbol named)
        {
            yield return named.OriginalDefinition;
            foreach (ITypeSymbol argument in named.TypeArguments)
            {
                pending.Push(argument);
            }
        }
    }
}

static string[] Strings(JsonElement element) => element.EnumerateArray().Select(item => item.GetString()!).ToArray();

static Dictionary<string, string[]> Map(JsonElement element) => element.EnumerateObject()
    .ToDictionary(item => item.Name, item => Strings(item.Value), StringComparer.Ordinal);

sealed record Ring(string Name, string Path, string[] Reach, string[] Frame, bool Pure, bool Cut);

sealed record Finding(string Path, int Line, string Ring, string Check, string Target, string Name, string Text);
'@

function Write-AuditHelper {
    # The binder is the .NET SDK's own Roslyn, referenced from the SDK folder so nothing is restored
    # from a feed. The helper reads the configuration and the manifest, binds every file in one
    # compilation, and writes one finding per line and name to the output path as JSON.
    param(
        [Parameter(Mandatory = $true)][string]$HelperFolder,
        [Parameter(Mandatory = $true)][string]$TargetFramework
    )

    $projectPath = Join-Path $HelperFolder 'AuditStructure.csproj'
    $programPath = Join-Path $HelperFolder 'Program.cs'
    $encoding = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($projectPath, $script:HelperProject.Replace('{TARGET_FRAMEWORK}', $TargetFramework), $encoding)
    [System.IO.File]::WriteAllText($programPath, $script:HelperProgram, $encoding)
    [System.IO.File]::WriteAllText((Join-Path $HelperFolder 'AuditBinder.cs'), $script:BinderSource, $encoding)
    return $projectPath
}

function Get-HelperCacheFolder {
    # The helper is compiled once per text and SDK and kept under the temp folder, so a later run
    # skips the restore and the build and pays for the binding alone. The build folder is not used
    # because the build tooling clears it on install.
    param(
        [Parameter(Mandatory = $true)][string]$ProjectName,
        [Parameter(Mandatory = $true)][string]$TargetFramework,
        [Parameter(Mandatory = $true)][string]$SdkVersion
    )

    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        $seed = $script:HelperProject + "`n" + $script:HelperProgram + "`n" + $script:BinderSource + "`n" + $TargetFramework + "`n" + $SdkVersion
        $digest = $sha.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($seed))
    }
    finally {
        $sha.Dispose()
    }

    $hash = ([System.BitConverter]::ToString($digest) -replace '-', '').Substring(0, 16).ToLowerInvariant()
    return Join-Path ([System.IO.Path]::GetTempPath()) ($ProjectName + '-AuditStructure' + [System.IO.Path]::DirectorySeparatorChar + $hash)
}

function Invoke-AuditHelper {
    param(
        [Parameter(Mandatory = $true)][string]$ProjectRoot,
        [Parameter(Mandatory = $true)][string]$ConfigPath,
        [Parameter(Mandatory = $true)][string]$ProjectName
    )

    $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($null -eq $dotnet) {
        throw 'The .NET SDK is required, but dotnet was not found on PATH.'
    }

    $nativePreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    $sdkOutput = & $dotnet.Source --version 2>&1
    $ErrorActionPreference = $nativePreference
    if ($LASTEXITCODE -ne 0) {
        throw "The .NET SDK version could not be read.`n$($sdkOutput -join [Environment]::NewLine)"
    }

    $sdkVersion = ([string]($sdkOutput | Select-Object -First 1)).Trim()
    if ($sdkVersion -notmatch '^(\d+)\.') {
        throw "The .NET SDK version is not recognized: '$sdkVersion'"
    }

    $targetFramework = "net$($Matches[1]).0"

    $previousNoLogo = $env:DOTNET_NOLOGO
    $previousTelemetry = $env:DOTNET_CLI_TELEMETRY_OPTOUT
    $env:DOTNET_NOLOGO = '1'
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'

    $temporaryFolder = Join-Path ([System.IO.Path]::GetTempPath()) ($ProjectName + '-AuditStructure-' + [Guid]::NewGuid().ToString('N'))
    [System.IO.Directory]::CreateDirectory($temporaryFolder) | Out-Null

    try {
        $cacheFolder = Get-HelperCacheFolder -ProjectName $ProjectName -TargetFramework $targetFramework -SdkVersion $sdkVersion
        $binaryPath = Join-Path (Join-Path $cacheFolder 'bin') 'AuditStructure.dll'
        if (-not (Test-Path -LiteralPath $binaryPath -PathType Leaf)) {
            Write-AuditLine 'Compiling the structure binder once for this SDK...' -ForegroundColor DarkGray
            $cacheParent = Split-Path -Parent $cacheFolder
            if (Test-Path -LiteralPath $cacheParent) {
                Get-ChildItem -LiteralPath $cacheParent -Directory |
                    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
            }

            $helperFolder = Join-Path $cacheFolder 'helper'
            [System.IO.Directory]::CreateDirectory($helperFolder) | Out-Null
            $projectPath = Write-AuditHelper -HelperFolder $helperFolder -TargetFramework $targetFramework
            $buildArguments = @('build', $projectPath, '--configuration', 'Release', '--nologo', '--verbosity', 'quiet',
                '--output', (Join-Path $cacheFolder 'bin'))
            $nativePreference = $ErrorActionPreference
            $ErrorActionPreference = 'Continue'
            $buildOutput = & $dotnet.Source @buildArguments 2>&1
            $ErrorActionPreference = $nativePreference
            if ($LASTEXITCODE -ne 0) {
                Remove-Item -LiteralPath $cacheFolder -Recurse -Force -ErrorAction SilentlyContinue
                throw "The structure binder could not be built.`n$($buildOutput -join [Environment]::NewLine)"
            }
        }

        $outputPath = Join-Path $temporaryFolder 'findings.json'

        $nativePreference = $ErrorActionPreference
        $ErrorActionPreference = 'Continue'
        $helperOutput = & $dotnet.Source $binaryPath $ConfigPath $ProjectRoot (Join-Path $PSScriptRoot 'auditbinder.json') $outputPath 2>&1
        $ErrorActionPreference = $nativePreference
        if ($LASTEXITCODE -ne 0) {
            throw "The structure binder failed.`n$($helperOutput -join [Environment]::NewLine)"
        }

        $json = Get-Content -LiteralPath $outputPath -Raw -Encoding UTF8
        return @(ConvertFrom-Json -InputObject $json)
    }
    finally {
        $env:DOTNET_NOLOGO = $previousNoLogo
        $env:DOTNET_CLI_TELEMETRY_OPTOUT = $previousTelemetry
        if (Test-Path -LiteralPath $temporaryFolder) {
            Remove-Item -LiteralPath $temporaryFolder -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
}

function Format-ReportCell {
    param([string]$Value)

    if ($null -eq $Value) {
        return ''
    }

    $single = $Value -replace '\s+', ' '
    if ($single.Length -gt 120) {
        $single = $single.Substring(0, 117) + '...'
    }

    return $single.Replace('|', '\|').Trim()
}

$projectRoot = Resolve-ProjectRoot -Path $Root
$config = Read-AuditConfig -ProjectRoot $projectRoot
$configPath = Join-Path $PSScriptRoot $script:ConfigDocument
$version = Read-ProjectVersion -ProjectRoot $projectRoot -Relative $config.report.versionFile
$sourceFiles = Get-ProjectSourceFiles -ProjectRoot $projectRoot -Config $config

$ringed = New-Object 'System.Collections.Generic.List[string]'
$skippedFiles = 0
foreach ($relativePath in $sourceFiles) {
    if ($null -eq (Get-PathRing -RelativePath $relativePath -Config $config)) {
        $skippedFiles++
        continue
    }

    [void]$ringed.Add((Join-AuditPath -ProjectRoot $projectRoot -Relative $relativePath))
}

if ($ringed.Count -eq 0) {
    throw "No tracked source file sits inside a declared ring under: $projectRoot"
}

$auditedFiles = $ringed.Count
Write-AuditLine ("Scanned: {0:N0} source files inside a ring" -f $auditedFiles) -ForegroundColor DarkGray

$findings = New-Object 'System.Collections.Generic.List[object]'
foreach ($finding in (Invoke-AuditHelper -ProjectRoot $projectRoot -ConfigPath $configPath -ProjectName ([string]$config.project))) {
    [void]$findings.Add([pscustomobject]@{
        Path   = [string]$finding.Path
        Line   = [int]$finding.Line
        Ring   = [string]$finding.Ring
        Check  = [string]$finding.Check
        Target = [string]$finding.Target
        Name   = [string]$finding.Name
        Text   = [string]$finding.Text
        Exempt = $false
    })
}

$exemptRows = New-Object 'System.Collections.Generic.List[object]'
foreach ($property in $config.exempt.PSObject.Properties) {
    $exemptPath = ([string]$property.Name).Replace('\', '/').Trim('/')
    foreach ($exemptName in $property.Value) {
        [void]$exemptRows.Add([pscustomobject]@{
            Path = $exemptPath
            Name = [string]$exemptName
            Used = 0
        })
    }
}

foreach ($finding in $findings) {
    foreach ($row in $exemptRows) {
        if ($finding.Path.Equals($row.Path, [System.StringComparison]::OrdinalIgnoreCase) -and
            (Test-NamePattern -Name $finding.Name -Pattern $row.Name)) {
            $finding.Exempt = $true
            $row.Used++
            break
        }
    }
}

$severityOf = @{}
foreach ($check in $script:CheckOrder) {
    $severityOf[$check] = [string]$config.checks.$check
}

$checkIndex = { [array]::IndexOf($script:CheckOrder, $_.Check) }
$live = @($findings | Where-Object { -not $_.Exempt })
$violations = @($live | Where-Object { $severityOf[$_.Check] -eq 'violation' } | Sort-Object -Property @{ Expression = $checkIndex }, @{ Expression = { Get-OrdinalKey $_.Ring } }, @{ Expression = { Get-OrdinalKey $_.Target } }, @{ Expression = { Get-OrdinalKey $_.Path } }, Line, @{ Expression = { Get-OrdinalKey $_.Name } })
$reviews = @($live | Where-Object { $severityOf[$_.Check] -eq 'review' } | Sort-Object -Property @{ Expression = $checkIndex }, @{ Expression = { Get-OrdinalKey $_.Ring } }, @{ Expression = { Get-OrdinalKey $_.Target } }, @{ Expression = { Get-OrdinalKey $_.Path } }, Line, @{ Expression = { Get-OrdinalKey $_.Name } })
$staleExempt = @($exemptRows | Where-Object { $_.Used -eq 0 } | Sort-Object -Property @{ Expression = { Get-OrdinalKey $_.Path } }, @{ Expression = { Get-OrdinalKey $_.Name } })

# A pair is one check from one ring to one target. Its count is the number of distinct hits, one
# per file, line, check, target and name, as in the convention tests. A violating pair is held to
# the ceiling written for it, an unwritten ceiling being zero.
$pairs = New-Object 'System.Collections.Generic.List[object]'
foreach ($group in ($live | Group-Object -Property Check, Ring, Target)) {
    $first = $group.Group[0]
    $key = "$($first.Check):$($first.Ring)>$($first.Target)"
    $fileCount = @($group.Group | Select-Object -ExpandProperty Path -Unique).Count
    $ceiling = 0
    foreach ($property in $config.ceilings.PSObject.Properties) {
        if ($property.Name -eq $key) {
            $ceiling = [int]$property.Value
        }
    }

    [void]$pairs.Add([pscustomobject]@{
        Key      = $key
        Check    = $first.Check
        Ring     = $first.Ring
        Target   = $first.Target
        Files    = $fileCount
        Lines    = $group.Count
        Severity = $severityOf[$first.Check]
        Ceiling  = $ceiling
    })
}

foreach ($property in $config.ceilings.PSObject.Properties) {
    if (@($pairs | Where-Object { $_.Key -eq $property.Name }).Count -eq 0) {
        $parts = $property.Name -split '[:>]', 3
        [void]$pairs.Add([pscustomobject]@{
            Key      = $property.Name
            Check    = $parts[0]
            Ring     = $parts[1]
            Target   = $parts[2]
            Files    = 0
            Lines    = 0
            Severity = $severityOf[$parts[0]]
            Ceiling  = [int]$property.Value
        })
    }
}

$pairs = @($pairs | Sort-Object -Property @{ Expression = { [array]::IndexOf($script:CheckOrder, $_.Check) } }, @{ Expression = { Get-OrdinalKey $_.Ring } }, @{ Expression = { Get-OrdinalKey $_.Target } })
$overCeiling = @($pairs | Where-Object { $_.Severity -eq 'violation' -and $_.Lines -gt $_.Ceiling })
$staleCeiling = @($pairs | Where-Object { $_.Ceiling -gt 0 -and ($_.Severity -ne 'violation' -or $_.Lines -lt $_.Ceiling) })

$reportFolder = Join-AuditPath -ProjectRoot $projectRoot -Relative $config.report.directory
if (-not (Test-Path -LiteralPath $reportFolder -PathType Container)) {
    [void][System.IO.Directory]::CreateDirectory($reportFolder)
}

$reportPath = Join-Path $reportFolder ($script:ReportName.Replace('{version}', $version))
$violationPath = Join-Path $reportFolder ($script:ViolationName.Replace('{version}', $version))

$counts = [ordered]@{}
foreach ($check in $script:CheckOrder) {
    $counts[$check] = @($live | Where-Object { $_.Check -eq $check }).Count
}

$report = New-Object 'System.Collections.Generic.List[string]'
[void]$report.Add("# Structure $version")
[void]$report.Add('')
[void]$report.Add("- Version: ``$version``")
[void]$report.Add("- Generation: $script:AuditGeneration")
[void]$report.Add("- Files audited: $auditedFiles")
[void]$report.Add("- Files outside a declared ring: $skippedFiles")
[void]$report.Add("- Violations: $($violations.Count)")
[void]$report.Add("- Review items: $($reviews.Count)")
[void]$report.Add("- Pairs above their ceiling: $($overCeiling.Count)")
[void]$report.Add("- Stale ceilings: $($staleCeiling.Count)")
[void]$report.Add('')
[void]$report.Add('The audit binds the source and reads the kind of every type one ring names from another. A')
[void]$report.Add('ring reaches the one ring the table names for it and carries the data of any ring inside it;')
[void]$report.Add('a class or an interface named from deeper than the neighbour is a reach, and a name from an')
[void]$report.Add('outer ring is never right. A ring names only the framework namespaces its frame lists, and')
[void]$report.Add('a pure ring never touches an ambient member. The composition root alone is free of the chain.')
[void]$report.Add('')
[void]$report.Add('## Rings')
[void]$report.Add('')
[void]$report.Add('| Ring | Path | Reaches | Pure |')
[void]$report.Add('|---|---|---|---|')
foreach ($ring in $config.rings) {
    $reaches = if (@($ring.reach).Count -eq 0) { 'nothing' } else { (@($ring.reach) | ForEach-Object { "``$_``" }) -join ', ' }
    [void]$report.Add("| ``$($ring.name)`` | ``$($ring.path)`` | $reaches | $(if ($ring.pure) { 'yes' } else { 'no' }) |")
}

[void]$report.Add('')
[void]$report.Add('## Checks')
[void]$report.Add('')
[void]$report.Add('| Check | Meaning | Severity | Lines |')
[void]$report.Add('|---|---|---|---|')
foreach ($check in $script:CheckOrder) {
    [void]$report.Add("| ``$check`` | $($script:CheckTitles[$check]) | $($severityOf[$check]) | $($counts[$check]) |")
}
$countTotal = ($script:CheckOrder | ForEach-Object { $counts[$_] } | Measure-Object -Sum).Sum
[void]$report.Add("| Total | | | $countTotal |")

[void]$report.Add('')
[void]$report.Add('## Pairs')
[void]$report.Add('')
if ($pairs.Count -eq 0) {
    [void]$report.Add('No ring named another ring.')
}
else {
    [void]$report.Add('| Check | Ring | Target | Files | Lines | Ceiling | Standing |')
    [void]$report.Add('|---|---|---|---|---|---|---|')
    foreach ($pair in $pairs) {
        $standing = if ($pair.Severity -ne 'violation') { $pair.Severity }
            elseif ($pair.Lines -gt $pair.Ceiling) { 'above ceiling' }
            elseif ($pair.Lines -lt $pair.Ceiling) { 'stale ceiling' }
            elseif ($pair.Lines -eq 0) { 'clean' }
            else { 'at ceiling' }
        [void]$report.Add("| ``$($pair.Check)`` | ``$($pair.Ring)`` | ``$($pair.Target)`` | $($pair.Files) | $($pair.Lines) | $($pair.Ceiling) | $standing |")
    }
}

[void]$report.Add('')
[void]$report.Add('## Names')
[void]$report.Add('')
$named = @($live | Where-Object { $severityOf[$_.Check] -ne 'allow' })
if ($named.Count -eq 0) {
    [void]$report.Add('No name stepped past its ring.')
}
else {
    [void]$report.Add('| Name | Target | Lines | Files | Heaviest check |')
    [void]$report.Add('|---|---|---|---|---|')
    $groups = @($named | Group-Object -Property Name, Target | Sort-Object -Property @{ Expression = { $_.Count }; Descending = $true }, @{ Expression = { Get-OrdinalKey $_.Name } })
    foreach ($group in $groups) {
        $first = $group.Group[0]
        $heaviest = @($group.Group | Sort-Object -Property @{ Expression = $checkIndex } | Select-Object -First 1).Check
        $fileCount = @($group.Group | Select-Object -ExpandProperty Path -Unique).Count
        [void]$report.Add("| ``$($first.Name)`` | ``$($first.Target)`` | $($group.Count) | $fileCount | ``$heaviest`` |")
    }
}

[void]$report.Add('')
[void]$report.Add('## Files')
[void]$report.Add('')
$fileGroups = @($named | Group-Object -Property Path | Sort-Object -Property @{ Expression = { $_.Count }; Descending = $true }, @{ Expression = { Get-OrdinalKey $_.Name } })
if ($fileGroups.Count -eq 0) {
    [void]$report.Add('No file stepped past its ring.')
}
else {
    [void]$report.Add('| File | Lines | Violations | Review |')
    [void]$report.Add('|---|---|---|---|')
    foreach ($group in $fileGroups) {
        $fileViolations = @($group.Group | Where-Object { $severityOf[$_.Check] -eq 'violation' }).Count
        $fileReviews = @($group.Group | Where-Object { $severityOf[$_.Check] -eq 'review' }).Count
        [void]$report.Add("| ``$($group.Name)`` | $($group.Count) | $fileViolations | $fileReviews |")
    }
}

[void]$report.Add('')
[void]$report.Add('## Exemptions')
[void]$report.Add('')
if ($exemptRows.Count -eq 0) {
    [void]$report.Add('The configuration declares no exemption.')
}
else {
    [void]$report.Add('| File | Name | Cleared |')
    [void]$report.Add('|---|---|---|')
    foreach ($row in ($exemptRows | Sort-Object -Property @{ Expression = { Get-OrdinalKey $_.Path } }, @{ Expression = { Get-OrdinalKey $_.Name } })) {
        [void]$report.Add("| ``$($row.Path)`` | ``$($row.Name)`` | $($row.Used) |")
    }
}

$violationReport = New-Object 'System.Collections.Generic.List[string]'
[void]$violationReport.Add("# Violated Structure $version")
[void]$violationReport.Add('')
[void]$violationReport.Add("- Version: ``$version``")
[void]$violationReport.Add("- Generation: $script:AuditGeneration")
[void]$violationReport.Add("- Violations: $($violations.Count)")
[void]$violationReport.Add("- Review items: $($reviews.Count)")
[void]$violationReport.Add("- Pairs above their ceiling: $($overCeiling.Count)")
[void]$violationReport.Add("- Stale ceilings: $($staleCeiling.Count)")
[void]$violationReport.Add("- Stale exemptions: $($staleExempt.Count)")
[void]$violationReport.Add('')
[void]$violationReport.Add('Every line below is a move until the user says otherwise. A ring that reaches past its')
[void]$violationReport.Add('neighbour makes the ring between them optional: the engine that calls a vault is a second')
[void]$violationReport.Add('use-case layer, the view that names the engine is a second presenter. Resolve a reach by')
[void]$violationReport.Add('handing the work to the neighbour ring, a frame by moving the framework call behind a port,')
[void]$violationReport.Add('and an ambient touch by taking the clock or the file through a port. Never resolve one by')
[void]$violationReport.Add('turning a class into a record so it reads as data.')
[void]$violationReport.Add('')
[void]$violationReport.Add('**Only the user grants an exemption** by adding the name to the `exempt` block of the audit')
[void]$violationReport.Add('configuration, and **only the user lowers a ceiling**, never raises one. The audit reads the')
[void]$violationReport.Add('binder alone, so a finding it raises may still be correct code; say why, and cite the')
[void]$violationReport.Add('`file:line` that was read.')
[void]$violationReport.Add('')
[void]$violationReport.Add('## Ceilings')
[void]$violationReport.Add('')
$held = @($pairs | Where-Object { $_.Severity -eq 'violation' -and ($_.Lines -gt 0 -or $_.Ceiling -gt 0) })
if ($held.Count -eq 0) {
    [void]$violationReport.Add('No violating pair was found and no ceiling is written.')
}
else {
    [void]$violationReport.Add('| Pair | Hits | Ceiling | Standing |')
    [void]$violationReport.Add('|---|---|---|---|')
    foreach ($pair in $held) {
        $standing = if ($pair.Lines -gt $pair.Ceiling) { 'above ceiling' } elseif ($pair.Lines -lt $pair.Ceiling) { 'stale ceiling' } else { 'at ceiling' }
        [void]$violationReport.Add("| ``$($pair.Key)`` | $($pair.Lines) | $($pair.Ceiling) | $standing |")
    }
}

[void]$violationReport.Add('')
[void]$violationReport.Add('## Violations')
[void]$violationReport.Add('')
if ($violations.Count -eq 0) {
    [void]$violationReport.Add('No structural violation was found.')
}
else {
    [void]$violationReport.Add('| Location | Check | Ring | Target | Name | Source |')
    [void]$violationReport.Add('|---|---|---|---|---|---|')
    foreach ($finding in $violations) {
        [void]$violationReport.Add("| ``$($finding.Path):$($finding.Line)`` | ``$($finding.Check)`` | ``$($finding.Ring)`` | ``$($finding.Target)`` | ``$($finding.Name)`` | ``$(Format-ReportCell -Value $finding.Text)`` |")
    }
}

[void]$violationReport.Add('')
[void]$violationReport.Add('## Review')
[void]$violationReport.Add('')
if ($reviews.Count -eq 0) {
    [void]$violationReport.Add('No review item was found.')
}
else {
    [void]$violationReport.Add('| Location | Check | Ring | Target | Name | Source |')
    [void]$violationReport.Add('|---|---|---|---|---|---|')
    foreach ($finding in $reviews) {
        [void]$violationReport.Add("| ``$($finding.Path):$($finding.Line)`` | ``$($finding.Check)`` | ``$($finding.Ring)`` | ``$($finding.Target)`` | ``$($finding.Name)`` | ``$(Format-ReportCell -Value $finding.Text)`` |")
    }
}

[void]$violationReport.Add('')
[void]$violationReport.Add('## Stale Exemptions')
[void]$violationReport.Add('')
if ($staleExempt.Count -eq 0) {
    [void]$violationReport.Add('Every declared exemption cleared a finding.')
}
else {
    [void]$violationReport.Add('| File | Name |')
    [void]$violationReport.Add('|---|---|')
    foreach ($row in $staleExempt) {
        [void]$violationReport.Add("| ``$($row.Path)`` | ``$($row.Name)`` |")
    }
}

$encoding = New-Object System.Text.UTF8Encoding($false)
$reportTemporary = $reportPath + '.tmp'
$violationTemporary = $violationPath + '.tmp'
[System.IO.File]::WriteAllText($reportTemporary, (($report.ToArray() -join "`n") + "`n"), $encoding)
[System.IO.File]::WriteAllText($violationTemporary, (($violationReport.ToArray() -join "`n") + "`n"), $encoding)
[System.IO.File]::Copy($reportTemporary, $reportPath, $true)
[System.IO.File]::Copy($violationTemporary, $violationPath, $true)
Remove-Item -LiteralPath $reportTemporary, $violationTemporary -Force

Write-AuditLine ''
Write-AuditLine 'Counters'
Write-AuditLine '--------'
Write-AuditLine ("Above ceiling   {0:N0}" -f $overCeiling.Count)
Write-AuditLine ("Stale ceilings  {0:N0}" -f $staleCeiling.Count)

Write-AuditSection -Title 'Findings by check'
$checkRows = @($script:CheckOrder | ForEach-Object { , @($_, $severityOf[$_], $counts[$_].ToString('N0')) })
$checkRows += , @('Total', '', $countTotal.ToString('N0'))
Write-AuditTable -Header @('Check', 'Severity', 'Findings') -Rows $checkRows

$shownPairs = @($pairs | Where-Object { $_.Severity -ne 'allow' })
if ($shownPairs.Count -gt 0) {
    Write-AuditSection -Title "Pairs ($($shownPairs.Count.ToString('N0')))"
    $pairRows = @($shownPairs | ForEach-Object {
        $standing = if ($_.Severity -eq 'review') { 'review' }
            elseif ($_.Lines -gt $_.Ceiling) { 'above ceiling' }
            elseif ($_.Lines -lt $_.Ceiling) { 'stale ceiling' }
            else { 'at ceiling' }
        , @($_.Key, $_.Files.ToString('N0'), $_.Lines.ToString('N0'), $_.Ceiling.ToString('N0'), $standing)
    })
    Write-AuditTable -Header @('Pair', 'Files', 'Lines', 'Ceiling', 'Standing') -Rows $pairRows
}

if ($reviews.Count -gt 0) {
    Write-AuditSection -Title "Review items ($($reviews.Count.ToString('N0')))"
    foreach ($finding in $reviews) {
        Write-AuditLine "$($finding.Path):$($finding.Line) $($finding.Check) $($finding.Name)"
    }
}

if ($staleExempt.Count -gt 0) {
    Write-AuditSection -Title "Stale exemptions ($($staleExempt.Count.ToString('N0')))"
    foreach ($row in $staleExempt) {
        Write-AuditLine "$($row.Path) $($row.Name)"
    }
}

Write-AuditLine ''
Write-AuditLine "Report: $reportPath"
Write-AuditLine "Report: $violationPath"

if ($Open) {
    Start-Process -FilePath $violationPath | Out-Null
}

if ($overCeiling.Count -gt 0 -or $staleCeiling.Count -gt 0 -or $staleExempt.Count -gt 0) {
    exit 1
}

exit 0
