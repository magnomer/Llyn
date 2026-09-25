<#
.SYNOPSIS
Audits the comment files that accompany source files and the sources themselves for stray comments.

.DESCRIPTION
Reads the project configuration from auditcomments.json next to this script, then
performs these actions on every run:
  1. Prints comment-file line totals for each folder under the source roots.
  2. Prints sources that have no comment file, and comment files that have no source.
  3. Prints comment lines that break the line rules: too many words, a forbidden
     character, or more than one sentence.
  4. Prints in-code comment lines found inside sources.
  5. Writes a Markdown report to {report.directory}\{prefix}{version}.md.

A generation names the set of checks the audit applies. auditnames, auditlines and
auditcomments share one generation number; the hand-written convention-test settings
carry it and the tests refuse a setting written at another generation.

Everything project-specific lives in auditcomments.json. The script itself
carries no project knowledge. No external modules or tools are required.

auditcomments.json shape:
  {
    "generation": 11,
    "project": "Llyn",
    "sources": {
      "roots": ["src", "tests"],
      "files": ["Directory.Build.props"],
      "commentPattern": "*.comment.md",
      "pairs": { ".xaml.cs": "{base}.comment.md", ".cs": "{base}.comment.md",
                 ".xaml": "{base}.xaml.comment.md", ".csproj": "{base}.comment.md" },
      "excludeSegments": [".git", "bin", "obj"],
      "excludeSuffixes": [".g.cs", ".Designer.cs"]
    },
    "rules": { "maxWords": 20, "forbidden": [";"], "sentenceMarks": [".", "!", "?"] },
    "remark": {
      "markers": { ".cs": ["//", "/*"], ".xaml": ["<!--"] },
      "exemptFiles": ["TAuditSetting.cs"]
    },
    "report": {
      "directory": "docs-work/audit",
      "versionFile": "version.json",
      "versionKey": "current-version",
      "prefix": "Comments-",
      "depth": 1
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

.PARAMETER Depth
Overrides report.depth: how many path segments under a root form a folder row.

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
auditcomments -Depth 2

.EXAMPLE
auditcomments -SourceRoots .\src -MaxWords 25
#>
[CmdletBinding()]
param(
    [string]$ConfigPath,
    [string[]]$SourceRoots,
    [ValidateRange(1, 8)]
    [int]$Depth,
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
    auditcomments [-ConfigPath <path>] [-SourceRoots <path[]>] [-Depth <number>]
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

    -Depth <number>
        Path segments under a root that form a folder row. Overrides
        report.depth. 1 lists projects, 2 lists their first-level folders.

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

    auditcomments -Depth 2
        Break folder totals down one level further.

    auditcomments -SourceRoots .\src -MaxWords 25
        Audit only src, allowing 25 words per line.
'@ | Write-Host
    exit 0
}

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$script:AuditGeneration = 11

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

    return $node
}

function Read-AuditConfig {
    param([Parameter(Mandatory = $true)][string]$Path)

    $pathFull = [System.IO.Path]::GetFullPath($Path)
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
    foreach ($key in @('generation', 'project', 'sources.roots', 'sources.files', 'sources.commentPattern', 'sources.pairs', 'sources.excludeSegments', 'sources.excludeSuffixes',
                       'rules.maxWords', 'rules.forbidden', 'rules.sentenceMarks',
                       'remark.markers', 'remark.exemptFiles',
                       'report.directory', 'report.versionFile', 'report.versionKey', 'report.prefix', 'report.depth')) {
        if ($null -eq (Get-ConfigNode -Document $config -Key $key)) {
            $problems.Add("missing key '$key'")
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
if (-not $PSBoundParameters.ContainsKey('Depth')) { $Depth = [int]$config.report.depth }
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
    Write-AuditLine $Text -ForegroundColor Cyan
    Write-AuditLine ('-' * $Text.Length) -ForegroundColor DarkGray
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

    Write-AuditLine ""
    Write-AuditLine $upper.ToString() -ForegroundColor Green
    Write-AuditLine $lower.ToString() -ForegroundColor Green
    Write-AuditLine $rule.ToString() -ForegroundColor Green

    $rowCount = $Columns[0].Values.Count
    for ($row = 0; $row -lt $rowCount; $row++) {
        $line = [System.Text.StringBuilder]::new()
        foreach ($column in $Columns) {
            if ($line.Length -gt 0) { [void]$line.Append($gap) }
            [void]$line.Append((Format-Cell -Text $column.Values[$row] -Width $column.Width -Right $column.Right))
        }
        Write-AuditLine $line.ToString()
    }
    Write-AuditLine ""
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
    $segments = @($relative -split '[\\/]')
    $take = [Math]::Min($Depth, $segments.Count)
    return (($segments | Select-Object -First $take) -join '\')
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

$stringLiteralPattern = [System.Text.RegularExpressions.Regex]::new('(?:"{3,})[\s\S]*?"{3,}|(?:@\$?|\$@)"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|''(?:\\.|[^''\\])''', [System.Text.RegularExpressions.RegexOptions]::Compiled)
$codeSpanPattern = [System.Text.RegularExpressions.Regex]::new('`[^`]*`', [System.Text.RegularExpressions.RegexOptions]::Compiled)
$sentencePattern = [System.Text.RegularExpressions.Regex]::new('[' + [System.Text.RegularExpressions.Regex]::Escape(-join $sentenceMarks) + ']\s+\p{Lu}', [System.Text.RegularExpressions.RegexOptions]::Compiled)

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
    if ($text.Length -eq 0 -or $text.StartsWith('#')) { return @() }
    if ($text -match '^[-*>]\s+') { $text = $text.Substring($Matches[0].Length) }

    $problems = [System.Collections.Generic.List[string]]::new()
    $words = @(@($text -split '\s+') | Where-Object { $_.Length -gt 0 }).Count
    if ($words -gt $MaxWords) { $problems.Add("$words words") }

    $prose = $codeSpanPattern.Replace($text, '')
    foreach ($token in $forbidden) {
        if ($prose.Contains($token)) { $problems.Add("forbidden '$token'") }
    }

    $sentences = 1 + $sentencePattern.Matches($prose).Count
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
    $files = Get-ChildItem -LiteralPath $rootFull -File -Recurse | Where-Object {
        -not (Test-IsExcludedPath -Path $_.FullName -FolderRoot $rootFull -ExcludedNames $excludedNames)
    }

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
$folderResults = @($folderMap.Values | Sort-Object -Property @{ Expression = 'Lines'; Descending = $true }, @{ Expression = 'Name'; Descending = $false })

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

$missingComments = @($sourceFiles | Where-Object { -not [System.IO.File]::Exists((Join-Path $_.Directory $_.Expected)) } | Sort-Object Relative)
$orphanComments = @($commentFiles | Where-Object {
    -not ($expectedByDirectory.ContainsKey($_.Directory) -and $expectedByDirectory[$_.Directory].Contains($_.Name))
} | Sort-Object Relative)

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
foreach ($source in $sourceFiles) {
    if (-not $markers.ContainsKey($source.Extension)) { continue }
    if ($exemptFiles.Contains($source.Name)) { continue }
    $tokens = $markers[$source.Extension]
    $number = 0
    $content = [System.IO.File]::ReadAllText($source.Full)
    $stripped = if ($source.Extension -eq '.cs') { Remove-StringLiteral -Text $content } else { $content }
    $raw = $content -split "`n"
    foreach ($code in ($stripped -split "`n")) {
        $number++
        $line = if ($number -le $raw.Count) { $raw[$number - 1] } else { '' }
        foreach ($token in $tokens) {
            $at = $code.IndexOf($token, [System.StringComparison]::Ordinal)
            if ($at -ge 0) {
                $remarkHits.Add([pscustomobject]@{ Relative = $source.Relative; Line = $number; Marker = $token; Text = $line.Trim() })
                break
            }
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
        Write-Warning "The version file is not valid JSON, using $version : $versionPathFull"
    }
}
else {
    Write-Warning "The version file was not found, using $version : $versionPathFull"
}

if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $reportDirectoryFull ("{0}{1}.md" -f $reportPrefix, $version)
}
$outputPathFull = [System.IO.Path]::GetFullPath($OutputPath)
[System.IO.Directory]::CreateDirectory([System.IO.Path]::GetDirectoryName($outputPathFull)) | Out-Null

$generatedAt = Get-Date

# Console output.
$folderColumns = @(
    (New-ConsoleColumn -Name 'Folder' -Right $false -Values @($folderResults | ForEach-Object { $_.Name })),
    (New-ConsoleColumn -Name 'Files' -Values @($folderResults | ForEach-Object { Format-Integer $_.Files })),
    (New-ConsoleColumn -Group 'Lines' -Name 'Raw' -Values @($folderResults | ForEach-Object { Format-Integer $_.Lines })),
    (New-ConsoleColumn -Group 'Lines' -Name 'Non-blank' -Values @($folderResults | ForEach-Object { Format-Integer $_.NonBlank })),
    (New-ConsoleColumn -Group 'Lines' -Name 'Share' -Values @($folderResults | ForEach-Object { Format-Percent $(if ($totalLines -gt 0) { ($_.Lines / [double]$totalLines) * 100.0 } else { 0.0 }) })),
    (New-ConsoleColumn -Group 'Sizes' -Name 'Total' -Values @($folderResults | ForEach-Object { Format-Integer $_.Bytes })),
    (New-ConsoleColumn -Group 'Sizes' -Name 'Share' -Values @($folderResults | ForEach-Object { Format-Percent $(if ($totalBytes -gt 0) { ($_.Bytes / [double]$totalBytes) * 100.0 } else { 0.0 }) })),
    (New-ConsoleColumn -Group 'Sizes' -Name 'Average' -Values @($folderResults | ForEach-Object { Format-Integer $(if ($_.Files -gt 0) { [Math]::Round($_.Bytes / [double]$_.Files) } else { 0 }) })),
    (New-ConsoleColumn -Name 'Density' -Values @($folderResults | ForEach-Object { Format-Ratio $(if ($_.SourceNonBlank -gt 0) { $_.NonBlank / [double]$_.SourceNonBlank } else { 0.0 }) }))
)

Write-SectionTitle "Comment lines by folder"
Write-GroupedConsoleTable -Columns $folderColumns
Write-AuditLine ("Total: {0:N0} comment lines in {1:N0} files." -f $totalLines, $totalFiles) -ForegroundColor Green

function Write-HitTable {
    param(
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][object[]]$Items,
        [Parameter(Mandatory = $true)][string]$Kind
    )

    if ($Items.Count -eq 0) {
        Write-AuditLine "None." -ForegroundColor DarkGray
        return
    }

    $columns = @(
        (New-ConsoleColumn -Name 'Line' -Values @($Items | ForEach-Object { Format-Integer $_.Line })),
        (New-ConsoleColumn -Name $Kind -Right $false -Values @($Items | ForEach-Object { [string]$_.($Kind) })),
        (New-ConsoleColumn -Name 'File' -Right $false -Values @($Items | ForEach-Object { $_.Relative }))
    )
    Write-GroupedConsoleTable -Columns $columns
}

Write-SectionTitle "Sources without a comment file ($($missingComments.Count))"
if ($missingComments.Count -eq 0) { Write-AuditLine "None." -ForegroundColor DarkGray }
foreach ($item in $missingComments) { Write-AuditLine "  $($item.Relative)  ->  $($item.Expected)" }

Write-SectionTitle "Comment files without a source ($($orphanComments.Count))"
if ($orphanComments.Count -eq 0) { Write-AuditLine "None." -ForegroundColor DarkGray }
foreach ($item in $orphanComments) { Write-AuditLine "  $($item.Relative)" }

Write-SectionTitle "Comment lines breaking the line rules ($($ruleHits.Count))"
Write-HitTable -Items @($ruleHits) -Kind 'Problem'

Write-SectionTitle "In-code comments ($($remarkHits.Count))"
Write-HitTable -Items @($remarkHits) -Kind 'Marker'

# Markdown output.
$report = [System.Text.StringBuilder]::new()
[void]$report.AppendLine("# Comment report - $version")
[void]$report.AppendLine()
[void]$report.AppendLine("- Generated: $($generatedAt.ToString('yyyy-MM-dd HH:mm:ss zzz'))")
[void]$report.AppendLine("- Source roots: $(ConvertTo-MarkdownCell ($sourceRootFulls -join '; '))")
[void]$report.AppendLine("- Source files: $(ConvertTo-MarkdownCell ($sourceFileFulls -join '; '))")
[void]$report.AppendLine("- Comment pattern: ``$commentPattern``")
[void]$report.AppendLine("- Folder depth: $Depth")
[void]$report.AppendLine("- Line rules: at most $MaxWords words, one sentence, none of $(ConvertTo-MarkdownCell (($forbidden | ForEach-Object { '`' + $_ + '`' }) -join ' '))")
[void]$report.AppendLine("- Excluded directories: $(ConvertTo-MarkdownCell (($excludedNames | Sort-Object) -join ', '))")
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
    [void]$report.AppendLine("## Read warnings")
    [void]$report.AppendLine()
    foreach ($readError in $readErrors) { [void]$report.AppendLine("- $(ConvertTo-MarkdownCell $readError)") }
}

[System.IO.File]::WriteAllText($outputPathFull, ($report.ToString() -replace "`r`n", "`n"), [System.Text.UTF8Encoding]::new($false))
Write-AuditLine ""
Write-AuditLine "Markdown report: $outputPathFull" -ForegroundColor Green

if ($readErrors.Count -gt 0) {
    Write-Warning "$($readErrors.Count) file(s) could not be read. See the Markdown report for details."
}

if ($Open) {
    Start-Process -FilePath $outputPathFull
}

return
