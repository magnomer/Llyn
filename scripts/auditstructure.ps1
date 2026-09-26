<#
.SYNOPSIS
Audit the ring chain of the source and the project edges, and report every step past a neighbour.

.DESCRIPTION
Reads the ring table from auditstructure.json, binds the tracked source with the shared binder of
auditbinder.cs, and classifies every symbol one ring names from another ring. A ring may reach the
one ring inside it that the table names for it; a data type of any inner ring may be carried across
any depth; nothing outward is ever named. A using of a deeper ring counts as data, and a method call
on a deeper record counts as behaviour. A pure ring names only the framework namespaces its frame
lists and never touches an ambient member such as the clock, the environment or the file system.
The project files under the source folder are held to the ring table edge by edge, no project hides
a reference from the table, and the cut projects that still compile transitively stay at a ceiling.

The reports are written to the configured report folder as AuditStructure-{version}.md and
AuditStructureViolated-{version}.md. Git and the .NET SDK are required, and the solution must be
built, since the shared binder reads the generated code and the host build output and refuses to
bind a tree with a compile error. No project source is modified.

The script reads no convention test file and no test report. The convention tests TAuditChain,
TAuditFrame and TAuditRing audit the same ground truth from their own settings, so either one
still tells the truth when the other is broken.

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
# AUDITSTRUCTURE GENERATION 15 - auditstructure.ps1.
# A generation is not a revision count. It names functionality, not edits, so editing one of these
# files is never on its own a reason to raise it. Raise it only when the audited outcome changes.
# A generation names the set of checks the audit applies. Two projects on the same generation audit
# the same things and their reports compare directly, whatever else differs between the files. A check
# added, removed, or changed in what it reports is a new generation; wording, plumbing, and refactoring
# leave it alone. Every audit script shares one generation number with the convention-test settings,
# and each refuses a configuration written at another generation.
# Generation 10 binds the source with Roslyn and holds every ring to one reach: a ring names the
# behaviour of the single ring the table lets it reach, carries the data of any inner ring, and
# names nothing outward. A pure ring names only the framework namespaces its frame lists and never
# touches an ambient member.
# A reach is a call, a construction, an implementation, a field or a base. A carry is a record, an
# enum, a struct or a delegate named in a signature or passed through. The distinction is the
# kind of the type named, read from the binder, not the position of the name in the line.
# A configuration is total: a missing key is an error, never a default, and an unknown key is an
# error rather than a silent no-op. A ceiling is the hit count a held pair may hold; a count above
# fails, a ceiling above the count is stale and fails too, so a ceiling only walks down. A check
# without a ceiling fails on its first hit. An exemption row is 'path:Name' or 'folder/*:Name' and
# clears a chain or frame hit alone; a row that matched nothing is stale and fails the run.
# Generation 11: nothing the structure audit reports changes; the number rises with the truth audit,
# which checks that a deportment field reaches no request, keeps one writer, holds no logic and
# treats no engine data.
# Generation 12: the audit reports what the convention tests TAuditChain, TAuditFrame and TAuditRing
# report. It binds through the shared binder of auditbinder.cs with no compile error, counts a
# ceiling in hits, counts a using of a deeper ring and a method call on a deeper record as a reach,
# walks using static names, and adds the expose, surface, stray, banned, floor, table, hidden and
# transitive checks.
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
    [switch]$Open,
    [switch]$NoPause,
    [Alias('?')]
    [switch]$Help
)

# Under Windows PowerShell 5.1 an advanced script evaluates a parameter default before
# $PSScriptRoot is available to it, so -Root arrives empty there while pwsh 7 resolves it.
if ([string]::IsNullOrWhiteSpace($Root)) {
    $Root = Split-Path -Parent $PSScriptRoot
}

# dotnet and git write UTF-8. A console still on the OEM code page would show every non-ASCII line
# garbled, so this process reads and writes UTF-8. Process-local: the calling console keeps its own.
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [Console]::OutputEncoding

