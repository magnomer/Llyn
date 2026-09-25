<#
.SYNOPSIS
    Run the project test suite.
.DESCRIPTION
    Runs every test project under the tests folder from the repository root.
    The test projects are discovered on disk, not through the solution. By
    default every project runs, the convention test project included. -Main
    skips the convention project and -Convention runs it alone.
    Given symbol names, only the tests a change inside those symbols can affect
    run: detector.ps1 finds them, and each test project runs with a filter that
    selects them. A project with no affected test is skipped. The convention
    project runs whole, since its audits read every source file.
    Supports VSTest filters, test discovery, repeated runs for investigating
    intermittent failures, and additional arguments passed to dotnet test.

    The script stops after the first failed run and returns dotnet test's exit
    code to the caller.

    Every project-specific value - project name, tests folder, convention
    project - lives in test.json. This script carries none, so the file is
    identical in every project at the same generation.
.PARAMETER Name
    One or more symbol names. Runs only the tests a change inside them can affect. Positional.
.PARAMETER Configuration
    Build configuration used by dotnet test: Debug (default) or Release.
.PARAMETER Filter
    VSTest filter expression used to select tests.
.PARAMETER Project
    Name of a single test project to run. Defaults to all. Overrides the scope switches.
.PARAMETER Full
    Run every test project, the convention project included. This is the default. Alias: -f.
.PARAMETER Convention
    Run the convention test project only. Alias: -c.
.PARAMETER Main
    Run every test project except the convention project. Alias: -m.
.PARAMETER NoBuild
    Pass --no-build to dotnet test. This also prevents restore.
.PARAMETER NoRestore
    Pass --no-restore while still allowing the test project to build.
.PARAMETER List
    Discover and list matching tests without running them. Cannot be combined with Repeat.
.PARAMETER Repeat
    Number of test runs. Execution stops on the first failure. Defaults to 1.
.PARAMETER Verbosity
    dotnet test logging verbosity. Defaults to minimal.
.PARAMETER AdditionalArguments
    Additional arguments forwarded unchanged to dotnet test.
.PARAMETER Help
    Display this help and exit without running tests. The alias -? is supported.
.EXAMPLE
    test
    Run the full Debug test suite once, the convention tests included.
.EXAMPLE
    test LTwin
    Run the tests a change inside LTwin can affect, and the convention tests.
.EXAMPLE
    test LTwin -m
    Run the tests a change inside LTwin can affect, without the convention tests.
.EXAMPLE
    test -m
    Run the Debug test suite once, without the convention tests.
.EXAMPLE
    test -c
    Run the convention tests only.
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
    Reuse existing build output and stop at the first failure across ten runs.
.EXAMPLE
    test -List
    List discovered tests without executing them.
.EXAMPLE
    test -AdditionalArguments '--logger', 'trx', '--results-directory', 'TestResults'
    Forward logger and results-directory options to dotnet test.
#>
#requires -Version 5.1
# TEST GENERATION 3 - test.ps1.
# A generation is not a revision count. It names functionality, not edits, so editing one of these
# files is never on its own a reason to raise it. Raise it only when the executed outcome changes.
# A generation names how the family finds and runs the tests. Two projects on the same generation
# run the same projects the same way from the same tree layout.
# Generation 2: every *.csproj one level under the tests folder is a test project, discovered on disk
# and run one by one through dotnet test; by default every project runs, the convention project
# included; -Main skips the convention project, -Convention runs it alone, -Project picks one by name;
# the first failing project ends the run and its exit code is returned.
# Generation 3: everything in generation 2; given symbol names, detector.ps1 names the affected tests,
# each test project runs with a filter selecting them - by method, by class when the method filter
# grows too long, whole when the class filter does too - a project with none is skipped, and the
# convention project runs whole.
# Every project-specific value lives in test.json, so this file is identical in every project at
# this generation.
[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string[]]$Name,

    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',

    [string]$Filter,

    [string]$Project,

    [Alias('f')]
    [switch]$Full,

    [Alias('c')]
    [switch]$Convention,

    [Alias('m')]
    [switch]$Main,

    [switch]$NoBuild,

    [switch]$NoRestore,

    [switch]$List,

    [ValidateRange(1, [int]::MaxValue)]
    [int]$Repeat = 1,

    [ValidateSet('quiet', 'minimal', 'normal', 'detailed', 'diagnostic')]
    [string]$Verbosity = 'minimal',

    [Parameter(ValueFromRemainingArguments)]
    [string[]]$AdditionalArguments,

    [Alias('?')]
    [switch]$Help
)

