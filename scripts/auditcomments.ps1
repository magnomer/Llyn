<#
.SYNOPSIS
Audits the comment files that accompany source files and the sources themselves for stray comments.

.DESCRIPTION
Reads the project configuration from auditcomments.json next to this script, then
performs these actions on every run:
  1. Prints the counters, one per finding kind below.
  2. Prints comment-file line totals for each folder under the source roots.
  3. Prints sources that have no comment file, and comment files that have no source.
  4. Prints comment lines that break the line rules: too many words, a forbidden
     character, or more than one sentence.
  5. Prints in-code comment lines found inside sources, every line of a block comment included.
  6. Prints signature headings that name no identifier of the source they describe.
  7. Writes a Markdown report to {report.directory}\{prefix}{version}.md.
The console follows scripts\report.md: widest view first, empty lists left out.

Everything project-specific lives in auditcomments.json. The script itself
carries no project knowledge. Files come from git: tracked and untracked files,
never ignored ones, as in the convention tests. Git is the only external tool required.

auditcomments.json shape:
  {
    "generation": 12,
    "project": "Llyn",
    "sources": {
      "roots": ["languages", "localization", "src", "tests", "themes"],
      "files": ["Directory.Build.props", "Llyn.slnx", "version.json"],
      "commentPattern": "*.comment.md",
      "pairs": { ".xaml.cs": "{base}.xaml.comment.md", ".cs": "{base}.comment.md",
                 ".xaml": "{base}.comment.md", ".csproj": "{base}.comment.md",
                 ".json": "{base}.comment.md", ".props": "{base}.comment.md",
                 ".slnx": "{base}.comment.md" },
      "excludeSegments": [".git", "bin", "obj"],
      "excludeSuffixes": [".g.cs", ".Designer.cs"]
    },
    "rules": { "maxWords": 20, "forbidden": [";"], "sentenceMarks": [".", "!", "?"],
               "abbreviations": ["e.g", "i.e", "etc", "vs", "cf"] },
    "remark": {
      "markers": { ".cs": ["//", "/*"], ".xaml": ["<!--"], ".props": ["<!--"] },
      "closers": { "/*": "*/", "<!--": "-->" },
      "exemptFiles": ["TAuditNameRegistry.cs"]
    },
    "report": {
      "directory": "docs-work/audit",
      "versionFile": "version.json",
      "versionKey": "current-version",
      "prefix": "Comments-",
      "segments": 1
    }
  }

Pairs map a source suffix to the comment file it expects, where {base} is the
file name with that suffix removed. Longer suffixes are matched first.

Files lists single sources, relative to the repository root, audited beside the
roots. They report under the (root) folder and are skipped when -SourceRoots is given.

.PARAMETER ConfigPath
Path to the JSON configuration. Defaults to auditcomments.json next to this script.

.PARAMETER SourceRoots
Overrides sources.roots for this run.

.PARAMETER Segments
Overrides report.segments: how many path segments under a root form a folder row.

.PARAMETER MaxWords
Overrides rules.maxWords for this run.

.PARAMETER OutputPath
Overrides the Markdown report path for this run.

.PARAMETER Open
Open the generated report after the audit finishes.

.PARAMETER NoPause
Do not stop at each console page. Paging is also off when output or input is redirected.

.PARAMETER Help
Display this help and exit without running the audit. The alias -? is supported.

.EXAMPLE
auditcomments

.EXAMPLE
auditcomments -Segments 2

.EXAMPLE
auditcomments -SourceRoots .\src -MaxWords 25
#>
#requires -Version 5.1
# AUDITCOMMENTS GENERATION 12 - auditcomments.ps1.
# A generation is not a revision count. It names functionality, not edits, so editing one of these
# files is never on its own a reason to raise it. Raise it only when the audited outcome changes.
# A generation names the set of checks the audit applies. Two projects on the same generation audit
# the same things and their reports compare directly, whatever else differs between the files. A check
# added, removed, or changed in what it reports is a new generation; wording, plumbing, and refactoring
# leave it alone. Every audit script shares one generation number with the convention-test settings,
# and each refuses a configuration written at another generation.
# Generation 11: nothing the comment audit reports changes; the number rises with the truth audit,
# which checks that a deportment field reaches no request, keeps one writer, holds no logic and
# treats no engine data.
# Generation 12: nothing the comment audit reports changes; the number rises with the convention tests,
# which bind with no compile error, count chain ceilings in names, count a using or a call on a
# deeper record as a reach, and exempt a contract name only where the type declares the interface.
[CmdletBinding()]
param(
    [string]$ConfigPath,
    [string[]]$SourceRoots,
    [ValidateRange(1, 8)]
    [int]$Segments,
    [ValidateRange(1, [int]::MaxValue)]
    [int]$MaxWords,
    [string]$OutputPath,
    [switch]$Open,
    [switch]$NoPause,
    [Alias('?')]
    [switch]$Help
)

if ([string]::IsNullOrWhiteSpace($ConfigPath)) {
    $ConfigPath = Join-Path $PSScriptRoot "auditcomments.json"
}

if ($Help) {
    @'
NAME
    auditcomments.ps1

SYNOPSIS
    Audit comment files and stray in-code comments, and create a Markdown report.

SYNTAX
    auditcomments [-ConfigPath <path>] [-SourceRoots <path[]>] [-Segments <number>]
        [-MaxWords <number>] [-OutputPath <path>] [-Open] [-NoPause] [-Help]

CONFIGURATION
    All project-specific values live in auditcomments.json next to the script:
    source roots, comment-file pattern, source-to-comment pairs, excluded
    directory names and suffixes, line rules, in-code comment markers and
    exempt files, report location, version file and key. Parameters below
    override it per run.

OPTIONS
    -ConfigPath <path>
        JSON configuration file. Defaults to .\auditcomments.json.

    -SourceRoots <path[]>
        Source directories to audit. Overrides sources.roots.

    -Segments <number>
        Path segments under a root that form a folder row. Overrides
        report.segments. 1 lists projects, 2 lists their first-level folders.

    -MaxWords <number>
        Word limit per comment line. Overrides rules.maxWords.

    -OutputPath <path>
        Markdown report path. By default, the project version is used to
        create a path under report.directory.

    -Open
        Open the generated report after the audit finishes.

    -NoPause
        Do not stop at each console page for a key. Off by itself when output
        or input is redirected.

    -Help, -?
        Display this help and exit without running the audit.

EXAMPLES
    auditcomments
        Audit with the configured settings.

    auditcomments -Segments 2
        Break folder totals down one level further.

    auditcomments -SourceRoots .\src -MaxWords 25
        Audit only src, allowing 25 words per line.
'@ | Write-Host
    exit 0
}

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$script:AuditGeneration = 12

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

