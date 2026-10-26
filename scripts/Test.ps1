<#
.SYNOPSIS
    Run the project test suite.
.DESCRIPTION
    Runs every test project under the tests folder from the repository root.
    The test projects are discovered on disk, not through the solution: every
    *.csproj at any depth under the tests folder, outside bin and obj folders. By
    default every project runs except the convention test project. -All adds the
    convention project and -Convention runs it alone.
    -Platform picks the test projects mapped to one platform in Test.json; the
    convention project runs beside them only when -All is given. A Windows
    project on a host that is not Windows is skipped with a notice line.
    Given symbol names, only the tests a change inside those symbols can affect
    run: TraceSymbol.ps1 finds them, and each test project runs with a filter that
    selects them. A project with no affected test is skipped. The convention
    project, when -All or -Convention selects it, runs whole, since its audits
    read every source file.
    With -Changed the symbols come from git: TraceSymbol.ps1 seeds its walk with
    the symbols holding every line changed since -Since, HEAD by default, and the
    run is narrowed the same way. Names given beside -Changed are seeds too.
    Every folder under the tests folder that holds a test project gives a switch,
    named by the last dot-separated part of the folder name: a folder Infrastructure
    gives -Infrastructure, a folder Llyn.Tests.Console gives -Console. Given one or
    more such switches, only the test projects inside those folders run. -Help
    lists the switches this tree gives.
    Supports VSTest filters, test discovery, repeated runs for investigating
    intermittent failures, and additional arguments passed to dotnet test.

    Every project builds first, one by one, then all run at once, each in its
    own process; a project's output prints whole when it ends. Every failing
    project is named. The script stops after the first failed run and returns
    the exit code of the first failing project in discovery order.

    Each run is timed from its start. At 10 minutes the projects still running
    are named once; at 15 minutes each is killed with its whole process tree,
    its output so far is printed, it is named as timed out, and the run fails
    with exit code 1 unless an earlier project in discovery order failed.

    Every project-specific value - tests folder, convention project, trx
    folder, platform projects - lives in Test.json. This script carries none, so the file is
    identical in every project at the same generation.
.PARAMETER Name
    One or more symbol names. Runs only the tests a change inside them can affect. Positional.
.PARAMETER Changed
    Runs only the tests the lines changed since -Since can affect, staged or not.
.PARAMETER Since
    The revision or tree -Changed compares the working tree with. Defaults to HEAD.
.PARAMETER Configuration
    Build configuration used by dotnet test: Debug (default) or Release.
.PARAMETER Filter
    VSTest filter expression used to select tests.
.PARAMETER Project
    Name of a single test project to run. Defaults to all. Overrides the scope switches and -Platform.
.PARAMETER Platform
    All (default), Internal or Windows. A platform other than All runs only the test projects Test.json
    maps to it, and the convention project when -All is given. Cannot be combined with -Convention.
.PARAMETER All
    Run every test project, the convention project included. Alias: -a.
.PARAMETER Convention
    Run the convention test project only. Alias: -c.
.PARAMETER Main
    Run every test project except the convention project. This is the default. Alias: -m.
.PARAMETER NoBuild
    Pass --no-build to dotnet test. This also prevents restore.
.PARAMETER NoRestore
    Pass --no-restore while still allowing the test project to build.
.PARAMETER List
    Discover and list matching tests without running them. Cannot be combined with Repeat.
.PARAMETER KeepTrx
    Keep a .trx result file per project and run in the trx folder Test.json names, named <Project>.<tree>.<yyyyMMdd-HHmmss>.trx,
    where <tree> is the first 12 hex digits of the working tree's git tree. Cannot be combined with List.
.PARAMETER Repeat
    Number of test runs. A failing run still finishes every project, and no later run starts. Defaults to 1.
.PARAMETER Verbosity
    dotnet test logging verbosity. Defaults to minimal.
.PARAMETER AdditionalArguments
    Additional arguments forwarded unchanged to dotnet test.
.PARAMETER Help
    Display this help and exit without running tests. The alias -? is supported.
.EXAMPLE
    test
    Run the Debug test suite once, without the convention tests.
.EXAMPLE
    test LTwin
    Run the tests a change inside LTwin can affect, without the convention tests.
.EXAMPLE
    test LTwin -a
    Run the tests a change inside LTwin can affect, and the convention tests.
.EXAMPLE
    test -Changed
    Run the tests the uncommitted edits can affect, without the convention tests.
.EXAMPLE
    test -Changed -Since 4b825dc
    Run the tests the edits since that tree can affect.
.EXAMPLE
    test -a
    Run the full Debug test suite once, the convention tests included.
.EXAMPLE
    test -c
    Run the convention tests only.
.EXAMPLE
    test -Infrastructure -Application
    Run only the test projects inside the Infrastructure and Application folders.
.EXAMPLE
    test -Platform Internal -a
    Run the portable tests and the convention tests.
.EXAMPLE
    test -Platform Windows
    Run the Windows tests only.
.EXAMPLE
    test -Configuration Release
    Run the Release test suite.
.EXAMPLE
    test -Project <name>
    Run one test project.
