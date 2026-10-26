<#
.SYNOPSIS
Scans the tests for problematic tests and reports each finding as a lead for a reader to judge.

.DESCRIPTION
Reads the configuration from ScanTest.json next to this script, then performs these actions:
  1. Reads the kinds document: the twelve kinds in fixed order, each with its hits.
  2. Prints one progress line per kind, the counters and the first lines of each kind.
  3. Writes the pages <report.directory>/ScanTest-<version>.md, .json and .html; the .html is the
     template ScanTest.html with the .json document embedded.
A scan is not an audit: it has no ceilings, no ledger and no failing status, and every count
is informational. It exits 0 whenever the scan completes, and non-zero only on an error.
ScanTest never runs tests; it reads the .trx files earlier runs left.

The scanner is a small C# helper embedded in this script. It lists the test files through git,
parses them with Roslyn, syntax only, and writes the kinds document as JSON. It is built once
into the temporary folder, cached under a hash of its text, the target framework and the SDK
version, and reused on later runs. The .NET SDK and git are the external tools required.

.PARAMETER ConfigPath
Path to the JSON configuration. Defaults to ScanTest.json next to this script.

.PARAMETER Trx
.trx files or folders to read; a folder gives the .trx files directly inside it. Defaults to
every .trx directly inside the folder named by the trx key of the configuration.

.PARAMETER NoHistory
Skip the git history read of Dead Target; every candidate then reports as unmatched.

.PARAMETER ReportDirectory
Folder the pages are written to. Overrides report.directory.

.PARAMETER Top
Overrides top: how many lines each kind shows on the console.

.PARAMETER Open
Open the Markdown report as well as the HTML page once the run finishes.

.PARAMETER NoOpen
Write the pages without opening the HTML page.

.PARAMETER NoPause
Do not stop at each console page. Paging is also off when output or input is redirected.

.PARAMETER Help
Display this help and exit without running. The alias -? is supported.

.EXAMPLE
ScanTest

.EXAMPLE
ScanTest -Trx TestResults -NoHistory -NoOpen
#>
#requires -Version 5.1
# SCANTEST - SCAN GENERATION 1.
# A generation is not a revision count. It names functionality, not edits, so editing this file is
# never on its own a reason to raise it. Raise it only when the kinds or the written documents change.
# Generation 1: the twelve kinds Dead Target, Skipped Test, Slow Test, Flaky Result, Blind Test,
# No Assert, Self Echo, Wrong Layer, Fixed Sleep, Long Wait, Copied Body and Unused Helper, read
# from syntax, git history and trx.
[CmdletBinding()]
param(
    [string]$ConfigPath = '',
    [string[]]$Trx = @(),
    [switch]$NoHistory,
    [string]$ReportDirectory = '',
    [ValidateRange(1, [int]::MaxValue)]
    [int]$Top,
    [switch]$Open,
    [switch]$NoOpen,
    [switch]$NoPause,
    [Alias('?')]
    [switch]$Help
)

