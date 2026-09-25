<#
.SYNOPSIS
Audits the surfaces for anything but calls and the drivers for deciding data, through the convention tests.

.DESCRIPTION
A surface is what the user sees: the veneer (Llyn.UIVeneer) for the GUI and the terminal
(Llyn.UITerminal) for the CUI. A surface member only calls a function:
no branch, no operator, no assignment, no held state, and no reach into the engine.
A driver drives its surface: the deportment (Llyn.UIDeportment) and the demeanor
(Llyn.UIDemeanor). A driver may control its medium freely, but it must not decide the
data: that stays with the Conduct gates and the engine.

Reads the project configuration from auditui.json next to this script, then performs
these actions on every run:
  1. Runs the two convention-test classes that walk the UI sources with Roslyn:
     TAuditStrict (the surfaces: Storage, Call, Engine, Reach, Trigger)
     and TAuditTruth (the drivers: the custody kinds, Treat and Taint).
  2. Reads the two Markdown reports those tests write under temp/audit.
  3. Triages every hit into a verdict with the ordered rules of triage.rules:
       violation  a real issue: a surface does more than call, or a driver decides data
       review     may be real: a person has to look at the line
       covered    repeats an issue counted at another hit, named in its why
       allow      not an issue: a driver controls its medium
     Every surface hit is a violation, since a surface has no tolerated logic.
  4. Prints a short console summary: the verdicts by kind, the violations by reason
     and the files with the most violations.
  5. Writes one Markdown report to {report.directory}\{prefix}{version}.md, which lists
     every hit with its verdict and the reason for it.

A generation names the set of checks the audit applies. auditnames, auditlines,
auditcomments and auditui share one generation number; the hand-written convention-test
settings carry it and the tests refuse a setting written at another generation.

Unlike the other audits, this script is not independent: the walk lives in the test
project, so the script runs dotnet test and reads what the tests wrote. Everything
project-specific lives in auditui.json. No external modules are required beyond the
.NET SDK the tests already need.

TRIAGE RULES
Each rule names the kinds it applies to and may add a reason, a name and a line regex,
which must all match. The line regex reads the trimmed source line the hit points at.
The first matching rule gives the verdict and its why.
A rule with "join" instead of "verdict" borrows the worst verdict of related hits:
  line    hits of the target kinds on the same file and line
  field   hits of the target kinds naming the same field
A join that finds a related hit at review or worse makes the hit covered, naming that hit.
A join that finds none falls through to the next rule.
A hit no rule matches is a review.

auditui.json shape:
  {
    "generation": 11,
    "project": "Llyn",
    "test": {
      "project": "tests/Llyn.Convention.Tests",
      "configuration": "Debug",
      "filter": "FullyQualifiedName~TAuditTruth|FullyQualifiedName~TAuditStrict"
    },
    "sources": {
      "veneer": "temp/audit/Strict-{version}.md",
      "deportment": "temp/audit/Custody-{version}.md"
    },
    "veneer": { "kinds": ["Storage", "Call", "Engine", "Reach", "Trigger"] },
    "deportment": { "kinds": ["Argument", "Guard", "Fork", "Mirror", "Mutation", "Shape", "Treat", "Taint"] },
    "triage": {
      "rules": [
        { "kinds": ["Call"], "verdict": "violation", "why": "the veneer does more than call a function" },
        { "kinds": ["Treat", "Taint"], "reason": "decides", "join": "line", "targets": ["Treat", "Taint"], "why": "condition judged at its operators" }
      ]
    },
    "report": {
      "directory": "docs-work/audit",
      "versionFile": "version.json",
      "versionKey": "current-version",
      "prefix": "UI-",
      "consoleFiles": 10
    }
  }

.PARAMETER ConfigPath
Path to the JSON configuration. Defaults to auditui.json next to this script.

.PARAMETER Configuration
Overrides test.configuration for this run: Debug or Release.

.PARAMETER NoBuild
Pass --no-build to dotnet test, reusing the last build of the test project.

.PARAMETER OutputPath
Overrides the Markdown report path for this run.

.PARAMETER Open
Open the generated report after the audit finishes.