.EXAMPLE
    test -Filter 'FullyQualifiedName~Tests'
    Run tests whose fully-qualified names match the filter.
.EXAMPLE
    test -Repeat 10 -NoBuild
    Reuse existing build output and run up to ten times; a failing run finishes every project, then no later run starts.
.EXAMPLE
    test -List
    List discovered tests without executing them.
.EXAMPLE
    test -AdditionalArguments '--logger', 'trx', '--results-directory', 'TestResults'
    Forward logger and results-directory options to dotnet test.
#>
#requires -Version 5.1
# TEST - TEST GENERATION 7.
# A generation is not a revision count. It names functionality, not edits, so editing one of these
# files is never on its own a reason to raise it. Raise it only when the executed outcome changes.
# A generation names how the family finds and runs the tests. Two projects on the same generation
# run the same projects the same way from the same tree layout.
# Generation 2: every *.csproj one level under the tests folder is a test project, discovered on disk
# and run one by one through dotnet test; by default every project runs, the convention project
# included; -Main skips the convention project, -Convention runs it alone, -Project picks one by name;
# the first failing project ends the run and its exit code is returned.
# Generation 3: everything in generation 2; given symbol names, TraceSymbol.ps1 names the affected tests,
# each test project runs with a filter selecting them - by method, by class when the method filter
# grows too long, whole when the class filter does too - a project with none is skipped, and the
# convention project runs whole.
# Generation 4: everything in generation 3; Test.json maps each platform to one test project or a list, -Platform
# runs only those projects beside the convention project, -Main still drops the convention project, and a
# Windows project on a host that is not Windows is skipped with a notice line instead of failing.
# Generation 5: everything in generation 4, except that every project builds first, one by one, and then
# all run at once in their own processes; every failing project is named, not only the first, and the
# first failing project in discovery order gives the exit code.
# Generation 6: everything in generation 5, except that the convention project runs only when -All or
# -Convention selects it; a plain run, a run given symbol names and a -Platform run all leave it out,
# and -Main names that default. Every folder under the tests folder that holds a test project gives a
# switch named by the last dot-separated part of its name, and one or more such switches run only the
# test projects inside those folders.
# Generation 7: everything in generation 6; -Changed narrows the run to the tests the lines changed since
# -Since can affect, found by TraceSymbol.ps1 at generation 2, as symbol names narrow it. -KeepTrx keeps
# one <Project>.<tree>.<yyyyMMdd-HHmmss>.trx per project and run in the trx folder Test.json names, for
# ScanTest to read.
# Every project-specific value lives in Test.json, so this file is identical in every project at
# this generation.
[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string[]]$Name,

    [switch]$Changed,

    [string]$Since,

    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',

    [string]$Filter,

    [string]$Project,

    [ValidateSet('All', 'Internal', 'Windows')]
    [string]$Platform = 'All',

    [Alias('a')]
    [switch]$All,

    [Alias('c')]
    [switch]$Convention,

    [Alias('m')]
    [switch]$Main,

    [switch]$NoBuild,

    [switch]$NoRestore,

    [switch]$List,

    [switch]$KeepTrx,

    [ValidateRange(1, [int]::MaxValue)]
    [int]$Repeat = 1,

    [ValidateSet('quiet', 'minimal', 'normal', 'detailed', 'diagnostic')]
    [string]$Verbosity = 'minimal',

    [Parameter(ValueFromRemainingArguments)]
    [string[]]$AdditionalArguments,

    [Alias('?')]
    [switch]$Help
)

dynamicparam {
    # Every folder under the tests folder that holds a test project, at any depth, gives one switch,
    # named by the last dot-separated part of the folder name: Infrastructure gives -Infrastructure and
    # Llyn.Tests.Console gives -Console. Folders that share a name share the switch. A name that a fixed
    # parameter or alias already takes, or that is no plain identifier, gives no switch. The map from
    # switch to folders is kept for the script body; a tree that cannot be read gives no switches, and
    # the body then reports the cause.
    $script:testFolders = @{}
    $dictionary = [System.Management.Automation.RuntimeDefinedParameterDictionary]::new()
    try {
        $config = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'Test.json') -Raw -Encoding UTF8 | ConvertFrom-Json
        $testsRoot = [System.IO.Path]::GetFullPath((Join-Path (Join-Path $PSScriptRoot '..') ([string]$config.tests)))
        $taken = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
        foreach ($parameter in $MyInvocation.MyCommand.Parameters.Values) {
            $null = $taken.Add($parameter.Name)
            foreach ($alias in $parameter.Aliases) {
                $null = $taken.Add($alias)
            }
        }

        $pending = [System.Collections.Generic.Stack[string]]::new()
        $pending.Push($testsRoot)
        while ($pending.Count -gt 0) {
            $folder = $pending.Pop()
            foreach ($child in [System.IO.Directory]::GetDirectories($folder)) {
                $leaf = [System.IO.Path]::GetFileName($child)
                if ($leaf -ne 'bin' -and $leaf -ne 'obj') {
                    $pending.Push($child)
                }
            }

            if ([System.IO.Directory]::GetFiles($folder, '*.csproj').Count -eq 0) {
                continue
            }

            for ($owner = $folder; $owner.Length -gt $testsRoot.Length; $owner = [System.IO.Path]::GetDirectoryName($owner)) {
                $switchName = @([System.IO.Path]::GetFileName($owner) -split '\.')[-1]
                if ($switchName -notmatch '^[A-Za-z_][A-Za-z0-9_]*$' -or $taken.Contains($switchName)) {
                    continue
                }

                # The map ignores case, as parameter names do, so Console and console share one switch.
                if (-not $script:testFolders.ContainsKey($switchName)) {
                    $script:testFolders[$switchName] = [System.Collections.Generic.List[string]]::new()
                }

                if (-not $script:testFolders[$switchName].Contains($owner)) {
                    $script:testFolders[$switchName].Add($owner)
                }
            }
        }
    }
    catch {
        $script:testFolders = @{}
    }

    foreach ($switchName in $script:testFolders.Keys) {
        $attributes = [System.Collections.ObjectModel.Collection[System.Attribute]]::new()
        $attributes.Add([System.Management.Automation.ParameterAttribute]::new())
        $dictionary.Add($switchName, [System.Management.Automation.RuntimeDefinedParameter]::new($switchName, [switch], $attributes))
    }

    return $dictionary
}

