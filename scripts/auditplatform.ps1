<#
.SYNOPSIS
Audit the platform axis of the source and report every project that breaks the portable and twin split.

.DESCRIPTION
Platform is a second axis that crosses every layer. A layer splits into a portable half that
targets the portable framework alone and holds no Windows code, and a Windows twin that holds
only that layer's Windows adaptation. This audit reads the platform table from auditplatform.json,
reads every project file under the configured root, and reports:

  Unmapped   a project on disk that the table does not name
  Absent     a project the table names that is not on disk
  Framework  a portable project not targeting exactly the portable framework, or a twin not
             targeting the Windows framework
  Reference  a portable project that references a twin or any Windows project
  Column     a twin that references a project outside its own column: its portable half and
             the other twins of that half
  Analyzer   a portable project where the platform analyzer rule is not an error
  Windows    a portable project that enables a Windows property, references a Windows package,
             or holds a source line that names a Windows API
  Empty      a twin that holds no source file

The host may reference every project, so its edges are never reported. The layer chain itself is
the structure audit's concern and is not repeated here.

The report is written to the configured report folder as {prefix}{version}.md. Git is required.
No project source is modified.

This script carries no project-specific value of its own. Everything a project chooses - the
table, the frameworks, the analyzer rule, the Windows markers - lives in auditplatform.json.

.PARAMETER Root
Project root to audit. Defaults to the directory containing this script's parent.

.PARAMETER ReportDirectory
Overrides report.directory for this run. Relative paths resolve against the project root.

.PARAMETER Open
Open the report after the audit finishes.

.PARAMETER NoPause
Do not stop at each console page. Paging is also off when output or input is redirected.

.PARAMETER Help
Display this help and exit. The alias -? is supported.

.EXAMPLE
auditplatform
Audit the current checkout.