.PARAMETER Help
Display this help and exit without running the audit. The alias -? is supported.

.EXAMPLE
auditui

.EXAMPLE
auditui -NoBuild -Open

.EXAMPLE
auditui -Configuration Release
#>
[CmdletBinding()]
param(
    [string]$ConfigPath,
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration,
    [switch]$NoBuild,
    [string]$OutputPath,
    [switch]$Open,
    [Alias('?')]
    [switch]$Help
)

if ([string]::IsNullOrWhiteSpace($ConfigPath)) {
    $ConfigPath = Join-Path $PSScriptRoot "auditui.json"
}

if ($Help) {
    @'
NAME
    auditui.ps1

SYNOPSIS
    Audit the surfaces for anything but calls and the drivers for deciding
    data, triage every hit into violation, review, covered or allow, and
    create a Markdown report.

SYNTAX
    auditui [-ConfigPath <path>] [-Configuration <Debug|Release>] [-NoBuild]
        [-OutputPath <path>] [-Open] [-Help]

CONFIGURATION
    All project-specific values live in auditui.json next to the script: the
    test project, configuration and filter, the two report files the tests
    write, the surface and driver hit kinds, the triage rules, report location,
    version file and key. Parameters below override it per run.

VERDICTS
    violation  A real issue: a surface does more than call, or a driver decides data.
    review     May be real: a person has to look at the line.
    covered    Repeats an issue counted at another hit.
    allow      Not an issue: a driver controls its medium.

    The console shows only the verdict counts, the violations by reason and
    the files with the most violations. The Markdown report lists every hit.

OPTIONS
    -ConfigPath <path>
        JSON configuration file. Defaults to .\auditui.json.

    -Configuration <Debug|Release>
        Build configuration for dotnet test. Overrides test.configuration.

    -NoBuild
        Reuse the last build of the test project.

    -OutputPath <path>
        Markdown report path. By default, the project version is used to
        create a path under report.directory.

    -Open
        Open the generated report after the audit finishes.

    -Help, -?
        Display this help and exit without running the audit.

EXAMPLES
    auditui
        Build the test project, run both audits, triage and report.

    auditui -NoBuild -Open
        Reuse the last build and open the report.

    auditui -Configuration Release
        Run against a Release build.
'@ | Write-Host
    exit 0
}

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# dotnet and git write UTF-8. A console still on the OEM code page, as one opened by the dispatcher
# without a profile is, would show every non-ASCII line garbled, so this process reads and writes
# UTF-8. Process-local: the calling console keeps its own code page.
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [Console]::OutputEncoding

$script:AuditGeneration = 11
$script:Verdicts = @('violation', 'review', 'covered', 'allow')
$script:VerdictRank = @{ 'violation' = 3; 'review' = 2; 'covered' = 1; 'allow' = 1 }
$script:VerdictColor = @{ 'violation' = 'Red'; 'review' = 'Yellow'; 'covered' = 'DarkGray'; 'allow' = 'DarkGray' }

