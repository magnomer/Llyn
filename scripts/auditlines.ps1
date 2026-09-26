<#
.SYNOPSIS
Counts source lines under the configured roots, prints the results, and creates reports.

.DESCRIPTION
Reads the project configuration from auditlines.json next to this script, then
performs these actions on every run:
  1. Prints the counters: files over the line limit, lines over the width, unreadable files.
  2. Prints line and size totals for each folder under the source roots.
  3. Prints every counted file that reaches the line limit.
  4. Prints every counted file in the warning band below the limit.
  5. Prints files with lines over the configured width, per extension.
  6. Prints the files that gained the most lines over the last commits.
  7. Records both size bands to {report.directory}\{filesPrefix}{version}.md.
  8. Writes a Markdown source-line report to {report.directory}\{linesPrefix}{version}.md.
The console follows scripts\report.md: widest view first, empty lists left out.
Comment-file totals print in auditcomments only.

Everything project-specific - source roots, counted extensions, comment-file
pattern, excluded directory names, thresholds, report location, version file -
lives in auditlines.json. The script itself carries no project knowledge.
Files come from git: tracked and untracked files, never ignored ones, as in the
convention tests. Segments and extensions compare without case.
Git is the only external tool required.

auditlines.json shape:
  {
    "generation": 12,
    "project": "Llyn",
    "sources": {
      "roots": ["src", "tests"],
      "extensions": [".cs", ".xaml", ".csproj"],
      "commentPattern": "*.comment.md",
      "excludeSegments": [".git", ".vs", "bin", "obj", "artifacts", "packages", "node_modules", "publish"]
    },
    "thresholds": { "limit": 500, "warning": 450 },
    "width": { ".cs": 120, ".xaml": 200 },
    "hotspot": { "commits": 30, "top": 15 },
    "report": {
      "directory": "docs-work/audit",
      "versionFile": "version.json",
      "versionKey": "current-version",
      "linesPrefix": "Lines-",
      "filesPrefix": "Files-",
      "segments": 1
    }
  }

Width maps an extension to the longest line it allows; extensions left out are
not checked. Hotspot reads git history for the files that gained the most lines.

.PARAMETER ConfigPath
Path to the JSON configuration. Defaults to auditlines.json next to this script.

.PARAMETER SourceRoots
Overrides sources.roots for this run.

.PARAMETER Extensions
Overrides sources.extensions for this run.

.PARAMETER LimitThreshold
Overrides thresholds.limit for this run.

.PARAMETER WarningThreshold
Overrides thresholds.warning for this run.

.PARAMETER Segments
Overrides report.segments: how many path segments under a root form a folder row.

.PARAMETER Commits
Overrides hotspot.commits: how many recent commits feed the hotspot table. 0 skips it.

.PARAMETER OutputPath
Overrides the Markdown line-count report path for this run.

.PARAMETER Open
Open the generated report after the audit finishes.

.PARAMETER NoPause
Do not stop at each console page. Paging is also off when output or input is redirected.

.PARAMETER Help
Display this help and exit without running the audit. The alias -? is supported.

.EXAMPLE
auditlines

.EXAMPLE
auditlines -LimitThreshold 600 -WarningThreshold 500

.EXAMPLE
auditlines -Extensions .cs, .xaml

.EXAMPLE
auditlines -SourceRoots .\src, .\tests
#>
#requires -Version 5.1
# AUDITLINES GENERATION 12 - auditlines.ps1.
# A generation is not a revision count. It names functionality, not edits, so editing one of these
# files is never on its own a reason to raise it. Raise it only when the audited outcome changes.
# A generation names the set of checks the audit applies. Two projects on the same generation audit
# the same things and their reports compare directly, whatever else differs between the files. A check
# added, removed, or changed in what it reports is a new generation; wording, plumbing, and refactoring
# leave it alone. Every audit script shares one generation number with the convention-test settings,
# and each refuses a configuration written at another generation.
# Generation 11: nothing the line audit reports changes; the number rises with the truth audit,
# which checks that a deportment field reaches no request, keeps one writer, holds no logic and
# treats no engine data.
# Generation 12: nothing the line audit reports changes; the number rises with the convention tests,
# which bind with no compile error, count chain ceilings in names, count a using or a call on a
# deeper record as a reach, and exempt a contract name only where the type declares the interface.
[CmdletBinding()]
param(
    [string]$ConfigPath,
    [string[]]$SourceRoots,
    [string[]]$Extensions,
    [ValidateRange(1, [int]::MaxValue)]
    [int]$LimitThreshold,
    [ValidateRange(0, [int]::MaxValue)]
    [int]$WarningThreshold,
    [ValidateRange(1, 8)]
    [int]$Segments,
    [ValidateRange(0, [int]::MaxValue)]
    [int]$Commits,
    [string]$OutputPath,
    [switch]$Open,
    [switch]$NoPause,
    [Alias('?')]
    [switch]$Help
)

if ([string]::IsNullOrWhiteSpace($ConfigPath)) {
    $ConfigPath = Join-Path $PSScriptRoot "auditlines.json"
}