# A script with a dynamicparam block keeps its body in a named block. The body is left at the margin,
# so the help text below prints as it reads.
end {

if ($Help) {
    @'
NAME
    Test.ps1

SYNOPSIS
    Run the project test suite.

SYNTAX
    test [<name> ...] [-Changed [-Since <revision>]]
        [-Configuration <Debug|Release>] [-Filter <expression>]
        [-Project <name>] [-Platform <All|Internal|Windows>]
        [-All | -Convention | -Main]
        [-NoBuild] [-NoRestore] [-List] [-KeepTrx]
        [-Repeat <count>] [-Verbosity <level>]
        [-AdditionalArguments <arguments[]>] [-<folder> ...] [-Help]

OPTIONS
    <name>
        Symbol to start from: a type, method, property or field. Several are
        allowed. Runs only the tests a change inside them can affect, found by
        TraceSymbol.ps1, and with -All the convention project whole.

    -Changed
        Run only the tests the lines changed since -Since can affect, staged
        or not, found by TraceSymbol.ps1 as for <name>. Names given beside it
        add their tests.

    -Since <revision>
        The revision or tree -Changed compares the working tree with.
        Defaults to HEAD.

    -Configuration <Debug|Release>
        Build configuration. Defaults to Debug.

    -Filter <expression>
        VSTest filter expression used to select tests.

    -Project <name>
        Run one test project under the tests folder. Defaults to all.
        Overrides the scope switches and -Platform.

    -Platform <All|Internal|Windows>
        Run the test projects Test.json maps to the platform, and the
        convention project when -All is given. Defaults to All, every
        project. A Windows project on a host that is not Windows is skipped
        with a notice line. Cannot be combined with -Convention.

    -All, -a
        Run every test project, the convention project included.

    -Convention, -c
        Run the convention test project only.

    -Main, -m
        Run every test project except the convention project. Default.

    -NoBuild
        Reuse existing build output and skip restore.

    -NoRestore
        Skip restore while still allowing the test project to build.

    -List
        List matching tests without running them. Cannot be used with -Repeat.

    -KeepTrx
        Keep a .trx result file per project and run in the trx folder
        Test.json names, named <Project>.<tree>.<yyyyMMdd-HHmmss>.trx. <tree> is the first 12 hex
        digits of the git tree of the whole working tree, staged or not.
        ScanTest reads these files. Cannot be used with -List.

    -Repeat <count>
        Number of runs. A failing run still finishes every project, and no
        later run starts. Defaults to 1.

    -Verbosity <level>
        dotnet test logging verbosity. Defaults to minimal.

    -AdditionalArguments <arguments[]>
        Additional arguments forwarded unchanged to dotnet test.

    -Help, -?
        Display this help and exit without running tests.

    -<folder>
        Run only the test projects inside the folders under the tests folder
        whose name, after its last dot, is <folder>. Several may be given. Cannot be combined with -All, -Convention, -Main, -Platform or
        -Project. FOLDERS below lists the switches this tree gives.

TIME LIMIT
    Each run is timed from its start. At 10 minutes the projects still
    running are named once; at 15 minutes each is killed with its whole
    process tree, its output so far is printed, and the run fails.

EXAMPLES
    test
        Run the Debug test suite once, without the convention tests.

    test LTwin
        Run the tests a change inside LTwin can affect, without the convention tests.

    test LTwin -a
        Run the tests a change inside LTwin can affect, and the convention tests.

    test -Changed
        Run the tests the uncommitted edits can affect.

    test -Changed -Since 4b825dc
        Run the tests the edits since that tree can affect.

    test -a
        Run the full Debug test suite once, the convention tests included.

    test -c
        Run the convention tests only.

    test -Platform Internal -a
        Run the portable tests and the convention tests.

    test -Platform Windows
        Run the Windows tests only.

    test -Configuration Release
        Run the Release test suite.

    test -Project <name>
        Run one test project.

    test -Filter 'FullyQualifiedName~Tests'
        Run tests whose fully-qualified names match the filter.

    test -Repeat 10 -NoBuild
        Reuse build output and run up to ten times; a failing run finishes
        every project, then no later run starts.

    test -List
        List discovered tests without executing them.

    test -AdditionalArguments '--logger', 'trx'
        Forward additional options to dotnet test.

    test -Infrastructure -Application
        Run only the test projects inside the Infrastructure and Application
        folders.
'@ | Write-Host

    # The folder switches come from the tree, so the help names the ones this tree gives.
    Write-Host ''
    Write-Host 'FOLDERS'
    if ($script:testFolders.Count -eq 0) {
        Write-Host '    No folder under the tests folder gives a switch.'
    }
    else {
        Write-Host '    Each switch runs only the test projects inside its folders. Several'
        Write-Host '    may be given, and each adds its folders.'
        Write-Host ''
        $helpRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..')).TrimEnd('\', '/') + [System.IO.Path]::DirectorySeparatorChar
        $switchNames = [string[]]@($script:testFolders.Keys)
        [System.Array]::Sort($switchNames, [System.StringComparer]::Ordinal)
        foreach ($switchName in $switchNames) {
            Write-Host ("    -{0}" -f $switchName)
            $folders = $script:testFolders[$switchName].ToArray()
            [System.Array]::Sort($folders, [System.StringComparer]::Ordinal)
            foreach ($folder in $folders) {
                Write-Host ("        {0}" -f $folder.Substring($helpRoot.Length))
            }
        }
    }

    exit 0
}

Write-Host 'TEST - TEST GENERATION 7' -ForegroundColor Blue

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# dotnet and git write UTF-8. A console still on the OEM code page, as one opened by the dispatcher
# without a profile is, would show every non-ASCII line garbled, so this process reads and writes
# UTF-8. Process-local: the calling console keeps its own code page.
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [Console]::OutputEncoding

# Each run of the selected projects is timed from its own start. A project still running at the warning
# is named once; one still running at the stop is killed with its whole process tree and fails the run.
$runWarningMinutes = 10
$runStopMinutes = 15

function Get-OrdinalKey {
    # Sort-Object compares text by culture, which Windows PowerShell 5.1 and pwsh 7 order differently.
    # Uppercase hexadecimal UTF-16 code units compare alike under every culture, so this key sorts ordinally.
    param([string]$Text)
    return [System.BitConverter]::ToString([System.Text.Encoding]::BigEndianUnicode.GetBytes($Text)).Replace('-', '')
}

function Find-TestProject {
    # Every *.csproj at any depth under the folder is a test project. A bin or obj folder holds build
    # output, never a project, so the walk does not enter one.
    param([string]$Folder)
    Get-ChildItem -LiteralPath $Folder -Filter '*.csproj' -File
    Get-ChildItem -LiteralPath $Folder -Directory |
        Where-Object { $_.Name -ne 'bin' -and $_.Name -ne 'obj' } |
        ForEach-Object { Find-TestProject -Folder $_.FullName }
}

function Read-TestConfig {
    $configPath = Join-Path $PSScriptRoot 'Test.json'
    if (-not (Test-Path -LiteralPath $configPath -PathType Leaf)) {
        throw "The test configuration was not found: $configPath"
    }

    try {
        $config = Get-Content -LiteralPath $configPath -Raw -Encoding UTF8 | ConvertFrom-Json
    }
    catch {
        throw "The test configuration is not valid JSON: $configPath`n$($_.Exception.Message)"
    }

    foreach ($key in @('generation', 'tests', 'convention', 'platforms')) {
        if (-not ($config.PSObject.Properties.Name -contains $key)) {
            throw "The test configuration must contain a $key property: $configPath"
        }
    }

    if ([int]$config.generation -ne 7) {
        throw "The test configuration is generation $($config.generation); this script is generation 7."
    }

    return $config
}

if (@($All, $Convention, $Main | Where-Object { $_ }).Count -gt 1) {
    throw '-All, -Convention and -Main cannot be combined.'
}

# A folder switch names its folders outright, so no other scope option applies beside it.
$folderSwitches = @($script:testFolders.Keys | Where-Object { $PSBoundParameters.ContainsKey($_) -and $PSBoundParameters[$_] })
if ($folderSwitches.Count -gt 0 -and ($All -or $Convention -or $Main -or $Platform -ne 'All' -or -not [string]::IsNullOrWhiteSpace($Project))) {
    throw 'A folder switch cannot be combined with -All, -Convention, -Main, -Platform or -Project.'
}

if ($Convention -and $Platform -ne 'All') {
    throw '-Convention and -Platform cannot be combined.'
}

if (-not [string]::IsNullOrWhiteSpace($Since) -and -not $Changed) {
    throw '-Since needs -Changed.'
}

$root = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$config = Read-TestConfig
$testsRoot = Join-Path $root ([string]$config.tests)

if (-not (Test-Path -LiteralPath $testsRoot -PathType Container)) {
    throw "The test folder was not found: $testsRoot"
}

$projects = @(
    Find-TestProject -Folder $testsRoot |
        Sort-Object -Property @{ Expression = { Get-OrdinalKey $_.Name } }
)

if ($folderSwitches.Count -gt 0) {
    # A project runs when it lies inside any folder of any switch given.
    $folders = @(foreach ($switchName in $folderSwitches) { $script:testFolders[$switchName] | ForEach-Object { $_ + [System.IO.Path]::DirectorySeparatorChar } })
    $projects = @($projects | Where-Object {
        $path = $_.FullName
        @($folders | Where-Object { $path.StartsWith($_, [System.StringComparison]::OrdinalIgnoreCase) }).Count -gt 0
    })
}
elseif (-not [string]::IsNullOrWhiteSpace($Project)) {
    $projects = @($projects | Where-Object { $_.BaseName -eq $Project })
    if ($projects.Count -eq 0) {
        throw "Test project was not found under tests: $Project"
    }
}
elseif ($Convention) {
    $projects = @($projects | Where-Object { $_.BaseName -eq [string]$config.convention })
    if ($projects.Count -eq 0) {
        throw "The convention test project was not found under tests: $($config.convention)"
    }
}
else {
    if ($Platform -ne 'All') {
        $mapped = $config.platforms.PSObject.Properties[$Platform]
        if ($null -eq $mapped) {
            throw "The test configuration maps no project to the platform: $Platform"
        }

        $platformProjects = @($mapped.Value | ForEach-Object { [string]$_ })
        foreach ($platformProject in $platformProjects) {
            if (@($projects | Where-Object { $_.BaseName -eq $platformProject }).Count -eq 0) {
                throw "The $Platform test project was not found under tests: $platformProject"
            }
        }

        $projects = @($projects | Where-Object { $platformProjects -contains $_.BaseName -or $_.BaseName -eq [string]$config.convention })
    }

    # The convention project runs only when -All asks for it; -Main names this default.
    if (-not $All) {
        $projects = @($projects | Where-Object { $_.BaseName -ne [string]$config.convention })
    }
}

# A Windows test project targets a Windows framework, which builds and runs on Windows alone.
$windowsMapped = $config.platforms.PSObject.Properties['Windows']
if ($null -ne $windowsMapped -and [System.Environment]::OSVersion.Platform -ne [System.PlatformID]::Win32NT) {
    $windowsProjects = @($windowsMapped.Value | ForEach-Object { [string]$_ })
    if (@($projects | Where-Object { $windowsProjects -contains $_.BaseName }).Count -gt 0) {
        Write-Host "$($windowsProjects -join ', ') is skipped: this host is not Windows."
        $projects = @($projects | Where-Object { $windowsProjects -notcontains $_.BaseName })
        if ($projects.Count -eq 0) {
            exit 0
        }
    }
}

if ($projects.Count -eq 0) {
    throw "No test project was found under: $testsRoot"
}

# A filter travels on the dotnet command line, whose length Windows caps near 32K characters, so a
# selection longer than this falls back from methods to classes, and from classes to the whole project.
$selectionLimit = 20000
$selections = @{}

if (($null -ne $Name -and $Name.Count -gt 0) -or $Changed) {
    $detector = Join-Path $PSScriptRoot 'TraceSymbol.ps1'
    if (-not (Test-Path -LiteralPath $detector -PathType Leaf)) {
        throw "TraceSymbol.ps1 was not found: $detector"
    }

    $detectorArguments = @{ Json = $true }
    if ($null -ne $Name -and $Name.Count -gt 0) {
        $detectorArguments['Name'] = $Name
    }
    if ($Changed) {
        $detectorArguments['Changed'] = $true
        if (-not [string]::IsNullOrWhiteSpace($Since)) {
            $detectorArguments['Since'] = $Since
        }
    }

    $detection = & $detector @detectorArguments | Out-String | ConvertFrom-Json
    $detected = @($detection.tests)
    $seedLabel = if (@($detection.seeds).Count -gt 0) { @($detection.seeds) -join ', ' } else { 'no changed symbol' }
    Write-Host ("{0}: {1} affected symbol(s), {2} affected test(s)." -f $seedLabel, @($detection.symbols).Count, $detected.Count) -ForegroundColor Cyan

    $projects = @(
        foreach ($testProject in $projects) {
            if ($testProject.BaseName -eq [string]$config.convention) {
                $testProject
                continue
            }

            $selected = @($detected | Where-Object { $_.project -eq $testProject.BaseName })
            if ($selected.Count -eq 0) {
                continue
            }

            $selection = @($selected | ForEach-Object { "FullyQualifiedName~.$($_.class).$($_.method)" } | Sort-Object -Property @{ Expression = { Get-OrdinalKey $_ } } -Unique) -join '|'
            if ($selection.Length -gt $selectionLimit) {
                $selection = @($selected | ForEach-Object { "FullyQualifiedName~.$($_.class)." } | Sort-Object -Property @{ Expression = { Get-OrdinalKey $_ } } -Unique) -join '|'
            }

            if ($selection.Length -gt $selectionLimit) {
                Write-Host "$($testProject.BaseName): the selection is too long for a filter, so the whole project runs." -ForegroundColor Yellow
            }
            else {
                $selections[$testProject.BaseName] = $selection
            }

            $testProject
        }
    )

    if ($projects.Count -eq 0) {
        Write-Host 'No affected test was found.' -ForegroundColor Green
        exit 0
    }
}

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if ($null -eq $dotnet) {
    throw 'The .NET SDK is required to run tests, but dotnet was not found on PATH.'
}

if ($List -and $Repeat -ne 1) {
    throw '-List cannot be combined with -Repeat.'
}

if ($List -and $KeepTrx) {
    throw '-List cannot be combined with -KeepTrx.'
}

$commonArguments = @(
    '--configuration'
    $Configuration
    '--verbosity'
    $Verbosity
    '--nologo'
)

# The projects run at once, and two builds of one shared reference collide on its output files, so
# every project builds first, one by one, and each run then reuses that output.
if (-not $NoBuild) {
    $buildArguments = $commonArguments
    if ($NoRestore) {
        $buildArguments += '--no-restore'
    }

    foreach ($testProject in $projects) {
        & $dotnet.Source build $testProject.FullName @buildArguments
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Build failed in $($testProject.BaseName) (exit code $LASTEXITCODE)." -ForegroundColor Red
            exit $LASTEXITCODE
        }
    }
}