if ($Help) {
    @'
NAME
    ScanTest.ps1

SYNOPSIS
    Scan the tests for problematic tests: each finding is a lead for a
    reader to judge, not a violation.

SYNTAX
    ScanTest [-ConfigPath <path>] [-Trx <path[]>] [-NoHistory]
             [-ReportDirectory <path>] [-Top <number>] [-Open] [-NoOpen]
             [-NoPause] [-Help]

CONFIGURATION
    All project-specific values live in ScanTest.json next to the script:
    the test, source and trx folders, the pool folders, the console row count,
    the report directory, version file and key, the report file-name prefix,
    the helper target framework, and the settings of each kind.

SCAN
    The kinds, in order: Dead Target, Skipped Test, Slow Test, Flaky Result,
    Blind Test, No Assert, Self Echo, Wrong Layer, Fixed Sleep, Long Wait,
    Copied Body and Unused Helper. No ceiling, no ledger, no pass or fail:
    every count is informational. The exit code is 0 unless the run itself
    breaks. ScanTest never runs tests.

OPTIONS
    -ConfigPath <path>
        Path to the JSON configuration. Defaults to ScanTest.json.

    -Trx <path[]>
        .trx files or folders to read; a folder gives the .trx files
        directly inside it. Defaults to every .trx directly inside the
        folder named by the trx key of the configuration, never in a subfolder.

    -NoHistory
        Skip the git history read of Dead Target.

    -ReportDirectory <path>
        Folder the pages are written to. Overrides report.directory.

    -Top <number>
        Lines each kind shows on the console. Overrides top. The pages
        always hold every line.

    -Open
        Open the Markdown report as well as the HTML page once the run
        finishes.

    -NoOpen
        Write the pages without opening the HTML page.

    -NoPause
        Do not stop at each console page for a key. Off by itself when output
        or input is redirected.

    -Help, -?
        Display this help and exit without running.

EXAMPLES
    ScanTest
        Scan with the configured settings.

    ScanTest -Trx TestResults -NoHistory
        Read every .trx directly in TestResults and skip the history read.
'@ | Write-Host
    exit 0
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [Console]::OutputEncoding

$script:ScanGeneration = 1
$script:ScanKinds = @('DeadTarget', 'SkippedTest', 'SlowTest', 'FlakyResult', 'BlindTest', 'NoAssert', 'SelfEcho', 'WrongLayer', 'FixedSleep', 'LongWait', 'CopiedBody', 'UnusedHelper')
$script:Invariant = [System.Globalization.CultureInfo]::InvariantCulture
$script:Utf8 = [System.Text.UTF8Encoding]::new($false)

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

function Write-ScanLine {
    param(
        [Parameter(Position = 0)][AllowEmptyString()][string]$Text = '',
        [ConsoleColor]$ForegroundColor
    )

    if ($script:PageLimit -gt 0) {
        $rows = [Math]::Max(1, [Math]::Ceiling($Text.Length / [double]$script:PageWidth))
        if ($script:PageCount + $rows -gt $script:PageLimit -and $script:PageCount -gt 0) {
            $prompt = '-- More -- (any key: next page, Q: no more pauses)'
            Write-Host $prompt -ForegroundColor DarkGray -NoNewline
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

Write-ScanLine "SCANTEST - SCAN GENERATION $script:ScanGeneration" -ForegroundColor Blue

function Get-ConfigNode {
    param(
        [Parameter(Mandatory = $true)]$Document,
        [Parameter(Mandatory = $true)][string]$Key
    )

    $node = $Document
    foreach ($segment in ($Key -split '\.')) {
        if ($null -eq $node -or -not ($node.PSObject.Properties.Name -ccontains $segment)) {
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
        if ($property.Value -is [System.Management.Automation.PSCustomObject]) {
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
        'positive' { return ($Value -is [int] -or $Value -is [long]) -and $Value -ge 1 }
        'seconds' { return ($Value -is [int] -or $Value -is [long] -or $Value -is [double] -or $Value -is [decimal]) -and $Value -gt 0 }
        'pattern' {
            if (-not ($Value -is [string]) -or [string]::IsNullOrWhiteSpace($Value)) { return $false }
            try { [void][regex]::new($Value, [System.Text.RegularExpressions.RegexOptions]::CultureInvariant) } catch { return $false }
            return $true
        }
        'strings' {
            if ($null -eq $Value -or -not ($Value -is [System.Array]) -or $Value.Count -eq 0) { return $false }
            foreach ($item in $Value) {
                if (-not ($item -is [string]) -or [string]::IsNullOrWhiteSpace($item)) { return $false }
            }
            return $true
        }
        default { throw "Unknown schema kind: $Kind" }
    }
}

function Read-ScanConfig {
    param([Parameter(Mandatory = $true)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "The test-scan configuration was not found: $Path"
    }

    try {
        $config = Get-Content -LiteralPath $Path -Raw -Encoding UTF8 | ConvertFrom-Json
    }
    catch {
        throw "The test-scan configuration is not valid JSON: $Path`n$($_.Exception.Message)"
    }

    $problems = [System.Collections.Generic.List[string]]::new()
    $schema = [ordered]@{
        'generation' = 'int'; 'project' = 'string'; 'tests' = 'string'; 'source' = 'string'; 'pool' = 'strings'; 'trx' = 'string'; 'top' = 'positive'
        'report.directory' = 'string'; 'report.versionFile' = 'string'; 'report.versionKey' = 'string'; 'report.prefix' = 'string'
        'helper.framework' = 'string'
        'kinds.SlowTest.testSeconds' = 'seconds'; 'kinds.SlowTest.classSeconds' = 'seconds'
        'kinds.WrongLayer.bootTypes' = 'strings'; 'kinds.WrongLayer.projects' = 'strings'; 'kinds.WrongLayer.uiPrefixes' = 'pattern'
        'kinds.WrongLayer.logicPrefix' = 'pattern'; 'markers' = 'strings'; 'kinds.BlindTest.theoryMarker' = 'string'; 'kinds.BlindTest.dataSuffix' = 'string'
        'kinds.WrongLayer.assertedPattern' = 'pattern'; 'kinds.WrongLayer.drawPattern' = 'pattern'
        'kinds.CopiedBody.minTokens' = 'positive'
        'kinds.DeadTarget.prefixPattern' = 'pattern'; 'kinds.DeadTarget.dottedPattern' = 'pattern'; 'kinds.DeadTarget.fileEndingPattern' = 'pattern'
        'kinds.DeadTarget.poolExtensions' = 'strings'; 'kinds.DeadTarget.looseCapitals' = 'positive'
        'kinds.SkippedTest.commentedPattern' = 'pattern'
        'kinds.BlindTest.broadThrows' = 'strings'; 'kinds.BlindTest.countPins' = 'strings'
        'kinds.FixedSleep.sleepPattern' = 'pattern'; 'kinds.FixedSleep.delayPattern' = 'pattern'
        'kinds.LongWait.timeoutPattern' = 'pattern'; 'kinds.LongWait.spinPattern' = 'pattern'; 'kinds.LongWait.blockingPattern' = 'pattern'
        'kinds.LongWait.pollingPattern' = 'pattern'; 'kinds.LongWait.clockPattern' = 'pattern'; 'kinds.LongWait.guardPattern' = 'pattern'
        'kinds.UnusedHelper.lifetimeNames' = 'strings'
    }

    $present = Get-ConfigLeaves -Node $config
    foreach ($key in $schema.Keys) {
        if ($key -cnotin $present) {
            $problems.Add("missing key '$key'")
        }
        elseif (-not (Test-ConfigValue -Value (Get-ConfigNode -Document $config -Key $key) -Kind $schema[$key])) {
            $problems.Add("key '$key' is not a valid $($schema[$key])")
        }
    }
    foreach ($key in $present) {
        if (-not $schema.Contains($key)) { $problems.Add("unknown key '$key'") }
    }

    $kinds = Get-ConfigNode -Document $config -Key 'kinds'
    if ($kinds -is [System.Management.Automation.PSCustomObject]) {
        foreach ($kind in $script:ScanKinds) {
            if (-not ((Get-ConfigNode -Document $kinds -Key $kind) -is [System.Management.Automation.PSCustomObject])) { $problems.Add("key 'kinds.$kind' is not an object") }
        }
    }
    else {
        $problems.Add("key 'kinds' is not an object")
    }

    if ($problems.Count -gt 0) {
        throw "The test-scan configuration is not valid: $Path`n  " + ($problems -join "`n  ")
    }
    if ([int]$config.generation -ne $script:ScanGeneration) {
        throw "The test-scan configuration is generation $($config.generation); this script is generation $script:ScanGeneration : $Path"
    }

    return $config
}

function Join-ScanPath {
    param(
        [Parameter(Mandatory = $true)][string]$Root,
        [Parameter(Mandatory = $true)][string]$Relative
    )

    if ([System.IO.Path]::IsPathRooted($Relative)) { return [System.IO.Path]::GetFullPath($Relative) }
    return [System.IO.Path]::GetFullPath((Join-Path $Root $Relative))
}

function Get-ScanVersion {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Key
    )

    $version = '0.0.0'
    if (-not [System.IO.File]::Exists($Path)) {
        Write-ScanLine "The version file was not found, using $version : $Path"
        return $version
    }
    try {
        $value = Get-ConfigNode -Document (Get-Content -LiteralPath $Path -Raw -Encoding UTF8 | ConvertFrom-Json) -Key $Key
        if (-not [string]::IsNullOrWhiteSpace([string]$value)) { $version = [string]$value }
    }
    catch {
        Write-ScanLine "The version file is not valid JSON, using $version : $Path"
    }
    return $version
}

function Resolve-TrxPaths {
    param(
        [Parameter(Mandatory = $true)][string]$Root,
        [Parameter(Mandatory = $true)][string]$Default,
        [string[]]$Paths = @()
    )

    $folder = Join-ScanPath -Root $Root -Relative $Default
    if ($Paths.Count -eq 0) {
        if (-not [System.IO.Directory]::Exists($folder)) { return @() }
        $Paths = @($folder)
    }
    $files = foreach ($path in $Paths) {
        $full = Join-ScanPath -Root $Root -Relative $path
        if ([System.IO.File]::Exists($full)) { $full }
        elseif ([System.IO.Directory]::Exists($full)) { [System.IO.Directory]::GetFiles($full, '*.trx', [System.IO.SearchOption]::TopDirectoryOnly) }
        else { throw "The trx path was not found: $full" }
    }
    # Sort-Object compares by culture and its -Unique ignores case, so the list is sorted and made unique ordinally.
    $unique = [System.Collections.Generic.List[string]]::new()
    $seen = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($file in @($files)) {
        if ($seen.Add([string]$file)) { $unique.Add([string]$file) }
    }
    $unique.Sort([System.StringComparer]::Ordinal)
    return @($unique)
}

function ConvertTo-ScanJson {
    param(
        [AllowNull()][object]$Value,
        [string]$Indent = ''
    )

    if ($null -eq $Value) { return 'null' }
    if ($Value -is [bool]) { return $(if ($Value) { 'true' } else { 'false' }) }
    if ($Value -is [string]) {
        $builder = [System.Text.StringBuilder]::new('"')
        foreach ($character in $Value.ToCharArray()) {
            $code = [int]$character
            if ($character -eq '"') { [void]$builder.Append('\"') }
            elseif ($character -eq '\') { [void]$builder.Append('\\') }
            elseif ($code -lt 0x20 -or $code -gt 0x7E -or $character -eq '<' -or $character -eq '>' -or $character -eq '&') { [void]$builder.Append('\u' + $code.ToString('x4')) }
            else { [void]$builder.Append($character) }
        }
        return $builder.Append('"').ToString()
    }
    if ($Value -is [int] -or $Value -is [long]) { return $Value.ToString($script:Invariant) }
    if ($Value -is [double] -or $Value -is [decimal]) { return ([double]$Value).ToString('0.##########', $script:Invariant) }
    $inner = $Indent + '  '
    if ($Value -is [System.Collections.IDictionary] -or $Value -is [System.Management.Automation.PSCustomObject]) {
        $names = @(if ($Value -is [System.Collections.IDictionary]) { @($Value.get_Keys()) } else { @($Value.PSObject.Properties | ForEach-Object { $_.Name }) })
        if ($names.Count -eq 0) { return '{}' }
        $pairs = foreach ($name in $names) { $inner + (ConvertTo-ScanJson ([string]$name)) + ': ' + (ConvertTo-ScanJson $Value.$name $inner) }
        return "{`n" + ($pairs -join ",`n") + "`n" + $Indent + '}'
    }
    if ($Value -is [System.Collections.IEnumerable]) {
        $items = @(foreach ($item in $Value) { $inner + (ConvertTo-ScanJson $item $inner) })
        if ($items.Count -eq 0) { return '[]' }
        return "[`n" + ($items -join ",`n") + "`n" + $Indent + ']'
    }
    throw "The scan document holds a value of an unexpected type: $($Value.GetType().FullName)"
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
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// Reads the settings, lists the test files through git, parses them without binding, and writes
// one JSON document: the test projects, the counters and the twelve kinds with their lines.
internal static class Program
{
    internal static readonly string[] KindKeys =
    {
        "DeadTarget", "SkippedTest", "SlowTest", "FlakyResult", "BlindTest", "NoAssert",
        "SelfEcho", "WrongLayer", "FixedSleep", "LongWait", "CopiedBody", "UnusedHelper"
    };

    internal static readonly (string Key, string Name)[] CounterNames =
    {
        ("tests", "Tests"), ("helpers", "Helpers"), ("assertingHelpers", "Asserting helpers"), ("bootingHelpers", "Booting helpers"),
        ("trxFiles", "Trx files"), ("trxResults", "Trx results"), ("historyLines", "History lines"), ("parseErrors", "Parse errors"), ("notices", "Notices")
    };

    // A read that failed does not abort the scan: it becomes a notice line and a count.
    private static readonly List<string> Notices = new();

    private static void Notice(string text)
    {
        Notices.Add(Flatten(text));
        Counters["notices"] = Notices.Count;
    }

    // An assert of these kinds fails on a missing or empty collection, so a loop holding it still pins the count.
    // The patterns, word lists and thresholds of each kind come from the settings, read once in Main.
    private static JsonElement KindSettings;

    private static JsonElement KindSetting(string kind, string key)
    {
        if (KindSettings.ValueKind == JsonValueKind.Object
            && KindSettings.TryGetProperty(kind, out JsonElement group) && group.ValueKind == JsonValueKind.Object
            && group.TryGetProperty(key, out JsonElement value))
        {
            return value;
        }

        throw new InvalidOperationException($"The scan settings lack the key 'kinds.{kind}.{key}'; add it to the configuration.");
    }

    private static Regex KindPattern(string kind, string key)
    {
        try
        {
            return new Regex(KindSetting(kind, key).GetString()!, RegexOptions.CultureInvariant);
        }
        catch (ArgumentException problem)
        {
            throw new InvalidOperationException($"The scan setting 'kinds.{kind}.{key}' is not a valid pattern: {problem.Message}");
        }
    }

    private static HashSet<string> KindWords(string kind, string key) =>
        new(KindSetting(kind, key).EnumerateArray().Select(item => item.GetString()!), StringComparer.Ordinal);

    private static HashSet<string> CountPins = new(StringComparer.Ordinal);

    private static HashSet<string> BroadThrows = new(StringComparer.Ordinal);

    private static HashSet<string> Markers = new(StringComparer.Ordinal);

    private static string TheoryMarker = "";

    private static string DataSuffix = "";

    private static Regex LogicPrefix = new("^$");

    private static Regex CommentedTest = new("^$");

    private static readonly Dictionary<string, List<ScanLine>> Kinds = KindKeys.ToDictionary(key => key, _ => new List<ScanLine>(), StringComparer.Ordinal);

    private static readonly Dictionary<string, long> Counters = CounterNames.ToDictionary(counter => counter.Key, _ => 0L, StringComparer.Ordinal);

    private static HashSet<string> PoolExtensions = new(StringComparer.Ordinal);

    private static HashSet<string> LifetimeNames = new(StringComparer.Ordinal);

    private static Regex PrefixLetter = new("^$");
    private static Regex DottedKey = new("^$");
    private static Regex FileEnding = new("^$");
    private static readonly Regex Word = new(@"[A-Za-z0-9_]+", RegexOptions.CultureInvariant);
    private static Regex Sleep = new("^$");
    private static Regex Delay = new("^$");
    private static Regex Clock = new("^$");
    private static Regex PollingBody = new("^$");
    private static Regex GuardSeconds = new("^$");
    private static (string Detail, Regex Pattern)[] WaitRules = Array.Empty<(string, Regex)>();
    private static int LooseCapitals;

    private static HashSet<string> Booting = new(StringComparer.Ordinal);
    private static HashSet<string> WrongProjects = new(StringComparer.Ordinal);
    private static Regex UiPrefixes = new("^$");
    private static Regex AssertedName = new("^$");
    private static Regex DrawName = new("^$");
    private static int MinTokens;
    private static string TestsRoot = "";

    private static int Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.Error.WriteLine("Usage: ScanTest.Helper <repository-root> <settings-json> <output-json>");
            return 2;
        }

        string root = Path.GetFullPath(args[0]);
        using JsonDocument settings = JsonDocument.Parse(File.ReadAllText(args[1], Encoding.UTF8));
        JsonElement setting = settings.RootElement;
        string tests = setting.GetProperty("tests").GetString()!.Replace('\\', '/').Trim('/');
        string source = setting.GetProperty("source").GetString()!.Replace('\\', '/').Trim('/');
        var bootTypes = new HashSet<string>(setting.GetProperty("bootTypes").EnumerateArray().Select(item => item.GetString()!), StringComparer.Ordinal);
        WrongProjects = new HashSet<string>(setting.GetProperty("wrongProjects").EnumerateArray().Select(item => item.GetString()!), StringComparer.Ordinal);
        KindSettings = setting.GetProperty("kinds");
        Markers = new HashSet<string>(setting.GetProperty("markers").EnumerateArray().Select(item => item.GetString()!), StringComparer.Ordinal);
        TheoryMarker = KindSetting("BlindTest", "theoryMarker").GetString()!;
        DataSuffix = KindSetting("BlindTest", "dataSuffix").GetString()!;
        LogicPrefix = KindPattern("WrongLayer", "logicPrefix");
        UiPrefixes = KindPattern("WrongLayer", "uiPrefixes");
        AssertedName = KindPattern("WrongLayer", "assertedPattern");
        DrawName = KindPattern("WrongLayer", "drawPattern");
        MinTokens = KindSetting("CopiedBody", "minTokens").GetInt32();
        PrefixLetter = KindPattern("DeadTarget", "prefixPattern");
        DottedKey = KindPattern("DeadTarget", "dottedPattern");
        FileEnding = KindPattern("DeadTarget", "fileEndingPattern");
        PoolExtensions = KindWords("DeadTarget", "poolExtensions");
        LooseCapitals = KindSetting("DeadTarget", "looseCapitals").GetInt32();
        CommentedTest = KindPattern("SkippedTest", "commentedPattern");
        BroadThrows = KindWords("BlindTest", "broadThrows");
        CountPins = KindWords("BlindTest", "countPins");
        Sleep = KindPattern("FixedSleep", "sleepPattern");
        Delay = KindPattern("FixedSleep", "delayPattern");
        Clock = KindPattern("LongWait", "clockPattern");
        PollingBody = KindPattern("LongWait", "pollingPattern");
        GuardSeconds = KindPattern("LongWait", "guardPattern");
        WaitRules = new[]
        {
            ("timeout", KindPattern("LongWait", "timeoutPattern")),
            ("spin", KindPattern("LongWait", "spinPattern")),
            ("blocking-wait", KindPattern("LongWait", "blockingPattern"))
        };
        LifetimeNames = KindWords("UnusedHelper", "lifetimeNames");
        bool history = setting.GetProperty("history").GetBoolean();
        string[] trxPaths = setting.GetProperty("trx").EnumerateArray().Select(item => item.GetString()!).ToArray();
        double testSeconds = setting.GetProperty("testSeconds").GetDouble();
        double classSeconds = setting.GetProperty("classSeconds").GetDouble();

        List<string> listed = GitFiles(root, new[] { tests + "/*.cs", tests + "/*.csproj", tests + "/*.xaml" });
        string markup = string.Concat(listed.Where(path => path.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase)).Select(path => ReadText(root, path) + "\n"));
        string[] sourceFiles = GitFiles(root, new[] { source + "/*.cs" }).ToArray();
        string pool = string.Concat(GitFiles(root, setting.GetProperty("pool").EnumerateArray().Select(item => item.GetString()!.Replace('\\', '/').Trim('/') + "/*"))
            .Where(path => PoolExtensions.Contains(Path.GetExtension(path).ToLowerInvariant()))
            .Distinct(StringComparer.Ordinal)
            .Select(path => ReadText(root, path) + "\n"));
        string[] codeFiles = listed.Where(path => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)).ToArray();
        string[] projects = listed
            .Where(path => path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            .Select(path => path.Substring(tests.Length + 1).Split('/'))
            .Where(parts => parts.Length == 2)
            .Select(parts => parts[0])
            .Distinct(StringComparer.Ordinal)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        var options = new CSharpParseOptions(LanguageVersion.Preview, DocumentationMode.None);
        var trees = new SyntaxTree[codeFiles.Length];
        Parallel.For(0, codeFiles.Length, i => trees[i] = CSharpSyntaxTree.ParseText(ReadText(root, codeFiles[i]), options, codeFiles[i]));
        var sourceTrees = new SyntaxTree[sourceFiles.Length];
        Parallel.For(0, sourceFiles.Length, i => sourceTrees[i] = CSharpSyntaxTree.ParseText(ReadText(root, sourceFiles[i]), options, sourceFiles[i]));
        Counters["parseErrors"] = trees.Concat(sourceTrees).Sum(tree => tree.GetDiagnostics().Count(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));

        var methods = trees.SelectMany(tree => tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()).ToList();
        var testMethods = methods.Where(IsTest).ToList();
        var helperMethods = methods.Where(method => !IsTest(method)).ToList();
        Counters["tests"] = testMethods.Count;
        Counters["helpers"] = helperMethods.Count;

        var bodies = helperMethods.Select(method => new HelperBody(method.Identifier.ValueText, ProjectOf(method.SyntaxTree.FilePath, tests), method))
            .Concat(trees.SelectMany(tree => tree.GetRoot().DescendantNodes().OfType<LocalFunctionStatementSyntax>())
                .Select(local => new HelperBody(local.Identifier.ValueText, ProjectOf(local.SyntaxTree.FilePath, tests), local)))
            .ToList();
        HashSet<string> asserting = Fixpoint(bodies, body => Calls(body.Node).Any(IsAssertCall), body => body.Name);
        var throwing = new HashSet<string>(bodies.Where(body => body.Node.DescendantNodes().Any(node => node is ThrowStatementSyntax or ThrowExpressionSyntax)).Select(body => body.Name), StringComparer.Ordinal);
        Booting = Fixpoint(bodies, body => Constructs(body.Node, bootTypes), body => body.Project + "/" + body.Name);
        Counters["assertingHelpers"] = asserting.Count;
        Counters["bootingHelpers"] = Booting.Count;
        TestsRoot = tests;

        foreach (SyntaxTree tree in trees)
        {
            foreach (var line in tree.GetText().Lines)
            {
                string text = line.ToString();
                if (CommentedTest.IsMatch(text)) Add("SkippedTest", tree.FilePath, line.LineNumber + 1, "", "commented", "a test attribute is commented out: " + Flatten(text));
            }
        }

        foreach (MethodDeclarationSyntax test in testMethods) ScanTest(test, asserting, throwing);
        HashSet<string>? removedTokens = null;
        if (history)
        {
            try
            {
                removedTokens = HistoryTokens(root, source);
            }
            catch (Exception failure) when (failure is InvalidOperationException or System.ComponentModel.Win32Exception or IOException)
            {
                Notice("The history was not read, so Dead Target reports every candidate as unmatched: " + failure.Message);
                history = false;
            }
        }

        DeadTargets(testMethods, trees, sourceTrees, pool, removedTokens);
        foreach (SyntaxTree tree in trees) WaitLines(tree);
        CopiedBodies(testMethods);
        UnusedHelpers(trees, markup + pool);
        TrxKinds(trxPaths, testMethods, testSeconds, classSeconds);

        using (FileStream stream = File.Create(args[2]))
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
        {
            writer.WriteStartObject();
            writer.WriteStartArray("projects");
            foreach (string project in projects) writer.WriteStringValue(project);
            writer.WriteEndArray();
            writer.WriteStartArray("trx");
            foreach (string name in TrxNames) writer.WriteStringValue(name);
            writer.WriteEndArray();
            writer.WriteBoolean("history", history);
            writer.WriteStartArray("notices");
            foreach (string notice in Notices) writer.WriteStringValue(notice);
            writer.WriteEndArray();
            writer.WriteStartArray("counters");
            foreach (var (key, name) in CounterNames)
            {
                writer.WriteStartObject();
                writer.WriteString("key", key);
                writer.WriteString("name", name);
                writer.WriteNumber("count", Counters[key]);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.WriteStartArray("kinds");
            foreach (string key in KindKeys) WriteKind(writer, key, Kinds[key]);
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        return 0;
    }

    private static void ScanTest(MethodDeclarationSyntax test, HashSet<string> asserting, HashSet<string> throwing)
    {
        string file = test.SyntaxTree.FilePath;
        string name = test.Identifier.ValueText;
        int line = LineOf(test.Identifier.GetLocation());
        var attributes = test.AttributeLists.SelectMany(list => list.Attributes).ToList();

        foreach (AttributeSyntax attribute in attributes)
        {
            AttributeArgumentSyntax? skip = attribute.ArgumentList?.Arguments.FirstOrDefault(argument => argument.NameEquals?.Name.Identifier.ValueText == "Skip");
            if (skip != null) Add("SkippedTest", file, LineOf(attribute.GetLocation()), name, "skip", "the test is skipped: " + Flatten(skip.Expression.ToString()));
        }

        SyntaxNode? body = (SyntaxNode?)test.Body ?? test.ExpressionBody;
        if (body == null) return;
        var calls = Calls(body).ToList();
        var asserts = calls.Where(IsAssertCall).ToList();
        bool direct = asserts.Count > 0;
        bool viaHelper = calls.Any(call => asserting.Contains(CalleeName(call)));

        if (!direct && !viaHelper)
        {
            bool throwsOnly = calls.Any(call => throwing.Contains(CalleeName(call)));
            Add("NoAssert", file, line, name, throwsOnly ? "helper-throws" : "none",
                throwsOnly ? "the only check is a helper that throws" : $"no assert among {calls.Count} calls");
        }
        else if (direct && !viaHelper && asserts.All(assert => assert.ArgumentList.Arguments.All(argument => SelfValue(argument.Expression, body))
            && assert.ArgumentList.Arguments.Any(argument => argument.Expression is IdentifierNameSyntax)
            && ((MemberAccessExpressionSyntax)assert.Expression).Name.Identifier.ValueText is not ("Fail" or "Skip")))
        {
            Add("SelfEcho", file, LineOf(asserts[0].GetLocation()), name, "literals", "every assert compares values the test set itself: " + Flatten(asserts[0].ToString()));
        }

        foreach (InvocationExpressionSyntax assert in asserts)
        {
            var arguments = assert.ArgumentList.Arguments.Select(argument => argument.Expression.ToString()).ToList();
            var access = (MemberAccessExpressionSyntax)assert.Expression;
            string operation = access.Name.Identifier.ValueText;
            string detail = "";
            if ((operation == "True" && arguments.FirstOrDefault() == "true") || (operation == "False" && arguments.FirstOrDefault() == "false")) detail = "tautology";
            else if (operation is "Equal" or "Same" or "StrictEqual" && arguments.Count >= 2 && arguments[0] == arguments[1]) detail = "self-compare";
            else if (operation == "NotNull" && assert.ArgumentList.Arguments.FirstOrDefault()?.Expression is BaseObjectCreationExpressionSyntax) detail = "notnull-new";
            else if (BroadThrows.Contains(operation) && access.Name is GenericNameSyntax generic && generic.TypeArgumentList.Arguments.ToString() is "Exception" or "System.Exception") detail = "broad-throws";
            if (detail.Length > 0) Add("BlindTest", file, LineOf(assert.GetLocation()), name, detail, Flatten(assert.ToString()));
        }

        if (direct)
        {
            if (asserts.All(assert => assert.Ancestors().TakeWhile(node => node != body).Any(node => node is IfStatementSyntax or ConditionalExpressionSyntax or SwitchSectionSyntax)))
            {
                Add("BlindTest", file, line, name, "guarded-asserts", "every assert sits under an if");
            }
            else
            {
                var loops = asserts.Select(assert => assert.Ancestors().TakeWhile(node => node != body).Where(node => node is CommonForEachStatementSyntax or ForStatementSyntax).ToList()).ToList();
                bool pinned = Enumerable.Range(0, asserts.Count).Any(index => loops[index].Count == 0 && CountPins.Contains(((MemberAccessExpressionSyntax)asserts[index].Expression).Name.Identifier.ValueText));
                var read = loops.SelectMany(list => list).Distinct().Where(loop => !LiteralSource(loop, body)).OrderBy(loop => loop.SpanStart).ToList();
                if (!pinned && loops.All(list => list.Count > 0) && read.Count > 0)
                {
                    Add("BlindTest", file, line, name, "loop-only-asserts", "every assert sits in a loop over a production read: " + Flatten(LoopHead(read[0])));
                }
            }
        }

        foreach (CatchClauseSyntax handler in body.DescendantNodes().OfType<CatchClauseSyntax>())
        {
            bool rethrows = handler.Block.DescendantNodes().Any(node => node is ThrowStatementSyntax or ThrowExpressionSyntax);
            bool captures = handler.Declaration is { } declaration && declaration.Identifier.ValueText.Length > 0
                && handler.Block.DescendantNodes().OfType<IdentifierNameSyntax>().Any(identifier => identifier.Identifier.ValueText == declaration.Identifier.ValueText);
            if (!rethrows && !captures && !Calls(handler.Block).Any(IsAssertCall))
            {
                Add("BlindTest", file, LineOf(handler.GetLocation()), name, "swallowing-catch", "the catch neither rethrows, asserts nor captures: " + Flatten(handler.ToString().Split('\n')[0]));
            }
        }

        foreach (ReturnStatementSyntax exit in body.DescendantNodes().OfType<ReturnStatementSyntax>())
        {
            if (exit.Parent is not (BlockSyntax { Parent: IfStatementSyntax } or IfStatementSyntax)) continue;
            if (exit.Ancestors().Any(node => node is LambdaExpressionSyntax or AnonymousMethodExpressionSyntax or LocalFunctionStatementSyntax)) continue;
            IfStatementSyntax guard = exit.Ancestors().OfType<IfStatementSyntax>().First();
            if (asserts.Any(assert => assert.SpanStart > exit.SpanStart) && !Calls(guard.Statement).Any(IsAssertCall))
            {
                Add("BlindTest", file, LineOf(exit.GetLocation()), name, "early-return", "returns before later asserts: " + Flatten(guard.ToString().Split('\n')[0]));
            }
        }

        if (attributes.Any(attribute => SimpleName(attribute.Name).EndsWith(TheoryMarker, StringComparison.Ordinal))
            && !attributes.Any(attribute => SimpleName(attribute.Name).EndsWith(DataSuffix, StringComparison.Ordinal)))
        {
            Add("BlindTest", file, line, name, "theory-no-data", "a theory without any data attribute");
        }

        string project = ProjectOf(file, TestsRoot);
        if (direct && WrongProjects.Contains(project) && calls.Any(call => Booting.Contains(project + "/" + CalleeName(call))))
        {
            var asserted = asserts.SelectMany(assert => assert.ArgumentList.DescendantNodes().OfType<SimpleNameSyntax>())
                .Select(simple => simple.Identifier.ValueText)
                .Where(text => AssertedName.IsMatch(text))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(text => text, StringComparer.Ordinal)
                .ToList();
            var logic = asserted.Where(text => LogicPrefix.IsMatch(text)).ToList();
            var lambdaParameters = new HashSet<string>(body.DescendantNodes().OfType<ParameterSyntax>()
                .Where(parameter => parameter.Parent?.Parent is LambdaExpressionSyntax || parameter.Parent is SimpleLambdaExpressionSyntax)
                .Select(parameter => parameter.Identifier.ValueText), StringComparer.Ordinal);
            bool touchesUi = body.DescendantNodes().OfType<SimpleNameSyntax>().Select(simple => simple.Identifier.ValueText)
                    .Any(text => UiPrefixes.IsMatch(text) || DrawName.IsMatch(text))
                || calls.Any(call => call.Expression is IdentifierNameSyntax callee && lambdaParameters.Contains(callee.Identifier.ValueText));
            if (logic.Count > 0 && !touchesUi)
            {
                Add("WrongLayer", file, line, name, "hops-" + logic.Count.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    "boots the interface to assert logic names only: " + string.Join(", ", logic));
            }
        }
    }

    private static void DeadTargets(List<MethodDeclarationSyntax> testMethods, SyntaxTree[] testTrees, SyntaxTree[] sourceTrees, string pool, HashSet<string>? removed)
    {
        var declared = new HashSet<string>(StringComparer.Ordinal);
        foreach (SyntaxTree tree in sourceTrees.Concat(testTrees))
        {
            foreach (SyntaxNode node in tree.GetRoot().DescendantNodes())
            {
                if (node is MethodDeclarationSyntax testMethod && IsTest(testMethod)) continue;
                string? name = node switch
                {
                    MethodDeclarationSyntax method => method.Identifier.ValueText,
                    PropertyDeclarationSyntax property => property.Identifier.ValueText,
                    BaseTypeDeclarationSyntax type => type.Identifier.ValueText,
                    DelegateDeclarationSyntax type => type.Identifier.ValueText,
                    VariableDeclaratorSyntax variable when variable.Parent?.Parent is BaseFieldDeclarationSyntax => variable.Identifier.ValueText,
                    ParameterSyntax parameter when parameter.Parent?.Parent is RecordDeclarationSyntax => parameter.Identifier.ValueText,
                    EnumMemberDeclarationSyntax member => member.Identifier.ValueText,
                    EventDeclarationSyntax member => member.Identifier.ValueText,
                    _ => null
                };
                if (name != null) declared.Add(name);
            }
        }

        var endings = new HashSet<string>(StringComparer.Ordinal);
        foreach (string name in declared)
        {
            string stripped = PrefixLetter.Replace(name, "");
            for (int start = 0; start < stripped.Length; start++) endings.Add(stripped.Substring(start));
        }

        foreach (MethodDeclarationSyntax test in testMethods)
        {
            string file = test.SyntaxTree.FilePath;
            string name = test.Identifier.ValueText;
            SyntaxNode? body = (SyntaxNode?)test.Body ?? test.ExpressionBody;
            if (body == null) continue;

            string first = name.Split('_')[0];
            if (name.Contains('_') && !endings.Contains(first) && !declared.Contains(first) && !Loose(first, body))
            {
                string? match = removed == null ? null : RemovedMatch(first, removed);
                if (match != null) Add("DeadTarget", file, LineOf(test.Identifier.GetLocation()), name, "renamed", $"{first} names {match}, which history shows removed");
                else Add("DeadTarget", file, LineOf(test.Identifier.GetLocation()), name, "unmatched", $"no declaration ends with {first}");
            }

            foreach (LiteralExpressionSyntax literal in Calls(body).Where(IsAssertCall).SelectMany(assert => assert.DescendantNodes().OfType<LiteralExpressionSyntax>()))
            {
                if (!literal.IsKind(SyntaxKind.StringLiteralExpression)) continue;
                string value = literal.Token.ValueText;
                if (DottedKey.IsMatch(value) && !FileEnding.IsMatch(value) && !pool.Contains(value, StringComparison.Ordinal))
                {
                    Add("DeadTarget", file, LineOf(literal.GetLocation()), name, "literal", $"\"{value}\" occurs nowhere outside the tests");
                }
            }
        }
    }

    // The body drives a member the first part ends with, or one that holds it: the name is loose, not dead.
    private static bool Loose(string first, SyntaxNode body) =>
        body.DescendantNodes().OfType<SimpleNameSyntax>()
            .Select(simple => PrefixLetter.Replace(simple.Identifier.ValueText, ""))
            .Where(text => text.Count(char.IsUpper) >= LooseCapitals)
            .Any(text => first.EndsWith(text, StringComparison.Ordinal) || text.Contains(first, StringComparison.Ordinal));

    private static string? RemovedMatch(string first, HashSet<string> removed)
    {
        for (char a = 'A'; a <= 'Z'; a++)
        {
            if (removed.Contains(a + first)) return a + first;
        }

        for (char a = 'A'; a <= 'Z'; a++)
        {
            for (char b = 'A'; b <= 'Z'; b++)
            {
                string candidate = a.ToString() + b + first;
                if (removed.Contains(candidate)) return candidate;
            }
        }

        return null;
    }

    private static HashSet<string> HistoryTokens(string root, string source)
    {
        ProcessStartInfo info = new("git")
        {
            WorkingDirectory = root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            UseShellExecute = false
        };
        foreach (string argument in new[] { "-c", "core.quotePath=false", "log", "-p", "-U0", "--format=", "--", ":(glob)" + source + "/**/*.cs" })
        {
            info.ArgumentList.Add(argument);
        }

        using Process process = Process.Start(info) ?? throw new InvalidOperationException("Git could not be started to read the history.");
        Task<string> error = process.StandardError.ReadToEndAsync();
        var tokens = new HashSet<string>(StringComparer.Ordinal);
        long lines = 0;
        for (string? line = process.StandardOutput.ReadLine(); line != null; line = process.StandardOutput.ReadLine())
        {
            if (line.Length == 0 || line[0] != '-' || line.StartsWith("---", StringComparison.Ordinal)) continue;
            lines++;
            foreach (Match word in Word.Matches(line)) tokens.Add(word.Value);
        }

        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException("Git failed to read the history.\n" + error.Result);
        }

        Counters["historyLines"] = lines;
        return tokens;
    }

    private static void WaitLines(SyntaxTree tree)
    {
        SyntaxNode root = tree.GetRoot();
        var waiting = new List<(int Line, string Owner, string Text, string Detail, bool Clock)>();
        foreach (var line in tree.GetText().Lines)
        {
            string text = line.ToString();
            if (text.TrimStart().StartsWith("//", StringComparison.Ordinal)) continue;
            int lineNumber = line.LineNumber + 1;
            string owner = OwnerName(root.FindToken(line.Start + (text.Length - text.TrimStart().Length)).Parent);
            if (Sleep.IsMatch(text)) Add("FixedSleep", tree.FilePath, lineNumber, owner, "sleep", Flatten(text));
            Match delay = Delay.Match(text);
            if (delay.Success)
            {
                SyntaxNode? node = root.FindToken(line.Start + delay.Index).Parent;
                bool guard = node != null && node.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().Any(call => CalleeName(call) == "WhenAny");
                if (guard) waiting.Add((lineNumber, owner, text, "whenany-delay", false));
                else Add("FixedSleep", tree.FilePath, lineNumber, owner, "delay", Flatten(text));
            }

            foreach (var (detail, pattern) in WaitRules)
            {
                if (pattern.IsMatch(text)) waiting.Add((lineNumber, owner, text, detail, false));
            }

            if (Clock.IsMatch(text)) waiting.Add((lineNumber, owner, text, "deadline", true));
        }

        foreach (SyntaxNode loop in root.DescendantNodes().Where(node => node is WhileStatementSyntax or DoStatementSyntax or ForStatementSyntax))
        {
            if (loop.DescendantNodes().Any(inner => inner is WhileStatementSyntax or DoStatementSyntax or ForStatementSyntax && PollingBody.IsMatch(inner.ToString()))) continue;
            if (PollingBody.IsMatch(loop.ToString())) waiting.Add((LineOf(loop.GetLocation()), OwnerName(loop), loop.ToString().Split('\n')[0], "polling", false));
        }

        var guarded = new HashSet<string>(waiting.Where(item => !item.Clock).Select(item => item.Owner), StringComparer.Ordinal);
        foreach (var item in waiting)
        {
            if (!item.Clock || guarded.Contains(item.Owner)) Add("LongWait", tree.FilePath, item.Line, item.Owner, item.Detail, Flatten(item.Text));
        }
    }

    private static string OwnerName(SyntaxNode? node) =>
        node?.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault()?.Identifier.ValueText ?? "";

    private static void CopiedBodies(List<MethodDeclarationSyntax> testMethods)
    {
        var exact = new Dictionary<string, List<MethodDeclarationSyntax>>(StringComparer.Ordinal);
        var shapes = new Dictionary<string, List<MethodDeclarationSyntax>>(StringComparer.Ordinal);
        foreach (MethodDeclarationSyntax test in testMethods)
        {
            SyntaxNode? body = (SyntaxNode?)test.Body ?? test.ExpressionBody;
            if (body == null) continue;
            var tokens = body.DescendantTokens().ToList();
            if (tokens.Count < MinTokens) continue;
            string text = string.Join(" ", tokens.Select(token => token.Text));
            string shape = string.Join(" ", tokens.Select(token =>
                token.IsKind(SyntaxKind.StringLiteralToken) || token.IsKind(SyntaxKind.NumericLiteralToken) || token.IsKind(SyntaxKind.CharacterLiteralToken) ? "L" : token.Text));
            (exact.TryGetValue(text, out var same) ? same : exact[text] = new()).Add(test);
            (shapes.TryGetValue(shape, out var like) ? like : shapes[shape] = new()).Add(test);
        }

        var exactSet = new HashSet<MethodDeclarationSyntax>(exact.Values.Where(group => group.Count > 1).SelectMany(group => group));
        foreach (var group in exact.Values.Where(group => group.Count > 1))
        {
            foreach (MethodDeclarationSyntax test in group.Skip(1)) AddCopy(test, group[0], "exact", "same body as ");
        }

        foreach (var group in shapes.Values.Where(group => group.Count > 1))
        {
            foreach (MethodDeclarationSyntax test in group.Skip(1).Where(item => !exactSet.Contains(item))) AddCopy(test, group[0], "same-shape", "only literals differ from ");
        }
    }

    private static void AddCopy(MethodDeclarationSyntax test, MethodDeclarationSyntax original, string detail, string lead) =>
        Add("CopiedBody", test.SyntaxTree.FilePath, LineOf(test.Identifier.GetLocation()), test.Identifier.ValueText, detail,
            lead + original.SyntaxTree.FilePath + ":" + LineOf(original.Identifier.GetLocation()).ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + original.Identifier.ValueText);

    private static void UnusedHelpers(SyntaxTree[] trees, string outside)
    {
        var outsideWords = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match word in Word.Matches(outside)) outsideWords.Add(word.Value);
        var mentions = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (SyntaxTree tree in trees)
        {
            foreach (SyntaxToken token in tree.GetRoot().DescendantTokens().Where(token => token.IsKind(SyntaxKind.IdentifierToken)))
            {
                mentions[token.ValueText] = mentions.TryGetValue(token.ValueText, out int seen) ? seen + 1 : 1;
            }
        }

        foreach (SyntaxTree tree in trees)
        {
            foreach (SyntaxNode node in tree.GetRoot().DescendantNodes())
            {
                (SyntaxToken Identifier, string What)? found = node switch
                {
                    MethodDeclarationSyntax method when !IsTest(method) && !Has(method.Modifiers, SyntaxKind.OverrideKeyword) && method.ExplicitInterfaceSpecifier == null && !LifetimeNames.Contains(method.Identifier.ValueText)
                        => (method.Identifier, "method"),
                    PropertyDeclarationSyntax property when !Has(property.Modifiers, SyntaxKind.OverrideKeyword) && property.ExplicitInterfaceSpecifier == null
                        => (property.Identifier, "property"),
                    VariableDeclaratorSyntax variable when variable.Parent?.Parent is FieldDeclarationSyntax
                        => (variable.Identifier, "field"),
                    BaseTypeDeclarationSyntax type when !type.DescendantNodes().OfType<MethodDeclarationSyntax>().Any(method => IsTest(method) || method.ParameterList.Parameters.FirstOrDefault()?.Modifiers.Any(SyntaxKind.ThisKeyword) == true)
                        => (type.Identifier, "type"),
                    _ => null
                };
                if (found is not { } hit) continue;
                string text = hit.Identifier.ValueText;
                if ((mentions.TryGetValue(text, out int count) ? count : 0) <= 1 && !outsideWords.Contains(text))
                {
                    Add("UnusedHelper", tree.FilePath, LineOf(hit.Identifier.GetLocation()), text, hit.What, $"the {hit.What} is named nowhere else");
                }
            }
        }
    }

    private static bool Has(SyntaxTokenList modifiers, SyntaxKind kind) => modifiers.Any(modifier => modifier.IsKind(kind));

    private static string ReadText(string root, string relative) =>
        File.ReadAllText(Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar)), Encoding.UTF8);

    private static HashSet<string> Fixpoint(List<HelperBody> bodies, Func<HelperBody, bool> seed, Func<HelperBody, string> key)
    {
        var found = new HashSet<string>(bodies.Where(seed).Select(key), StringComparer.Ordinal);
        for (bool changed = true; changed;)
        {
            changed = false;
            foreach (HelperBody body in bodies)
            {
                if (found.Contains(key(body))) continue;
                if (Calls(body.Node).Any(call => found.Contains(key(body with { Name = CalleeName(call) }))))
                {
                    found.Add(key(body));
                    changed = true;
                }
            }
        }

        return found;
    }

    private static bool Constructs(SyntaxNode node, HashSet<string> types) =>
        node.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().Any(creation => types.Contains(TypeName(creation.Type)))
        || node.DescendantNodes().OfType<ImplicitObjectCreationExpressionSyntax>().Any(creation => creation.Parent?.Parent?.Parent is VariableDeclarationSyntax declaration && types.Contains(TypeName(declaration.Type)));

    // A value the test set itself: a literal, or a local bound to a literal that only asserts read.
    private static bool SelfValue(ExpressionSyntax expression, SyntaxNode body)
    {
        if (expression is LiteralExpressionSyntax) return true;
        if (expression is not IdentifierNameSyntax identifier) return false;
        var declarations = body.DescendantNodes().OfType<VariableDeclaratorSyntax>().Where(item => item.Identifier.ValueText == identifier.Identifier.ValueText).ToList();
        if (declarations.Count != 1 || declarations[0].Initializer?.Value is not LiteralExpressionSyntax) return false;
        return body.DescendantNodes().OfType<IdentifierNameSyntax>()
            .Where(read => read.Identifier.ValueText == identifier.Identifier.ValueText)
            .All(read => read.Ancestors().OfType<InvocationExpressionSyntax>().All(IsAssertCall) && read.Ancestors().OfType<InvocationExpressionSyntax>().Any());
    }

    // A loop whose source the test wrote down: a literal count, a collection literal, or a fixed enumeration.
    private static bool LiteralSource(SyntaxNode loop, SyntaxNode body)
    {
        if (loop is ForStatementSyntax counted)
        {
            return counted.Condition is BinaryExpressionSyntax comparison && comparison.Right is LiteralExpressionSyntax;
        }

        ExpressionSyntax source = ((CommonForEachStatementSyntax)loop).Expression;
        if (source is IdentifierNameSyntax identifier)
        {
            var declarations = body.DescendantNodes().OfType<VariableDeclaratorSyntax>().Where(item => item.Identifier.ValueText == identifier.Identifier.ValueText).ToList();
            if (declarations.Count != 1 || declarations[0].Initializer == null) return false;
            source = declarations[0].Initializer!.Value;
        }

        if (source is CollectionExpressionSyntax or ArrayCreationExpressionSyntax or ImplicitArrayCreationExpressionSyntax or InitializerExpressionSyntax) return true;
        if (source is BaseObjectCreationExpressionSyntax creation) return creation.Initializer != null;
        if (source is InvocationExpressionSyntax invocation && invocation.Expression is MemberAccessExpressionSyntax member)
        {
            string target = member.Expression.ToString();
            string method = member.Name.Identifier.ValueText;
            if (target == "Enum" && method == "GetValues") return true;
            if (target == "Enumerable" && method == "Range") return invocation.ArgumentList.Arguments.All(argument => argument.Expression is LiteralExpressionSyntax);
        }

        return false;
    }

    private static string LoopHead(SyntaxNode loop) => loop switch
    {
        ForEachStatementSyntax each => "foreach (" + each.Type + " " + each.Identifier.ValueText + " in " + each.Expression + ")",
        ForEachVariableStatementSyntax each => "foreach (" + each.Variable + " in " + each.Expression + ")",
        ForStatementSyntax counted => "for (" + counted.Declaration + "; " + counted.Condition + "; " + string.Join(", ", counted.Incrementors) + ")",
        _ => loop.ToString().Split('\n')[0]
    };

    private static bool IsTest(MethodDeclarationSyntax method) =>
        method.AttributeLists.SelectMany(list => list.Attributes).Any(attribute =>
        {
            string name = SimpleName(attribute.Name);
            return Markers.Any(marker => name.EndsWith(marker, StringComparison.Ordinal) || name.EndsWith(marker + "Attribute", StringComparison.Ordinal));
        });

    private static bool IsAssertCall(InvocationExpressionSyntax call) =>
        call.Expression is MemberAccessExpressionSyntax access && access.Expression.ToString() is "Assert" or "Xunit.Assert";

    private static IEnumerable<InvocationExpressionSyntax> Calls(SyntaxNode node) => node.DescendantNodes().OfType<InvocationExpressionSyntax>();

    private static string CalleeName(InvocationExpressionSyntax call) => call.Expression switch
    {
        MemberAccessExpressionSyntax access => access.Name.Identifier.ValueText,
        SimpleNameSyntax simple => simple.Identifier.ValueText,
        MemberBindingExpressionSyntax binding => binding.Name.Identifier.ValueText,
        _ => ""
    };

    private static string SimpleName(NameSyntax name) => name switch
    {
        QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
        AliasQualifiedNameSyntax aliased => aliased.Name.Identifier.ValueText,
        SimpleNameSyntax simple => simple.Identifier.ValueText,
        _ => name.ToString()
    };

    private static string TypeName(TypeSyntax type) => type is NameSyntax name ? SimpleName(name) : type.ToString();

    private static string ProjectOf(string path, string tests)
    {
        string[] parts = path.Substring(tests.Length + 1).Split('/');
        return parts.Length > 1 ? parts[0] : "";
    }

    private static int LineOf(Location location) => location.GetLineSpan().StartLinePosition.Line + 1;

    private static string Flatten(string text)
    {
        string flat = Regex.Replace(text, @"\s+", " ").Trim();
        return flat.Length <= 160 ? flat : flat.Substring(0, 157) + "...";
    }

    private static void Add(string kind, string file, int line, string name, string detail, string message) =>
        Kinds[kind].Add(new ScanLine(file, line, name, detail, message));

    private static void WriteKind(Utf8JsonWriter writer, string key, List<ScanLine> lines)
    {
        var ordered = lines
            .OrderByDescending(item => item.Milliseconds)
            .ThenBy(item => item.File, StringComparer.Ordinal)
            .ThenBy(item => item.Line)
            .ThenBy(item => item.Name, StringComparer.Ordinal)
            .ThenBy(item => item.Detail, StringComparer.Ordinal)
            .ThenBy(item => item.Message, StringComparer.Ordinal)
            .ToList();
        writer.WriteStartObject();
        writer.WriteString("key", key);
        writer.WriteString("name", Regex.Replace(key, "(?<=[a-z])(?=[A-Z])", " "));
        writer.WriteNumber("hits", ordered.Count);
        writer.WriteNumber("files", ordered.Select(item => item.File).Distinct(StringComparer.Ordinal).Count());
        writer.WriteStartArray("lines");
        foreach (ScanLine item in ordered)
        {
            writer.WriteStartObject();
            writer.WriteString("file", item.File);
            writer.WriteNumber("line", item.Line);
            writer.WriteString("name", item.Name);
            writer.WriteString("detail", item.Detail);
            writer.WriteString("message", item.Message);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
        if (key == "SlowTest")
        {
            writer.WriteStartArray("classes");
            foreach (TrxClass item in Classes.OrderByDescending(item => item.Sum).ThenBy(item => item.Project, StringComparer.Ordinal).ThenBy(item => item.Name, StringComparer.Ordinal))
            {
                writer.WriteStartObject();
                writer.WriteString("project", item.Project);
                writer.WriteString("name", item.Name);
                writer.WriteNumber("count", item.Count);
                writer.WriteNumber("sum", item.Sum);
                writer.WriteNumber("span", item.Span);
                writer.WriteNumber("gap", item.Gap);
                writer.WriteNumber("floor", item.Floor);
                writer.WriteNumber("body", item.Body);
                writer.WriteBoolean("slow", item.Slow);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
        writer.WriteEndObject();
    }

    private static readonly Regex TrxFileName = new(@"^(?<project>.+)\.(?<tree>[0-9a-f]{12})\.(?<stamp>\d{8}-\d{6})\.trx$", RegexOptions.CultureInvariant);
    private static readonly System.Globalization.CultureInfo Invariant = System.Globalization.CultureInfo.InvariantCulture;
    private static readonly List<TrxClass> Classes = new();
    private static string[] TrxNames = Array.Empty<string>();

    private static TrxRun ReadTrx(string path)
    {
        System.Xml.Linq.XNamespace ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
        var document = System.Xml.Linq.XDocument.Load(path);
        var classes = new Dictionary<string, (string ClassName, string Method)>(StringComparer.Ordinal);
        foreach (var test in document.Descendants(ns + "UnitTest"))
        {
            var method = test.Element(ns + "TestMethod");
            if (method != null) classes[(string?)test.Attribute("id") ?? ""] = ((string?)method.Attribute("className") ?? "", (string?)method.Attribute("name") ?? "");
        }

        var results = new List<TrxResult>();
        foreach (var result in document.Descendants(ns + "UnitTestResult"))
        {
            classes.TryGetValue((string?)result.Attribute("testId") ?? "", out var owner);
            string className = owner.ClassName ?? "";
            string testName = (string?)result.Attribute("testName") ?? "";
            if (className.Length > 0 && testName.StartsWith(className + ".", StringComparison.Ordinal)) testName = testName.Substring(className.Length + 1);
            string? duration = (string?)result.Attribute("duration");
            string? start = (string?)result.Attribute("startTime");
            string? end = (string?)result.Attribute("endTime");
            results.Add(new TrxResult(
                className, testName, owner.Method ?? testName, (string?)result.Attribute("outcome") ?? "",
                duration == null ? 0 : (long)TimeSpan.Parse(duration, Invariant).TotalMilliseconds,
                start == null ? DateTimeOffset.MinValue : DateTimeOffset.Parse(start, Invariant),
                end == null ? DateTimeOffset.MinValue : DateTimeOffset.Parse(end, Invariant)));
        }

        string name = Path.GetFileName(path);
        Match parts = TrxFileName.Match(name);
        DateTimeOffset started = results.Count == 0 ? DateTimeOffset.MinValue : results.Min(item => item.Start);
        return parts.Success
            ? new TrxRun(name, parts.Groups["project"].Value, parts.Groups["tree"].Value, started, results)
            : new TrxRun(name, Path.GetFileNameWithoutExtension(name), "", started, results);
    }

    private static string Seconds(long milliseconds) => (milliseconds / 1000.0).ToString("0.000", Invariant);

    private static void TrxKinds(string[] paths, List<MethodDeclarationSyntax> testMethods, double testSeconds, double classSeconds)
    {
        var places = new Dictionary<string, (string File, int Line)>(StringComparer.Ordinal);
        foreach (MethodDeclarationSyntax test in testMethods.OrderBy(item => item.SyntaxTree.FilePath, StringComparer.Ordinal))
        {
            string type = test.Ancestors().OfType<BaseTypeDeclarationSyntax>().FirstOrDefault()?.Identifier.ValueText ?? "";
            places.TryAdd(type + "." + test.Identifier.ValueText, (test.SyntaxTree.FilePath, LineOf(test.Identifier.GetLocation())));
        }

        (string File, int Line) Place(TrxRun run, TrxResult result) =>
            places.TryGetValue(result.ClassName.Split('.', '+').Last() + "." + result.Method, out var place) ? place : (run.Name, 0);

        var runs = new List<TrxRun>();
        foreach (string trxPath in paths.OrderBy(item => item, StringComparer.Ordinal))
        {
            try
            {
                runs.Add(ReadTrx(trxPath));
            }
            catch (Exception failure) when (failure is System.Xml.XmlException or IOException or UnauthorizedAccessException or FormatException or OverflowException or ArgumentException)
            {
                Notice("The trx file " + Path.GetFileName(trxPath) + " was skipped: " + failure.Message);
            }
        }

        Counters["trxFiles"] = runs.Count;
        Counters["trxResults"] = runs.Sum(run => run.Results.Count);
        TrxNames = runs.Select(run => run.Name).ToArray();

        var spent = new Dictionary<string, long>(StringComparer.Ordinal);
        var newest = runs.GroupBy(run => run.Project, StringComparer.Ordinal)
            .Select(group => group.OrderBy(run => run.Started).ThenBy(run => run.Name, StringComparer.Ordinal).Last())
            .OrderBy(run => run.Project, StringComparer.Ordinal);
        foreach (TrxRun run in newest)
        {
            foreach (var group in run.Results.GroupBy(result => result.ClassName, StringComparer.Ordinal))
            {
                var members = group.ToList();
                long sum = members.Sum(result => result.Milliseconds);
                long floor = members.Min(result => result.Milliseconds);
                long span = (long)(members.Max(result => result.End) - members.Min(result => result.Start)).TotalMilliseconds;
                Classes.Add(new TrxClass(run.Project, group.Key, members.Count, sum, span, Math.Max(0, span - sum), floor, sum - members.Count * floor, sum >= classSeconds * 1000));
                foreach (TrxResult result in members)
                {
                    var (file, line) = Place(run, result);
                    if (result.Outcome == "NotExecuted") Add("SkippedTest", file, line, result.TestName, "not-executed", "the trx run did not execute it: " + run.Name);
                    // Only a test at or over testSeconds with a source line is a hit; every other duration feeds only its class row.
                    if (line > 0 && result.Milliseconds >= testSeconds * 1000)
                        Kinds["SlowTest"].Add(new ScanLine(file, line, result.TestName, "slow",
                            $"{Seconds(result.Milliseconds)} s, setup about {Seconds(floor)} s, body {Seconds(result.Milliseconds - floor)} s", result.Milliseconds));
                    string key = file + "\n" + result.Method;
                    spent[key] = Math.Max(spent.TryGetValue(key, out long seen) ? seen : 0, result.Milliseconds);
                }
            }
        }

        foreach (var group in runs.GroupBy(run => run.Project + "\n" + run.Tree, StringComparer.Ordinal).OrderBy(group => group.Key, StringComparer.Ordinal))
        {
            var ordered = group.OrderBy(run => run.Started).ThenBy(run => run.Name, StringComparer.Ordinal).ToList();
            if (ordered.Count < 2) continue;
            var outcomes = new SortedDictionary<string, List<string>>(StringComparer.Ordinal);
            var where = new Dictionary<string, (string File, int Line, string Name)>(StringComparer.Ordinal);
            foreach (TrxRun run in ordered)
            {
                foreach (TrxResult result in run.Results)
                {
                    string key = result.ClassName + "." + result.TestName;
                    (outcomes.TryGetValue(key, out var list) ? list : outcomes[key] = new()).Add(result.Outcome);
                    var (file, line) = Place(run, result);
                    where[key] = (file, line, result.TestName);
                }
            }

            foreach (var (key, list) in outcomes)
            {
                if (!list.Contains("Passed") || !list.Contains("Failed")) continue;
                var (file, line, name) = where[key];
                string tree = ordered[0].Tree.Length > 0 ? " of tree " + ordered[0].Tree : "";
                Add("FlakyResult", file, line, name, "flip", $"outcomes over {list.Count} runs{tree}: {string.Join(", ", list)}");
            }
        }

        var waits = Kinds["LongWait"];
        for (int i = 0; i < waits.Count; i++)
        {
            ScanLine wait = waits[i];
            if (!spent.TryGetValue(wait.File + "\n" + wait.Name, out long took)) continue;
            Match guard = GuardSeconds.Match(wait.Message);
            if (!guard.Success) continue;
            double limit = guard.Groups["ms"].Success
                ? double.Parse(guard.Groups["ms"].Value, Invariant) / 1000
                : double.Parse(guard.Groups["value"].Value, Invariant) * (guard.Groups["unit"].Value switch { "Milliseconds" => 0.001, "Minutes" => 60.0, _ => 1.0 });
            if (took >= limit * 1000) waits[i] = wait with { Message = wait.Message + $" (the trx run took {Seconds(took)} s, reaching this {limit.ToString("0.###", Invariant)} s guard)" };
        }
    }

    private static List<string> GitFiles(string root, IEnumerable<string> patterns)
    {
        ProcessStartInfo info = new("git")
        {
            WorkingDirectory = root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            UseShellExecute = false
        };
        foreach (string argument in new[] { "-c", "core.quotePath=false", "ls-files", "--cached", "--others", "--exclude-standard", "--" })
        {
            info.ArgumentList.Add(argument);
        }

        foreach (string pattern in patterns)
        {
            info.ArgumentList.Add(":(icase)" + pattern);
        }

        using Process process = Process.Start(info) ?? throw new InvalidOperationException("Git could not be started to list the test files.");
        Task<string> error = process.StandardError.ReadToEndAsync();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException("Git failed to list the test files.\n" + error.Result);
        }

        var files = new List<string>();
        foreach (string line in output.Split('\n'))
        {
            string relative = line.Trim();
            if (relative.Length == 0 || relative.Split('/').Any(segment => segment is "bin" or "obj")) continue;
            if (File.Exists(Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar)))) files.Add(relative);
        }

        files.Sort(StringComparer.Ordinal);
        return files;
    }
}

internal sealed record HelperBody(string Name, string Project, SyntaxNode Node);

internal sealed record ScanLine(string File, int Line, string Name, string Detail, string Message, long Milliseconds = 0);

internal sealed record TrxResult(string ClassName, string TestName, string Method, string Outcome, long Milliseconds, DateTimeOffset Start, DateTimeOffset End);

internal sealed record TrxRun(string Name, string Project, string Tree, DateTimeOffset Started, List<TrxResult> Results);

internal sealed record TrxClass(string Project, string Name, int Count, long Sum, long Span, long Gap, long Floor, long Body, bool Slow);
'@

function Invoke-ScanNative {
    param(
        [Parameter(Mandatory = $true)][string]$FilePath,
        [Parameter(Mandatory = $true)][string[]]$Arguments
    )

    $nativePreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        $output = @(& $FilePath @Arguments 2>&1 | ForEach-Object { [string]$_ })
        $exitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $nativePreference
    }

    return [pscustomobject]@{ Lines = $output; ExitCode = $exitCode }
}

function Get-HelperBinary {
    param([string]$DotnetPath, [string]$ProjectName, [string]$TargetFramework)

    $sdk = Invoke-ScanNative -FilePath $DotnetPath -Arguments @('--version')
    if ($sdk.ExitCode -ne 0) {
        throw "The .NET SDK version could not be read.`n$($sdk.Lines -join [Environment]::NewLine)"
    }

    $sdkVersion = ([string]($sdk.Lines | Select-Object -First 1)).Trim()
    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        $seed = $script:HelperProject + "`n" + $script:HelperProgram + "`n" + $TargetFramework + "`n" + $sdkVersion
        $digest = $sha.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($seed))
    }
    finally {
        $sha.Dispose()
    }

    $hash = ([System.BitConverter]::ToString($digest) -replace '-', '').Substring(0, 16).ToLowerInvariant()
    $cacheParent = Join-Path ([System.IO.Path]::GetTempPath()) ($ProjectName + '-ScanTest')
    $cacheFolder = Join-Path $cacheParent $hash
    $binaryPath = Join-Path (Join-Path $cacheFolder 'bin') 'ScanTest.Helper.dll'
    if (Test-Path -LiteralPath $binaryPath -PathType Leaf) {
        return $binaryPath
    }

    Write-ScanLine 'Compiling the test scanner once for this SDK...' -ForegroundColor DarkGray
    if (Test-Path -LiteralPath $cacheParent) {
        Get-ChildItem -LiteralPath $cacheParent -Directory | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    }

    $helperFolder = Join-Path $cacheFolder 'helper'
    [System.IO.Directory]::CreateDirectory($helperFolder) | Out-Null
    $projectPath = Join-Path $helperFolder 'ScanTest.Helper.csproj'
    [System.IO.File]::WriteAllText($projectPath, $script:HelperProject.Replace('{TARGET_FRAMEWORK}', $TargetFramework), $script:Utf8)
    [System.IO.File]::WriteAllText((Join-Path $helperFolder 'Program.cs'), $script:HelperProgram, $script:Utf8)
    $build = Invoke-ScanNative -FilePath $DotnetPath -Arguments @('build', $projectPath, '--configuration', 'Release', '--nologo', '--verbosity', 'quiet', '--output', (Join-Path $cacheFolder 'bin'))
    if ($build.ExitCode -ne 0 -or -not (Test-Path -LiteralPath $binaryPath -PathType Leaf)) {
        Remove-Item -LiteralPath $cacheFolder -Recurse -Force -ErrorAction SilentlyContinue
        throw "The test scanner could not be built.`n$($build.Lines -join [Environment]::NewLine)"
    }

    return $binaryPath
}

function Invoke-ScanHelper {
    param(
        [Parameter(Mandatory = $true)]$Config,
        [Parameter(Mandatory = $true)][string]$Root,
        [bool]$History,
        [string[]]$TrxFiles = @()
    )

    $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($null -eq $dotnet) { throw 'The .NET SDK is required, but dotnet was not found on PATH.' }

    $previousNoLogo = $env:DOTNET_NOLOGO
    $previousTelemetry = $env:DOTNET_CLI_TELEMETRY_OPTOUT
    $env:DOTNET_NOLOGO = '1'
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
    $temporaryFolder = Join-Path ([System.IO.Path]::GetTempPath()) ([string]$Config.project + '-ScanTest-' + [Guid]::NewGuid().ToString('N'))
    [System.IO.Directory]::CreateDirectory($temporaryFolder) | Out-Null
    try {
        $helperPath = Get-HelperBinary -DotnetPath $dotnet.Source -ProjectName ([string]$Config.project) -TargetFramework ([string]$Config.helper.framework)
        $settingsPath = Join-Path $temporaryFolder 'settings.json'
        $resultPath = Join-Path $temporaryFolder 'scan.json'
        $settings = [ordered]@{
            tests = [string]$Config.tests; source = [string]$Config.source; pool = @($Config.pool | ForEach-Object { [string]$_ }); history = [bool]$History
            bootTypes = @($Config.kinds.WrongLayer.bootTypes | ForEach-Object { [string]$_ }); wrongProjects = @($Config.kinds.WrongLayer.projects | ForEach-Object { [string]$_ })
            uiPrefixes = [string]$Config.kinds.WrongLayer.uiPrefixes; minTokens = [int]$Config.kinds.CopiedBody.minTokens
            markers = @($Config.markers | ForEach-Object { [string]$_ })
            kinds = $Config.kinds
            trx =@($TrxFiles); testSeconds = [double]$Config.kinds.SlowTest.testSeconds; classSeconds = [double]$Config.kinds.SlowTest.classSeconds
        }
        [System.IO.File]::WriteAllText($settingsPath, (ConvertTo-ScanJson $settings), $script:Utf8)
        $run = Invoke-ScanNative -FilePath $dotnet.Source -Arguments @($helperPath, $Root, $settingsPath, $resultPath)
        if ($run.ExitCode -ne 0 -or -not (Test-Path -LiteralPath $resultPath -PathType Leaf)) {
            throw "The test scanner failed.`n$($run.Lines -join [Environment]::NewLine)"
        }
        return [System.IO.File]::ReadAllText($resultPath, $script:Utf8)
    }
    finally {
        $env:DOTNET_NOLOGO = $previousNoLogo
        $env:DOTNET_CLI_TELEMETRY_OPTOUT = $previousTelemetry
        Remove-Item -LiteralPath $temporaryFolder -Recurse -Force -ErrorAction SilentlyContinue
    }
}

function Read-ScanDocument {
    param([Parameter(Mandatory = $true)][string]$Text)

    $document = $Text | ConvertFrom-Json
    $keys = @(@($document.kinds) | ForEach-Object { [string]$_.key })
    if (($keys -join ',') -cne ($script:ScanKinds -join ',')) {
        throw "The scan document lists the kinds $($keys -join ', ') instead of $($script:ScanKinds -join ', ')."
    }
    return $document
}

function ConvertTo-MarkdownCell {
    param([AllowNull()][object]$Value)
    return ([string]$Value).Replace('|', '\|').Replace("`r`n", '<br>').Replace("`n", '<br>').Replace("`r", '<br>')
}

function Format-ScanLine {
    param([Parameter(Mandatory = $true)]$Line)

    $text = '`{0}:{1}`' -f $Line.file, ([long]$Line.line).ToString($script:Invariant)
    if (-not [string]::IsNullOrEmpty([string]$Line.name)) { $text += ' `' + $Line.name + '`' }
    if (-not [string]::IsNullOrEmpty([string]$Line.detail)) { $text += ' ' + $Line.detail + ':' }
    return $text + ' ' + (ConvertTo-MarkdownCell $Line.message)
}

$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
if ([string]::IsNullOrWhiteSpace($ConfigPath)) { $ConfigPath = Join-Path $PSScriptRoot 'ScanTest.json' }
$ConfigPath = [System.IO.Path]::GetFullPath($ConfigPath)
$config = Read-ScanConfig -Path $ConfigPath
if (-not $PSBoundParameters.ContainsKey('Top')) { $Top = [int]$config.top }
if ([string]::IsNullOrWhiteSpace($ReportDirectory)) { $ReportDirectory = [string]$config.report.directory }
$reportFolder = Join-ScanPath -Root $repoRoot -Relative $ReportDirectory
$version = Get-ScanVersion -Path (Join-ScanPath -Root $repoRoot -Relative ([string]$config.report.versionFile)) -Key ([string]$config.report.versionKey)
$testFolder = Join-ScanPath -Root $repoRoot -Relative ([string]$config.tests)
if (-not [System.IO.Directory]::Exists($testFolder)) { throw "The test folder was not found: $testFolder" }
$trxFiles = @(Resolve-TrxPaths -Root $repoRoot -Default ([string]$config.trx) -Paths $Trx)

$document = Read-ScanDocument -Text (Invoke-ScanHelper -Config $config -Root $repoRoot -History (-not $NoHistory) -TrxFiles $trxFiles)
Write-ScanLine ('Scanned: {0} test projects, {1} trx files, history {2}' -f @($document.projects).Count, $trxFiles.Count, $(if ($document.history) { 'read' } else { 'skipped' })) -ForegroundColor DarkGray
$notices = @(@($document.notices) | Where-Object { $null -ne $_ } | ForEach-Object { [string]$_ })
foreach ($notice in $notices) { Write-ScanLine ('Notice: ' + $notice) -ForegroundColor Yellow }
$kinds = @($document.kinds)
$counters = @($document.counters)
$trxRead = @(@($document.trx) | Where-Object { $null -ne $_ } | ForEach-Object { [string]$_ })

$countWidth = ([string]$kinds.Count).Length
for ($index = 0; $index -lt $kinds.Count; $index++) {
    $kind = $kinds[$index]
    Write-ScanLine ('[{0}/{1}] {2}  {3} hits' -f ([string]($index + 1)).PadLeft($countWidth), $kinds.Count, $kind.name, $kind.hits)
}

Write-ScanLine ''
foreach ($counter in $counters) {
    Write-ScanLine ('{0}  {1}' -f ([string]$counter.name).PadRight(18), $counter.count)
}
foreach ($kind in @($kinds | Where-Object { [long]$_.hits -gt 0 })) {
    Write-ScanLine ''
    Write-ScanLine ('{0} ({1})' -f $kind.name, $kind.hits) -ForegroundColor Cyan
    foreach ($line in @(@($kind.lines) | Select-Object -First $Top)) { Write-ScanLine ('  ' + (Format-ScanLine $line)) }
}

$counterValue = @{}
foreach ($counter in $counters) { $counterValue[[string]$counter.key] = [long]$counter.count }
$md = [System.Collections.Generic.List[string]]::new()
$md.Add("# Test scan $version")
$md.Add('')
$md.Add("- Generation: $script:ScanGeneration")
$md.Add("- Test projects: $(@($document.projects).Count)")
$md.Add("- Test methods: $($counterValue['tests'])")
$md.Add("- Helpers: $($counterValue['helpers'])")
$md.Add('- Trx files read: ' + $(if ($trxRead.Count -eq 0) { 'none' } else { ($trxRead | ForEach-Object { '`' + $_ + '`' }) -join ', ' }))
$md.Add('- History read: ' + $(if ($document.history) { 'yes' } else { 'no' }))
foreach ($notice in $notices) { $md.Add('- Notice: ' + (ConvertTo-MarkdownCell $notice)) }
$md.Add('')
$md.Add('Every count is informational: a scan has no ceilings, no ledger and no failing status.')
$md.Add('The kinds and their settings live in `scripts/ScanTest.json`.')
$md.Add('')
$md.Add('## Counters')
$md.Add('')
$md.Add('| Counter | Count |')
$md.Add('|---|---:|')
foreach ($counter in $counters) { $md.Add(('| {0} | {1} |' -f (ConvertTo-MarkdownCell $counter.name), $counter.count)) }
$md.Add('')
$md.Add('## Hits by kind')
$md.Add('')
$md.Add('| Kind | Hits | Files |')
$md.Add('|---|---:|---:|')
$hitsTotal = [long]0
$filesTotal = [long]0
foreach ($kind in $kinds) {
    $md.Add(('| {0} | {1} | {2} |' -f $kind.name, $kind.hits, $kind.files))
    $hitsTotal += [long]$kind.hits
    $filesTotal += [long]$kind.files
}
$md.Add(('| Total | {0} | {1} |' -f $hitsTotal, $filesTotal))
foreach ($kind in $kinds) {
    $md.Add('')
    $md.Add(('## {0} ({1})' -f $kind.name, $kind.hits))
    $md.Add('')
    foreach ($line in @($kind.lines)) { $md.Add('- ' + (Format-ScanLine $line)) }
    if ([string]$kind.key -ceq 'SlowTest' -and @($kind.classes).Count -gt 0) {
        $md.Add('')
        $md.Add('### By class')
        $md.Add('')
        $md.Add('| Project | Class | Tests | Sum s | Span s | Gap s | Floor s | Body s | Slow |')
        $md.Add('|---|---|---:|---:|---:|---:|---:|---:|---|')
        foreach ($class in @($kind.classes)) {
            $seconds = @(foreach ($field in @('sum', 'span', 'gap', 'floor', 'body')) { ([double]$class.$field / 1000).ToString('0.000', $script:Invariant) })
            $md.Add(('| {0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8} |' -f (ConvertTo-MarkdownCell $class.project), (ConvertTo-MarkdownCell $class.name), $class.count, $seconds[0], $seconds[1], $seconds[2], $seconds[3], $seconds[4], $(if ($class.slow) { 'yes' } else { '' })))
        }
    }
}

$page = [ordered]@{
    project = [string]$config.project; version = $version; generation = $script:ScanGeneration
    generated = [DateTimeOffset]::Now.ToString('yyyy-MM-ddTHH:mm:sszzz', $script:Invariant)
    projects = @($document.projects); trx = $trxRead; history = [bool]$document.history; notices = $notices
    counters = $counters; kinds = $kinds
}

$prefix = [string]$config.report.prefix
$reportPath = Join-Path $reportFolder ($prefix + $version + '.md')
$jsonPath = Join-Path $reportFolder ($prefix + $version + '.json')
$pagePath = Join-Path $reportFolder ($prefix + $version + '.html')
$pageData = ConvertTo-ScanJson $page
[System.IO.Directory]::CreateDirectory($reportFolder) | Out-Null
[System.IO.File]::WriteAllText($reportPath, (($md -join "`n") + "`n"), $script:Utf8)
[System.IO.File]::WriteAllText($jsonPath, ($pageData + "`n"), $script:Utf8)

# Page output: the same JSON document inside the template ScanTest.html, beside the Markdown report.
# The JSON escapes <, > and &, so it cannot close the script element that holds it.
$pageTemplatePath = Join-Path $PSScriptRoot 'ScanTest.html'
if (-not [System.IO.File]::Exists($pageTemplatePath)) { throw "The page template was not found: $pageTemplatePath" }
$pageTemplate = [System.IO.File]::ReadAllText($pageTemplatePath, $script:Utf8)
foreach ($marker in @('/*__DATA__*/', '__TITLE__')) {
    if (-not $pageTemplate.Contains($marker)) { throw "The page template lacks the marker $marker : $pageTemplatePath" }
}
$pageText = $pageTemplate.Replace('__TITLE__', [System.Net.WebUtility]::HtmlEncode([string]$config.project)).Replace('/*__DATA__*/', $pageData.Trim())
[System.IO.File]::WriteAllText($pagePath, ($pageText -replace "`r`n", "`n"), $script:Utf8)

Write-ScanLine ''
Write-ScanLine "Report: $reportPath"
Write-ScanLine "Page: $pagePath"
Write-ScanLine "Data: $jsonPath"

if ($Open) {
    Invoke-Item -LiteralPath $reportPath
}

if (-not $NoOpen) {
    Invoke-Item -LiteralPath $pagePath
}

exit 0