Write-AuditLine "AUDITCOMMENTS GENERATION $script:AuditGeneration" -ForegroundColor Cyan


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

function Get-ConfigLeaves {
    param(
        [Parameter(Mandatory = $true)]$Node,
        [AllowEmptyString()][string]$Path = ''
    )

    $leaves = [System.Collections.Generic.List[string]]::new()
    foreach ($property in $Node.PSObject.Properties) {
        $child = if ($Path.Length -eq 0) { $property.Name } else { $Path + '.' + $property.Name }
        if ($property.Value -is [System.Management.Automation.PSCustomObject] -and $child -notin @('sources.pairs', 'remark.markers', 'remark.closers')) {
            foreach ($leaf in (Get-ConfigLeaves -Node $property.Value -Path $child)) { $leaves.Add($leaf) }
        }
        else {
            $leaves.Add($child)
        }
    }

    return , $leaves
}

function Test-ConfigValue {
    param(
        $Value,
        [Parameter(Mandatory = $true)][string]$Kind
    )

    switch ($Kind) {
        'string' { return $Value -is [string] -and -not [string]::IsNullOrWhiteSpace($Value) }
        'int' { return ($Value -is [int] -or $Value -is [long]) -and $Value -ge 0 }
        'strings' {
            if ($null -eq $Value -or -not ($Value -is [System.Array])) { return $false }
            foreach ($item in $Value) {
                if (-not ($item -is [string]) -or [string]::IsNullOrWhiteSpace($item)) { return $false }
            }
            return $true
        }
        'map' {
            if ($null -eq $Value -or -not ($Value -is [System.Management.Automation.PSCustomObject])) { return $false }
            foreach ($property in $Value.PSObject.Properties) {
                if (-not ($property.Value -is [string]) -or [string]::IsNullOrWhiteSpace($property.Value)) { return $false }
            }
            return $true
        }
        'mapStrings' {
            if ($null -eq $Value -or -not ($Value -is [System.Management.Automation.PSCustomObject])) { return $false }
            foreach ($property in $Value.PSObject.Properties) {
                if (-not (Test-ConfigValue -Value $property.Value -Kind 'strings')) { return $false }
            }
            return $true
        }
        default { throw "Unknown schema kind: $Kind" }
    }
}

function Resolve-UserPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    return $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($Path)
}

function Read-AuditConfig {
    param([Parameter(Mandatory = $true)][string]$Path)

    $pathFull = Resolve-UserPath -Path $Path
    if (-not (Test-Path -LiteralPath $pathFull -PathType Leaf)) {
        throw "The comment-audit configuration was not found: $pathFull"
    }

    try {
        $config = Get-Content -LiteralPath $pathFull -Raw -Encoding UTF8 | ConvertFrom-Json
    }
    catch {
        throw "The comment-audit configuration is not valid JSON: $pathFull`n$($_.Exception.Message)"
    }

    $problems = [System.Collections.Generic.List[string]]::new()
    $schema = [ordered]@{
        'generation' = 'int'; 'project' = 'string'
        'sources.roots' = 'strings'; 'sources.files' = 'strings'; 'sources.commentPattern' = 'string'; 'sources.pairs' = 'map'
        'sources.excludeSegments' = 'strings'; 'sources.excludeSuffixes' = 'strings'
        'rules.maxWords' = 'int'; 'rules.forbidden' = 'strings'; 'rules.sentenceMarks' = 'strings'; 'rules.abbreviations' = 'strings'
        'remark.markers' = 'mapStrings'; 'remark.closers' = 'map'; 'remark.exemptFiles' = 'strings'
        'report.directory' = 'string'; 'report.versionFile' = 'string'; 'report.versionKey' = 'string'; 'report.prefix' = 'string'; 'report.segments' = 'int'
    }

    $present = Get-ConfigLeaves -Node $config
    foreach ($key in $schema.Keys) {
        if ($key -notin $present) {
            $problems.Add("missing key '$key'")
        }
        elseif (-not (Test-ConfigValue -Value (Get-ConfigNode -Document $config -Key $key) -Kind $schema[$key])) {
            $problems.Add("key '$key' is not a valid $($schema[$key])")
        }
    }

    foreach ($key in $present) {
        if (-not $schema.Contains($key)) {
            $problems.Add("unknown key '$key'")
        }
    }

    if ($problems.Count -gt 0) {
        throw "The comment-audit configuration is not valid: $pathFull`n  " + ($problems -join "`n  ")
    }

    return $config
}

function Join-AuditPath {
    param(
        [Parameter(Mandatory = $true)][string]$Root,
        [Parameter(Mandatory = $true)][string]$Relative
    )

    if ([System.IO.Path]::IsPathRooted($Relative)) {
        return [System.IO.Path]::GetFullPath($Relative)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $Root $Relative))
}

$repoRootFull = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$config = Read-AuditConfig -Path $ConfigPath

if ([int]$config.generation -ne $script:AuditGeneration) {
    throw "The comment-audit configuration is generation $($config.generation) but this tooling is generation $script:AuditGeneration.`nA generation names the set of checks applied, so the two must match: $ConfigPath"
}

if (-not $PSBoundParameters.ContainsKey('SourceRoots')) { $SourceRoots = @($config.sources.roots) }
if (-not $PSBoundParameters.ContainsKey('Segments')) { $Segments = [int]$config.report.segments }
if (-not $PSBoundParameters.ContainsKey('MaxWords')) { $MaxWords = [int]$config.rules.maxWords }

