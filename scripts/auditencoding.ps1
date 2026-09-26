<#
.SYNOPSIS
Audits every tracked text file for one encoding form: UTF-8, no mark, LF, a final newline, clean text.

.DESCRIPTION
Reads the project configuration from auditencoding.json next to this script, lists the text
files through git, reads each one as raw bytes and reports:

  Invalid UTF-8           a byte sequence that is not UTF-8, at the offset of the first bad byte
  Byte order marks        a byte order mark anywhere in the file, the first three bytes included
  Carriage returns        a carriage return, paired with a line feed or alone
  Missing final newlines  a non-empty file whose last byte is not a line feed
  Control characters      a raw C0 control other than tab, line feed and carriage return, or delete
  Double-encoded text     UTF-8 that was read back through a single-byte page and saved again
  Tabs in sources         a tab in a file whose suffix is listed under tabless
  Non-ASCII scripts       a byte outside ASCII in a file whose suffix is listed under ascii

Files come from git: tracked and untracked files, never ignored ones, as in the convention
tests. Excluded segments and skipped names compare without case. The console follows
scripts\report.md. No report file is written, since the hit list is the whole result.

This script carries no project-specific value of its own. Everything a project chooses - the
kinds of file, the skipped names, the suffixes and the patterns - lives in auditencoding.json.
Git is the only external tool required.

auditencoding.json shape:
  {
    "generation": 15,
    "project": "Llyn",
    "sources": {
      "include": ["*.cs", "*.md", "*.ps1"],
      "excludeSegments": [".git", "bin", "obj"],
      "skip": ["version.json"]
    },
    "tabless": [".cs", ".xaml"],
    "ascii": [".ps1"],
    "patterns": {
      "control": "<regex>",
      "trail": "<regex>",
      "mojibake": "<regex, where {trail} stands for the trail pattern>"
    }
  }

.PARAMETER Root
Project root to audit. Defaults to the directory containing this script's parent.

.PARAMETER ConfigPath
Path to the JSON configuration. Defaults to auditencoding.json next to this script.

.PARAMETER Help
Display this help and exit. The alias -? is supported.

.EXAMPLE
auditencoding
Audit the current checkout.

.EXAMPLE
auditencoding -Root D:\temp\sample
Audit another git working tree with this configuration.
#>
#requires -Version 5.1
# AUDITENCODING GENERATION 15 - auditencoding.ps1.
# A generation is not a revision count. It names functionality, not edits, so editing one of these
# files is never on its own a reason to raise it. Raise it only when the audited outcome changes.
# A generation names the set of checks the audit applies. Two projects on the same generation audit
# the same things and their reports compare directly, whatever else differs between the files. A check
# added, removed, or changed in what it reports is a new generation; wording, plumbing, and refactoring
# leave it alone. Every audit script shares one generation number with the convention-test settings,
# and each refuses a configuration written at another generation.
# Generation 12: the first generation of this audit. It reports the eight kinds of the convention test.
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
    auditencoding.ps1

SYNOPSIS
    Audit every tracked text file for valid UTF-8 with no byte order mark,
    LF line breaks, a final newline, no raw control character, no
    double-encoded text, no tab in sources and pure ASCII in scripts.

SYNTAX
    auditencoding [-Root <path>] [-ConfigPath <path>] [-Help]

CONFIGURATION
    All project-specific values live in auditencoding.json next to the
    script: the git pathspecs, the excluded segments, the skipped names, the
    tabless and ascii suffixes and the control and mojibake patterns.

RESULT
    Invalid UTF-8           A byte sequence that is not UTF-8.
    Byte order marks        A byte order mark anywhere in the file.
    Carriage returns        A carriage return, paired or alone.
    Missing final newlines  A non-empty file not ending in a line feed.
    Control characters      A raw control character.
    Double-encoded text     UTF-8 read back through a single-byte page.
    Tabs in sources         A tab in a tabless file.
    Non-ASCII scripts       A byte outside ASCII in a script.

    The exit code is 1 when any Result gate is above 0 and 0 otherwise.