Write-Host "AUDITUI GENERATION $script:AuditGeneration" -ForegroundColor Cyan


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
        throw "The UI-audit configuration was not found: $pathFull"
    }

    try {
        $config = Get-Content -LiteralPath $pathFull -Raw -Encoding UTF8 | ConvertFrom-Json
    }
    catch {
        throw "The UI-audit configuration is not valid JSON: $pathFull`n$($_.Exception.Message)"
    }

    $problems = [System.Collections.Generic.List[string]]::new()
    foreach ($key in @('generation', 'project',
                       'test.project', 'test.configuration', 'test.filter',
                       'sources.veneer', 'sources.deportment',
                       'veneer.kinds', 'deportment.kinds', 'triage.rules',
                       'report.directory', 'report.versionFile', 'report.versionKey', 'report.prefix', 'report.consoleFiles')) {
        if ($null -eq (Get-ConfigNode -Document $config -Key $key)) {
            $problems.Add("missing key '$key'")
        }
    }

    $index = 0
    foreach ($rule in @(Get-ConfigNode -Document $config -Key 'triage.rules')) {
        $index++
        if ($null -eq $rule) { continue }
        $names = @($rule.PSObject.Properties.Name)
        if ($names -notcontains 'kinds' -or $names -notcontains 'why') { $problems.Add("triage rule $index needs 'kinds' and 'why'") }
        if ($names -contains 'join') {
            if ([string]$rule.join -notin @('line', 'field')) { $problems.Add("triage rule $index has join '$($rule.join)', not line or field") }
            if ($names -notcontains 'targets') { $problems.Add("triage rule $index joins without 'targets'") }
        }
        elseif ($names -notcontains 'verdict' -or [string]$rule.verdict -notin $script:Verdicts) {
            $problems.Add("triage rule $index needs a verdict of violation, review or allow")
        }
        foreach ($pattern in @('reason', 'name', 'line')) {
            if ($names -contains $pattern) {
                try { [void][regex]::new([string]$rule.$pattern) }
                catch { $problems.Add("triage rule $index has an invalid $pattern regex: $($_.Exception.Message)") }
            }
        }
    }

    if ($problems.Count -gt 0) {
        throw "The UI-audit configuration is not valid: $pathFull`n  " + ($problems -join "`n  ")
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
    throw "The UI-audit configuration is generation $($config.generation) but this tooling is generation $script:AuditGeneration.`nA generation names the set of checks applied, so the two must match: $ConfigPath"
}

if (-not $PSBoundParameters.ContainsKey('Configuration')) { $Configuration = [string]$config.test.configuration }

$testProjectFull = Join-AuditPath -Root $repoRootFull -Relative $config.test.project
$testFilter = [string]$config.test.filter
$veneerKinds = @($config.veneer.kinds | ForEach-Object { [string]$_ })
$deportmentKinds = @($config.deportment.kinds | ForEach-Object { [string]$_ })
$triageRules = @($config.triage.rules)
$consoleFiles = [int]$config.report.consoleFiles
$reportDirectoryFull = Join-AuditPath -Root $repoRootFull -Relative $config.report.directory
$versionPathFull = Join-AuditPath -Root $repoRootFull -Relative $config.report.versionFile
$versionKey = [string]$config.report.versionKey
$reportPrefix = [string]$config.report.prefix

if (-not [System.IO.Directory]::Exists($testProjectFull)) {
    throw "The test project directory was not found: $testProjectFull"
}

function ConvertTo-MarkdownCell {
    param([AllowNull()][object]$Value)

    return ([string]$Value).Replace('|', '\|').Replace("`r`n", '<br>').Replace("`n", '<br>').Replace("`r", '<br>')
}

function Format-Integer {
    param([long]$Value)

    return $Value.ToString("N0", [System.Globalization.CultureInfo]::InvariantCulture)
}

function Format-Percent {
    param([double]$Value)

    return $Value.ToString("0.0", [System.Globalization.CultureInfo]::InvariantCulture) + " %"
}