if ($Help) {
    @'
NAME
    auditlines.ps1

SYNOPSIS
    Count source lines under the configured roots and create Markdown reports.

SYNTAX
    auditlines [-ConfigPath <path>] [-SourceRoots <path[]>]
        [-Extensions <extension[]>] [-LimitThreshold <number>]
        [-WarningThreshold <number>] [-Segments <number>] [-Commits <number>]
        [-OutputPath <path>] [-Open] [-NoPause] [-Help]

CONFIGURATION
    All project-specific values live in auditlines.json next to the script:
    source roots, counted extensions, comment-file pattern, excluded
    directory names, thresholds, report directory, version file and key,
    and report file-name prefixes. Parameters below override it per run.

OPTIONS
    -ConfigPath <path>
        JSON configuration file. Defaults to .\auditlines.json.

    -SourceRoots <path[]>
        Source directories to count. Overrides sources.roots.

    -Extensions <extension[]>
        Extensions to count. Overrides sources.extensions.

    -LimitThreshold <number>
        Last line count that passes. Overrides thresholds.limit.

    -WarningThreshold <number>
        Last line count below the warning band. Overrides
        thresholds.warning.

    -Segments <number>
        Path segments under a root that form a folder row. Overrides
        report.segments. 1 lists projects, 2 lists their first-level folders.

    -Commits <number>
        Recent commits that feed the hotspot table. Overrides
        hotspot.commits. 0 skips the table.

    -OutputPath <path>
        Markdown line-count report path. By default, the project version is
        used to create a path under report.directory.

    -Open
        Open the generated line-count report after the audit finishes.

    -NoPause
        Do not stop at each console page for a key. Off by itself when output
        or input is redirected.

    -Help, -?
        Display this help and exit without running the audit.

EXAMPLES
    auditlines
        Audit with the configured settings.

    auditlines -LimitThreshold 600 -WarningThreshold 500
        Report files at 600 lines or more, and files from 500 to 599 lines.

    auditlines -Extensions .cs, .xaml
        Count only C# and XAML files.

    auditlines -SourceRoots .\src, .\tests
        Count both the src and tests source roots.
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

Write-AuditLine "AUDITLINES GENERATION $script:AuditGeneration" -ForegroundColor Cyan


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
        throw "The line-audit configuration was not found: $pathFull"
    }

    try {
        $config = Get-Content -LiteralPath $pathFull -Raw -Encoding UTF8 | ConvertFrom-Json
    }
    catch {
        throw "The line-audit configuration is not valid JSON: $pathFull`n$($_.Exception.Message)"
    }

    $problems = [System.Collections.Generic.List[string]]::new()
    foreach ($key in @('generation', 'project', 'sources.roots', 'sources.extensions', 'sources.commentPattern', 'sources.excludeSegments',
                       'thresholds.limit', 'thresholds.warning', 'width', 'hotspot.commits', 'hotspot.top',
                       'report.directory', 'report.versionFile', 'report.versionKey', 'report.linesPrefix', 'report.filesPrefix', 'report.segments')) {
        if ($null -eq (Get-ConfigNode -Document $config -Key $key)) {
            $problems.Add("missing key '$key'")
        }
    }

    if ($problems.Count -gt 0) {
        throw "The line-audit configuration is not valid: $pathFull`n  " + ($problems -join "`n  ")
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
    throw "The line-audit configuration is generation $($config.generation) but this tooling is generation $script:AuditGeneration.`nA generation names the set of checks applied, so the two must match: $ConfigPath"
}

if (-not $PSBoundParameters.ContainsKey('SourceRoots')) { $SourceRoots = @($config.sources.roots) }
if (-not $PSBoundParameters.ContainsKey('Extensions')) { $Extensions = @($config.sources.extensions) }
if (-not $PSBoundParameters.ContainsKey('LimitThreshold')) { $LimitThreshold = [int]$config.thresholds.limit }
if (-not $PSBoundParameters.ContainsKey('WarningThreshold')) { $WarningThreshold = [int]$config.thresholds.warning }
if (-not $PSBoundParameters.ContainsKey('Segments')) { $Segments = [int]$config.report.segments }
if (-not $PSBoundParameters.ContainsKey('Commits')) { $Commits = [int]$config.hotspot.commits }
$hotspotCommits = $Commits
$hotspotTop = [int]$config.hotspot.top

$widthLimits = @{}
foreach ($property in $config.width.PSObject.Properties) {
    $widthLimits[([string]$property.Name).ToLowerInvariant()] = [int]$property.Value
}

$commentPattern = [string]$config.sources.commentPattern
$commentSuffix = $commentPattern.TrimStart('*')
$reportDirectoryFull = Join-AuditPath -Root $repoRootFull -Relative $config.report.directory
$versionPathFull = Join-AuditPath -Root $repoRootFull -Relative $config.report.versionFile
$versionKey = [string]$config.report.versionKey
$linesPrefix = [string]$config.report.linesPrefix
$filesPrefix = [string]$config.report.filesPrefix