$commonArguments += '--no-build'

if ($List) {
    $commonArguments += '--list-tests'
}

if ($null -ne $AdditionalArguments -and $AdditionalArguments.Count -gt 0) {
    $commonArguments += $AdditionalArguments
}

function Get-TreeHash {
    # The tree of the whole working tree, staged or not, written through a temporary index so the real
    # index stays untouched.
    param([Parameter(Mandatory = $true)][string]$Root)

    $indexPath = Join-Path ([System.IO.Path]::GetTempPath()) ('Test-index-' + [guid]::NewGuid().ToString('N'))
    $previousIndex = $env:GIT_INDEX_FILE
    $nativePreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        $realIndex = [string](& git -C $Root rev-parse --path-format=absolute --git-path index 2>$null)
        if ($LASTEXITCODE -eq 0 -and [System.IO.File]::Exists($realIndex.Trim())) {
            [System.IO.File]::Copy($realIndex.Trim(), $indexPath)
        }
        $env:GIT_INDEX_FILE = $indexPath
        $null = & git -C $Root add --all -- . 2>&1
        if ($LASTEXITCODE -ne 0) { throw 'git add failed while the working tree was hashed for -KeepTrx.' }
        $tree = [string](& git -C $Root write-tree 2>$null)
        if ($LASTEXITCODE -ne 0 -or $tree.Trim() -notmatch '^[0-9a-f]{12,}$') { throw 'git write-tree failed while the working tree was hashed for -KeepTrx.' }
        return $tree.Trim().Substring(0, 12)
    }
    finally {
        $env:GIT_INDEX_FILE = $previousIndex
        $ErrorActionPreference = $nativePreference
        Remove-Item -LiteralPath $indexPath -Force -ErrorAction SilentlyContinue
    }
}