function Write-SectionTitle {
    param([Parameter(Mandatory = $true)][string]$Text)

    Write-Host ""
    Write-Host $Text -ForegroundColor Cyan
    Write-Host ('-' * $Text.Length) -ForegroundColor DarkGray
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

$veneerPathFull = Join-AuditPath -Root $repoRootFull -Relative ([string]$config.sources.veneer).Replace('{version}', $version)
$deportmentPathFull = Join-AuditPath -Root $repoRootFull -Relative ([string]$config.sources.deportment).Replace('{version}', $version)

# The walk lives in the convention tests. Run only the two audit classes and let them write
# their reports; a failing fact (once enforced) is reported but never stops the audit.
$generatedAt = Get-Date
foreach ($stale in @($veneerPathFull, $deportmentPathFull)) {
    if ([System.IO.File]::Exists($stale)) { [System.IO.File]::Delete($stale) }
}

$testArguments = [System.Collections.Generic.List[string]]::new()
[void]$testArguments.Add('test')
[void]$testArguments.Add($testProjectFull)
[void]$testArguments.Add('--configuration')
[void]$testArguments.Add($Configuration)
[void]$testArguments.Add('--filter')
[void]$testArguments.Add($testFilter)
[void]$testArguments.Add('--nologo')
[void]$testArguments.Add('--verbosity')
[void]$testArguments.Add('quiet')
if ($NoBuild) { [void]$testArguments.Add('--no-build') }

Write-SectionTitle "Convention tests"
Write-Host "  dotnet $($testArguments -join ' ')" -ForegroundColor DarkGray
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$testOutput = & dotnet @testArguments 2>&1 | ForEach-Object { [string]$_ }
$testExit = $LASTEXITCODE
$stopwatch.Stop()

$testColor = if ($testExit -eq 0) { 'Green' } else { 'Red' }
$testSummary = @($testOutput | Where-Object { $_ -match '^\s*(Passed!|Failed!|\uD1B5\uACFC!|\uC131\uACF5!|\uC2E4\uD328!|Test summary|Tests passed|Tests failed|error )' })
foreach ($line in $testSummary) { Write-Host "  $($line.Trim())" -ForegroundColor $testColor }
Write-Host ("  Exit code {0} after {1:0.0} s." -f $testExit, $stopwatch.Elapsed.TotalSeconds) -ForegroundColor $testColor

$missingReports = @(@($veneerPathFull, $deportmentPathFull) | Where-Object { -not [System.IO.File]::Exists($_) })
if ($missingReports.Count -gt 0) {
    foreach ($line in $testOutput) { Write-Host "  $line" -ForegroundColor DarkGray }
    throw "The tests wrote no report at:`n  " + ($missingReports -join "`n  ")
}

# Each test report is Markdown with a fixed shape: `- Key: value` bullets, one two- or
# four-column table, then `## Kind` sections of `- ``path:line`` ``name`` reason` hits.
function Read-TestReport {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Audit
    )

    $bullets = @{}
    $rows = [System.Collections.Generic.List[object]]::new()
    $hits = [System.Collections.Generic.List[object]]::new()
    $section = ''
    foreach ($line in [System.IO.File]::ReadAllLines($Path)) {
        if ($line -match '^## (.+)$') { $section = $Matches[1].Trim(); continue }
        if ($line -match '^- ([A-Za-z ]+): (.*)$' -and $section -eq '') { $bullets[$Matches[1]] = $Matches[2].Trim(); continue }
        if ($line -match '^- `(.+?):(\d+)` `(.*?)` (.*)$') {
            $reason = $Matches[4]
            $waived = $reason.EndsWith(' (waived)')
            if ($waived) { $reason = $reason.Substring(0, $reason.Length - ' (waived)'.Length) }
            $hits.Add([pscustomobject]@{
                Audit = $Audit; Kind = $section; Path = $Matches[1]; Line = [int]$Matches[2]; Name = $Matches[3]
                Reason = $reason; Waived = $waived; Verdict = $null; Why = ''; Stage = 0
            })
            continue
        }
        if ($line -match '^\|(.+)\|$') {
            $cells = @($Matches[1] -split '\|' | ForEach-Object { $_.Trim().Trim('`') })
            if ($cells.Count -gt 0 -and $cells[0] -notmatch '^-+$' -and $cells[0] -notin @('Field', 'Class')) { $rows.Add($cells) }
        }
    }

    return [pscustomobject]@{ Bullets = $bullets; Rows = @($rows); Hits = @($hits) }
}

$veneer = Read-TestReport -Path $veneerPathFull -Audit 'Veneer'
$deportment = Read-TestReport -Path $deportmentPathFull -Audit 'Deportment'
$allHits = @($veneer.Hits) + @($deportment.Hits)

# The source line of every hit, so a rule can read the code the walker pointed at.
$sourceLines = @{}
foreach ($hit in $allHits) {
    if (-not $sourceLines.ContainsKey($hit.Path)) {
        $sourceFull = Join-AuditPath -Root $repoRootFull -Relative $hit.Path
        $sourceLines[$hit.Path] = if ([System.IO.File]::Exists($sourceFull)) { [System.IO.File]::ReadAllLines($sourceFull) } else { @() }
    }
    $lines = $sourceLines[$hit.Path]
    $text = if ($hit.Line -ge 1 -and $hit.Line -le $lines.Count) { $lines[$hit.Line - 1].Trim() } else { '' }
    $hit | Add-Member -NotePropertyName Text -NotePropertyValue $text
}