if ($LimitThreshold -lt 1) {
    throw "thresholds.limit ($LimitThreshold) must be at least 1."
}

if ($WarningThreshold -ge $LimitThreshold) {
    throw "WarningThreshold ($WarningThreshold) must be lower than LimitThreshold ($LimitThreshold)."
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

$sourceRootFulls = [System.Collections.Generic.List[string]]::new()
foreach ($root in $SourceRoots) {
    if ([string]::IsNullOrWhiteSpace($root)) {
        continue
    }

    $rootFull = Join-AuditPath -Root $repoRootFull -Relative $root
    if (-not [System.IO.Directory]::Exists($rootFull)) {
        throw "Source directory not found: $rootFull"
    }

    [void]$sourceRootFulls.Add($rootFull)
}

if ($sourceRootFulls.Count -eq 0) {
    throw "At least one source root must be supplied."
}

$normalizedExtensions = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
foreach ($extension in $Extensions) {
    if ([string]::IsNullOrWhiteSpace($extension)) {
        continue
    }

    $normalized = $extension.Trim()
    if (-not $normalized.StartsWith('.')) {
        $normalized = "." + $normalized
    }

    [void]$normalizedExtensions.Add($normalized)
}

if ($normalizedExtensions.Count -eq 0) {
    throw "At least one file extension must be supplied."
}

$excludedNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
foreach ($segment in @($config.sources.excludeSegments)) {
    if (-not [string]::IsNullOrWhiteSpace($segment)) {
        [void]$excludedNames.Add([string]$segment)
    }
}

$folderResults = [System.Collections.Generic.List[object]]::new()
$fileResults = [System.Collections.Generic.List[object]]::new()
$extensionTotals = @{}
$readErrors = [System.Collections.Generic.List[string]]::new()

function Get-FolderKey {
    param(
        [Parameter(Mandatory = $true)][string]$RootFull,
        [Parameter(Mandatory = $true)][string]$FileFull
    )

    $directory = [System.IO.Path]::GetDirectoryName($FileFull)
    $relative = $directory.Substring($RootFull.Length).TrimStart([char[]]@('\', '/'))
    if ($relative.Length -eq 0) { return "(root)" }
    $parts = @($relative -split '[\\/]')
    $take = [Math]::Min($Segments, $parts.Count)
    return (($parts | Select-Object -First $take) -join '\')
}

function New-FolderResult {
    param([Parameter(Mandatory = $true)][string]$Name)

    return [pscustomobject]@{
        Name = $Name
        Files = 0
        Lines = [long]0
        NonBlankLines = [long]0
        BlankLines = [long]0
        Bytes = [long]0
        CommentFiles = 0
        CommentLines = [long]0
        CommentNonBlankLines = [long]0
        CommentBytes = [long]0
    }
}

$folderMap = [ordered]@{}
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
        $isComment = $file.Name.EndsWith($commentSuffix, [System.StringComparison]::OrdinalIgnoreCase)
        if (-not $isComment -and -not $normalizedExtensions.Contains($file.Extension)) { continue }

        $folderKey = Get-FolderKey -RootFull $rootFull -FileFull $file.FullName
        if (-not $folderMap.Contains($folderKey)) { $folderMap[$folderKey] = New-FolderResult -Name $folderKey }
        $folder = $folderMap[$folderKey]

        $extensionKey = $file.Extension.ToLowerInvariant()
        $widthLimit = if ($widthLimits.ContainsKey($extensionKey)) { $widthLimits[$extensionKey] } else { 0 }
        [long]$fileLines = 0
        [long]$fileNonBlankLines = 0
        [int]$fileMaxWidth = 0
        [int]$fileOverWidth = 0

        try {
            foreach ($line in [System.IO.File]::ReadLines($file.FullName)) {
                $fileLines++
                if (-not [string]::IsNullOrWhiteSpace($line)) { $fileNonBlankLines++ }
                if ($line.Length -gt $fileMaxWidth) { $fileMaxWidth = $line.Length }
                if ($widthLimit -gt 0 -and $line.Length -gt $widthLimit) { $fileOverWidth++ }
            }
        }
        catch {
            $readErrors.Add("$($file.FullName): $($_.Exception.Message)")
            continue
        }

        if ($isComment) {
            $folder.CommentFiles++
            $folder.CommentLines += $fileLines
            $folder.CommentNonBlankLines += $fileNonBlankLines
            $folder.CommentBytes += $file.Length
            continue
        }

        $fileResults.Add([pscustomobject]@{
            Folder = $folderKey
            Name = $file.Name
            RelativePath = Get-RelativePathSafe -BasePath $repoRootFull -Path $file.FullName
            Extension = $extensionKey
            Lines = $fileLines
            NonBlankLines = $fileNonBlankLines
            BlankLines = $fileLines - $fileNonBlankLines
            Bytes = $file.Length
            MaxWidth = $fileMaxWidth
            OverWidth = $fileOverWidth
            WidthLimit = $widthLimit
        })

        $folder.Files++
        $folder.Lines += $fileLines
        $folder.NonBlankLines += $fileNonBlankLines
        $folder.BlankLines += $fileLines - $fileNonBlankLines
        $folder.Bytes += $file.Length

        if (-not $extensionTotals.ContainsKey($extensionKey)) {
            $extensionTotals[$extensionKey] = [pscustomobject]@{ Extension = $extensionKey; Files = 0; Lines = [long]0; NonBlankLines = [long]0 }
        }
        $extensionTotals[$extensionKey].Files++
        $extensionTotals[$extensionKey].Lines += $fileLines
        $extensionTotals[$extensionKey].NonBlankLines += $fileNonBlankLines
    }
}
foreach ($entry in $folderMap.Values) { $folderResults.Add($entry) }

if ($fileResults.Count -eq 0) {
    throw "No source file was scanned under the configured roots, so the audit cannot judge."
}

$folderResults = @(
    $folderResults |
        Sort-Object -Property @{ Expression = 'Lines'; Descending = $true }, @{ Expression = { Get-OrdinalKey $_.Name } }
)

$fileResults = @(
    $fileResults |
        Sort-Object -Property @{ Expression = 'Lines'; Descending = $true }, @{ Expression = { Get-OrdinalKey $_.RelativePath } }
)

$overLimitFiles = @($fileResults | Where-Object { $_.Lines -gt $LimitThreshold })
$warningFiles = @($fileResults | Where-Object { $_.Lines -gt $WarningThreshold -and $_.Lines -le $LimitThreshold })

$overLimitTitle = "Files over $LimitThreshold lines"
$warningTitle = "Files at $($WarningThreshold + 1)-$LimitThreshold lines"

[long]$totalLines = 0
[long]$totalNonBlankLines = 0
[long]$totalBytes = 0
[int]$totalFiles = 0
[long]$totalCommentLines = 0
[long]$totalCommentNonBlankLines = 0
[long]$totalCommentBytes = 0
[int]$totalCommentFiles = 0
foreach ($item in $folderResults) {
    $totalLines += $item.Lines
    $totalNonBlankLines += $item.NonBlankLines
    $totalBytes += $item.Bytes
    $totalFiles += $item.Files
    $totalCommentLines += $item.CommentLines
    $totalCommentNonBlankLines += $item.CommentNonBlankLines
    $totalCommentBytes += $item.CommentBytes
    $totalCommentFiles += $item.CommentFiles
}

$largestFolder = if ($folderResults.Count -gt 0) { $folderResults[0] } else { $null }
$generatedAt = Get-Date
$extensionLabel = (($normalizedExtensions | Sort-Object -Property @{ Expression = { Get-OrdinalKey $_ } }) -join ", ")
$blankLines = $totalLines - $totalNonBlankLines

# Version, read from the configured version file and key.
$version = "0.0.0"
if ([System.IO.File]::Exists($versionPathFull)) {
    try {
        $versionData = Get-Content -LiteralPath $versionPathFull -Raw -Encoding UTF8 | ConvertFrom-Json
        $versionValue = Get-ConfigNode -Document $versionData -Key $versionKey
        if (-not [string]::IsNullOrWhiteSpace([string]$versionValue)) {
            $version = [string]$versionValue
        }
    }
    catch {
        Write-AuditLine "The version file is not valid JSON, using $version : $versionPathFull"
    }
}
else {
    Write-AuditLine "The version file was not found, using $version : $versionPathFull"
}

$filesFileName = "{0}{1}.md" -f $filesPrefix, $version
if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $reportDirectoryFull ("{0}{1}.md" -f $linesPrefix, $version)
}

$outputPathFull = [System.IO.Path]::GetFullPath($OutputPath)
$outputDirectory = [System.IO.Path]::GetDirectoryName($outputPathFull)
if ([string]::IsNullOrWhiteSpace($outputDirectory)) {
    throw "The output path must include a valid directory: $outputPathFull"
}

[System.IO.Directory]::CreateDirectory($outputDirectory) | Out-Null

function New-FileConsoleColumns {
    param(
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][object[]]$Items
    )

    return @(
        (New-ConsoleColumn -Group 'Lines' -Name 'Raw' -Values @($Items | ForEach-Object { Format-Integer $_.Lines })),
        (New-ConsoleColumn -Group 'Lines' -Name 'Non-blank' -Values @($Items | ForEach-Object { Format-Integer $_.NonBlankLines })),
        (New-ConsoleColumn -Group 'Sizes' -Name 'Raw' -Values @($Items | ForEach-Object { Format-Integer $_.Bytes })),
        (New-ConsoleColumn -Name 'Location' -Right $false -Values @($Items | ForEach-Object { [string][System.IO.Path]::GetDirectoryName($_.RelativePath) })),
        (New-ConsoleColumn -Name 'File' -Right $false -Values @($Items | ForEach-Object { $_.Name }))
    )
}