$commentPattern = [string]$config.sources.commentPattern
$commentSuffix = $commentPattern.TrimStart('*')
$forbidden = @($config.rules.forbidden | ForEach-Object { [string]$_ })
$sentenceMarks = @($config.rules.sentenceMarks | ForEach-Object { [string]$_ })
$exemptFiles = [System.Collections.Generic.HashSet[string]]::new([string[]]@($config.remark.exemptFiles | ForEach-Object { [string]$_ }), [System.StringComparer]::OrdinalIgnoreCase)
$reportDirectoryFull = Join-AuditPath -Root $repoRootFull -Relative $config.report.directory
$versionPathFull = Join-AuditPath -Root $repoRootFull -Relative $config.report.versionFile
$versionKey = [string]$config.report.versionKey
$reportPrefix = [string]$config.report.prefix

$pairs = [System.Collections.Generic.List[object]]::new()
foreach ($property in $config.sources.pairs.PSObject.Properties) {
    $pairs.Add([pscustomobject]@{ Suffix = [string]$property.Name; Template = [string]$property.Value })
}
$pairs = @($pairs | Sort-Object -Property @{ Expression = { $_.Suffix.Length }; Descending = $true })

$markers = @{}
foreach ($property in $config.remark.markers.PSObject.Properties) {
    $markers[[string]$property.Name] = @($property.Value | ForEach-Object { [string]$_ })
}

$excludedNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
foreach ($segment in @($config.sources.excludeSegments)) {
    if (-not [string]::IsNullOrWhiteSpace($segment)) { [void]$excludedNames.Add([string]$segment) }
}
$excludedSuffixes = @($config.sources.excludeSuffixes | ForEach-Object { [string]$_ })

if ($MaxWords -lt 1) {
    throw "rules.maxWords ($MaxWords) must be at least 1."
}

function ConvertTo-MarkdownCell {
    param([AllowNull()][object]$Value)

    return ([string]$Value).Replace('|', '\|').Replace("`r`n", '<br>').Replace("`n", '<br>').Replace("`r", '<br>')
}

function Format-Integer {
    param([long]$Value)

    return $Value.ToString("N0", [System.Globalization.CultureInfo]::InvariantCulture)
}

function Format-Ratio {
    param([double]$Value)

    return $Value.ToString("0.00", [System.Globalization.CultureInfo]::InvariantCulture)
}

function Format-Percent {
    param([double]$Value)

    return $Value.ToString("0.0", [System.Globalization.CultureInfo]::InvariantCulture) + " %"
}

function Get-RelativePathSafe {
    param(
        [Parameter(Mandatory = $true)][string]$BasePath,
        [Parameter(Mandatory = $true)][string]$Path
    )

    $baseFull = [System.IO.Path]::GetFullPath($BasePath)
    if (-not $baseFull.EndsWith([System.IO.Path]::DirectorySeparatorChar.ToString())) {
        $baseFull += [System.IO.Path]::DirectorySeparatorChar
    }

    $baseUri = [System.Uri]::new($baseFull)
    $pathUri = [System.Uri]::new([System.IO.Path]::GetFullPath($Path))
    return [System.Uri]::UnescapeDataString($baseUri.MakeRelativeUri($pathUri).ToString()).Replace('/', [System.IO.Path]::DirectorySeparatorChar)
}

function Test-IsExcludedPath {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$FolderRoot,
        [Parameter(Mandatory = $true)][System.Collections.Generic.HashSet[string]]$ExcludedNames
    )

    $relative = $Path.Substring($FolderRoot.Length).TrimStart([char[]]@('\', '/'))
    foreach ($segment in ($relative -split '[\\/]')) {
        if ($ExcludedNames.Contains($segment)) {
            return $true
        }
    }

    return $false
}

function Write-SectionTitle {
    param([Parameter(Mandatory = $true)][string]$Text)

    Write-AuditLine ""
    Write-AuditLine $Text
    Write-AuditLine ('-' * $Text.Length)
}

function Format-Cell {
    param(
        [string]$Text,
        [int]$Width,
        [bool]$Right
    )

    if ($Right) { return $Text.PadLeft($Width) }
    return $Text.PadRight($Width)
}

# Renders a table whose header has two rows: group names above, column names below.
# Each column is @{ Group = ''; Name = ''; Right = $true; Values = @() }.
# A column without a group prints its name on the upper row and leaves the lower row blank.
function Write-GroupedConsoleTable {
    param(
        [Parameter(Mandatory = $true)][object[]]$Columns
    )

    $gap = "  "
    foreach ($column in $Columns) {
        $width = [Math]::Max($column.Width, $column.Name.Length)
        foreach ($value in $column.Values) {
            if ($value.Length -gt $width) { $width = $value.Length }
        }
        $column.Width = $width
    }

    $groups = [System.Collections.Generic.List[object]]::new()
    $index = 0
    while ($index -lt $Columns.Count) {
        $column = $Columns[$index]
        $last = $index
        if (-not [string]::IsNullOrEmpty($column.Group)) {
            while ($last + 1 -lt $Columns.Count -and $Columns[$last + 1].Group -eq $column.Group) { $last++ }
        }
        $groups.Add([pscustomobject]@{ First = $index; Last = $last; Label = if ([string]::IsNullOrEmpty($column.Group)) { $column.Name } else { $column.Group } })
        $index = $last + 1
    }

    foreach ($group in $groups) {
        $span = 0
        for ($i = $group.First; $i -le $group.Last; $i++) {
            $span += $Columns[$i].Width
            if ($i -lt $group.Last) { $span += $gap.Length }
        }
        if ($group.Label.Length -gt $span) {
            $Columns[$group.Last].Width += $group.Label.Length - $span
        }
    }

    $upper = [System.Text.StringBuilder]::new()
    foreach ($group in $groups) {
        $span = 0
        for ($i = $group.First; $i -le $group.Last; $i++) {
            $span += $Columns[$i].Width
            if ($i -lt $group.Last) { $span += $gap.Length }
        }
        if ($upper.Length -gt 0) { [void]$upper.Append($gap) }
        [void]$upper.Append($group.Label.PadRight($span))
    }

    $lower = [System.Text.StringBuilder]::new()
    $rule = [System.Text.StringBuilder]::new()
    foreach ($column in $Columns) {
        if ($lower.Length -gt 0) { [void]$lower.Append($gap); [void]$rule.Append($gap) }
        $label = if ([string]::IsNullOrEmpty($column.Group)) { "" } else { $column.Name }
        [void]$lower.Append((Format-Cell -Text $label -Width $column.Width -Right $column.Right))
        [void]$rule.Append('-' * $column.Width)
    }

    Write-AuditLine $upper.ToString().TrimEnd()
    if (@($Columns | Where-Object { -not [string]::IsNullOrEmpty($_.Group) }).Count -gt 0) {
        Write-AuditLine $lower.ToString().TrimEnd()
    }
    Write-AuditLine $rule.ToString()

    $rowCount = $Columns[0].Values.Count
    for ($row = 0; $row -lt $rowCount; $row++) {
        $line = [System.Text.StringBuilder]::new()
        foreach ($column in $Columns) {
            if ($line.Length -gt 0) { [void]$line.Append($gap) }
            [void]$line.Append((Format-Cell -Text $column.Values[$row] -Width $column.Width -Right $column.Right))
        }
        Write-AuditLine $line.ToString().TrimEnd()
    }
}

function New-ConsoleColumn {
    param(
        [string]$Group,
        [Parameter(Mandatory = $true)][string]$Name,
        [bool]$Right = $true,
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][string[]]$Values
    )

    return [pscustomobject]@{ Group = $Group; Name = $Name; Right = $Right; Values = $Values; Width = 0 }
}