function Get-Count {
    param([Parameter(Mandatory = $true)]$Report, [Parameter(Mandatory = $true)][string]$Key)

    if ($Report.Bullets.ContainsKey($Key)) { return [int]$Report.Bullets[$Key] }
    return 0
}

$veneerCount = Get-Count -Report $veneer -Key 'Veneer types'

# Triage. A rule applies when its kinds hold the hit's kind and its reason, name and line
# regexes, when given, all match. Plain rules settle a hit at once. A join rule defers the hit: line
# joins run in the second stage and field joins in the third, so every join reads verdicts
# already settled. The first stage settles every hit whose first matching rule is plain.
function Test-TriageRule {
    param([Parameter(Mandatory = $true)]$Rule, [Parameter(Mandatory = $true)]$Hit)

    if (@($Rule.kinds) -notcontains $Hit.Kind) { return $false }
    $names = @($Rule.PSObject.Properties.Name)
    if ($names -contains 'reason' -and $Hit.Reason -notmatch [string]$Rule.reason) { return $false }
    if ($names -contains 'name' -and $Hit.Name -notmatch [string]$Rule.name) { return $false }
    if ($names -contains 'line' -and $Hit.Text -notmatch [string]$Rule.line) { return $false }
    return $true
}

$hitsByPath = @{}
foreach ($hit in $allHits) {
    if (-not $hitsByPath.ContainsKey($hit.Path)) { $hitsByPath[$hit.Path] = [System.Collections.Generic.List[object]]::new() }
    $hitsByPath[$hit.Path].Add($hit)
}


function Find-JoinTarget {
    param([Parameter(Mandatory = $true)]$Rule, [Parameter(Mandatory = $true)]$Hit)

    $targets = @($Rule.targets)
    $pool = switch ([string]$Rule.join) {
        'line' { @($hitsByPath[$Hit.Path] | Where-Object { $_.Line -eq $Hit.Line }) }
        'field' { @($allHits | Where-Object { $_.Name -eq $Hit.Name }) }
    }

    $worst = $null
    foreach ($candidate in $pool) {
        if ([object]::ReferenceEquals($candidate, $Hit) -or $targets -notcontains $candidate.Kind -or $null -eq $candidate.Verdict) { continue }
        if ($script:VerdictRank[$candidate.Verdict] -lt $script:VerdictRank['review']) { continue }
        if ($null -eq $worst -or $script:VerdictRank[$candidate.Verdict] -gt $script:VerdictRank[$worst.Verdict]) { $worst = $candidate }
    }

    return $worst
}

function Set-HitVerdict {
    param([Parameter(Mandatory = $true)]$Hit, [int]$Stage)

    foreach ($rule in $triageRules) {
        if (-not (Test-TriageRule -Rule $rule -Hit $Hit)) { continue }
        $join = if (@($rule.PSObject.Properties.Name) -contains 'join') { [string]$rule.join } else { '' }
        if ($join -eq '') {
            $Hit.Verdict = [string]$rule.verdict
            $Hit.Why = [string]$rule.why
            return
        }

        $ruleStage = if ($join -eq 'line') { 2 } else { 3 }
        if ($Stage -lt $ruleStage) {
            $Hit.Stage = $ruleStage
            return
        }
        if ($Stage -gt $ruleStage) { continue }

        $target = Find-JoinTarget -Rule $rule -Hit $Hit
        if ($null -ne $target) {
            $Hit.Verdict = 'covered'
            $Hit.Why = "{0}: {1} {2} at line {3}, {4}" -f [string]$rule.why, $target.Verdict, $target.Kind, $target.Line, $target.Why
            return
        }
    }

    $Hit.Verdict = 'review'
    $Hit.Why = 'no triage rule matched'
}

foreach ($hit in $allHits) { Set-HitVerdict -Hit $hit -Stage 1 }
foreach ($stage in @(2, 3)) {
    foreach ($hit in @($allHits | Where-Object { $null -eq $_.Verdict -and $_.Stage -eq $stage })) { Set-HitVerdict -Hit $hit -Stage $stage }
}