$trxFolder = ''
$treeHash = ''
if ($KeepTrx) {
    $trxSetting = $config.PSObject.Properties['trx']
    if ($null -eq $trxSetting -or [string]::IsNullOrWhiteSpace([string]$trxSetting.Value)) {
        throw 'The test configuration must contain a trx property naming the folder -KeepTrx writes to: ' + (Join-Path $PSScriptRoot 'Test.json')
    }
    $trxFolder = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($root, [string]$trxSetting.Value))
    $treeHash = Get-TreeHash -Root $root
    $null = [System.IO.Directory]::CreateDirectory($trxFolder)
    Write-Host "Keeping trx files in $trxFolder for tree $treeHash" -ForegroundColor DarkGray
}

function ConvertTo-Argument {
    # Start-Process takes one command line, so each argument is quoted the way the C runtime splits it.
    param([string]$Text)
    if ($Text -ne '' -and $Text -notmatch '[\s"]') {
        return $Text
    }

    return '"' + (($Text -replace '(\\*)"', '$1$1\"') -replace '(\\+)$', '$1$1') + '"'
}

function Read-TestOutput {
    # File.ReadAllLines lets no other handle write, so a process still holding the file open for writing
    # would fail the read and end the script. Sharing ReadWrite lets a live writer stay; no retry is needed.
    param([string]$Path)
    $stream = [System.IO.FileStream]::new($Path, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::ReadWrite)
    try {
        $reader = [System.IO.StreamReader]::new($stream, [Console]::OutputEncoding)
        while ($null -ne ($line = $reader.ReadLine())) {
            $line
        }
    }
    finally {
        $stream.Dispose()
    }
}