# Gives columns that share a group and name the same width across every table passed in.
function Sync-ConsoleColumnWidth {
    param(
        [Parameter(Mandatory = $true)][object[][]]$Tables
    )

    $widths = @{}
    foreach ($table in $Tables) {
        foreach ($column in $table) {
            $key = "$($column.Group)|$($column.Name)"
            $width = $column.Name.Length
            foreach ($value in $column.Values) {
                if ($value.Length -gt $width) { $width = $value.Length }
            }
            if (-not $widths.ContainsKey($key) -or $widths[$key] -lt $width) { $widths[$key] = $width }
        }
    }

    foreach ($table in $Tables) {
        foreach ($column in $table) {
            $column.Width = $widths["$($column.Group)|$($column.Name)"]
        }
    }
}


function Test-IsExcludedSuffix {
    param([Parameter(Mandatory = $true)][string]$Name)

    foreach ($suffix in $excludedSuffixes) {
        if ($Name.EndsWith($suffix, [System.StringComparison]::OrdinalIgnoreCase)) { return $true }
    }

    return $false
}

function Get-FolderKey {
    param(
        [Parameter(Mandatory = $true)][string]$RootFull,
        [Parameter(Mandatory = $true)][string]$FileFull
    )

    $directory = [System.IO.Path]::GetDirectoryName($FileFull)
    $relative = $directory.Substring($RootFull.Length).TrimStart([char[]]@('\', '/'))
    if ($relative.Length -eq 0) { return [System.IO.Path]::GetFileName($RootFull) }
    $parts = @($relative -split '[\\/]')
    $take = [Math]::Min($Segments, $parts.Count)
    return (($parts | Select-Object -First $take) -join '\')
}

function Get-PairFor {
    param([Parameter(Mandatory = $true)][string]$Name)

    foreach ($pair in $pairs) {
        if ($Name.EndsWith($pair.Suffix, [System.StringComparison]::OrdinalIgnoreCase)) {
            $base = $Name.Substring(0, $Name.Length - $pair.Suffix.Length)
            return [pscustomobject]@{ Suffix = $pair.Suffix; Base = $base; Expected = $pair.Template.Replace('{base}', $base) }
        }
    }

    return $null
}

$stringLiteralPattern = [System.Text.RegularExpressions.Regex]::new('(?:"{3,})[\s\S]*?"{3,}|(?:@\$?|\$@)"(?:[^"]|"")*"|"(?:\\.|[^"\\\r\n])*"|''(?:\\.|[^''\\\r\n])''', [System.Text.RegularExpressions.RegexOptions]::Compiled)
$codeSpanPattern = [System.Text.RegularExpressions.Regex]::new('`[^`]*`', [System.Text.RegularExpressions.RegexOptions]::Compiled)
$abbreviationAlternatives = (@($config.rules.abbreviations | ForEach-Object { [System.Text.RegularExpressions.Regex]::Escape([string]$_) }) -join '|')
$abbreviationPattern = [System.Text.RegularExpressions.Regex]::new('(?<![\p{L}.])(?:' + $abbreviationAlternatives + ')\.', [System.Text.RegularExpressions.RegexOptions]::Compiled -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
$sentencePattern = [System.Text.RegularExpressions.Regex]::new('[' + [System.Text.RegularExpressions.Regex]::Escape(-join $sentenceMarks) + ']\s+\p{L}', [System.Text.RegularExpressions.RegexOptions]::Compiled)

function Remove-StringLiteral {
    param([Parameter(Mandatory = $true)][AllowEmptyString()][string]$Text)

    return $stringLiteralPattern.Replace($Text, [System.Text.RegularExpressions.MatchEvaluator]{
        param($match)
        $breaks = @($match.Value.ToCharArray() | Where-Object { $_ -eq "`n" }).Count
        return '""' + ("`n" * $breaks)
    })
}

function Test-CommentLine {
    param([Parameter(Mandatory = $true)][AllowEmptyString()][string]$Line)

    $text = $Line.Trim()
    $heading = $text -match '^#+\s*'
    if ($heading) { $text = $text.Substring($Matches[0].Length) }
    if ($text.Length -eq 0) { return @() }
    if ($text -match '^[-*>]\s+') { $text = $text.Substring($Matches[0].Length) }

    $prose = $codeSpanPattern.Replace($text, '')
    $counted = if ($heading) { $prose } else { $text }
    $problems = [System.Collections.Generic.List[string]]::new()
    $words = @(@($counted -split '\s+') | Where-Object { $_.Length -gt 0 }).Count
    if ($words -gt $MaxWords) { $problems.Add("$words words") }

    foreach ($token in $forbidden) {
        if ($prose.Contains($token)) { $problems.Add("forbidden '$token'") }
    }

    $sentences = 1 + $sentencePattern.Matches($abbreviationPattern.Replace($prose, 'abbr')).Count
    if ($sentences -gt 1) { $problems.Add("$sentences sentences") }

    return @($problems)
}

$sourceRootFulls = [System.Collections.Generic.List[string]]::new()
foreach ($root in $SourceRoots) {
    if ([string]::IsNullOrWhiteSpace($root)) { continue }
    $rootFull = Join-AuditPath -Root $repoRootFull -Relative $root
    if (-not [System.IO.Directory]::Exists($rootFull)) { throw "Source directory not found: $rootFull" }
    [void]$sourceRootFulls.Add($rootFull)
}

if ($sourceRootFulls.Count -eq 0) { throw "At least one source root must be supplied." }

$sourceFileFulls = [System.Collections.Generic.List[string]]::new()
if (-not $PSBoundParameters.ContainsKey('SourceRoots')) {
    foreach ($entry in @($config.sources.files)) {
        if ([string]::IsNullOrWhiteSpace($entry)) { continue }
        $fileFull = Join-AuditPath -Root $repoRootFull -Relative $entry
        if (-not [System.IO.File]::Exists($fileFull)) { throw "Source file not found: $fileFull" }
        if ($null -eq (Get-PairFor -Name ([System.IO.Path]::GetFileName($fileFull)))) { throw "Source file has no pair: $fileFull" }
        [void]$sourceFileFulls.Add($fileFull)
    }
}

$sourceFiles = [System.Collections.Generic.List[object]]::new()
$commentFiles = [System.Collections.Generic.List[object]]::new()
$readErrors = [System.Collections.Generic.List[string]]::new()

function Measure-Lines {
    param([Parameter(Mandatory = $true)][string]$Path)

    [long]$lines = 0
    [long]$nonBlank = 0
    foreach ($line in [System.IO.File]::ReadLines($Path)) {
        $lines++
        if (-not [string]::IsNullOrWhiteSpace($line)) { $nonBlank++ }
    }

    return [pscustomobject]@{ Lines = $lines; NonBlank = $nonBlank }
}

foreach ($rootFull in $sourceRootFulls) {
    $listed = & git -C $rootFull -c core.quotePath=false ls-files --cached --others --exclude-standard --full-name -- . 2>$null
    if ($LASTEXITCODE -ne 0) {
        throw "Git could not enumerate the files under $rootFull, so the audit cannot judge."
    }

    $files = @($listed | ForEach-Object {
        $full = [System.IO.Path]::GetFullPath((Join-Path $repoRootFull ([string]$_)))
        if ([System.IO.File]::Exists($full)) { [System.IO.FileInfo]::new($full) }
    } | Where-Object {
        -not (Test-IsExcludedPath -Path $_.FullName -FolderRoot $rootFull -ExcludedNames $excludedNames)
    })

    foreach ($file in $files) {
        $relative = Get-RelativePathSafe -BasePath $repoRootFull -Path $file.FullName
        $folder = Get-FolderKey -RootFull $rootFull -FileFull $file.FullName

        if ($file.Name.EndsWith($commentSuffix, [System.StringComparison]::OrdinalIgnoreCase)) {
            try { $count = Measure-Lines -Path $file.FullName }
            catch { $readErrors.Add("$($file.FullName): $($_.Exception.Message)"); continue }
            $commentFiles.Add([pscustomobject]@{
                Full = $file.FullName; Relative = $relative; Folder = $folder; Name = $file.Name
                Directory = $file.DirectoryName; Lines = $count.Lines; NonBlank = $count.NonBlank; Bytes = $file.Length
            })
            continue
        }

        if (Test-IsExcludedSuffix -Name $file.Name) { continue }
        $pair = Get-PairFor -Name $file.Name
        if ($null -eq $pair) { continue }

        try { $count = Measure-Lines -Path $file.FullName }
        catch { $readErrors.Add("$($file.FullName): $($_.Exception.Message)"); continue }
        $sourceFiles.Add([pscustomobject]@{
            Full = $file.FullName; Relative = $relative; Folder = $folder; Name = $file.Name
            Directory = $file.DirectoryName; Extension = $file.Extension.ToLowerInvariant()
            Suffix = $pair.Suffix; Expected = $pair.Expected; Lines = $count.Lines; NonBlank = $count.NonBlank
        })
    }
}

foreach ($fileFull in $sourceFileFulls) {
    $file = Get-Item -LiteralPath $fileFull
    $pair = Get-PairFor -Name $file.Name
    try { $count = Measure-Lines -Path $file.FullName }
    catch { $readErrors.Add("$($file.FullName): $($_.Exception.Message)"); continue }
    $sourceFiles.Add([pscustomobject]@{
        Full = $file.FullName; Relative = (Get-RelativePathSafe -BasePath $repoRootFull -Path $file.FullName); Folder = "(root)"; Name = $file.Name
        Directory = $file.DirectoryName; Extension = $file.Extension.ToLowerInvariant()
        Suffix = $pair.Suffix; Expected = $pair.Expected; Lines = $count.Lines; NonBlank = $count.NonBlank
    })

    $commentFull = Join-Path $file.DirectoryName $pair.Expected
    if (-not [System.IO.File]::Exists($commentFull)) { continue }
    $comment = Get-Item -LiteralPath $commentFull
    try { $count = Measure-Lines -Path $comment.FullName }
    catch { $readErrors.Add("$($comment.FullName): $($_.Exception.Message)"); continue }
    $commentFiles.Add([pscustomobject]@{
        Full = $comment.FullName; Relative = (Get-RelativePathSafe -BasePath $repoRootFull -Path $comment.FullName); Folder = "(root)"; Name = $comment.Name
        Directory = $comment.DirectoryName; Lines = $count.Lines; NonBlank = $count.NonBlank; Bytes = $comment.Length
    })
}

if ($sourceFiles.Count -eq 0) {
    throw "No source file was scanned under the configured roots, so the audit cannot judge."
}

# Folder totals.
$folderMap = [ordered]@{}
foreach ($source in $sourceFiles) {
    if (-not $folderMap.Contains($source.Folder)) {
        $folderMap[$source.Folder] = [pscustomobject]@{ Name = $source.Folder; SourceFiles = 0; SourceNonBlank = [long]0; Files = 0; Lines = [long]0; NonBlank = [long]0; Bytes = [long]0 }
    }
    $folderMap[$source.Folder].SourceFiles++
    $folderMap[$source.Folder].SourceNonBlank += $source.NonBlank
}
foreach ($comment in $commentFiles) {
    if (-not $folderMap.Contains($comment.Folder)) {
        $folderMap[$comment.Folder] = [pscustomobject]@{ Name = $comment.Folder; SourceFiles = 0; SourceNonBlank = [long]0; Files = 0; Lines = [long]0; NonBlank = [long]0; Bytes = [long]0 }
    }
    $folderMap[$comment.Folder].Files++
    $folderMap[$comment.Folder].Lines += $comment.Lines
    $folderMap[$comment.Folder].NonBlank += $comment.NonBlank
    $folderMap[$comment.Folder].Bytes += $comment.Bytes
}
$folderResults = @($folderMap.Values | Sort-Object -Property @{ Expression = 'Lines'; Descending = $true }, @{ Expression = { Get-OrdinalKey $_.Name } })

[long]$totalLines = 0
[long]$totalBytes = 0
[int]$totalFiles = 0
foreach ($item in $folderResults) {
    $totalLines += $item.Lines
    $totalBytes += $item.Bytes
    $totalFiles += $item.Files
}

# Pairing in both directions.
$expectedByDirectory = @{}
foreach ($source in $sourceFiles) {
    if (-not $expectedByDirectory.ContainsKey($source.Directory)) {
        $expectedByDirectory[$source.Directory] = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    }
    [void]$expectedByDirectory[$source.Directory].Add($source.Expected)
}

$missingComments = @($sourceFiles | Where-Object { -not [System.IO.File]::Exists((Join-Path $_.Directory $_.Expected)) } | Sort-Object -Property @{ Expression = { Get-OrdinalKey $_.Relative } })
$orphanComments = @($commentFiles | Where-Object {
    -not ($expectedByDirectory.ContainsKey($_.Directory) -and $expectedByDirectory[$_.Directory].Contains($_.Name))
} | Sort-Object -Property @{ Expression = { Get-OrdinalKey $_.Relative } })

# Line rules inside comment files.
$ruleHits = [System.Collections.Generic.List[object]]::new()
foreach ($comment in $commentFiles) {
    $number = 0
    foreach ($line in [System.IO.File]::ReadLines($comment.Full)) {
        $number++
        $problems = @(Test-CommentLine -Line $line)
        if ($problems.Count -gt 0) {
            $ruleHits.Add([pscustomobject]@{ Relative = $comment.Relative; Line = $number; Problem = ($problems -join ', '); Text = $line.Trim() })
        }
    }
}

# In-code comments inside sources.
$remarkHits = [System.Collections.Generic.List[object]]::new()
$blockClosers = @{}
foreach ($property in $config.remark.closers.PSObject.Properties) { $blockClosers[[string]$property.Name] = [string]$property.Value }
foreach ($source in $sourceFiles) {
    if (-not $markers.ContainsKey($source.Extension)) { continue }
    if ($exemptFiles.Contains($source.Name)) { continue }
    $tokens = $markers[$source.Extension]
    $number = 0
    $content = [System.IO.File]::ReadAllText($source.Full)
    $stripped = if ($source.Extension -eq '.cs') { Remove-StringLiteral -Text $content } else { $content }
    $raw = $content -split "`n"
    $openToken = $null
    foreach ($code in ($stripped -split "`n")) {
        $number++
        $line = if ($number -le $raw.Count) { $raw[$number - 1] } else { '' }
        if ($null -ne $openToken) {
            $remarkHits.Add([pscustomobject]@{ Relative = $source.Relative; Line = $number; Marker = $openToken; Text = $line.Trim() })
            if ($code.IndexOf($blockClosers[$openToken], [System.StringComparison]::Ordinal) -ge 0) { $openToken = $null }
            continue
        }
        foreach ($token in $tokens) {
            $at = $code.IndexOf($token, [System.StringComparison]::Ordinal)
            if ($at -ge 0) {
                $remarkHits.Add([pscustomobject]@{ Relative = $source.Relative; Line = $number; Marker = $token; Text = $line.Trim() })
                if ($blockClosers.ContainsKey($token) -and $code.IndexOf($blockClosers[$token], $at + $token.Length, [System.StringComparison]::Ordinal) -lt 0) {
                    $openToken = $token
                }
                break
            }
        }
    }
}

$headingHits = [System.Collections.Generic.List[object]]::new()
$headingPattern = [System.Text.RegularExpressions.Regex]::new('^##\s+`(?<span>[^`]+)`\s*$')
$fileNamePattern = [System.Text.RegularExpressions.Regex]::new('^[\w.]+\.(cs|xaml|json|csproj|props|slnx|md)$')
$genericPattern = [System.Text.RegularExpressions.Regex]::new('<[^<>]*>')
$identifierPattern = [System.Text.RegularExpressions.Regex]::new('@?[A-Za-z_][A-Za-z0-9_]*')
foreach ($comment in $commentFiles) {
    $stem = $comment.Full.Substring(0, $comment.Full.Length - $commentSuffix.Length)
    $owners = @(@('', '.cs', '.xaml', '.xaml.cs') | ForEach-Object { $stem + $_ } | Where-Object { [System.IO.File]::Exists($_) })
    if ($owners.Count -eq 0 -or $exemptFiles.Contains([System.IO.Path]::GetFileName($owners[0]))) { continue }
    $ownerText = -join @($owners | ForEach-Object { [System.IO.File]::ReadAllText($_) })
    $number = 0
    foreach ($line in [System.IO.File]::ReadLines($comment.Full)) {
        $number++
        $heading = $headingPattern.Match($line)
        if (-not $heading.Success) { continue }
        $span = $heading.Groups['span'].Value
        if ($span.StartsWith('<') -or $fileNamePattern.IsMatch($span)) { continue }
        $stop = $span.IndexOfAny([char[]]@('(', '=', ';', '{', ':'))
        $head = if ($stop -lt 0) { $span } else { $span.Substring(0, $stop) }
        while ($genericPattern.IsMatch($head)) { $head = $genericPattern.Replace($head, '') }
        $identifiers = $identifierPattern.Matches($head)
        if ($identifiers.Count -eq 0) { continue }
        $name = $identifiers[$identifiers.Count - 1].Value
        if (-not [System.Text.RegularExpressions.Regex]::IsMatch($ownerText, '\b' + [System.Text.RegularExpressions.Regex]::Escape($name) + '\b')) {
            $headingHits.Add([pscustomobject]@{ Relative = $comment.Relative; Line = $number; Problem = $name; Text = $line.Trim() })
        }
    }
}

# Version, read from the configured version file and key.
$version = "0.0.0"
if ([System.IO.File]::Exists($versionPathFull)) {
    try {
        $versionData = Get-Content -LiteralPath $versionPathFull -Raw -Encoding UTF8 | ConvertFrom-Json
        $versionValue = Get-ConfigNode -Document $versionData -Key $versionKey
        if (-not [string]::IsNullOrWhiteSpace([string]$versionValue)) { $version = [string]$versionValue }
    }
    catch {
        Write-AuditLine "The version file is not valid JSON, using $version : $versionPathFull"
    }
}
else {
    Write-AuditLine "The version file was not found, using $version : $versionPathFull"
}

if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $reportDirectoryFull ("{0}{1}.md" -f $reportPrefix, $version)
}
$outputPathFull = Resolve-UserPath -Path $OutputPath
[System.IO.Directory]::CreateDirectory([System.IO.Path]::GetDirectoryName($outputPathFull)) | Out-Null

$generatedAt = Get-Date

# Console output.
Write-AuditLine ("Scanned: {0:N0} source files, {1:N0} comment files" -f $sourceFiles.Count, $commentFiles.Count) -ForegroundColor DarkGray

$counterRows = @(
    @('Sources without a comment file', $missingComments.Count),
    @('Comment files without a source', $orphanComments.Count),
    @('Comment lines breaking the line rules', $ruleHits.Count),
    @('In-code comments', $remarkHits.Count),
    @('Headings naming nothing in their source', $headingHits.Count),
    @('Unreadable files', $readErrors.Count)
)
$counterWidth = ($counterRows | ForEach-Object { $_[0].Length } | Measure-Object -Maximum).Maximum
Write-SectionTitle "Counters"
foreach ($counterRow in $counterRows) {
    Write-AuditLine ("{0}  {1:N0}" -f $counterRow[0].PadRight($counterWidth), $counterRow[1])
}

$tableRows = @($folderResults) + @([pscustomobject]@{
    Name = 'Total'
    Files = $totalFiles
    Lines = $totalLines
    NonBlank = [long](($folderResults | Measure-Object -Property NonBlank -Sum).Sum)
    Bytes = $totalBytes
    SourceNonBlank = [long](($folderResults | Measure-Object -Property SourceNonBlank -Sum).Sum)
})
$folderColumns = @(
    (New-ConsoleColumn -Name 'Folder' -Right $false -Values @($tableRows | ForEach-Object { $_.Name })),
    (New-ConsoleColumn -Name 'Files' -Values @($tableRows | ForEach-Object { Format-Integer $_.Files })),
    (New-ConsoleColumn -Group 'Lines' -Name 'Raw' -Values @($tableRows | ForEach-Object { Format-Integer $_.Lines })),
    (New-ConsoleColumn -Group 'Lines' -Name 'Non-blank' -Values @($tableRows | ForEach-Object { Format-Integer $_.NonBlank })),
    (New-ConsoleColumn -Group 'Lines' -Name 'Share' -Values @($tableRows | ForEach-Object { Format-Percent $(if ($totalLines -gt 0) { ($_.Lines / [double]$totalLines) * 100.0 } else { 0.0 }) })),
    (New-ConsoleColumn -Group 'Sizes' -Name 'Total' -Values @($tableRows | ForEach-Object { Format-Integer $_.Bytes })),
    (New-ConsoleColumn -Group 'Sizes' -Name 'Share' -Values @($tableRows | ForEach-Object { Format-Percent $(if ($totalBytes -gt 0) { ($_.Bytes / [double]$totalBytes) * 100.0 } else { 0.0 }) })),
    (New-ConsoleColumn -Group 'Sizes' -Name 'Average' -Values @($tableRows | ForEach-Object { Format-Integer $(if ($_.Files -gt 0) { [Math]::Round($_.Bytes / [double]$_.Files) } else { 0 }) })),
    (New-ConsoleColumn -Name 'Density' -Values @($tableRows | ForEach-Object { Format-Ratio $(if ($_.SourceNonBlank -gt 0) { $_.NonBlank / [double]$_.SourceNonBlank } else { 0.0 }) }))
)

Write-SectionTitle "Comment lines by folder"
Write-GroupedConsoleTable -Columns $folderColumns

function Write-HitTable {
    param(
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][object[]]$Items,
        [Parameter(Mandatory = $true)][string]$Kind
    )

    $columns = @(
        (New-ConsoleColumn -Name 'Line' -Values @($Items | ForEach-Object { [string]$_.Line })),
        (New-ConsoleColumn -Name $Kind -Right $false -Values @($Items | ForEach-Object { [string]$_.($Kind) })),
        (New-ConsoleColumn -Name 'File' -Right $false -Values @($Items | ForEach-Object { $_.Relative }))
    )
    Write-GroupedConsoleTable -Columns $columns
}

if ($missingComments.Count -gt 0) {
    Write-SectionTitle ("Sources without a comment file ({0:N0})" -f $missingComments.Count)
    foreach ($item in $missingComments) { Write-AuditLine "$($item.Relative) -> $($item.Expected)" }
}

if ($orphanComments.Count -gt 0) {
    Write-SectionTitle ("Comment files without a source ({0:N0})" -f $orphanComments.Count)
    foreach ($item in $orphanComments) { Write-AuditLine $item.Relative }
}

if ($ruleHits.Count -gt 0) {
    Write-SectionTitle ("Comment lines breaking the line rules ({0:N0})" -f $ruleHits.Count)
    Write-HitTable -Items @($ruleHits) -Kind 'Problem'
}

if ($headingHits.Count -gt 0) {
    Write-SectionTitle ("Headings naming nothing in their source ({0:N0})" -f $headingHits.Count)
    Write-HitTable -Items @($headingHits) -Kind 'Problem'
}

if ($remarkHits.Count -gt 0) {
    Write-SectionTitle ("In-code comments ({0:N0})" -f $remarkHits.Count)
    Write-HitTable -Items @($remarkHits) -Kind 'Marker'
}

if ($readErrors.Count -gt 0) {
    Write-SectionTitle ("Unreadable files ({0:N0})" -f $readErrors.Count)
    foreach ($readError in $readErrors) { Write-AuditLine $readError }
}

# Markdown output.
$report = [System.Text.StringBuilder]::new()
[void]$report.AppendLine("# Comment report - $version")
[void]$report.AppendLine()
[void]$report.AppendLine("- Generated: $($generatedAt.ToString('yyyy-MM-dd HH:mm:ss zzz'))")
[void]$report.AppendLine("- Source roots: $(ConvertTo-MarkdownCell ($sourceRootFulls -join '; '))")
[void]$report.AppendLine("- Source files: $(ConvertTo-MarkdownCell ($sourceFileFulls -join '; '))")
[void]$report.AppendLine("- Comment pattern: ``$commentPattern``")
[void]$report.AppendLine("- Folder segments: $Segments")
[void]$report.AppendLine("- Line rules: at most $MaxWords words, one sentence, none of $(ConvertTo-MarkdownCell (($forbidden | ForEach-Object { '`' + $_ + '`' }) -join ' '))")
[void]$report.AppendLine("- Excluded directories: $(ConvertTo-MarkdownCell (($excludedNames | Sort-Object -Property @{ Expression = { Get-OrdinalKey $_ } }) -join ', '))")
[void]$report.AppendLine()
[void]$report.AppendLine("## Summary")
[void]$report.AppendLine()
[void]$report.AppendLine("| Metric | Value |")
[void]$report.AppendLine("|--------|------:|")
[void]$report.AppendLine("| Folders | $(Format-Integer $folderResults.Count) |")
[void]$report.AppendLine("| Source files | $(Format-Integer $sourceFiles.Count) |")
[void]$report.AppendLine("| Comment files | $(Format-Integer $totalFiles) |")
[void]$report.AppendLine("| Comment lines | $(Format-Integer $totalLines) |")
[void]$report.AppendLine("| Sources without a comment file | $(Format-Integer $missingComments.Count) |")
[void]$report.AppendLine("| Comment files without a source | $(Format-Integer $orphanComments.Count) |")
[void]$report.AppendLine("| Lines breaking the line rules | $(Format-Integer $ruleHits.Count) |")
[void]$report.AppendLine("| In-code comments | $(Format-Integer $remarkHits.Count) |")
[void]$report.AppendLine("| Headings naming nothing in their source | $(Format-Integer $headingHits.Count) |")
[void]$report.AppendLine()
[void]$report.AppendLine("## Comment lines by folder")
[void]$report.AppendLine()
[void]$report.AppendLine("Density is the number of non-blank comment lines written per non-blank source line.")
[void]$report.AppendLine()
[void]$report.AppendLine("| Folder | Sources | Files | Lines | Non-blank | Share | Bytes | Average bytes | Density |")
[void]$report.AppendLine("|--------|--------:|------:|------:|----------:|------:|------:|--------------:|--------:|")
foreach ($item in $folderResults) {
    $share = if ($totalLines -gt 0) { ($item.Lines / [double]$totalLines) * 100.0 } else { 0.0 }
    $density = if ($item.SourceNonBlank -gt 0) { $item.NonBlank / [double]$item.SourceNonBlank } else { 0.0 }
    $average = if ($item.Files -gt 0) { [Math]::Round($item.Bytes / [double]$item.Files) } else { 0 }
    [void]$report.AppendLine("| $(ConvertTo-MarkdownCell $item.Name) | $(Format-Integer $item.SourceFiles) | $(Format-Integer $item.Files) | $(Format-Integer $item.Lines) | $(Format-Integer $item.NonBlank) | $(Format-Percent $share) | $(Format-Integer $item.Bytes) | $(Format-Integer $average) | $(Format-Ratio $density) |")
}
[void]$report.AppendLine()
[void]$report.AppendLine("## Sources without a comment file")
[void]$report.AppendLine()
if ($missingComments.Count -eq 0) { [void]$report.AppendLine("None.") }
else {
    [void]$report.AppendLine("| Source | Expected |")
    [void]$report.AppendLine("|--------|----------|")
    foreach ($item in $missingComments) { [void]$report.AppendLine("| $(ConvertTo-MarkdownCell $item.Relative) | $(ConvertTo-MarkdownCell $item.Expected) |") }
}
[void]$report.AppendLine()
[void]$report.AppendLine("## Comment files without a source")
[void]$report.AppendLine()
if ($orphanComments.Count -eq 0) { [void]$report.AppendLine("None.") }
else { foreach ($item in $orphanComments) { [void]$report.AppendLine("- $(ConvertTo-MarkdownCell $item.Relative)") } }
[void]$report.AppendLine()
[void]$report.AppendLine("## Comment lines breaking the line rules")
[void]$report.AppendLine()
if ($ruleHits.Count -eq 0) { [void]$report.AppendLine("None.") }
else {
    [void]$report.AppendLine("| File | Line | Problem | Text |")
    [void]$report.AppendLine("|------|-----:|---------|------|")
    foreach ($item in $ruleHits) { [void]$report.AppendLine("| $(ConvertTo-MarkdownCell $item.Relative) | $(Format-Integer $item.Line) | $(ConvertTo-MarkdownCell $item.Problem) | $(ConvertTo-MarkdownCell $item.Text) |") }
}
[void]$report.AppendLine()
[void]$report.AppendLine("## In-code comments")
[void]$report.AppendLine()
if ($remarkHits.Count -eq 0) { [void]$report.AppendLine("None.") }
else {
    [void]$report.AppendLine("| File | Line | Marker | Text |")
    [void]$report.AppendLine("|------|-----:|--------|------|")
    foreach ($item in $remarkHits) { [void]$report.AppendLine("| $(ConvertTo-MarkdownCell $item.Relative) | $(Format-Integer $item.Line) | $(ConvertTo-MarkdownCell $item.Marker) | $(ConvertTo-MarkdownCell $item.Text) |") }
}
if ($readErrors.Count -gt 0) {
    [void]$report.AppendLine()
    [void]$report.AppendLine("## Read errors")
    [void]$report.AppendLine()
    foreach ($readError in $readErrors) { [void]$report.AppendLine("- $(ConvertTo-MarkdownCell $readError)") }
}

[System.IO.File]::WriteAllText($outputPathFull, ($report.ToString() -replace "`r`n", "`n"), [System.Text.UTF8Encoding]::new($false))
Write-AuditLine ""
Write-AuditLine "Report: $outputPathFull"

if ($Open) {
    Start-Process -FilePath $outputPathFull
}

if (($missingComments.Count + $orphanComments.Count + $ruleHits.Count + $remarkHits.Count + $headingHits.Count + $readErrors.Count) -gt 0) {
    exit 1
}

exit 0