$kinds = @($veneerKinds + $deportmentKinds)
$kindIndex = @{}
for ($i = 0; $i -lt $kinds.Count; $i++) { $kindIndex[$kinds[$i]] = $i }
$sortKey = @(
    @{ Expression = { $kindIndex[$_.Kind] } },
    @{ Expression = { $_.Path } },
    @{ Expression = { $_.Line } }
)
$byVerdict = @{}
foreach ($verdict in $script:Verdicts) { $byVerdict[$verdict] = @($allHits | Where-Object { $_.Verdict -eq $verdict } | Sort-Object -Property $sortKey) }

function Get-VerdictCount {
    param([string]$Kind, [Parameter(Mandatory = $true)][string]$Verdict)

    return @($byVerdict[$Verdict] | Where-Object { $null -eq $Kind -or $Kind -eq '' -or $_.Kind -eq $Kind }).Count
}

function Get-WhyGroup {
    param([Parameter(Mandatory = $true)][AllowEmptyCollection()][object[]]$Items)

    return @($Items |
        Group-Object -Property @{ Expression = { ($_.Why -split ': ', 2)[0] } } |
        Sort-Object -Property @{ Expression = { $_.Count }; Descending = $true }, Name)
}

# Console output: counts only. The report carries every hit.
Write-SectionTitle "Verdicts by kind"
$kindWidth = [Math]::Max(16, ($kinds | ForEach-Object { "$_".Length } | Measure-Object -Maximum).Maximum + 10)
Write-Host ("  {0}{1,10}{2,8}{3,9}{4,8}{5,8}" -f 'Kind'.PadRight($kindWidth), 'Violation', 'Review', 'Covered', 'Allow', 'Hits') -ForegroundColor Green
foreach ($kind in $kinds) {
    $audit = if ($veneerKinds -contains $kind) { 'Veneer' } else { 'Deportment' }
    $violation = Get-VerdictCount -Kind $kind -Verdict 'violation'
    $review = Get-VerdictCount -Kind $kind -Verdict 'review'
    $covered = Get-VerdictCount -Kind $kind -Verdict 'covered'
    $allow = Get-VerdictCount -Kind $kind -Verdict 'allow'
    $color = if ($violation -gt 0) { 'Red' } elseif ($review -gt 0) { 'Yellow' } else { 'Gray' }
    Write-Host ("  {0}{1,10}{2,8}{3,9}{4,8}{5,8}" -f "$audit $kind".PadRight($kindWidth), $violation, $review, $covered, $allow, ($violation + $review + $covered + $allow)) -ForegroundColor $color
}
Write-Host ("  {0}{1,10}{2,8}{3,9}{4,8}{5,8}" -f 'Total'.PadRight($kindWidth), $byVerdict['violation'].Count, $byVerdict['review'].Count, $byVerdict['covered'].Count, $byVerdict['allow'].Count, $allHits.Count) -ForegroundColor Cyan

foreach ($verdict in @('violation', 'review')) {
    $groups = Get-WhyGroup -Items $byVerdict[$verdict]
    $title = if ($verdict -eq 'violation') { 'Violations by reason' } else { 'Review by reason' }
    Write-SectionTitle "$title ($($byVerdict[$verdict].Count))"
    if ($groups.Count -eq 0) { Write-Host "  None." -ForegroundColor DarkGray; continue }
    foreach ($group in $groups) {
        Write-Host ("  {0,5}  {1}" -f $group.Count, $group.Name) -ForegroundColor $script:VerdictColor[$verdict]
    }
}

$violationFiles = @($byVerdict['violation'] | Group-Object -Property Path | Sort-Object -Property @{ Expression = { $_.Count }; Descending = $true }, Name)
Write-SectionTitle "Files with the most violations ($($violationFiles.Count) files)"
if ($violationFiles.Count -eq 0) { Write-Host "  None." -ForegroundColor DarkGray }
foreach ($group in @($violationFiles | Select-Object -First $consoleFiles)) {
    Write-Host ("  {0,5}  {1}" -f $group.Count, $group.Name) -ForegroundColor Red
}
if ($violationFiles.Count -gt $consoleFiles) {
    Write-Host ("  ... and {0} more files. The Markdown report lists every hit." -f ($violationFiles.Count - $consoleFiles)) -ForegroundColor DarkGray
}