function Get-ProcessParent {
    # Maps every live process id to its parent id and start time. Windows PowerShell 5.1 has neither
    # Process.Parent nor Kill($true), so the map comes from Win32_Process on Windows and from ps elsewhere,
    # alike in both shells. ps names a live parent only, since an orphan is adopted, so it needs no start time.
    $map = @{}
    if ([System.Environment]::OSVersion.Platform -eq [System.PlatformID]::Win32NT) {
        foreach ($item in @(Get-CimInstance -ClassName Win32_Process -Property ProcessId, ParentProcessId, CreationDate)) {
            $map[[int]$item.ProcessId] = [pscustomobject]@{ Parent = [int]$item.ParentProcessId; Started = $item.CreationDate }
        }
    }
    else {
        foreach ($line in @(& ps -A -o pid= -o ppid=)) {
            $fields = @($line.Trim() -split '\s+')
            if ($fields.Count -eq 2) {
                $map[[int]$fields[0]] = [pscustomobject]@{ Parent = [int]$fields[1]; Started = $null }
            }
        }
    }

    return $map
}

function Stop-ProcessTree {
    # dotnet test starts vstest and testhost as children, and killing dotnet alone leaves them running and
    # holding files, so every descendant is found first, while the tree is whole, and then each is killed.
    param([System.Diagnostics.Process]$Process)
    if ($Process.HasExited) {
        return $false
    }

    $parents = $null
    $tree = [System.Collections.Generic.List[int]]::new()
    $tree.Add($Process.Id)
    try {
        $parents = Get-ProcessParent
        for ($index = 0; $index -lt $tree.Count; $index++) {
            $parentId = $tree[$index]
            foreach ($childId in @($parents.Keys)) {
                $child = $parents[$childId]
                if ($child.Parent -ne $parentId -or $tree.Contains($childId)) {
                    continue
                }

                # Windows keeps the parent id of a parent that ended, and may give that id to a new process;
                # a child started before the process now holding the id is not its child.
                $parent = $parents[$parentId]
                if ($null -ne $child.Started -and $null -ne $parent -and $null -ne $parent.Started -and $child.Started -lt $parent.Started) {
                    continue
                }

                $tree.Add($childId)
            }
        }
    }
    catch {
        Write-Host "The child processes of $($Process.Id) could not be listed, so it is killed alone: $($_.Exception.Message)" -ForegroundColor Yellow
    }

    # The root is killed first, so it starts no new child; each process is then awaited so its files are
    # released. A process that ends between the listing and its kill throws, and is passed over. An id that
    # ended after the listing may pass to an unrelated process, so a child is opened only while its start
    # time equals the listed one; the listing gives whole microseconds, so both are compared at that grain, in whole ticks, since a division would pass through double.
    $members = @(foreach ($id in $tree) {
        try {
            if ($id -eq $Process.Id) {
                $Process
                continue
            }

            $member = [System.Diagnostics.Process]::GetProcessById($id)
            $listed = if ($null -ne $parents -and $parents.ContainsKey($id)) { $parents[$id].Started } else { $null }
            if ($null -ne $listed -and ($member.StartTime.Ticks - $member.StartTime.Ticks % 10) -ne ($listed.Ticks - $listed.Ticks % 10)) {
                continue
            }

            $member
        }
        catch {
        }
    })
    # The caller learns whether this kill ended the root; a root that exited on its own first finished.
    $killed = $false
    foreach ($member in $members) {
        try {
            if (-not $member.HasExited) {
                $member.Kill()
                if ($member.Id -eq $Process.Id) {
                    $killed = $true
                }
            }
        }
        catch {
        }
    }
    foreach ($member in $members) {
        try {
            $null = $member.WaitForExit(5000)
        }
        catch {
        }
    }

    return $killed
}