if ($Help) {
    @'
NAME
    test.ps1

SYNOPSIS
    Run the project test suite.

SYNTAX
    test [<name> ...] [-Configuration <Debug|Release>] [-Filter <expression>]
        [-Project <name>] [-Full | -Convention | -Main]
        [-NoBuild] [-NoRestore] [-List]
        [-Repeat <count>] [-Verbosity <level>]
        [-AdditionalArguments <arguments[]>] [-Help]

OPTIONS
    <name>
        Symbol to start from: a type, method, property or field. Several are
        allowed. Runs only the tests a change inside them can affect, found by
        detector.ps1, and the convention project whole.

    -Configuration <Debug|Release>
        Build configuration. Defaults to Debug.

    -Filter <expression>
        VSTest filter expression used to select tests.

    -Project <name>
        Run one test project under the tests folder. Defaults to all.
        Overrides the scope switches.

    -Full, -f
        Run every test project, the convention project included. Default.

    -Convention, -c
        Run the convention test project only.

    -Main, -m
        Run every test project except the convention project.

    -NoBuild
        Reuse existing build output and skip restore.

    -NoRestore
        Skip restore while still allowing the test project to build.

    -List
        List matching tests without running them. Cannot be used with -Repeat.

    -Repeat <count>
        Number of runs, stopping at the first failure. Defaults to 1.

    -Verbosity <level>
        dotnet test logging verbosity. Defaults to minimal.

    -AdditionalArguments <arguments[]>
        Additional arguments forwarded unchanged to dotnet test.

    -Help, -?
        Display this help and exit without running tests.

EXAMPLES
    test
        Run the full Debug test suite once, the convention tests included.

    test LTwin
        Run the tests a change inside LTwin can affect, and the convention tests.

    test LTwin -m
        Run the tests a change inside LTwin can affect, without the convention tests.

    test -m
        Run the Debug test suite once, without the convention tests.

    test -c
        Run the convention tests only.

    test -Configuration Release
        Run the Release test suite.

    test -Project <name>
        Run one test project.

    test -Filter 'FullyQualifiedName~Tests'
        Run tests whose fully-qualified names match the filter.

    test -Repeat 10 -NoBuild
        Reuse build output and stop at the first failure across ten runs.

    test -List
        List discovered tests without executing them.

    test -AdditionalArguments '--logger', 'trx'
        Forward additional options to dotnet test.
'@ | Write-Host
    exit 0
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# dotnet and git write UTF-8. A console still on the OEM code page, as one opened by the dispatcher
# without a profile is, would show every non-ASCII line garbled, so this process reads and writes
# UTF-8. Process-local: the calling console keeps its own code page.
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [Console]::OutputEncoding

function Read-TestConfig {
    $configPath = Join-Path $PSScriptRoot 'test.json'
    if (-not (Test-Path -LiteralPath $configPath -PathType Leaf)) {
        throw "The test configuration was not found: $configPath"
    }

    try {
        $config = Get-Content -LiteralPath $configPath -Raw -Encoding UTF8 | ConvertFrom-Json
    }
    catch {
        throw "The test configuration is not valid JSON: $configPath`n$($_.Exception.Message)"
    }

    foreach ($key in @('generation', 'project', 'tests', 'convention')) {
        if (-not ($config.PSObject.Properties.Name -contains $key)) {
            throw "The test configuration must contain a $key property: $configPath"
        }
    }

    return $config
}

if (@($Full, $Convention, $Main | Where-Object { $_ }).Count -gt 1) {
    throw '-Full, -Convention and -Main cannot be combined.'
}

$root = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$config = Read-TestConfig
$testsRoot = Join-Path $root ([string]$config.tests)

if (-not (Test-Path -LiteralPath $testsRoot -PathType Container)) {
    throw "The test folder was not found: $testsRoot"
}

$projects = @(
    Get-ChildItem -LiteralPath $testsRoot -Directory |
        ForEach-Object { Get-ChildItem -LiteralPath $_.FullName -Filter '*.csproj' -File } |
        Sort-Object -Property Name
)

if (-not [string]::IsNullOrWhiteSpace($Project)) {
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
elseif ($Main) {
    $projects = @($projects | Where-Object { $_.BaseName -ne [string]$config.convention })
}

if ($projects.Count -eq 0) {
    throw "No test project was found under: $testsRoot"
}

# A filter travels on the dotnet command line, whose length Windows caps near 32K characters, so a
# selection longer than this falls back from methods to classes, and from classes to the whole project.
$selectionLimit = 20000
$selections = @{}

if ($null -ne $Name -and $Name.Count -gt 0) {
    $detector = Join-Path $PSScriptRoot 'detector.ps1'
    if (-not (Test-Path -LiteralPath $detector -PathType Leaf)) {
        throw "The detector was not found: $detector"
    }

    $detection = & $detector -Name $Name -Json | Out-String | ConvertFrom-Json
    $detected = @($detection.tests)
    Write-Host ("{0}: {1} affected symbol(s), {2} affected test(s)." -f ($Name -join ', '), @($detection.symbols).Count, $detected.Count) -ForegroundColor Cyan

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

            $selection = @($selected | ForEach-Object { "FullyQualifiedName~.$($_.class).$($_.method)" } | Sort-Object -Unique) -join '|'
            if ($selection.Length -gt $selectionLimit) {
                $selection = @($selected | ForEach-Object { "FullyQualifiedName~.$($_.class)." } | Sort-Object -Unique) -join '|'
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

$commonArguments = @(
    '--configuration'
    $Configuration
    '--verbosity'
    $Verbosity
    '--nologo'
)

if ($NoBuild) {
    $commonArguments += '--no-build'
}
elseif ($NoRestore) {
    $commonArguments += '--no-restore'
}

if ($List) {
    $commonArguments += '--list-tests'
}

if ($null -ne $AdditionalArguments -and $AdditionalArguments.Count -gt 0) {
    $commonArguments += $AdditionalArguments
}

$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$exitCode = 0

Push-Location $root
try {
    for ($run = 1; $run -le $Repeat; $run++) {
        if ($Repeat -gt 1) {
            Write-Host "Test run $run of $Repeat" -ForegroundColor Cyan
        }

        foreach ($testProject in $projects) {
            Write-Host $testProject.BaseName -ForegroundColor Cyan

            $projectFilter = $Filter
            if ($selections.ContainsKey($testProject.BaseName)) {
                $selection = $selections[$testProject.BaseName]
                $projectFilter = if ([string]::IsNullOrWhiteSpace($Filter)) { $selection } else { "($Filter)&($selection)" }
            }

            $filterArguments = @()
            if (-not [string]::IsNullOrWhiteSpace($projectFilter)) {
                $filterArguments = @('--filter', $projectFilter)
            }

            & $dotnet.Source @('test', $testProject.FullName) @filterArguments @commonArguments
            $exitCode = $LASTEXITCODE

            if ($exitCode -ne 0) {
                Write-Host "Tests failed in $($testProject.BaseName) on run $run of $Repeat (exit code $exitCode)." -ForegroundColor Red
                break
            }
        }

        if ($exitCode -ne 0) {
            break
        }
    }
}
finally {
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