function Write-FileConsoleTable {
    param(
        [Parameter(Mandatory = $true)][object[]]$Columns
    )

    Write-GroupedConsoleTable -Columns $Columns
}

function Add-FileMarkdownTable {
    param(
        [Parameter(Mandatory = $true)][System.Text.StringBuilder]$Builder,
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][object[]]$Items
    )

    if ($Items.Count -eq 0) {
        [void]$Builder.AppendLine("None.")
        return
    }

    [void]$Builder.AppendLine("| Lines | Non-blank | Folder | File |")
    [void]$Builder.AppendLine("|------:|----------:|--------|------|")
    foreach ($item in $Items) {
        [void]$Builder.AppendLine("| $(Format-Integer $item.Lines) | $(Format-Integer $item.NonBlankLines) | $(ConvertTo-MarkdownCell $item.Folder) | $(ConvertTo-MarkdownCell $item.RelativePath) |")
    }
}

# Record both inventories to the report directory.
[System.IO.Directory]::CreateDirectory($reportDirectoryFull) | Out-Null
$filesPathFull = Join-Path $reportDirectoryFull $filesFileName

$filesBuilder = [System.Text.StringBuilder]::new()
[void]$filesBuilder.AppendLine("# Large file report - $version")
[void]$filesBuilder.AppendLine()
[void]$filesBuilder.AppendLine("Generated $($generatedAt.ToString('yyyy-MM-dd HH:mm:ss zzz')). $($overLimitFiles.Count) file(s) over $LimitThreshold lines, $($warningFiles.Count) file(s) in the $($WarningThreshold + 1)-$LimitThreshold line band.")
[void]$filesBuilder.AppendLine()
[void]$filesBuilder.AppendLine("## $overLimitTitle")
[void]$filesBuilder.AppendLine()
Add-FileMarkdownTable -Builder $filesBuilder -Items $overLimitFiles
[void]$filesBuilder.AppendLine()
[void]$filesBuilder.AppendLine("## $warningTitle")
[void]$filesBuilder.AppendLine()
Add-FileMarkdownTable -Builder $filesBuilder -Items $warningFiles
[System.IO.File]::WriteAllText($filesPathFull, ($filesBuilder.ToString() -replace "`r`n", "`n"), [System.Text.UTF8Encoding]::new($false))