.EXAMPLE
auditplatform -ReportDirectory D:\temp\audit -Open
Write the report elsewhere and open it.
#>
#requires -Version 5.1
# AUDITPLATFORM GENERATION 11 - auditplatform.ps1.
# A generation names the set of checks the audit family applies. Every audit script shares one
# generation number with the convention-test settings, and each refuses a configuration written at
# another generation. Raise it only when the audited outcome changes, for the whole family at once.
# Generation 11 reads the project files and the portable sources and reports eight kinds.
[CmdletBinding()]
param(
    [string]$Root,
    [string]$ReportDirectory,
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
    auditplatform.ps1

SYNOPSIS
    Report every project that breaks the portable and twin split.

SYNTAX
    auditplatform [-Root <path>] [-ReportDirectory <path>] [-Open] [-NoPause] [-Help]

OPTIONS
    -Root <path>             Project root. Defaults to the parent of the scripts folder.
    -ReportDirectory <path>  Overrides report.directory for this run.
    -Open                    Open the report when done.
    -NoPause                 No console paging.
    -Help                    Show this help.

KINDS
    Unmapped, Absent, Framework, Reference, Column, Analyzer, Windows, Empty.
    See the script header for definitions.
'@ | Write-Host
    exit 0
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$script:AuditGeneration = 11
$script:AuditKinds = @('Unmapped', 'Absent', 'Framework', 'Reference', 'Column', 'Analyzer', 'Windows', 'Empty')

[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [Console]::OutputEncoding

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

Write-AuditLine "AUDITPLATFORM GENERATION $script:AuditGeneration" -ForegroundColor Cyan
$script:PathSeparators = [char[]]@([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar)

function Resolve-ProjectRoot {
    param([string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        throw 'The project root is empty.'
    }

    $item = Get-Item -LiteralPath $Path -ErrorAction Stop
    if (-not $item.PSIsContainer) {
        throw "The project root is not a directory: $Path"
    }

    return $item.FullName.TrimEnd($script:PathSeparators)
}

function Read-AuditConfig {
    param([string]$ConfigPath)

    if (-not (Test-Path -LiteralPath $ConfigPath -PathType Leaf)) {
        throw "The audit configuration was not found: $ConfigPath"
    }

    try {
        $config = Get-Content -LiteralPath $ConfigPath -Raw -Encoding UTF8 | ConvertFrom-Json
    }
    catch {
        throw "The audit configuration is not valid JSON: $ConfigPath`n$($_.Exception.Message)"
    }

    $required = @(
        'generation', 'project',
        'projects.root', 'projects.table',
        'framework.portable', 'framework.twin',
        'analyzer.rule',
        'windows.properties', 'windows.packages', 'windows.patterns',
        'sources.extensions', 'sources.excludeSegments', 'sources.excludeSuffixes',
        'report.directory', 'report.versionFile', 'report.versionKey', 'report.prefix'
    )

    foreach ($key in $required) {
        $node = $config
        foreach ($segment in $key.Split('.')) {
            if ($null -eq $node -or -not ($node.PSObject.Properties.Name -contains $segment)) {
                throw "The audit configuration has no key '$key': $ConfigPath"
            }
            $node = $node.$segment
        }
    }

    if ([int]$config.generation -ne $script:AuditGeneration) {
        throw "The audit configuration is generation $($config.generation); this script is generation $script:AuditGeneration."
    }

    $names = @{}
    foreach ($row in @($config.projects.table)) {
        $name = [string]$row.name
        $role = [string]$row.role
        if ([string]::IsNullOrWhiteSpace($name)) {
            throw "A platform table row has no name: $ConfigPath"
        }
        if ($names.ContainsKey($name)) {
            throw "The platform table names '$name' twice: $ConfigPath"
        }
        if (@('portable', 'twin', 'host') -notcontains $role) {
            throw "The platform table gives '$name' the unknown role '$role': $ConfigPath"
        }
        $names[$name] = $row
    }

    foreach ($row in @($config.projects.table)) {
        if ([string]$row.role -ne 'twin') {
            continue
        }
        $half = if ($row.PSObject.Properties.Name -contains 'half') { [string]$row.half } else { '' }
        if (-not $names.ContainsKey($half) -or [string]$names[$half].role -ne 'portable') {
            throw "The twin '$($row.name)' names no portable half in the platform table: $ConfigPath"
        }
    }

    return $config
}

function Read-ProjectVersion {
    param([string]$ProjectRoot, $Config)

    $versionPath = Join-Path $ProjectRoot $Config.report.versionFile
    if (-not (Test-Path -LiteralPath $versionPath -PathType Leaf)) {
        throw "Version file was not found: $versionPath"
    }

    try {
        $versionData = Get-Content -LiteralPath $versionPath -Raw -Encoding UTF8 | ConvertFrom-Json
    }
    catch {
        throw "Version file is not valid JSON: $versionPath`n$($_.Exception.Message)"
    }

    $version = [string]$versionData.($Config.report.versionKey)
    if ($version -notmatch '^\d+\.\d+\.\d+$') {
        throw "Version must contain three numeric components: '$version'"
    }

    return $version
}

function Test-ExcludedRelativePath {
    param([string]$RelativePath, $Config)

    $segments = $RelativePath -split '[\\/]'
    foreach ($segment in $segments) {
        if ($Config.sources.excludeSegments -contains $segment) {
            return $true
        }
    }

    $fileName = $segments[$segments.Length - 1]
    $extension = [System.IO.Path]::GetExtension($fileName)
    if ($extension -ne '.csproj' -and -not ($Config.sources.extensions -contains $extension)) {
        return $true
    }

    foreach ($suffix in $Config.sources.excludeSuffixes) {
        if ($fileName.EndsWith($suffix, [System.StringComparison]::OrdinalIgnoreCase)) {
            return $true
        }
    }

    return $false
}

function Get-TrackedFiles {
    param([string]$ProjectRoot, $Config)

    $git = Get-Command git -ErrorAction SilentlyContinue
    if ($null -eq $git) {
        throw 'Git is required to enumerate project files, but git was not found on PATH.'
    }

    if (-not (Test-Path -LiteralPath (Join-Path $ProjectRoot '.git'))) {
        throw "The project root is not a Git working tree: $ProjectRoot"
    }

    $lsArguments = @('-c', 'core.quotePath=false', '-C', $ProjectRoot,
        'ls-files', '--cached', '--others', '--exclude-standard', '--', [string]$Config.projects.root)
    $gitOutput = & $git.Source @lsArguments 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Git could not enumerate project files.`n$($gitOutput -join [Environment]::NewLine)"
    }

    $files = New-Object 'System.Collections.Generic.List[string]'
    foreach ($entry in $gitOutput) {
        $relativePath = ([string]$entry).Trim()
        if ([string]::IsNullOrWhiteSpace($relativePath) -or (Test-ExcludedRelativePath -RelativePath $relativePath -Config $Config)) {
            continue
        }

        $fullPath = Join-Path $ProjectRoot ($relativePath.Replace([char]'/', [System.IO.Path]::DirectorySeparatorChar))
        if (Test-Path -LiteralPath $fullPath -PathType Leaf) {
            $files.Add($relativePath)
        }
    }

    return @($files.ToArray() | Sort-Object -Unique)
}

function Read-XmlText {
    param([string]$Path)

    [xml]$document = Get-Content -LiteralPath $Path -Raw -Encoding UTF8
    return $document
}

function Get-XmlValues {
    param([xml]$Document, [string]$Element)

    return @($Document.SelectNodes("//*[local-name()='$Element']") | ForEach-Object { [string]$_.InnerText.Trim() })
}

function Get-XmlIncludes {
    param([xml]$Document, [string]$Element)

    return @($Document.SelectNodes("//*[local-name()='$Element']") |
        ForEach-Object { [string]$_.GetAttribute('Include') } |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
}

function Get-ImportChain {
    param([string]$ProjectRoot, [string]$ProjectFolder)

    $chain = New-Object 'System.Collections.Generic.List[string]'
    $folder = $ProjectFolder
    while ($true) {
        foreach ($name in @('Directory.Build.props', 'Directory.Build.targets')) {
            $candidate = Join-Path $folder $name
            if (Test-Path -LiteralPath $candidate -PathType Leaf) {
                $chain.Add($candidate)
            }
        }
        if ($folder.TrimEnd($script:PathSeparators) -eq $ProjectRoot) {
            break
        }
        $parent = Split-Path -Parent $folder
        if ([string]::IsNullOrWhiteSpace($parent) -or $parent -eq $folder) {
            break
        }
        $folder = $parent
    }

    return @($chain.ToArray())
}

function Get-ConfigChain {
    param([string]$ProjectRoot, [string]$ProjectFolder)

    $chain = New-Object 'System.Collections.Generic.List[string]'
    $folder = $ProjectFolder
    while ($true) {
        foreach ($name in @('.editorconfig', '.globalconfig')) {
            $candidate = Join-Path $folder $name
            if (Test-Path -LiteralPath $candidate -PathType Leaf) {
                $chain.Add($candidate)
            }
        }
        if ($folder.TrimEnd($script:PathSeparators) -eq $ProjectRoot) {
            break
        }
        $parent = Split-Path -Parent $folder
        if ([string]::IsNullOrWhiteSpace($parent) -or $parent -eq $folder) {
            break
        }
        $folder = $parent
    }

    return @($chain.ToArray())
}

function Test-AnalyzerError {
    param([string]$ProjectRoot, [string]$ProjectPath, [string]$Rule)

    $folder = Split-Path -Parent $ProjectPath
    $asError = $false
    $allAsErrors = $false
    $notAsError = $false

    foreach ($path in @(@(Get-ImportChain -ProjectRoot $ProjectRoot -ProjectFolder $folder) + $ProjectPath)) {
        $document = Read-XmlText -Path $path
        foreach ($value in (Get-XmlValues -Document $document -Element 'WarningsAsErrors')) {
            if (($value -split '[;,\s]+') -contains $Rule) {
                $asError = $true
            }
        }
        foreach ($value in (Get-XmlValues -Document $document -Element 'WarningsNotAsErrors')) {
            if (($value -split '[;,\s]+') -contains $Rule) {
                $notAsError = $true
            }
        }
        foreach ($value in (Get-XmlValues -Document $document -Element 'TreatWarningsAsErrors')) {
            if ($value -eq 'true') {
                $allAsErrors = $true
            }
        }
    }

    $pattern = '^\s*dotnet_diagnostic\.' + [regex]::Escape($Rule) + '\.severity\s*=\s*(?<level>\w+)'
    foreach ($path in (Get-ConfigChain -ProjectRoot $ProjectRoot -ProjectFolder $folder)) {
        foreach ($line in [System.IO.File]::ReadAllLines($path)) {
            $match = [regex]::Match($line, $pattern)
            if ($match.Success) {
                return $match.Groups['level'].Value -eq 'error'
            }
        }
    }

    return $asError -or ($allAsErrors -and -not $notAsError)
}

function Read-ProjectFacts {
    param([string]$ProjectRoot, [string]$RelativePath)

    $fullPath = Join-Path $ProjectRoot ($RelativePath.Replace([char]'/', [System.IO.Path]::DirectorySeparatorChar))
    $document = Read-XmlText -Path $fullPath
    $frameworks = @(@(Get-XmlValues -Document $document -Element 'TargetFramework') +
        @(Get-XmlValues -Document $document -Element 'TargetFrameworks') |
        ForEach-Object { $_ -split ';' } |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    $references = @(Get-XmlIncludes -Document $document -Element 'ProjectReference' |
        ForEach-Object { [System.IO.Path]::GetFileNameWithoutExtension(($_ -split '[\\/]')[-1]) } |
        Sort-Object -Unique)
    $packages = @(Get-XmlIncludes -Document $document -Element 'PackageReference' | Sort-Object -Unique)

    return [pscustomobject]@{
        Name       = [System.IO.Path]::GetFileNameWithoutExtension($RelativePath)
        Path       = $RelativePath
        FullPath   = $fullPath
        Folder     = [System.IO.Path]::GetDirectoryName($RelativePath).Replace('\', '/')
        Document   = $document
        Frameworks = $frameworks
        References = $references
        Packages   = $packages
    }
}

$script:Hits = New-Object 'System.Collections.Generic.List[object]'

function Add-AuditHit {
    param([string]$Kind, [string]$Project, [string]$Path, [int]$Line, [string]$Detail)

    $script:Hits.Add([pscustomobject]@{
        Kind    = $Kind
        Project = $Project
        Path    = $Path
        Line    = $Line
        Detail  = $Detail
    })
}

$projectRoot = Resolve-ProjectRoot -Path $Root
$configPath = Join-Path $PSScriptRoot 'auditplatform.json'
$config = Read-AuditConfig -ConfigPath $configPath
$version = Read-ProjectVersion -ProjectRoot $projectRoot -Config $config

$reportFolder = if ([string]::IsNullOrWhiteSpace($ReportDirectory)) { [string]$config.report.directory } else { $ReportDirectory }
if (-not [System.IO.Path]::IsPathRooted($reportFolder)) {
    $reportFolder = Join-Path $projectRoot $reportFolder
}
$reportPath = Join-Path $reportFolder ([string]$config.report.prefix + $version + '.md')

[string[]]$trackedFiles = @(Get-TrackedFiles -ProjectRoot $projectRoot -Config $config)
[string[]]$projectFiles = @($trackedFiles | Where-Object { $_.EndsWith('.csproj', [System.StringComparison]::OrdinalIgnoreCase) })
if ($projectFiles.Length -eq 0) {
    throw "No project files were found under: $(Join-Path $projectRoot $config.projects.root)"
}
Write-AuditLine "Project files: $($projectFiles.Length)" -ForegroundColor DarkGray

$table = @{}
foreach ($row in @($config.projects.table)) {
    $table[[string]$row.name] = $row
}

$facts = @{}
foreach ($relativePath in $projectFiles) {
    $fact = Read-ProjectFacts -ProjectRoot $projectRoot -RelativePath $relativePath
    $facts[$fact.Name] = $fact
}

$portableFramework = [string]$config.framework.portable
$twinFramework = [string]$config.framework.twin

function Get-ProjectRole {
    param([string]$Name)

    if ($table.ContainsKey($Name)) {
        return [string]$table[$Name].role
    }
    return ''
}

function Get-ProjectColumn {
    param([string]$Name)

    $role = Get-ProjectRole -Name $Name
    if ($role -eq 'twin') {
        return [string]$table[$Name].half
    }
    if ($role -eq 'portable') {
        return $Name
    }
    return ''
}

function Test-WindowsProject {
    param([string]$Name)

    if ((Get-ProjectRole -Name $Name) -eq 'twin') {
        return $true
    }
    if ($facts.ContainsKey($Name)) {
        foreach ($framework in $facts[$Name].Frameworks) {
            if ($framework -match '-windows') {
                return $true
            }
        }
    }
    return $false
}

foreach ($name in ($facts.Keys | Sort-Object)) {
    if (-not $table.ContainsKey($name)) {
        Add-AuditHit -Kind 'Unmapped' -Project $name -Path $facts[$name].Path -Line 0 -Detail 'the platform table does not name this project'
    }
}

foreach ($row in @($config.projects.table)) {
    $name = [string]$row.name
    if (-not $facts.ContainsKey($name)) {
        Add-AuditHit -Kind 'Absent' -Project $name -Path '' -Line 0 -Detail "the table names this $($row.role), but no project file exists"
    }
}

$sourceFiles = @($trackedFiles | Where-Object { -not $_.EndsWith('.csproj', [System.StringComparison]::OrdinalIgnoreCase) })
$patterns = @($config.windows.patterns | ForEach-Object { [regex]::new([string]$_, [System.Text.RegularExpressions.RegexOptions]::Compiled) })

foreach ($name in ($facts.Keys | Sort-Object)) {
    if (-not $table.ContainsKey($name)) {
        continue
    }

    $fact = $facts[$name]
    $role = Get-ProjectRole -Name $name
    $frameworkText = ($fact.Frameworks -join ';')

    if ($role -eq 'portable') {
        if ($fact.Frameworks.Length -ne 1 -or $fact.Frameworks[0] -ne $portableFramework) {
            Add-AuditHit -Kind 'Framework' -Project $name -Path $fact.Path -Line 0 -Detail "targets '$frameworkText', not exactly '$portableFramework'"
        }

        foreach ($reference in $fact.References) {
            if (Test-WindowsProject -Name $reference) {
                Add-AuditHit -Kind 'Reference' -Project $name -Path $fact.Path -Line 0 -Detail "references the Windows project $reference"
            }
        }

        if (-not (Test-AnalyzerError -ProjectRoot $projectRoot -ProjectPath $fact.FullPath -Rule ([string]$config.analyzer.rule))) {
            Add-AuditHit -Kind 'Analyzer' -Project $name -Path $fact.Path -Line 0 -Detail "$($config.analyzer.rule) is not an error"
        }

        foreach ($property in @($config.windows.properties)) {
            foreach ($value in (Get-XmlValues -Document $fact.Document -Element ([string]$property))) {
                if ($value -eq 'true') {
                    Add-AuditHit -Kind 'Windows' -Project $name -Path $fact.Path -Line 0 -Detail "enables $property"
                }
            }
        }

        foreach ($package in $fact.Packages) {
            if (@($config.windows.packages) -contains $package) {
                Add-AuditHit -Kind 'Windows' -Project $name -Path $fact.Path -Line 0 -Detail "references the Windows package $package"
            }
        }

        foreach ($source in $sourceFiles) {
            if (-not $source.StartsWith($fact.Folder + '/', [System.StringComparison]::Ordinal)) {
                continue
            }
            $sourcePath = Join-Path $projectRoot ($source.Replace([char]'/', [System.IO.Path]::DirectorySeparatorChar))
            $lines = [System.IO.File]::ReadAllLines($sourcePath)
            for ($index = 0; $index -lt $lines.Length; $index++) {
                foreach ($pattern in $patterns) {
                    $match = $pattern.Match($lines[$index])
                    if ($match.Success) {
                        Add-AuditHit -Kind 'Windows' -Project $name -Path $source -Line ($index + 1) -Detail "names $($match.Value)"
                        break
                    }
                }
            }
        }
    }
    elseif ($role -eq 'twin') {
        if ($fact.Frameworks.Length -ne 1 -or -not $fact.Frameworks[0].StartsWith($twinFramework, [System.StringComparison]::Ordinal)) {
            Add-AuditHit -Kind 'Framework' -Project $name -Path $fact.Path -Line 0 -Detail "targets '$frameworkText', not '$twinFramework'"
        }

        $column = Get-ProjectColumn -Name $name
        foreach ($reference in $fact.References) {
            if ((Get-ProjectColumn -Name $reference) -ne $column) {
                Add-AuditHit -Kind 'Column' -Project $name -Path $fact.Path -Line 0 -Detail "references $reference outside the $column column"
            }
        }

        $held = @($sourceFiles | Where-Object { $_.StartsWith($fact.Folder + '/', [System.StringComparison]::Ordinal) })
        if ($held.Length -eq 0) {
            Add-AuditHit -Kind 'Empty' -Project $name -Path $fact.Path -Line 0 -Detail 'the twin holds no source file'
        }
    }
}

$counts = [ordered]@{}
foreach ($kind in $script:AuditKinds) {
    $counts[$kind] = @($script:Hits | Where-Object { $_.Kind -eq $kind }).Length
}

$report = New-Object System.Text.StringBuilder
[void]$report.AppendLine("# Platform audit - $($config.project) $version")
[void]$report.AppendLine()
[void]$report.AppendLine("AUDITPLATFORM GENERATION $script:AuditGeneration")
[void]$report.AppendLine()
[void]$report.AppendLine('## Table')
[void]$report.AppendLine()
[void]$report.AppendLine('| Project | Role | Column | On disk | Targets | References |')
[void]$report.AppendLine('|---|---|---|---|---|---|')
$names = @(@($table.Keys) + @($facts.Keys) | Sort-Object -Unique)
foreach ($name in $names) {
    $role = Get-ProjectRole -Name $name
    $roleText = if ($role -eq '') { 'unmapped' } else { $role }
    $column = Get-ProjectColumn -Name $name
    $onDisk = if ($facts.ContainsKey($name)) { 'yes' } else { 'no' }
    $targets = if ($facts.ContainsKey($name)) { ($facts[$name].Frameworks -join ', ') } else { '' }
    $references = if ($facts.ContainsKey($name)) { ($facts[$name].References -join ', ') } else { '' }
    [void]$report.AppendLine("| $name | $roleText | $column | $onDisk | $targets | $references |")
}
[void]$report.AppendLine()
[void]$report.AppendLine('## Counts')
[void]$report.AppendLine()
[void]$report.AppendLine('| Kind | Hits |')
[void]$report.AppendLine('|---|---|')
foreach ($kind in $script:AuditKinds) {
    [void]$report.AppendLine("| $kind | $($counts[$kind]) |")
}
[void]$report.AppendLine("| Total | $($script:Hits.Count) |")

foreach ($kind in $script:AuditKinds) {
    $kindHits = @($script:Hits | Where-Object { $_.Kind -eq $kind } | Sort-Object Project, Path, Line)
    if ($kindHits.Length -eq 0) {
        continue
    }
    [void]$report.AppendLine()
    [void]$report.AppendLine("## $kind")
    [void]$report.AppendLine()
    foreach ($hit in $kindHits) {
        $where = if ($hit.Path -eq '') { '' } elseif ($hit.Line -gt 0) { " ``$($hit.Path):$($hit.Line)``" } else { " ``$($hit.Path)``" }
        [void]$report.AppendLine("- $($hit.Project)$where - $($hit.Detail)")
    }
}

[System.IO.Directory]::CreateDirectory($reportFolder) | Out-Null
$stagingPath = $reportPath + '.tmp'
[System.IO.File]::WriteAllText($stagingPath, ($report.ToString() -replace "`r`n", "`n"), [System.Text.UTF8Encoding]::new($false))
Move-Item -LiteralPath $stagingPath -Destination $reportPath -Force

foreach ($kind in $script:AuditKinds) {
    $color = if ($counts[$kind] -eq 0) { 'Green' } else { 'Yellow' }
    Write-AuditLine ("{0,-10} {1,4}" -f $kind, $counts[$kind]) -ForegroundColor $color
    foreach ($hit in @($script:Hits | Where-Object { $_.Kind -eq $kind } | Sort-Object Project, Path, Line)) {
        $where = if ($hit.Line -gt 0) { " $($hit.Path):$($hit.Line)" } else { '' }
        Write-AuditLine "  $($hit.Project)$where - $($hit.Detail)" -ForegroundColor DarkGray
    }
}
Write-AuditLine ("{0,-10} {1,4}" -f 'Total', $script:Hits.Count) -ForegroundColor Cyan
Write-AuditLine "Report: $reportPath"

if ($Open -and (Test-Path -LiteralPath $reportPath -PathType Leaf)) {
    Invoke-Item -LiteralPath $reportPath
}