OPTIONS
    -Root <path>
        Project root to audit. Defaults to the parent of the script folder.

    -ConfigPath <path>
        JSON configuration file. Defaults to .\auditencoding.json.

    -Help, -?
        Display this help and exit without running the audit.

EXAMPLES
    auditencoding
        Audit the current checkout.

    auditencoding -Root D:\temp\sample
        Audit another git working tree with this configuration.
'@ | Write-Host
    exit 0
}

# Under Windows PowerShell 5.1 an advanced script evaluates a parameter default before
# $PSScriptRoot is available to it, so the defaults are resolved here.
if ([string]::IsNullOrWhiteSpace($Root)) {
    $Root = Split-Path -Parent $PSScriptRoot
}
if ([string]::IsNullOrWhiteSpace($ConfigPath)) {
    $ConfigPath = Join-Path $PSScriptRoot 'auditencoding.json'
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [Console]::OutputEncoding

$script:AuditGeneration = 15
$script:Invariant = [System.Globalization.CultureInfo]::InvariantCulture

Write-Host "AUDITENCODING GENERATION $script:AuditGeneration" -ForegroundColor Blue

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
        throw "The encoding-audit configuration was not found: $pathFull"
    }

    try {
        $config = [System.IO.File]::ReadAllText($pathFull, [System.Text.UTF8Encoding]::new($false)) | ConvertFrom-Json
    }
    catch {
        throw "The encoding-audit configuration is not valid JSON: $pathFull`n$($_.Exception.Message)"
    }

    $problems = [System.Collections.Generic.List[string]]::new()
    foreach ($key in @('generation', 'project', 'sources.include', 'sources.excludeSegments', 'sources.skip',
                       'tabless', 'ascii', 'patterns.control', 'patterns.trail', 'patterns.mojibake')) {
        if ($null -eq (Get-ConfigNode -Document $config -Key $key)) {
            $problems.Add("missing key '$key'")
        }
    }

    if ($problems.Count -gt 0) {
        throw "The encoding-audit configuration is not valid: $pathFull`n  " + ($problems -join "`n  ")
    }

    if ([int]$config.generation -ne $script:AuditGeneration) {
        throw "The encoding-audit configuration is generation $($config.generation) but this tooling is generation $script:AuditGeneration.`nA generation names the set of checks applied, so the two must match: $pathFull"
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
    # the pathspecs without case, then excluded segments and skipped names drop out, files gone from
    # disk drop out, and the full paths sort ordinally without case.
    param(
        [Parameter(Mandatory = $true)][string]$ProjectRoot,
        [Parameter(Mandatory = $true)][string[]]$Include,
        [AllowEmptyCollection()][string[]]$Segments = @(),
        [AllowEmptyCollection()][string[]]$Skip = @()
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

    $segmentSet = [System.Collections.Generic.HashSet[string]]::new([string[]]$Segments, [System.StringComparer]::OrdinalIgnoreCase)
    $skipSet = [System.Collections.Generic.HashSet[string]]::new([string[]]$Skip, [System.StringComparer]::OrdinalIgnoreCase)
    $separator = [System.IO.Path]::DirectorySeparatorChar
    $seen = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    $files = [System.Collections.Generic.List[object]]::new()
    foreach ($line in $output.Split("`n")) {
        $relative = $line.Trim()
        if ($relative.Length -eq 0) { continue }
        $parts = $relative.Split('/')
        $excluded = $false
        foreach ($part in $parts) {
            if ($segmentSet.Contains($part)) { $excluded = $true; break }
        }
        if ($excluded -or $skipSet.Contains($parts[$parts.Length - 1])) { continue }
        $full = [System.IO.Path]::Combine($ProjectRoot, $relative.Replace('/', $separator))
        if (-not $seen.Add($full) -or -not [System.IO.File]::Exists($full)) { continue }
        $files.Add([pscustomobject]@{ Full = $full; Relative = $relative })
    }

    $byFull = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    foreach ($file in $files) { $byFull[$file.Full] = $file }
    $keys = [string[]]@($files | ForEach-Object { $_.Full })
    [System.Array]::Sort($keys, [System.StringComparer]::OrdinalIgnoreCase)
    return , [object[]]@($keys | ForEach-Object { $byFull[$_] })
}

function Get-LineAt {
    param([Parameter(Mandatory = $true)][string]$Text, [Parameter(Mandatory = $true)][int]$Index)

    $before = $Text.Substring(0, $Index)
    return $before.Length - $before.Replace("`n", '').Length + 1
}

function Find-PatternLine {
    param([Parameter(Mandatory = $true)][AllowEmptyString()][string]$Text, [Parameter(Mandatory = $true)][regex]$Pattern, [string]$Label)

    $match = $Pattern.Match($Text)
    if (-not $match.Success) { return $null }
    return '{0} at line {1}' -f $Label, (Get-LineAt -Text $Text -Index $match.Index)
}

function Find-InvalidOffset {
    # The offset of the first ill-formed sequence, as the .NET decoder reports it in the test.
    # The exception index of .NET Framework counts from an internal buffer, so the walk is done here.
    param([Parameter(Mandatory = $true)][byte[]]$Bytes)

    $index = 0
    while ($index -lt $Bytes.Length) {
        $lead = [int]$Bytes[$index]
        if ($lead -lt 0x80) { $index++; continue }
        $low = 0x80
        $high = 0xBF
        if ($lead -ge 0xC2 -and $lead -le 0xDF) { $count = 1 }
        elseif ($lead -eq 0xE0) { $count = 2; $low = 0xA0 }
        elseif ($lead -eq 0xED) { $count = 2; $high = 0x9F }
        elseif ($lead -ge 0xE1 -and $lead -le 0xEF) { $count = 2 }
        elseif ($lead -eq 0xF0) { $count = 3; $low = 0x90 }
        elseif ($lead -eq 0xF4) { $count = 3; $high = 0x8F }
        elseif ($lead -ge 0xF1 -and $lead -le 0xF3) { $count = 3 }
        else { return $index }
        for ($step = 1; $step -le $count; $step++) {
            $position = $index + $step
            if ($position -ge $Bytes.Length) { return $index }
            $trail = [int]$Bytes[$position]
            $floor = if ($step -eq 1) { $low } else { 0x80 }
            $ceiling = if ($step -eq 1) { $high } else { 0xBF }
            if ($trail -lt $floor -or $trail -gt $ceiling) { return $index }
        }
        $index += $count + 1
    }

    return $Bytes.Length
}

function Test-Suffix {
    param([Parameter(Mandatory = $true)][string]$Path, [AllowEmptyCollection()][string[]]$Suffixes)

    foreach ($suffix in $Suffixes) {
        if ($Path.EndsWith($suffix, [System.StringComparison]::OrdinalIgnoreCase)) { return $true }
    }
    return $false
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

$projectRoot = Resolve-ProjectRoot -Path $Root
$config = Read-AuditConfig -Path $ConfigPath

$include = [string[]]@($config.sources.include | ForEach-Object { [string]$_ })
$segments = [string[]]@($config.sources.excludeSegments | ForEach-Object { [string]$_ })
$skip = [string[]]@($config.sources.skip | ForEach-Object { [string]$_ })
$tabless = [string[]]@($config.tabless | ForEach-Object { [string]$_ })
$ascii = [string[]]@($config.ascii | ForEach-Object { [string]$_ })
$controlPattern = [regex]::new([string]$config.patterns.control)
$mojibakePattern = [regex]::new(([string]$config.patterns.mojibake).Replace('{trail}', [string]$config.patterns.trail))
$tabPattern = [regex]::new("`t")
$highPattern = [regex]::new('[\u0080-\u00FF]')
$mark = [string]::new([char[]]@([char]0xEF, [char]0xBB, [char]0xBF))

$strict = [System.Text.UTF8Encoding]::new($false, $true)
$loose = [System.Text.Encoding]::UTF8
$latin = [System.Text.Encoding]::GetEncoding(28591)

$files = Get-GitFiles -ProjectRoot $projectRoot -Include $include -Segments $segments -Skip $skip
if ($files.Count -eq 0) {
    throw "No tracked text file was enumerated under $projectRoot; the audit would pass vacuously."
}

$kinds = @(
    @{ Label = 'Invalid UTF-8'; Hits = [System.Collections.Generic.List[string]]::new() },
    @{ Label = 'Byte order marks'; Hits = [System.Collections.Generic.List[string]]::new() },
    @{ Label = 'Carriage returns'; Hits = [System.Collections.Generic.List[string]]::new() },
    @{ Label = 'Missing final newlines'; Hits = [System.Collections.Generic.List[string]]::new() },
    @{ Label = 'Control characters'; Hits = [System.Collections.Generic.List[string]]::new() },
    @{ Label = 'Double-encoded text'; Hits = [System.Collections.Generic.List[string]]::new() },
    @{ Label = 'Tabs in sources'; Hits = [System.Collections.Generic.List[string]]::new() },
    @{ Label = 'Non-ASCII scripts'; Hits = [System.Collections.Generic.List[string]]::new() }
)

foreach ($file in $files) {
    $bytes = [System.IO.File]::ReadAllBytes($file.Full)
    $relative = $file.Relative
    $raw = $latin.GetString($bytes)
    $text = $loose.GetString($bytes)

    $invalid = $false
    try {
        [void]$strict.GetString($bytes)
    }
    catch {
        $invalid = $true
    }
    if ($invalid) {
        $kinds[0].Hits.Add(('{0}: invalid byte sequence at offset {1}' -f $relative, (Find-InvalidOffset -Bytes $bytes)))
    }

    $offset = $raw.IndexOf($mark, [System.StringComparison]::Ordinal)
    if ($offset -ge 0) {
        $kinds[1].Hits.Add(('{0}: byte order mark at offset {1}' -f $relative, $offset))
    }

    $returns = $raw.Length - $raw.Replace("`r", '').Length
    if ($returns -gt 0) {
        $kinds[2].Hits.Add(('{0}: {1} carriage return(s)' -f $relative, $returns))
    }

    if ($bytes.Length -gt 0 -and $bytes[$bytes.Length - 1] -ne 10) {
        $kinds[3].Hits.Add(('{0}: no newline at end of file' -f $relative))
    }

    $reason = Find-PatternLine -Text $text -Pattern $controlPattern -Label 'control character'
    if ($null -ne $reason) { $kinds[4].Hits.Add("${relative}: $reason") }

    $reason = Find-PatternLine -Text $text -Pattern $mojibakePattern -Label 'double-encoded text'
    if ($null -ne $reason) { $kinds[5].Hits.Add("${relative}: $reason") }

    if (Test-Suffix -Path $relative -Suffixes $tabless) {
        $reason = Find-PatternLine -Text $text -Pattern $tabPattern -Label 'tab'
        if ($null -ne $reason) { $kinds[6].Hits.Add("${relative}: $reason") }
    }

    if (Test-Suffix -Path $relative -Suffixes $ascii) {
        $reason = Find-PatternLine -Text $raw -Pattern $highPattern -Label 'non-ASCII byte'
        if ($null -ne $reason) { $kinds[7].Hits.Add("${relative}: $reason") }
    }
}

Write-Host ('Scanned: {0} text files' -f (Format-Integer $files.Count)) -ForegroundColor DarkGray

$meanings = @{
    'Invalid UTF-8' = 'files with a byte sequence that is not UTF-8'
    'Byte order marks' = 'files with a byte order mark'
    'Carriage returns' = 'files with a carriage return'
    'Missing final newlines' = 'non-empty files not ending in a line feed'
    'Control characters' = 'files with a raw control character'
    'Double-encoded text' = 'files with UTF-8 read back through a single-byte page'
    'Tabs in sources' = 'tabless files containing a tab'
    'Non-ASCII scripts' = 'scripts with a byte outside ASCII'
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