# Lines over the configured width, per extension.
$wideFiles = @($fileResults | Where-Object { $_.OverWidth -gt 0 } | Sort-Object -Property @{ Expression = 'OverWidth'; Descending = $true }, @{ Expression = { Get-OrdinalKey $_.RelativePath } })
$widthLabel = (($widthLimits.Keys | Sort-Object -Property @{ Expression = { Get-OrdinalKey $_ } } | ForEach-Object { "$_ $($widthLimits[$_])" }) -join ", ")
$wideTitle = "Files with lines over the width limit"
[long]$wideLineCount = ($fileResults | Measure-Object -Property OverWidth -Sum).Sum

# Hotspots: files that gained the most lines over the last commits, read from git.
$hotspots = @()
$hotspotNote = ''
$hotspotTitle = "Hotspots over the last $hotspotCommits commit(s)"
if ($hotspotCommits -gt 0) {
    $gitArguments = @('-C', $repoRootFull, '-c', 'core.quotepath=false', 'log', '--numstat', '--format=', "-n", "$hotspotCommits", '--') + @($sourceRootFulls)
    $numstat = $null
    try { $numstat = & git @gitArguments 2>$null } catch { $numstat = $null }
    if ($LASTEXITCODE -eq 0 -and $null -ne $numstat) {
        $growth = @{}
        foreach ($row in @($numstat)) {
            $parts = [string]$row -split "`t", 3
            if ($parts.Count -lt 3 -or $parts[0] -eq '-' ) { continue }
            # A rename reads "dir/{old => new}/file" or "old => new"; the file now lives at the new side.
            $path = ([string]$parts[2] -replace '\{[^{}]* => ([^{}]*)\}', '$1') -replace '//+', '/'
            if ($path.Contains(' => ')) { $path = ($path -split ' => ', 2)[1] }
            $extension = [System.IO.Path]::GetExtension($path)
            if (-not $normalizedExtensions.Contains($extension)) { continue }
            if (-not $growth.ContainsKey($path)) { $growth[$path] = [pscustomobject]@{ Path = $path; Added = [long]0; Deleted = [long]0 } }
            $growth[$path].Added += [long]$parts[0]
            $growth[$path].Deleted += [long]$parts[1]
        }
        $hotspots = @($growth.Values | Sort-Object -Property @{ Expression = 'Added'; Descending = $true }, @{ Expression = { Get-OrdinalKey $_.Path } } | Select-Object -First $hotspotTop)
    }
    else {
        $hotspotNote = "Git history could not be read, so hotspots are skipped."
    }

}

# Console output, from the widest view to the narrowest.
Write-AuditLine ("Scanned: {0:N0} source files" -f $fileResults.Count) -ForegroundColor DarkGray