$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$exitCode = 0
$running = @()
$outputRoot = Join-Path ([System.IO.Path]::GetTempPath()) ('Test-' + [guid]::NewGuid().ToString('N'))
$null = New-Item -ItemType Directory -Path $outputRoot

Push-Location $root
try {
    for ($run = 1; $run -le $Repeat; $run++) {
        if ($Repeat -gt 1) {
            Write-Host "Test run $run of $Repeat" -ForegroundColor Cyan
        }

        # Every project runs at once in its own process. Its output goes to a file and is printed whole
        # when the process ends, so the lines of two projects never interleave.
        $runWatch = [System.Diagnostics.Stopwatch]::StartNew()
        $runStamp = [DateTime]::Now.ToString('yyyyMMdd-HHmmss', [System.Globalization.CultureInfo]::InvariantCulture)
        $warned = $false
        $running = @(foreach ($testProject in $projects) {
            $projectFilter = $Filter
            if ($selections.ContainsKey($testProject.BaseName)) {
                $selection = $selections[$testProject.BaseName]
                $projectFilter = if ([string]::IsNullOrWhiteSpace($Filter)) { $selection } else { "($Filter)&($selection)" }
            }

            $filterArguments = @()
            if (-not [string]::IsNullOrWhiteSpace($projectFilter)) {
                $filterArguments = @('--filter', $projectFilter)
            }

            $trxArguments = @()
            if ($KeepTrx) {
                $trxPath = Join-Path $trxFolder ('{0}.{1}.{2}.trx' -f $testProject.BaseName, $treeHash, $runStamp)
                $trxArguments = @('--logger', "trx;LogFileName=$trxPath")
            }

            $arguments = @('test', $testProject.FullName) + $filterArguments + $commonArguments + $trxArguments
            # Each run writes its own files, so no run reopens a file a leftover process of an earlier run still holds.
            $outputPath = Join-Path $outputRoot "$($testProject.BaseName).$run.out"
            $errorPath = Join-Path $outputRoot "$($testProject.BaseName).$run.err"
            $process = Start-Process -FilePath $dotnet.Source -ArgumentList (@($arguments | ForEach-Object { ConvertTo-Argument $_ }) -join ' ') `
                -WorkingDirectory $root -NoNewWindow -PassThru -RedirectStandardOutput $outputPath -RedirectStandardError $errorPath
            # Windows PowerShell 5.1 reports no exit code for a process whose handle was never read.
            $null = $process.Handle
            [pscustomobject]@{ Project = $testProject; Process = $process; Output = $outputPath; Error = $errorPath; Shown = $false; TimedOut = $false }
        })

        while (@($running | Where-Object { -not $_.Shown }).Count -gt 0) {
            foreach ($entry in @($running | Where-Object { -not $_.Shown -and $_.Process.HasExited })) {
                $entry.Process.WaitForExit()
                $entry.Shown = $true
                Write-Host ('{0} ({1:N1}s)' -f $entry.Project.BaseName, ($entry.Process.ExitTime - $entry.Process.StartTime).TotalSeconds) -ForegroundColor Cyan
                foreach ($path in @($entry.Output, $entry.Error)) {
                    Read-TestOutput -Path $path
                }
            }

            $unfinished = @($running | Where-Object { -not $_.Shown })
            if ($unfinished.Count -eq 0) {
                break
            }

            if (-not $warned -and $runWatch.Elapsed.TotalMinutes -ge $runWarningMinutes) {
                $warned = $true
                Write-Host ("Run $run of $Repeat passed the $runWarningMinutes-minute warning; still running: {0}. Each is stopped at $runStopMinutes minutes." -f (@($unfinished | ForEach-Object { $_.Project.BaseName }) -join ', ')) -ForegroundColor Yellow
            }

            if ($runWatch.Elapsed.TotalMinutes -ge $runStopMinutes) {
                # A project that exited after the last poll finished, so it is shown as finished, with its own
                # exit code; only one this kill ended is named timed out.
                foreach ($entry in $unfinished) {
                    $entry.Shown = $true
                    $killed = Stop-ProcessTree -Process $entry.Process
                    if (-not $killed -and $entry.Process.HasExited) {
                        $entry.Process.WaitForExit()
                        Write-Host ('{0} ({1:N1}s)' -f $entry.Project.BaseName, ($entry.Process.ExitTime - $entry.Process.StartTime).TotalSeconds) -ForegroundColor Cyan
                        foreach ($path in @($entry.Output, $entry.Error)) {
                            Read-TestOutput -Path $path
                        }

                        continue
                    }

                    $entry.TimedOut = $true
                    Write-Host ('{0} timed out after {1:N1}s; its output so far follows.' -f $entry.Project.BaseName, $runWatch.Elapsed.TotalSeconds) -ForegroundColor Red
                    foreach ($path in @($entry.Output, $entry.Error)) {
                        Read-TestOutput -Path $path
                    }
                }

                break
            }

            Start-Sleep -Milliseconds 200
        }

        # Every failing project is named, and the first in discovery order gives the exit code. A project
        # killed at the stop has no exit code of its own, so it gives 1.
        foreach ($entry in $running) {
            if ($entry.TimedOut) {
                Write-Host "Tests timed out in $($entry.Project.BaseName) on run $run of $Repeat (stopped at $runStopMinutes minutes)." -ForegroundColor Red
                if ($exitCode -eq 0) {
                    $exitCode = 1
                }
            }
            elseif ($entry.Process.ExitCode -ne 0) {
                Write-Host "Tests failed in $($entry.Project.BaseName) on run $run of $Repeat (exit code $($entry.Process.ExitCode))." -ForegroundColor Red
                if ($exitCode -eq 0) {
                    $exitCode = $entry.Process.ExitCode
                }
            }
        }

        if ($exitCode -ne 0) {
            break
        }
    }
}
finally {
    # A run broken off, by an error or by Ctrl+C, leaves its projects running; each is killed with its tree.
    foreach ($entry in $running) {
        $null = Stop-ProcessTree -Process $entry.Process
    }

    # A process that outlives its project may still hold an output file for a moment, so the removal is
    # tried again; a folder that stays is named rather than left behind in silence.
    for ($attempt = 1; $attempt -le 5; $attempt++) {
        try {
            Remove-Item -LiteralPath $outputRoot -Recurse -Force -ErrorAction Stop
            break
        }
        catch {
            if ($attempt -eq 5) {
                Write-Host "The test output folder could not be removed: $outputRoot ($($_.Exception.Message))" -ForegroundColor Yellow
            }
            else {
                Start-Sleep -Milliseconds 500
            }
        }
    }

    Pop-Location
    $stopwatch.Stop()
}

if ($exitCode -eq 0) {
    if ($List) {
        Write-Host ("Test discovery completed in {0:N1}s." -f $stopwatch.Elapsed.TotalSeconds) -ForegroundColor Green
    }
    else {
        Write-Host ("Tests passed ({0} project(s), {1} run(s), {2:N1}s)." -f $projects.Count, $Repeat, $stopwatch.Elapsed.TotalSeconds) -ForegroundColor Green
    }
}

exit $exitCode
}