# Markdown output.
$report = [System.Text.StringBuilder]::new()
[void]$report.AppendLine("# UI report - $version")
[void]$report.AppendLine()
[void]$report.AppendLine("- Generated: $($generatedAt.ToString('yyyy-MM-dd HH:mm:ss zzz'))")
[void]$report.AppendLine("- Generation: $script:AuditGeneration")
[void]$report.AppendLine("- Test project: ``$(ConvertTo-MarkdownCell $config.test.project)`` ($Configuration)")
[void]$report.AppendLine("- Test filter: ``$(ConvertTo-MarkdownCell $testFilter)``")
[void]$report.AppendLine("- Test exit code: $testExit")
[void]$report.AppendLine("- Surface enforced: $($veneer.Bullets['Enforced'])")
[void]$report.AppendLine("- Driver enforced: $($deportment.Bullets['Enforced'])")
[void]$report.AppendLine("- Violations: $(Format-Integer $byVerdict['violation'].Count)")
[void]$report.AppendLine("- Review: $(Format-Integer $byVerdict['review'].Count)")
[void]$report.AppendLine("- Covered: $(Format-Integer $byVerdict['covered'].Count)")
[void]$report.AppendLine("- Allowed: $(Format-Integer $byVerdict['allow'].Count)")
[void]$report.AppendLine()
[void]$report.AppendLine("A surface member only calls a function, and every surface hit is a violation.")
[void]$report.AppendLine("A driver may control its medium but must not decide the data.")
[void]$report.AppendLine("A review may be real and needs a person to look at the line.")
[void]$report.AppendLine("A covered hit repeats an issue counted at another hit, which its why names.")
[void]$report.AppendLine("An allowed hit is a driver controlling its medium.")
[void]$report.AppendLine("The triage rules live in ``scripts/auditui.json``.")
[void]$report.AppendLine()
[void]$report.AppendLine("## Verdicts by kind")
[void]$report.AppendLine()
[void]$report.AppendLine("| Audit | Kind | Violation | Review | Covered | Allow | Hits |")
[void]$report.AppendLine("|-------|------|----------:|-------:|--------:|------:|-----:|")
foreach ($kind in $kinds) {
    $audit = if ($veneerKinds -contains $kind) { 'Veneer' } else { 'Deportment' }
    $violation = Get-VerdictCount -Kind $kind -Verdict 'violation'
    $review = Get-VerdictCount -Kind $kind -Verdict 'review'
    $covered = Get-VerdictCount -Kind $kind -Verdict 'covered'
    $allow = Get-VerdictCount -Kind $kind -Verdict 'allow'
    [void]$report.AppendLine("| $audit | $kind | $(Format-Integer $violation) | $(Format-Integer $review) | $(Format-Integer $covered) | $(Format-Integer $allow) | $(Format-Integer ($violation + $review + $covered + $allow)) |")
}
[void]$report.AppendLine("| | Total | $(Format-Integer $byVerdict['violation'].Count) | $(Format-Integer $byVerdict['review'].Count) | $(Format-Integer $byVerdict['covered'].Count) | $(Format-Integer $byVerdict['allow'].Count) | $(Format-Integer $allHits.Count) |")

function Add-VerdictSection {
    param(
        [Parameter(Mandatory = $true)][string]$Title,
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][object[]]$Items
    )

    [void]$report.AppendLine()
    [void]$report.AppendLine("## $Title ($(Format-Integer $Items.Count))")
    [void]$report.AppendLine()
    if ($Items.Count -eq 0) { [void]$report.AppendLine("None."); return }

    [void]$report.AppendLine("| Reason | Hits |")
    [void]$report.AppendLine("|--------|-----:|")
    foreach ($group in (Get-WhyGroup -Items $Items)) {
        [void]$report.AppendLine("| $(ConvertTo-MarkdownCell $group.Name) | $(Format-Integer $group.Count) |")
    }
    [void]$report.AppendLine()
    [void]$report.AppendLine("| Kind | File | Line | Name | Code | Found | Why |")
    [void]$report.AppendLine("|------|------|-----:|------|------|-------|-----|")
    foreach ($item in $Items) {
        $found = if ($item.Waived) { "$($item.Reason) (waived)" } else { $item.Reason }
        [void]$report.AppendLine("| $($item.Kind) | $(ConvertTo-MarkdownCell $item.Path) | $(Format-Integer $item.Line) | ``$(ConvertTo-MarkdownCell $item.Name)`` | ``$(ConvertTo-MarkdownCell $item.Text)`` | $(ConvertTo-MarkdownCell $found) | $(ConvertTo-MarkdownCell $item.Why) |")
    }
}