$counterRows = @(
    @($overLimitTitle, $overLimitFiles.Count),
    @('Lines over the width limit', $wideLineCount),
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
    NonBlankLines = [long](($folderResults | Measure-Object -Property NonBlankLines -Sum).Sum)
    Bytes = $totalBytes
})
Write-SectionTitle "Source lines by folder"
Write-GroupedConsoleTable -Columns @(
    (New-ConsoleColumn -Name 'Folder' -Right $false -Values @($tableRows | ForEach-Object { $_.Name })),
    (New-ConsoleColumn -Name 'Files' -Values @($tableRows | ForEach-Object { Format-Integer $_.Files })),
    (New-ConsoleColumn -Group 'Lines' -Name 'Raw' -Values @($tableRows | ForEach-Object { Format-Integer $_.Lines })),
    (New-ConsoleColumn -Group 'Lines' -Name 'Non-blank' -Values @($tableRows | ForEach-Object { Format-Integer $_.NonBlankLines })),
    (New-ConsoleColumn -Group 'Lines' -Name 'Share' -Values @($tableRows | ForEach-Object { Format-Percent $(if ($totalLines -gt 0) { ($_.Lines / [double]$totalLines) * 100.0 } else { 0.0 }) })),
    (New-ConsoleColumn -Group 'Sizes' -Name 'Total' -Values @($tableRows | ForEach-Object { Format-Integer $_.Bytes })),
    (New-ConsoleColumn -Group 'Sizes' -Name 'Share' -Values @($tableRows | ForEach-Object { Format-Percent $(if ($totalBytes -gt 0) { ($_.Bytes / [double]$totalBytes) * 100.0 } else { 0.0 }) })),
    (New-ConsoleColumn -Group 'Sizes' -Name 'Average' -Values @($tableRows | ForEach-Object { Format-Integer $(if ($_.Files -gt 0) { [Math]::Round($_.Bytes / [double]$_.Files) } else { 0 }) }))
)

$overLimitColumns = New-FileConsoleColumns -Items $overLimitFiles
$warningColumns = New-FileConsoleColumns -Items $warningFiles
Sync-ConsoleColumnWidth -Tables @($overLimitColumns, $warningColumns)

if ($overLimitFiles.Count -gt 0) {
    Write-SectionTitle ("{0} ({1:N0})" -f $overLimitTitle, $overLimitFiles.Count)
    Write-FileConsoleTable -Columns $overLimitColumns
}

if ($warningFiles.Count -gt 0) {
    Write-SectionTitle ("{0} ({1:N0})" -f $warningTitle, $warningFiles.Count)
    Write-FileConsoleTable -Columns $warningColumns
}

if ($wideFiles.Count -gt 0) {
    Write-SectionTitle ("{0} ({1:N0})" -f $wideTitle, $wideFiles.Count)
    Write-GroupedConsoleTable -Columns @(
        (New-ConsoleColumn -Name 'Over' -Values @($wideFiles | ForEach-Object { Format-Integer $_.OverWidth })),
        (New-ConsoleColumn -Name 'Widest' -Values @($wideFiles | ForEach-Object { Format-Integer $_.MaxWidth })),
        (New-ConsoleColumn -Name 'Limit' -Values @($wideFiles | ForEach-Object { Format-Integer $_.WidthLimit })),
        (New-ConsoleColumn -Name 'Location' -Right $false -Values @($wideFiles | ForEach-Object { [string][System.IO.Path]::GetDirectoryName($_.RelativePath) })),
        (New-ConsoleColumn -Name 'File' -Right $false -Values @($wideFiles | ForEach-Object { $_.Name }))
    )
}