if ($Help) {
    @'
NAME
    auditstructure.ps1

SYNOPSIS
    Audit the ring chain of the source and the project edges, and report every
    step past a neighbour.

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

CHECKS
    Held by a ceiling, one per pair check:Ring>Target, counted in hits:
        outward, reach, cross, expose, frame, ambient
    Held by the transitive ceiling, counted in cut projects:
        transitive
    Never allowed, failing on the first hit:
        surface, stray, banned, floor, table, hidden
    Reported only:
        carry, neighbour
    The frame and ambient checks read the pure rings alone. A configuration
    that lets a ring reach more than one ring, or whose cut differs from its
    shell folders, is refused before the audit runs.

CONFIGURATION
    auditstructure.json beside this script. A ring's folder is the source
    folder followed by the ring name. An exemption row is 'path:Name' or
    'folder/*:Name', matched ordinally, in exempt.chain or exempt.frame.
    The script reads no convention test file and no test report.

OUTPUT
    The console follows scripts\report.md: scope, result, findings by
    check, pairs, then one list per gate above zero.
    <report folder>\AuditStructure-{version}.md
    <report folder>\AuditStructureViolated-{version}.md

EXIT STATUS
    0   Every counter is 0.
    1   A counter is above 0: a pair above or below its ceiling, a stale
        exemption, or a hit of a check that allows none.

EXAMPLES
    auditstructure
        Audit the current checkout.

    auditstructure -Root C:\path\to\project -Open
        Audit a specific checkout and open the violation report.
'@ | Write-Host
    exit 0
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$script:AuditGeneration = 15
$script:ConfigDocument = 'auditstructure.json'
$script:BinderSource = [System.IO.File]::ReadAllText((Join-Path $PSScriptRoot 'auditbinder.cs'))
$script:PathSeparators = [char[]]@([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar)
$script:ItemLimit = 50

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

function Get-OrdinalSorted {
    param([string[]]$Items)

    $copy = [string[]]@($Items)
    [System.Array]::Sort($copy, [System.StringComparer]::Ordinal)
    return , $copy
}

function Write-AuditLine {
    param(
        [Parameter(Position = 0)][AllowEmptyString()][string]$Text = '',
        [ConsoleColor]$ForegroundColor,
        [string]$Lead = '',
        [ConsoleColor]$LeadColor = [ConsoleColor]::Gray
    )

    if ($script:PageLimit -gt 0) {
        $rows = [Math]::Max(1, [Math]::Ceiling(($Lead.Length + $Text.Length) / [double]$script:PageWidth))
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

    if ($Lead.Length -gt 0) {
        Write-Host $Lead -ForegroundColor $LeadColor -NoNewline
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
    Write-AuditLine $Title -ForegroundColor Blue
    Write-AuditLine ('-' * $Title.Length) -ForegroundColor DarkGray
}

function Write-AuditResult {
    param([object[]]$Rows)

    Write-AuditSection -Title 'Result'
    $statusWidth = 6
    $countWidth = [Math]::Max(5, ($Rows | ForEach-Object { $_.Count.ToString('N0').Length } | Measure-Object -Maximum).Maximum)
    $gateWidth = [Math]::Max(4, ($Rows | ForEach-Object { $_.Gate.Length } | Measure-Object -Maximum).Maximum)
    $meaningWidth = [Math]::Max(7, ($Rows | ForEach-Object { $_.Meaning.Length } | Measure-Object -Maximum).Maximum)
    Write-AuditLine ("{0}  {1}  {2}  Meaning" -f 'Status'.PadRight($statusWidth), 'Count'.PadLeft($countWidth), 'Gate'.PadRight($gateWidth)) -ForegroundColor Cyan
    Write-AuditLine (@(('-' * $statusWidth), ('-' * $countWidth), ('-' * $gateWidth), ('-' * $meaningWidth)) -join '  ') -ForegroundColor Cyan
    foreach ($row in $Rows) {
        $failing = $row.Count -gt 0
        $status = if ($failing) { 'FAIL' } else { 'OK' }
        $text = "{0}  {1}  {2}  {3}" -f ''.PadRight($statusWidth - $status.Length), $row.Count.ToString('N0').PadLeft($countWidth), $row.Gate.PadRight($gateWidth), $row.Meaning
        Write-AuditLine $text -Lead $status -LeadColor $(if ($failing) { 'Red' } else { 'Green' })
    }

    $failed = @($Rows | Where-Object { $_.Count -gt 0 })
    Write-AuditLine ''
    if ($failed.Count -eq 0) {
        Write-AuditLine ("PASS: all {0} gates at 0." -f $Rows.Count) -ForegroundColor Green
    }
    else {
        $sections = ($failed | ForEach-Object { '"' + $_.Section + '"' }) -join ', '
        Write-AuditLine ("FAIL: {0} of {1} gates above 0. See {2}." -f $failed.Count, $Rows.Count, $sections) -ForegroundColor Red
    }
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
    Write-AuditLine (& $format $Header) -ForegroundColor Cyan
    Write-AuditLine (($widths | ForEach-Object { '-' * $_ }) -join '  ') -ForegroundColor Cyan
    foreach ($row in $Rows) {
        Write-AuditLine (& $format ([string[]]$row))
    }
}

function Write-AuditItems {
    # A list section: the heading counts every row, and a long list is cut short with a pointer to the report.
    param([string]$Title, [string[]]$Lines)

    if ($Lines.Count -eq 0) {
        return
    }

    Write-AuditSection -Title "$Title ($($Lines.Count.ToString('N0')))"
    $shown = [Math]::Min($Lines.Count, $script:ItemLimit)
    for ($index = 0; $index -lt $shown; $index++) {
        Write-AuditLine $Lines[$index]
    }

    if ($Lines.Count -gt $shown) {
        Write-AuditLine ("... and {0:N0} more in the report." -f ($Lines.Count - $shown))
    }
}

Write-AuditLine "AUDITSTRUCTURE GENERATION $script:AuditGeneration" -ForegroundColor Blue

$script:ReportName = 'AuditStructure-{version}.md'
$script:ViolationName = 'AuditStructureViolated-{version}.md'
$script:VersionKey = 'current-version'

# Each entry is a path through the document and the kind of value that must be found there.
# 'string', 'count' for an integer of zero or more, 'string[]', 'ring[]' for the ring table,
# 'map[]' for an object whose every property is an array of strings, or 'map[int]' for an object
# whose every property is an integer of zero or more.
$script:AuditSchema = [ordered]@{
    'generation'       = 'count'
    'project'          = 'string'
    'framework'        = 'string'
    'source'           = 'string'
    'rings'            = 'ring[]'
    'host.name'        = 'string'
    'host.reach'       = 'string[]'
    'shells'           = 'string[]'
    'imports'          = 'string[]'
    'ambient'          = 'string[]'
    'surface'          = 'map[]'
    'stray'            = 'map[]'
    'banned'           = 'map[]'
    'floor'            = 'map[int]'
    'ceilings'         = 'map[int]'
    'transitive'       = 'count'
    'exempt.chain'     = 'string[]'
    'exempt.frame'     = 'string[]'
    'report.directory' = 'string'
    'report.versionFile' = 'string'
}

$script:RingSchema = [ordered]@{
    'name'  = 'string'
    'reach' = 'string[]'
    'frame' = 'string[]'
    'pure'  = 'bool'
    'cut'   = 'bool'
}

# A held check counts its hits per pair against a ceiling. A hard check allows no hit at all.
# A reported check never gates. The transitive check counts cut projects against its own ceiling.
$script:HeldChecks = @('outward', 'reach', 'cross', 'expose', 'frame', 'ambient')
$script:HardChecks = @('surface', 'stray', 'banned', 'floor', 'table', 'hidden')
$script:ChainChecks = @('neighbour', 'carry', 'outward', 'reach', 'cross', 'expose', 'surface')
$script:FrameChecks = @('frame', 'ambient')

# The order is the order the checks are reported in, heaviest first.
$script:CheckOrder = @('outward', 'reach', 'cross', 'expose', 'frame', 'ambient', 'transitive',
    'surface', 'stray', 'banned', 'floor', 'table', 'hidden', 'carry', 'neighbour')

$script:CheckTitles = @{
    'outward'    = 'Outer ring named from an inner ring'
    'reach'      = 'Behaviour reached past the neighbour ring'
    'cross'      = 'Name from below the cut inside a UI ring'
    'expose'     = 'Surface signature naming a type from below its neighbour'
    'frame'      = 'Framework namespace outside the frame of a pure ring'
    'ambient'    = 'Ambient member touched from a pure ring'
    'transitive' = 'Cut project compiling against rings past its neighbour'
    'surface'    = 'Neighbour name outside the surface the ring may reach'
    'stray'      = 'Type declared in a ring that may not hold it'
    'banned'     = 'Banned word inside a folder'
    'floor'      = 'Ring holding fewer source files than its floor'
    'table'      = 'Project edge differing from the ring table'
    'hidden'     = 'Reference or source link bypassing the ring table'
    'carry'      = 'Data carried from a deeper ring'
    'neighbour'  = 'Neighbour ring reached'
}

$script:HardLabels = [ordered]@{
    'surface' = 'Outside surface'
    'stray'   = 'Stray types'
    'banned'  = 'Banned words'
    'floor'   = 'Thin rings'
    'table'   = 'Table drift'
    'hidden'  = 'Hidden references'
}

function Get-CheckGate {
    param([string]$Check)

    if ($script:HeldChecks -ccontains $Check -or $Check -ceq 'transitive') { return 'held by ceiling' }
    if ($script:HardChecks -ccontains $Check) { return 'none allowed' }
    return 'reported only'
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
        if ($null -eq $node -or -not ($node -is [System.Management.Automation.PSCustomObject]) -or
            -not ($node.PSObject.Properties.Name -ccontains $segment)) {
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
        'count' { return ($Value -is [int] -or $Value -is [long]) -and $Value -ge 0 }
        'bool' { return $Value -is [bool] }
        'string[]' { return Test-StringArray -Value $Value }
        'ring[]' {
            if ($null -eq $Value -or -not ($Value -is [System.Array]) -or $Value.Count -eq 0) { return $false }
            foreach ($ring in $Value) {
                if ($null -eq $ring -or -not ($ring -is [System.Management.Automation.PSCustomObject])) { return $false }
                foreach ($key in $script:RingSchema.Keys) {
                    if (-not ($ring.PSObject.Properties.Name -ccontains $key)) { return $false }
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

    $ringsNode = Get-AuditNode -Document $config -Key 'rings'
    if ($ringsNode.Found -and (Test-AuditValue -Kind 'ring[]' -Value $ringsNode.Value)) {
        $names = [string[]]@($ringsNode.Value | ForEach-Object { [string]$_.name })
        $seen = New-Object 'System.Collections.Generic.HashSet[string]' ([System.StringComparer]::Ordinal)
        foreach ($name in $names) {
            if (-not $seen.Add($name)) {
                [void]$problems.Add("ring '$name' is declared more than once")
            }
        }

        foreach ($ring in $ringsNode.Value) {
            $reach = @($ring.reach)
            if ($reach.Count -gt 1) {
                [void]$problems.Add("ring '$($ring.name)' reaches $($reach -join ', '), more than one ring")
            }

            foreach ($target in $reach) {
                if (-not ($names -ccontains $target)) {
                    [void]$problems.Add("ring '$($ring.name)' reaches an undeclared ring '$target'")
                }

                if ($target -ceq $ring.name) {
                    [void]$problems.Add("ring '$($ring.name)' reaches itself")
                }
            }

            if ($ring.pure -and @($ring.frame).Count -eq 0) {
                [void]$problems.Add("pure ring '$($ring.name)' lists no frame namespace")
            }

            if (-not $ring.pure -and @($ring.frame).Count -gt 0) {
                [void]$problems.Add("ring '$($ring.name)' is not pure, so no frame check reads its frame; leave it empty")
            }
        }

        $hostNode = Get-AuditNode -Document $config -Key 'host.name'
        if ($hostNode.Found -and $names -ccontains [string]$hostNode.Value) {
            [void]$problems.Add("host '$($hostNode.Value)' is also declared as a ring")
        }

        $shellsNode = Get-AuditNode -Document $config -Key 'shells'
        if ($shellsNode.Found -and (Test-StringArray -Value $shellsNode.Value)) {
            $cut = Get-OrdinalSorted -Items @($ringsNode.Value | Where-Object { $_.cut } | ForEach-Object { [string]$_.name })
            $shells = Get-OrdinalSorted -Items @($shellsNode.Value | ForEach-Object {
                $folder = ([string]$_).Replace('\', '/').TrimEnd('/')
                $folder.Substring($folder.LastIndexOf('/') + 1)
            })
            if (($cut -join "`n") -cne ($shells -join "`n")) {
                [void]$problems.Add("the cut holds [$($cut -join ', ')] but the UI rings are [$($shells -join ', ')]")
            }
        }
    }

    $ceilingsNode = Get-AuditNode -Document $config -Key 'ceilings'
    if ($ceilingsNode.Found -and (Test-AuditValue -Kind 'map[int]' -Value $ceilingsNode.Value)) {
        $held = $script:HeldChecks -join '|'
        foreach ($property in $ceilingsNode.Value.PSObject.Properties) {
            if ($property.Name -cnotmatch "^($held):[^>]+>.+$") {
                [void]$problems.Add("ceiling '$($property.Name)' must be written as check:Ring>Target for a held check ($($script:HeldChecks -join ', '))")
            }
        }
    }

    foreach ($list in @('chain', 'frame')) {
        $exemptNode = Get-AuditNode -Document $config -Key "exempt.$list"
        if ($exemptNode.Found -and (Test-StringArray -Value $exemptNode.Value)) {
            foreach ($row in $exemptNode.Value) {
                $split = $row.LastIndexOf(':')
                if ($split -le 0 -or $split -ge $row.Length - 1) {
                    [void]$problems.Add("exemption '$row' in exempt.$list must be written as path:Name or folder/*:Name")
                }
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

function Test-ExemptRow {
    # The row format of the convention tests: 'path:Name' or 'folder/*:Name', every part ordinal.
    param(
        [Parameter(Mandatory = $true)][string]$Row,
        [Parameter(Mandatory = $true)][AllowEmptyString()][string]$Path,
        [Parameter(Mandatory = $true)][AllowEmptyString()][string]$Name
    )

    $split = $Row.LastIndexOf(':')
    $pattern = $Row.Substring(0, $split)
    if (-not [string]::Equals($Row.Substring($split + 1), $Name, [System.StringComparison]::Ordinal)) {
        return $false
    }

    if ($pattern.EndsWith('/*', [System.StringComparison]::Ordinal)) {
        return $Path.StartsWith($pattern.Substring(0, $pattern.Length - 1), [System.StringComparison]::Ordinal)
    }

    return [string]::Equals($pattern, $Path, [System.StringComparison]::Ordinal)
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
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.14.0" />
  </ItemGroup>
</Project>
'@

$script:HelperProgram = @'
using System.Collections.Concurrent;
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
string source = config.GetProperty("source").GetString()!.Replace('\\', '/').Trim('/') + "/";
List<Ring> rings = config.GetProperty("rings").EnumerateArray()
    .Select(ring => new Ring(
        ring.GetProperty("name").GetString()!,
        source + ring.GetProperty("name").GetString()! + "/",
        Strings(ring.GetProperty("reach")),
        Strings(ring.GetProperty("frame")),
        ring.GetProperty("pure").GetBoolean(),
        ring.GetProperty("cut").GetBoolean()))
    .ToList();
string hostName = config.GetProperty("host").GetProperty("name").GetString()!;
string[] hostReach = Strings(config.GetProperty("host").GetProperty("reach"));
string[] imports = Strings(config.GetProperty("imports"));
string[] ambient = Strings(config.GetProperty("ambient"));
Dictionary<string, string[]> surfaces = Map(config.GetProperty("surface"));
Dictionary<string, string[]> strays = Map(config.GetProperty("stray"));
Dictionary<string, string[]> banned = Map(config.GetProperty("banned"));
Dictionary<string, int> floors = config.GetProperty("floor").EnumerateObject()
    .ToDictionary(item => item.Name, item => item.Value.GetInt32(), StringComparer.Ordinal);
string[] itemNames = ["ProjectReference", "Reference", "Compile"];

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

LAuditBinder binder = LAuditBinder.LAuditBinderRead(projectRoot, binderPath);
CSharpCompilation compilation = binder.LAuditCompilation;

ConcurrentDictionary<string, Ring?> ringOfPath = new(StringComparer.Ordinal);
Ring? RingOf(string relative) => ringOfPath.GetOrAdd(relative, key => rings
    .FirstOrDefault(candidate => key.StartsWith(candidate.Path, StringComparison.OrdinalIgnoreCase)));

Ring? SpaceRing(string space) => rings
    .Where(candidate => space == candidate.Name || space.StartsWith(candidate.Name + ".", StringComparison.Ordinal))
    .MaxBy(candidate => candidate.Name.Length);

Ring? SourceRing(INamedTypeSymbol type)
{
    Location? location = type.Locations.FirstOrDefault(place => place.IsInSource);
    return location?.SourceTree is null ? null : RingOf(binder.LAuditRelativeRead(location.SourceTree.FilePath));
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

int outside = 0;
ConcurrentBag<List<Finding>> bags = [];
Parallel.ForEach(binder.LAuditTrees, tree =>
{
    string relative = binder.LAuditRelativeRead(tree.FilePath);
    Ring? ring = RingOf(relative);
    List<Finding> findings = [];

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
                findings.Add(new Finding(relative, index + 1, ring?.Name ?? "", "banned", folder, word, text.Trim()));
            }
        }
    }

    if (ring is null)
    {
        Interlocked.Increment(ref outside);
        bags.Add(findings);
        return;
    }

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

        string check = ring.Reach.Contains(target.Name) ? "neighbour"
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
        if (ring.Pure && directive.Name is not null)
        {
            string space = directive.Name.ToString();
            if (!space.StartsWith(project + ".", StringComparison.Ordinal) && !ring.Frame.Contains(space))
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
        Location? location = type.Locations.FirstOrDefault(place => place.IsInSource);
        if (location is null)
        {
            if (header || !ring.Pure)
            {
                continue;
            }

            string space = NamespaceOf(type.ContainingNamespace);
            if (space.Length > 0 && !ring.Frame.Contains(space))
            {
                Add(line, "frame", space, space);
            }

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

            continue;
        }

        Ring? targetRing = RingOf(binder.LAuditRelativeRead(location.SourceTree!.FilePath));
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
        int line = declaration.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        foreach (string pattern in strays.GetValueOrDefault(ring.Name, []).Where(pattern => Regex.IsMatch(declared, pattern)))
        {
            findings.Add(new Finding(relative, line, ring.Name, "stray", pattern, declared, LineText(tree, line)));
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

        string relative = binder.LAuditRelativeRead(location.SourceTree!.FilePath);
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

Dictionary<string, int> files = binder.LAuditTrees
    .Select(tree => RingOf(binder.LAuditRelativeRead(tree.FilePath))?.Name)
    .Where(name => name is not null)
    .GroupBy(name => name!, StringComparer.Ordinal)
    .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);
foreach ((string ringName, int floor) in floors)
{
    int count = files.GetValueOrDefault(ringName);
    if (count < floor)
    {
        all.Add(new Finding(source + ringName, 0, ringName, "floor", ringName, $"{count} source file(s), floor {floor}", ""));
    }
}

List<string> projectFiles = LAuditBinder.LAuditFileRead(projectRoot, [source + "*.csproj"], [], [], [], []);
Dictionary<string, string> projectOf = projectFiles
    .ToDictionary(path => Path.GetFileNameWithoutExtension(path), path => path, StringComparer.Ordinal);
Dictionary<string, string[]> edges = rings.ToDictionary(ring => ring.Name, ring => ring.Reach, StringComparer.Ordinal);
edges[hostName] = hostReach;
foreach (string name in projectOf.Keys.Union(edges.Keys, StringComparer.Ordinal).Order(StringComparer.Ordinal))
{
    string? projectFile = projectOf.GetValueOrDefault(name);
    string[] actual = projectFile is null ? [] : EdgeOf(projectFile);
    string[] expected = edges.GetValueOrDefault(name, []);
    string where = projectFile is null ? source + name : binder.LAuditRelativeRead(projectFile);
    foreach (string edge in actual.Except(expected, StringComparer.Ordinal))
    {
        all.Add(new Finding(where, 0, name, "table", edge, $"{name} -> {edge} is referenced but not in the ring table", ""));
    }

    foreach (string edge in expected.Except(actual, StringComparer.Ordinal))
    {
        all.Add(new Finding(where, 0, name, "table", edge, $"{name} -> {edge} is in the ring table but not referenced", ""));
    }
}

foreach (string projectFile in projectFiles)
{
    string folder = Path.GetDirectoryName(projectFile)!;
    string owner = binder.LAuditRelativeRead(projectFile);
    string name = Path.GetFileNameWithoutExtension(projectFile);
    foreach (XElement item in XDocument.Load(projectFile).Descendants())
    {
        string include = (string?)item.Attribute("Include") ?? string.Empty;
        bool linked = item.Name.LocalName == "Compile"
            && (item.Attribute("Link") is not null
                || !Path.GetFullPath(Path.Combine(folder, include)).StartsWith(
                    folder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase));
        bool binary = item.Name.LocalName == "Reference" && item.Elements().Any(child => child.Name.LocalName == "HintPath");
        if (linked || binary)
        {
            all.Add(new Finding(owner, 0, name, "hidden", item.Name.LocalName, $"{item.Name.LocalName} {include}", ""));
        }
    }

    bool closed = XDocument.Load(projectFile).Descendants()
        .Where(node => node.Name.LocalName == "DisableTransitiveProjectReferences")
        .Any(node => node.Value.Trim().Equals("true", StringComparison.OrdinalIgnoreCase));
    if (rings.Any(ring => ring.Cut && ring.Name == name) && !closed)
    {
        all.Add(new Finding(owner, 0, name, "transitive", name, $"{name} compiles against rings past its neighbour", ""));
    }
}

foreach (string import in LAuditBinder.LAuditFileRead(projectRoot, imports.Select(name => "*" + name), [], [], [], []))
{
    string owner = binder.LAuditRelativeRead(import);
    all.AddRange(XDocument.Load(import).Descendants()
        .Where(item => itemNames.Contains(item.Name.LocalName, StringComparer.Ordinal))
        .Select(item => new Finding(owner, 0, "", "hidden", item.Name.LocalName, item.Name.LocalName, "")));
}

all = all
    .OrderBy(finding => finding.Check, StringComparer.Ordinal)
    .ThenBy(finding => finding.Ring, StringComparer.Ordinal)
    .ThenBy(finding => finding.Target, StringComparer.Ordinal)
    .ThenBy(finding => finding.Path, StringComparer.Ordinal)
    .ThenBy(finding => finding.Line)
    .ThenBy(finding => finding.Name, StringComparer.Ordinal)
    .ToList();

int audited = binder.LAuditTrees.Count - outside;
JsonSerializerOptions options = new() { WriteIndented = false };
File.WriteAllText(outputPath, JsonSerializer.Serialize(new Summary(audited, outside, all), options));
return 0;

static string[] EdgeOf(string projectFile) => XDocument.Load(projectFile).Descendants()
    .Where(node => node.Name.LocalName == "ProjectReference")
    .Select(node => ((string?)node.Attribute("Include") ?? string.Empty).Replace('\\', '/'))
    .Select(include => Path.GetFileNameWithoutExtension(include[(include.LastIndexOf('/') + 1)..]))
    .Order(StringComparer.Ordinal)
    .ToArray();

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

sealed record Summary(int Files, int Outside, List<Finding> Findings);
'@

function Write-AuditHelper {
    # The binder is Roslyn 4.14.0, the version the convention tests pin, restored once into the
    # package cache. The helper reads the configuration, binds every file through the shared binder,
    # and writes the file counts and every finding to the output path as JSON.
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
        [Parameter(Mandatory = $true)][string]$ProjectName,
        [Parameter(Mandatory = $true)][string]$TargetFramework
    )

    $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($null -eq $dotnet) {
        throw 'The .NET SDK is required, but dotnet was not found on PATH.'
    }

    if ($null -eq (Get-Command git -ErrorAction SilentlyContinue)) {
        throw 'Git is required to enumerate project source files, but git was not found on PATH.'
    }

    if (-not (Test-Path -LiteralPath (Join-Path $ProjectRoot '.git'))) {
        throw "The project root is not a Git working tree: $ProjectRoot"
    }

    $nativePreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    $sdkOutput = & $dotnet.Source --version 2>&1
    $ErrorActionPreference = $nativePreference
    if ($LASTEXITCODE -ne 0) {
        throw "The .NET SDK version could not be read.`n$($sdkOutput -join [Environment]::NewLine)"
    }

    $sdkVersion = ([string]($sdkOutput | Select-Object -First 1)).Trim()

    $previousNoLogo = $env:DOTNET_NOLOGO
    $previousTelemetry = $env:DOTNET_CLI_TELEMETRY_OPTOUT
    $env:DOTNET_NOLOGO = '1'
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'

    $temporaryFolder = Join-Path ([System.IO.Path]::GetTempPath()) ($ProjectName + '-AuditStructure-' + [Guid]::NewGuid().ToString('N'))
    [System.IO.Directory]::CreateDirectory($temporaryFolder) | Out-Null

    try {
        $cacheFolder = Get-HelperCacheFolder -ProjectName $ProjectName -TargetFramework $TargetFramework -SdkVersion $sdkVersion
        $binaryPath = Join-Path (Join-Path $cacheFolder 'bin') 'AuditStructure.dll'
        if (-not (Test-Path -LiteralPath $binaryPath -PathType Leaf)) {
            Write-AuditLine 'Compiling the structure binder once for this SDK...'
            $cacheParent = Split-Path -Parent $cacheFolder
            if (Test-Path -LiteralPath $cacheParent) {
                Get-ChildItem -LiteralPath $cacheParent -Directory |
                    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
            }

            $helperFolder = Join-Path $cacheFolder 'helper'
            [System.IO.Directory]::CreateDirectory($helperFolder) | Out-Null
            $projectPath = Write-AuditHelper -HelperFolder $helperFolder -TargetFramework $TargetFramework
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

        return (Get-Content -LiteralPath $outputPath -Raw -Encoding UTF8 | ConvertFrom-Json)
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

function Format-Location {
    # A source hit is path:line; a project, folder or ring finding has no line and prints its path.
    param($Finding)

    if ($Finding.Line -gt 0) {
        return "$($Finding.Path):$($Finding.Line)"
    }

    return $Finding.Path
}

function Format-FindingLine {
    param($Finding)

    $location = Format-Location -Finding $Finding
    switch -CaseSensitive ($Finding.Check) {
        'surface' { return "$location $($Finding.Name) outside $($Finding.Ring)>$($Finding.Target)" }
        'stray' { return "$location $($Finding.Name) ~ $($Finding.Target)" }
        default { return "$location $($Finding.Name)" }
    }
}

$projectRoot = Resolve-ProjectRoot -Path $Root
$config = Read-AuditConfig -ProjectRoot $projectRoot
$configPath = Join-Path $PSScriptRoot $script:ConfigDocument
$version = Read-ProjectVersion -ProjectRoot $projectRoot -Relative $config.report.versionFile
$source = ([string]$config.source).Replace('\', '/').Trim('/') + '/'

$result = Invoke-AuditHelper -ProjectRoot $projectRoot -ConfigPath $configPath -ProjectName ([string]$config.project) -TargetFramework ([string]$config.framework)
$auditedFiles = [int]$result.Files
$skippedFiles = [int]$result.Outside
if ($auditedFiles -eq 0) {
    throw "No tracked source file sits inside a declared ring under: $projectRoot"
}

Write-AuditLine ("Scanned: {0:N0} source files inside a ring, {1:N0} outside" -f $auditedFiles, $skippedFiles) -ForegroundColor DarkGray

$findings = New-Object 'System.Collections.Generic.List[object]'
foreach ($finding in @($result.Findings | ForEach-Object { $_ })) {
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

# An exemption clears a chain hit from the chain list or a frame hit from the frame list, the first
# matching row winning. A project, folder or declaration finding is never exempt.
$exemptRows = New-Object 'System.Collections.Generic.List[object]'
foreach ($list in @('chain', 'frame')) {
    foreach ($row in @($config.exempt.$list)) {
        [void]$exemptRows.Add([pscustomobject]@{ List = $list; Row = [string]$row; Used = 0 })
    }
}

if ($exemptRows.Count -gt 0) {
    foreach ($finding in $findings) {
        $list = if ($script:ChainChecks -ccontains $finding.Check) { 'chain' }
            elseif ($script:FrameChecks -ccontains $finding.Check) { 'frame' }
            else { '' }
        if ($list.Length -eq 0) {
            continue
        }

        foreach ($row in $exemptRows) {
            if ($row.List -ceq $list -and (Test-ExemptRow -Row $row.Row -Path $finding.Path -Name $finding.Name)) {
                $finding.Exempt = $true
                $row.Used++
                break
            }
        }
    }
}

$staleExempt = @($exemptRows | Where-Object { $_.Used -eq 0 })

$live = New-Object 'System.Collections.Generic.List[object]'
$counts = @{}
foreach ($check in $script:CheckOrder) {
    $counts[$check] = 0
}

foreach ($finding in $findings) {
    if ($finding.Exempt) {
        continue
    }

    [void]$live.Add($finding)
    if (-not $counts.ContainsKey($finding.Check)) {
        throw "The structure binder reported an unknown check: $($finding.Check)"
    }

    $counts[$finding.Check]++
}

# A pair is one held check from one ring to one target. Its count is the number of distinct hits,
# one per file, line, check, target and name, as in the convention tests. A pair is held to the
# ceiling written for it, an unwritten ceiling being zero. The transitive pair counts cut projects.
$pairOf = New-Object 'System.Collections.Generic.Dictionary[string,object]' ([System.StringComparer]::Ordinal)
function Get-AuditPair {
    param([string]$Key, [string]$Check, [string]$Ring, [string]$Target)

    if (-not $pairOf.ContainsKey($Key)) {
        $pairOf[$Key] = [pscustomobject]@{
            Key     = $Key
            Check   = $Check
            Ring    = $Ring
            Target  = $Target
            Files   = New-Object 'System.Collections.Generic.HashSet[string]' ([System.StringComparer]::Ordinal)
            Hits    = New-Object 'System.Collections.Generic.List[object]'
            Ceiling = 0
        }
    }

    return $pairOf[$Key]
}

foreach ($finding in $live) {
    if ($script:HeldChecks -ccontains $finding.Check) {
        $pair = Get-AuditPair -Key "$($finding.Check):$($finding.Ring)>$($finding.Target)" -Check $finding.Check -Ring $finding.Ring -Target $finding.Target
    }
    elseif ($finding.Check -ceq 'transitive') {
        $pair = Get-AuditPair -Key 'transitive' -Check 'transitive' -Ring '' -Target ''
    }
    else {
        continue
    }

    [void]$pair.Files.Add($finding.Path)
    [void]$pair.Hits.Add($finding)
}

foreach ($property in $config.ceilings.PSObject.Properties) {
    $parts = $property.Name -split '[:>]', 3
    $pair = Get-AuditPair -Key $property.Name -Check $parts[0] -Ring $parts[1] -Target $parts[2]
    $pair.Ceiling = [int]$property.Value
}

if ([int]$config.transitive -gt 0) {
    $pair = Get-AuditPair -Key 'transitive' -Check 'transitive' -Ring '' -Target ''
    $pair.Ceiling = [int]$config.transitive
}

$pairs = @($pairOf.Values | Sort-Object -Property @{ Expression = { [array]::IndexOf($script:CheckOrder, $_.Check) } }, @{ Expression = { Get-OrdinalKey $_.Ring } }, @{ Expression = { Get-OrdinalKey $_.Target } })
$overCeiling = @($pairs | Where-Object { $_.Hits.Count -gt $_.Ceiling })
$staleCeiling = @($pairs | Where-Object { $_.Hits.Count -lt $_.Ceiling })
$heldPairs = @($pairs | Where-Object { $_.Hits.Count -gt 0 -and $_.Hits.Count -le $_.Ceiling })

$hardOf = @{}
foreach ($check in $script:HardChecks) {
    $hardOf[$check] = New-Object 'System.Collections.Generic.List[object]'
}

foreach ($finding in $live) {
    if ($hardOf.ContainsKey($finding.Check)) {
        [void]$hardOf[$finding.Check].Add($finding)
    }
}

# The violations are the hits of every pair above its ceiling and every hit of a check that allows none.
$violations = New-Object 'System.Collections.Generic.List[object]'
foreach ($pair in $overCeiling) {
    foreach ($hit in $pair.Hits) {
        [void]$violations.Add($hit)
    }
}

foreach ($check in $script:HardChecks) {
    foreach ($hit in $hardOf[$check]) {
        [void]$violations.Add($hit)
    }
}

$heldHits = New-Object 'System.Collections.Generic.List[object]'
foreach ($pair in $heldPairs) {
    foreach ($hit in $pair.Hits) {
        [void]$heldHits.Add($hit)
    }
}

$countTotal = 0
foreach ($check in $script:CheckOrder) {
    $countTotal += $counts[$check]
}

$counters = [ordered]@{
    'Above ceiling'    = $overCeiling.Count
    'Stale ceilings'   = $staleCeiling.Count
    'Stale exemptions' = $staleExempt.Count
}
foreach ($check in $script:HardLabels.Keys) {
    $counters[$script:HardLabels[$check]] = $hardOf[$check].Count
}

$reportFolder = Join-AuditPath -ProjectRoot $projectRoot -Relative $config.report.directory
if (-not (Test-Path -LiteralPath $reportFolder -PathType Container)) {
    [void][System.IO.Directory]::CreateDirectory($reportFolder)
}

$reportPath = Join-Path $reportFolder ($script:ReportName.Replace('{version}', $version))
$violationPath = Join-Path $reportFolder ($script:ViolationName.Replace('{version}', $version))

$gated = @($live | Where-Object { $script:HeldChecks -ccontains $_.Check -or $script:HardChecks -ccontains $_.Check })

$report = New-Object 'System.Collections.Generic.List[string]'
[void]$report.Add("# Structure $version")
[void]$report.Add('')
[void]$report.Add("- Version: ``$version``")
[void]$report.Add("- Generation: $script:AuditGeneration")
[void]$report.Add("- Files audited: $auditedFiles")
[void]$report.Add("- Files outside a declared ring: $skippedFiles")
foreach ($label in $counters.Keys) {
    [void]$report.Add("- ${label}: $($counters[$label])")
}
[void]$report.Add('')
[void]$report.Add('The audit binds the source and reads the kind of every type one ring names from another. A')
[void]$report.Add('ring reaches the one ring the table names for it and carries the data of any ring inside it;')
[void]$report.Add('a class or an interface named from deeper than the neighbour is a reach, and a name from an')
[void]$report.Add('outer ring is never right. A pure ring names only the framework namespaces its frame lists')
[void]$report.Add('and never touches an ambient member. The project files hold the ring table edge by edge.')
[void]$report.Add('')
[void]$report.Add('## Rings')
[void]$report.Add('')
[void]$report.Add('| Ring | Folder | Reaches | Pure | Cut |')
[void]$report.Add('|---|---|---|---|---|')
foreach ($ring in $config.rings) {
    $reaches = if (@($ring.reach).Count -eq 0) { 'nothing' } else { (@($ring.reach) | ForEach-Object { "``$_``" }) -join ', ' }
    [void]$report.Add("| ``$($ring.name)`` | ``$source$($ring.name)`` | $reaches | $(if ($ring.pure) { 'yes' } else { 'no' }) | $(if ($ring.cut) { 'yes' } else { 'no' }) |")
}
$hostReaches = (@($config.host.reach) | ForEach-Object { "``$_``" }) -join ', '
[void]$report.Add("| ``$($config.host.name)`` (host) | ``$source$($config.host.name)`` | $hostReaches | no | no |")

[void]$report.Add('')
[void]$report.Add('## Checks')
[void]$report.Add('')
[void]$report.Add('| Check | Meaning | Gate | Hits |')
[void]$report.Add('|---|---|---|---|')
foreach ($check in $script:CheckOrder) {
    [void]$report.Add("| ``$check`` | $($script:CheckTitles[$check]) | $(Get-CheckGate -Check $check) | $($counts[$check]) |")
}
[void]$report.Add("| Total | | | $countTotal |")

[void]$report.Add('')
[void]$report.Add('## Pairs')
[void]$report.Add('')
if ($pairs.Count -eq 0) {
    [void]$report.Add('No held pair has a hit or a ceiling.')
}
else {
    [void]$report.Add('| Pair | Files | Hits | Ceiling | Standing |')
    [void]$report.Add('|---|---|---|---|---|')
    foreach ($pair in $pairs) {
        $standing = if ($pair.Hits.Count -gt $pair.Ceiling) { 'above ceiling' }
            elseif ($pair.Hits.Count -lt $pair.Ceiling) { 'stale ceiling' }
            else { 'at ceiling' }
        [void]$report.Add("| ``$($pair.Key)`` | $($pair.Files.Count) | $($pair.Hits.Count) | $($pair.Ceiling) | $standing |")
    }
}

[void]$report.Add('')
[void]$report.Add('## Names')
[void]$report.Add('')
if ($gated.Count -eq 0) {
    [void]$report.Add('No name stepped past its ring.')
}
else {
    [void]$report.Add('| Name | Target | Hits | Files | Heaviest check |')
    [void]$report.Add('|---|---|---|---|---|')
    $nameOf = New-Object 'System.Collections.Generic.Dictionary[string,object]' ([System.StringComparer]::Ordinal)
    foreach ($finding in $gated) {
        $key = "$($finding.Name)`n$($finding.Target)"
        if (-not $nameOf.ContainsKey($key)) {
            $nameOf[$key] = [pscustomobject]@{
                Name  = $finding.Name
                Target = $finding.Target
                Hits  = 0
                Files = New-Object 'System.Collections.Generic.HashSet[string]' ([System.StringComparer]::Ordinal)
                Heaviest = $finding.Check
            }
        }

        $entry = $nameOf[$key]
        $entry.Hits++
        [void]$entry.Files.Add($finding.Path)
        if ([array]::IndexOf($script:CheckOrder, $finding.Check) -lt [array]::IndexOf($script:CheckOrder, $entry.Heaviest)) {
            $entry.Heaviest = $finding.Check
        }
    }

    $entries = @($nameOf.Values | Sort-Object -Property @{ Expression = { $_.Hits }; Descending = $true }, @{ Expression = { Get-OrdinalKey $_.Name } }, @{ Expression = { Get-OrdinalKey $_.Target } })
    foreach ($entry in $entries) {
        [void]$report.Add("| ``$($entry.Name)`` | ``$($entry.Target)`` | $($entry.Hits) | $($entry.Files.Count) | ``$($entry.Heaviest)`` |")
    }
}

[void]$report.Add('')
[void]$report.Add('## Files')
[void]$report.Add('')
if ($gated.Count -eq 0) {
    [void]$report.Add('No file stepped past its ring.')
}
else {
    [void]$report.Add('| File | Hits |')
    [void]$report.Add('|---|---|')
    $fileOf = New-Object 'System.Collections.Generic.Dictionary[string,int]' ([System.StringComparer]::Ordinal)
    foreach ($finding in $gated) {
        if (-not $fileOf.ContainsKey($finding.Path)) {
            $fileOf[$finding.Path] = 0
        }

        $fileOf[$finding.Path]++
    }

    $fileRows = @($fileOf.GetEnumerator() | Sort-Object -Property @{ Expression = { $_.Value }; Descending = $true }, @{ Expression = { Get-OrdinalKey $_.Key } })
    foreach ($row in $fileRows) {
        [void]$report.Add("| ``$($row.Key)`` | $($row.Value) |")
    }
}

[void]$report.Add('')
[void]$report.Add('## Exemptions')
[void]$report.Add('')
if ($exemptRows.Count -eq 0) {
    [void]$report.Add('The configuration declares no exemption.')
}
else {
    [void]$report.Add('| List | Row | Cleared |')
    [void]$report.Add('|---|---|---|')
    foreach ($row in $exemptRows) {
        [void]$report.Add("| $($row.List) | ``$($row.Row)`` | $($row.Used) |")
    }
}

$violationReport = New-Object 'System.Collections.Generic.List[string]'
[void]$violationReport.Add("# Violated Structure $version")
[void]$violationReport.Add('')
[void]$violationReport.Add("- Version: ``$version``")
[void]$violationReport.Add("- Generation: $script:AuditGeneration")
foreach ($label in $counters.Keys) {
    [void]$violationReport.Add("- ${label}: $($counters[$label])")
}
[void]$violationReport.Add("- Hits held by ceilings: $($heldHits.Count)")
[void]$violationReport.Add('')
[void]$violationReport.Add('Every violation below is a move until the user says otherwise. A ring that reaches past its')
[void]$violationReport.Add('neighbour makes the ring between them optional: the engine that calls a vault is a second')
[void]$violationReport.Add('use-case layer, the view that names the engine is a second presenter. Resolve a reach by')
[void]$violationReport.Add('handing the work to the neighbour ring, a frame by moving the framework call behind a port,')
[void]$violationReport.Add('and an ambient touch by taking the clock or the file through a port. Never resolve one by')
[void]$violationReport.Add('turning a class into a record so it reads as data.')
[void]$violationReport.Add('')
[void]$violationReport.Add('**Only the user grants an exemption** by adding a row to the `exempt` block of the audit')
[void]$violationReport.Add('configuration, and **only the user lowers a ceiling**, never raises one. The audit reads the')
[void]$violationReport.Add('binder alone, so a finding it raises may still be correct code; say why, and cite the')
[void]$violationReport.Add('`file:line` that was read.')
[void]$violationReport.Add('')
[void]$violationReport.Add('## Ceilings')
[void]$violationReport.Add('')
if ($pairs.Count -eq 0) {
    [void]$violationReport.Add('No held pair has a hit and no ceiling is written.')
}
else {
    [void]$violationReport.Add('| Pair | Hits | Ceiling | Standing |')
    [void]$violationReport.Add('|---|---|---|---|')
    foreach ($pair in $pairs) {
        $standing = if ($pair.Hits.Count -gt $pair.Ceiling) { 'above ceiling' } elseif ($pair.Hits.Count -lt $pair.Ceiling) { 'stale ceiling' } else { 'at ceiling' }
        [void]$violationReport.Add("| ``$($pair.Key)`` | $($pair.Hits.Count) | $($pair.Ceiling) | $standing |")
    }
}

function Add-FindingTable {
    param($Lines, $Items, [string]$Empty)

    if ($Items.Count -eq 0) {
        [void]$Lines.Add($Empty)
        return
    }

    [void]$Lines.Add('| Location | Check | Ring | Target | Name | Source |')
    [void]$Lines.Add('|---|---|---|---|---|---|')
    foreach ($finding in $Items) {
        [void]$Lines.Add("| ``$(Format-Location -Finding $finding)`` | ``$($finding.Check)`` | ``$($finding.Ring)`` | ``$($finding.Target)`` | ``$(Format-ReportCell -Value $finding.Name)`` | ``$(Format-ReportCell -Value $finding.Text)`` |")
    }
}

[void]$violationReport.Add('')
[void]$violationReport.Add('## Violations')
[void]$violationReport.Add('')
Add-FindingTable -Lines $violationReport -Items $violations -Empty 'No structural violation was found.'

[void]$violationReport.Add('')
[void]$violationReport.Add('## Held by ceilings')
[void]$violationReport.Add('')
[void]$violationReport.Add('These hits sit within the ceiling of their pair. They pass today, and each one fixed lowers a ceiling.')
[void]$violationReport.Add('')
Add-FindingTable -Lines $violationReport -Items $heldHits -Empty 'No hit is held by a ceiling.'

[void]$violationReport.Add('')
[void]$violationReport.Add('## Stale Exemptions')
[void]$violationReport.Add('')
if ($staleExempt.Count -eq 0) {
    [void]$violationReport.Add('Every declared exemption cleared a finding.')
}
else {
    [void]$violationReport.Add('| List | Row |')
    [void]$violationReport.Add('|---|---|')
    foreach ($row in $staleExempt) {
        [void]$violationReport.Add("| $($row.List) | ``$($row.Row)`` |")
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

$meanings = @{
    'Above ceiling'     = 'ring pairs with more hits than their ceiling'
    'Stale ceilings'    = 'ring pairs with fewer hits than their ceiling'
    'Stale exemptions'  = 'exemption rows that cleared no finding'
    'Outside surface'   = 'names reached outside a pair''s listed surface'
    'Stray types'       = 'types matching a home pattern found elsewhere'
    'Banned words'      = 'banned names found in their folder'
    'Thin rings'        = 'projects below their floor'
    'Table drift'       = 'ring table rows that disagree with the projects'
    'Hidden references' = 'references the table does not declare'
}
$gateRows = @(foreach ($label in $counters.Keys) {
    [pscustomobject]@{ Gate = $label; Count = [int]$counters[$label]; Meaning = $meanings[$label]; Section = $label }
})
Write-AuditResult -Rows $gateRows

Write-AuditSection -Title 'Findings by check'
$checkRows = @($script:CheckOrder | ForEach-Object { , @($_, (Get-CheckGate -Check $_), $counts[$_].ToString('N0')) })
$checkRows += , @('Total', '', $countTotal.ToString('N0'))
Write-AuditTable -Header @('Check', 'Gate', 'Hits') -Rows $checkRows

if ($pairs.Count -gt 0) {
    Write-AuditSection -Title "Pairs ($($pairs.Count.ToString('N0')))"
    $pairRows = @($pairs | ForEach-Object {
        $standing = if ($_.Hits.Count -gt $_.Ceiling) { 'above ceiling' }
            elseif ($_.Hits.Count -lt $_.Ceiling) { 'stale ceiling' }
            else { 'at ceiling' }
        , @($_.Key, $_.Files.Count.ToString('N0'), $_.Hits.Count.ToString('N0'), $_.Ceiling.ToString('N0'), $standing)
    })
    Write-AuditTable -Header @('Pair', 'Files', 'Hits', 'Ceiling', 'Standing') -Rows $pairRows
}

Write-AuditItems -Title 'Above ceiling' -Lines ([string[]]@($overCeiling | ForEach-Object { "$($_.Key): $($_.Hits.Count.ToString('N0')) hit(s), ceiling $($_.Ceiling.ToString('N0'))" }))
Write-AuditItems -Title 'Hits above ceiling' -Lines ([string[]]@($overCeiling | ForEach-Object { $_.Hits } | ForEach-Object { Format-FindingLine -Finding $_ }))
Write-AuditItems -Title 'Stale ceilings' -Lines ([string[]]@($staleCeiling | ForEach-Object { "$($_.Key): $($_.Hits.Count.ToString('N0')) hit(s), ceiling $($_.Ceiling.ToString('N0'))" }))
Write-AuditItems -Title 'Stale exemptions' -Lines ([string[]]@($staleExempt | ForEach-Object { "exempt.$($_.List) $($_.Row)" }))
foreach ($check in $script:HardLabels.Keys) {
    Write-AuditItems -Title $script:HardLabels[$check] -Lines ([string[]]@($hardOf[$check] | ForEach-Object { Format-FindingLine -Finding $_ }))
}

Write-AuditLine ''
Write-AuditLine "Report: $reportPath"
Write-AuditLine "Report: $violationPath"

if ($Open) {
    Start-Process -FilePath $violationPath | Out-Null
}

foreach ($label in $counters.Keys) {
    if ($counters[$label] -gt 0) {
        exit 1
    }
}

exit 0