Add-VerdictSection -Title 'Violations' -Items $byVerdict['violation']
Add-VerdictSection -Title 'Review' -Items $byVerdict['review']
Add-VerdictSection -Title 'Covered' -Items $byVerdict['covered']
Add-VerdictSection -Title 'Allowed' -Items $byVerdict['allow']

[void]$report.AppendLine()
[void]$report.AppendLine("## Deportment: fields by hit count")
[void]$report.AppendLine()
[void]$report.AppendLine("A custody hit is a deportment field whose value reaches an engine request, a field with an engine writer and a deportment writer, or a field holding engine state.")
[void]$report.AppendLine()
if ($deportment.Rows.Count -eq 0) { [void]$report.AppendLine("None.") }
else {
    [void]$report.AppendLine("| Field | Hits | Share |")
    [void]$report.AppendLine("|-------|-----:|------:|")
    $fieldTotal = 0
    foreach ($row in $deportment.Rows) { $fieldTotal += [int]$row[1] }
    foreach ($row in $deportment.Rows) {
        $share = if ($fieldTotal -gt 0) { ([int]$row[1] / [double]$fieldTotal) * 100.0 } else { 0.0 }
        [void]$report.AppendLine("| ``$(ConvertTo-MarkdownCell $row[0])`` | $(Format-Integer ([int]$row[1])) | $(Format-Percent $share) |")
    }
}
[void]$report.AppendLine()
[void]$report.AppendLine("## Veneer: types by member ($(Format-Integer $veneerCount))")
[void]$report.AppendLine()
[void]$report.AppendLine("A veneer type is any type declared under the veneer include. Storage is a field, an auto-property or a primary constructor parameter, Call a line that is not a plain call, Engine a line naming an engine symbol.")
[void]$report.AppendLine()
if ($veneer.Rows.Count -eq 0) { [void]$report.AppendLine("None.") }
else {
    [void]$report.AppendLine("| Type | Storage | Call | Engine | Total |")
    [void]$report.AppendLine("|------|--------:|-----:|-------:|------:|")
    foreach ($row in @($veneer.Rows | Sort-Object -Property @{ Expression = { [int]$_[1] + [int]$_[2] + [int]$_[3] }; Descending = $true }, @{ Expression = { $_[0] } })) {
        $total = [int]$row[1] + [int]$row[2] + [int]$row[3]
        [void]$report.AppendLine("| ``$(ConvertTo-MarkdownCell $row[0])`` | $(Format-Integer ([int]$row[1])) | $(Format-Integer ([int]$row[2])) | $(Format-Integer ([int]$row[3])) | $(Format-Integer $total) |")
    }
}

[System.IO.File]::WriteAllText($outputPathFull, ($report.ToString() -replace "`r`n", "`n"), [System.Text.UTF8Encoding]::new($false))
Write-Host ""
Write-Host ("Violations: {0}, review: {1}, covered: {2}, allowed: {3}, of {4} hit(s)." -f (Format-Integer $byVerdict['violation'].Count), (Format-Integer $byVerdict['review'].Count), (Format-Integer $byVerdict['covered'].Count), (Format-Integer $byVerdict['allow'].Count), (Format-Integer $allHits.Count)) -ForegroundColor $(if ($byVerdict['violation'].Count -gt 0) { 'Red' } else { 'Green' })
Write-Host "Markdown report: $outputPathFull" -ForegroundColor Green

if ($testExit -ne 0) {
    Write-Warning "dotnet test returned $testExit. An enforced audit has hits over or under its ceiling, or the test project did not build."
}

if ($Open) {
    Start-Process -FilePath $outputPathFull
}

return