if ($hotspotCommits -gt 0) {
    Write-SectionTitle $hotspotTitle
    if ($hotspotNote -ne '') {
        Write-AuditLine $hotspotNote
    }
    elseif ($hotspots.Count -gt 0) {
        $currentLines = @{}
        foreach ($item in $fileResults) { $currentLines[$item.RelativePath.Replace('\', '/')] = $item.Lines }
        Write-GroupedConsoleTable -Columns @(
            (New-ConsoleColumn -Name 'Added' -Values @($hotspots | ForEach-Object { Format-Integer $_.Added })),
            (New-ConsoleColumn -Name 'Deleted' -Values @($hotspots | ForEach-Object { Format-Integer $_.Deleted })),
            (New-ConsoleColumn -Name 'Net' -Values @($hotspots | ForEach-Object { Format-Integer ($_.Added - $_.Deleted) })),
            (New-ConsoleColumn -Name 'Now' -Values @($hotspots | ForEach-Object { if ($currentLines.ContainsKey($_.Path)) { Format-Integer $currentLines[$_.Path] } else { "-" } })),
            (New-ConsoleColumn -Name 'File' -Right $false -Values @($hotspots | ForEach-Object { $_.Path }))
        )
    }
}

if ($readErrors.Count -gt 0) {
    Write-SectionTitle ("Unreadable files ({0:N0})" -f $readErrors.Count)
    foreach ($readError in $readErrors) { Write-AuditLine $readError }
}

# Markdown output: repository summary, large files, folder totals, and extension totals.
$reportBuilder = [System.Text.StringBuilder]::new()
[void]$reportBuilder.AppendLine("# Source line report - $version")
[void]$reportBuilder.AppendLine()
[void]$reportBuilder.AppendLine("- Generated: $($generatedAt.ToString('yyyy-MM-dd HH:mm:ss zzz'))")
[void]$reportBuilder.AppendLine("- Source roots: $(ConvertTo-MarkdownCell ($sourceRootFulls -join '; '))")
[void]$reportBuilder.AppendLine("- Counted extensions: $(ConvertTo-MarkdownCell $extensionLabel)")
[void]$reportBuilder.AppendLine("- Excluded directories: $(ConvertTo-MarkdownCell (($excludedNames | Sort-Object -Property @{ Expression = { Get-OrdinalKey $_ } }) -join ', '))")
[void]$reportBuilder.AppendLine("- Folder segments: $Segments")
[void]$reportBuilder.AppendLine()
[void]$reportBuilder.AppendLine("## Summary")
[void]$reportBuilder.AppendLine()
[void]$reportBuilder.AppendLine("| Metric | Value |")
[void]$reportBuilder.AppendLine("|--------|------:|")
[void]$reportBuilder.AppendLine("| Folders | $(Format-Integer $folderResults.Count) |")
[void]$reportBuilder.AppendLine("| Counted files | $(Format-Integer $totalFiles) |")
[void]$reportBuilder.AppendLine("| Physical lines | $(Format-Integer $totalLines) |")
[void]$reportBuilder.AppendLine("| Non-blank lines | $(Format-Integer $totalNonBlankLines) |")
[void]$reportBuilder.AppendLine("| Blank lines | $(Format-Integer $blankLines) |")
[void]$reportBuilder.AppendLine("| Total bytes | $(Format-Integer $totalBytes) |")
[void]$reportBuilder.AppendLine("| Comment files | $(Format-Integer $totalCommentFiles) |")
[void]$reportBuilder.AppendLine("| Comment lines | $(Format-Integer $totalCommentLines) |")
[void]$reportBuilder.AppendLine("| $overLimitTitle | $(Format-Integer $overLimitFiles.Count) |")
[void]$reportBuilder.AppendLine("| $warningTitle | $(Format-Integer $warningFiles.Count) |")
[void]$reportBuilder.AppendLine("| Lines over the width limit | $(Format-Integer $wideLineCount) |")
if ($null -ne $largestFolder) {
    [void]$reportBuilder.AppendLine("| Largest folder | $(ConvertTo-MarkdownCell $largestFolder.Name) ($(Format-Integer $largestFolder.Lines) lines) |")
}
[void]$reportBuilder.AppendLine()
[void]$reportBuilder.AppendLine("## $overLimitTitle")
[void]$reportBuilder.AppendLine()
[void]$reportBuilder.AppendLine('Both inventories are also written to `' + (Get-RelativePathSafe -BasePath $repoRootFull -Path $filesPathFull) + '`.')
[void]$reportBuilder.AppendLine()
Add-FileMarkdownTable -Builder $reportBuilder -Items $overLimitFiles
[void]$reportBuilder.AppendLine()
[void]$reportBuilder.AppendLine("## $warningTitle")
[void]$reportBuilder.AppendLine()
Add-FileMarkdownTable -Builder $reportBuilder -Items $warningFiles
[void]$reportBuilder.AppendLine()
[void]$reportBuilder.AppendLine("## $wideTitle ($widthLabel)")
[void]$reportBuilder.AppendLine()
if ($wideFiles.Count -eq 0) { [void]$reportBuilder.AppendLine("None.") }
else {
    [void]$reportBuilder.AppendLine("| Over | Widest | Limit | File |")
    [void]$reportBuilder.AppendLine("|-----:|-------:|------:|------|")
    foreach ($item in $wideFiles) {
        [void]$reportBuilder.AppendLine("| $(Format-Integer $item.OverWidth) | $(Format-Integer $item.MaxWidth) | $(Format-Integer $item.WidthLimit) | $(ConvertTo-MarkdownCell $item.RelativePath) |")
    }
}
[void]$reportBuilder.AppendLine()
if ($hotspotCommits -gt 0) {
    [void]$reportBuilder.AppendLine("## $hotspotTitle")
    [void]$reportBuilder.AppendLine()
    if ($hotspots.Count -eq 0) { [void]$reportBuilder.AppendLine("None.") }
    else {
        [void]$reportBuilder.AppendLine("| Added | Deleted | Net | File |")
        [void]$reportBuilder.AppendLine("|------:|--------:|----:|------|")
        foreach ($item in $hotspots) {
            [void]$reportBuilder.AppendLine("| $(Format-Integer $item.Added) | $(Format-Integer $item.Deleted) | $(Format-Integer ($item.Added - $item.Deleted)) | $(ConvertTo-MarkdownCell $item.Path) |")
        }
    }
    [void]$reportBuilder.AppendLine()
}
[void]$reportBuilder.AppendLine("## Lines by source folder")
[void]$reportBuilder.AppendLine()
[void]$reportBuilder.AppendLine("| Folder | Files | Lines | Non-blank | Share | Bytes | Size share | Average bytes |")
[void]$reportBuilder.AppendLine("|--------|------:|------:|----------:|------:|------:|-----------:|--------------:|")
foreach ($item in $folderResults) {
    $share = if ($totalLines -gt 0) { ($item.Lines / [double]$totalLines) * 100.0 } else { 0.0 }
    $sizeShare = if ($totalBytes -gt 0) { ($item.Bytes / [double]$totalBytes) * 100.0 } else { 0.0 }
    $averageSize = if ($item.Files -gt 0) { $item.Bytes / [double]$item.Files } else { 0.0 }
    [void]$reportBuilder.AppendLine("| $(ConvertTo-MarkdownCell $item.Name) | $(Format-Integer $item.Files) | $(Format-Integer $item.Lines) | $(Format-Integer $item.NonBlankLines) | $(Format-Percent $share) | $(Format-Integer $item.Bytes) | $(Format-Percent $sizeShare) | $([Math]::Round($averageSize).ToString('N0', [System.Globalization.CultureInfo]::InvariantCulture)) |")
}
[void]$reportBuilder.AppendLine()
[void]$reportBuilder.AppendLine("## Comment lines by source folder")
[void]$reportBuilder.AppendLine()
[void]$reportBuilder.AppendLine('Counts `' + $commentPattern + '` files under each folder.')
[void]$reportBuilder.AppendLine()
[void]$reportBuilder.AppendLine("Density is the number of non-blank comment lines written per non-blank source line.")
[void]$reportBuilder.AppendLine()
[void]$reportBuilder.AppendLine("| Folder | Files | Lines | Non-blank | Share | Bytes | Average bytes | Density |")
[void]$reportBuilder.AppendLine("|--------|------:|------:|----------:|------:|------:|--------------:|--------:|")
foreach ($item in $folderResults) {
    $share = if ($totalCommentLines -gt 0) { ($item.CommentLines / [double]$totalCommentLines) * 100.0 } else { 0.0 }
    $density = if ($item.NonBlankLines -gt 0) { $item.CommentNonBlankLines / [double]$item.NonBlankLines } else { 0.0 }
    $averageSize = if ($item.CommentFiles -gt 0) { $item.CommentBytes / [double]$item.CommentFiles } else { 0.0 }
    [void]$reportBuilder.AppendLine("| $(ConvertTo-MarkdownCell $item.Name) | $(Format-Integer $item.CommentFiles) | $(Format-Integer $item.CommentLines) | $(Format-Integer $item.CommentNonBlankLines) | $(Format-Percent $share) | $(Format-Integer $item.CommentBytes) | $([Math]::Round($averageSize).ToString('N0', [System.Globalization.CultureInfo]::InvariantCulture)) | $(Format-Ratio $density) |")
}
[void]$reportBuilder.AppendLine()
[void]$reportBuilder.AppendLine("## Lines by extension")
[void]$reportBuilder.AppendLine()
[void]$reportBuilder.AppendLine("| Extension | Files | Lines | Non-blank | Blank | Share |")
[void]$reportBuilder.AppendLine("|-----------|------:|------:|----------:|------:|------:|")
$sortedExtensionTotals = @(
    $extensionTotals.Values |
        Sort-Object -Property @{ Expression = 'Lines'; Descending = $true }, @{ Expression = { Get-OrdinalKey $_.Extension } }
)
foreach ($item in $sortedExtensionTotals) {
    $share = if ($totalLines -gt 0) { ($item.Lines / [double]$totalLines) * 100.0 } else { 0.0 }
    [void]$reportBuilder.AppendLine("| $(ConvertTo-MarkdownCell $item.Extension) | $(Format-Integer $item.Files) | $(Format-Integer $item.Lines) | $(Format-Integer $item.NonBlankLines) | $(Format-Integer ($item.Lines - $item.NonBlankLines)) | $(Format-Percent $share) |")
}

if ($readErrors.Count -gt 0) {
    [void]$reportBuilder.AppendLine()
    [void]$reportBuilder.AppendLine("## Read warnings")
    [void]$reportBuilder.AppendLine()
    foreach ($readError in $readErrors) {
        [void]$reportBuilder.AppendLine("- $(ConvertTo-MarkdownCell $readError)")
    }
}

[System.IO.File]::WriteAllText($outputPathFull, ($reportBuilder.ToString() -replace "`r`n", "`n"), [System.Text.UTF8Encoding]::new($false))

Write-AuditLine ""
Write-AuditLine "Report: $outputPathFull"
Write-AuditLine "Report: $filesPathFull"

if ($Open) {
    Start-Process -FilePath $outputPathFull
}

if (($overLimitFiles.Count + $wideLineCount + $readErrors.Count) -gt 0) {
    exit 1
}

exit 0
