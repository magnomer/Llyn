<#
.SYNOPSIS
Finds the giant object hiding behind split files: merges every partial type and measures how
tightly its parts are woven together.

.DESCRIPTION
A type split over several partial files can look tidy on disk while the compiler still sees one
object. This audit parses the source with Roslyn, merges the parts of every type, and reports:

  - Parts, lines, members and state slots of the merged type. A part is one declaration of
    the type, so one file may hold several parts.
  - Member references that cross from one part into another (cross references).
  - Weave: the share of parts the largest component of the member graph spans, members being
    linked by "reads, writes or calls". Free is the same share once hub state is removed.

A verdict per type, the same rules the convention test TAuditObject applies:
  single    one part, within the size thresholds
  large     one part, at thresholds.lines, thresholds.members or thresholds.state
  split     several parts, not a monolith
  monolith  at least thresholds.parts parts and thresholds.span lines, and either the largest
            component spans thresholds.weave of the parts once hub state is removed or the
            type carries thresholds.density cross references per member
A hub is a state slot reached from thresholds.hub or more parts. While enforced is true, the
monolith, hub and large counts must stay within their ceilings, and every split type within the
parts its row in parts names, an unnamed type holding one. A ceiling above its count is stale and
always fails.

Binding goes through the shared binder of auditbinder.cs and auditbinder.json: the tracked
sources, the generated code and the host build output, with no compile error allowed. The
solution must be built first. The scope line counts the listed source files the binder walked.
The helper targets helper.framework, the framework of the convention tests, and is compiled
once per text and SDK into the temp folder. The console lists the split types, every monolith,
hub and large type; a Markdown report with every hit is written to
{report.directory}\{prefix}{version}.md, in the same form as the report of the convention test.

Everything project-specific lives in auditobject.json next to this script. No project source
is modified. Git and the .NET SDK are required.

.PARAMETER Root
Project root to audit. Defaults to the directory containing this script's parent.

.PARAMETER ReportDirectory
Overrides report.directory for this run. Relative paths resolve against the project root.

.PARAMETER Top
Overrides console.top: how many split types the console table shows.

.PARAMETER Open
Open the report after the audit finishes.

.PARAMETER NoPause
Do not stop at each console page. Paging is also off when output or input is redirected.

.PARAMETER Help
Display this help and exit. The alias -? is supported.

.EXAMPLE
auditobject
Audit the current checkout.

.EXAMPLE
auditobject -ReportDirectory D:\temp\audit -Top 10 -Open
Write the report elsewhere, show ten rows, open the report.
#>
#requires -Version 5.1
# AUDITOBJECT GENERATION 12 - auditobject.ps1.
# A generation is not a revision count. It names functionality, not edits, so editing one of these
# files is never on its own a reason to raise it. Raise it only when the audited outcome changes.
# A generation names the set of checks the audit applies. Two projects on the same generation audit
# the same things and their reports compare directly, whatever else differs between the files. A check
# added, removed, or changed in what it reports is a new generation; wording, plumbing, and refactoring
# leave it alone. Every audit script shares one generation number with the convention-test settings,
# and each refuses a configuration written at another generation.
# Generation 9 merges partial types, counts parts, lines, members and state, measures shared state
# and cross-part references, and computes member-graph components to grade each split type.
# Generation 11: nothing the object audit reports changes; the number rises with the truth audit,
# which checks that a deportment field reaches no request, keeps one writer, holds no logic and
# treats no engine data.
# Generation 12: the audit applies the rules of the convention test: the monolith rule with hubs and
# density, the hub and large counts, and a part ceiling per split type, bound with no compile error.
[CmdletBinding()]
param(
    [string]$Root,
    [string]$ReportDirectory,
    [int]$Top = 0,
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
    auditobject.ps1

SYNOPSIS
    Merge partial types and grade how tightly their parts are woven together.

SYNTAX
    auditobject [-Root <path>] [-ReportDirectory <path>] [-Top <n>] [-Open] [-NoPause] [-Help]

OPTIONS
    -Root <path>             Project root. Defaults to the parent of the scripts folder.
    -ReportDirectory <path>  Overrides report.directory for this run.
    -Top <n>                 Overrides console.top.
    -Open                    Open the report when done.
    -NoPause                 No console paging.
    -Help                    Show this help.

VERDICTS
    single, large, split, monolith. See the script header for definitions.

COUNTERS
    Above ceiling   monolith, hub, large or part counts above their ceiling while enforced is true.
    Stale ceilings  ceilings above their count, enforced or not.
'@ | Write-Host
    exit 0
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$script:AuditGeneration = 12

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

Write-AuditLine "AUDITOBJECT GENERATION $script:AuditGeneration" -ForegroundColor Cyan
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
        'generation', 'project', 'enforced', 'helper.framework',
        'sources.roots', 'sources.extensions', 'sources.excludeSegments',
        'sources.excludeSuffixes', 'sources.excludePrefixes',
        'thresholds.parts', 'thresholds.span', 'thresholds.hub', 'thresholds.weave', 'thresholds.density',
        'thresholds.lines', 'thresholds.members', 'thresholds.state',
        'ceiling.monolith', 'ceiling.hub', 'ceiling.large', 'parts',
        'console.top',
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
        throw "The object-audit configuration is generation $($config.generation) but this tooling is generation $script:AuditGeneration.`nA generation names the set of checks applied, so the two must match: $ConfigPath"
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
    if ([string]::IsNullOrWhiteSpace($version)) {
        return 'unknown'
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
    foreach ($suffix in $Config.sources.excludeSuffixes) {
        if ($fileName.EndsWith($suffix, [System.StringComparison]::OrdinalIgnoreCase)) {
            return $true
        }
    }

    foreach ($prefix in $Config.sources.excludePrefixes) {
        if ($fileName.StartsWith($prefix, [System.StringComparison]::Ordinal)) {
            return $true
        }
    }

    return $false
}

function Get-ProjectSourceFiles {
    param([string]$ProjectRoot, $Config)

    $git = Get-Command git -ErrorAction SilentlyContinue
    if ($null -eq $git) {
        throw 'Git is required to enumerate project source files, but git was not found on PATH.'
    }

    if (-not (Test-Path -LiteralPath (Join-Path $ProjectRoot '.git'))) {
        throw "The project root is not a Git working tree: $ProjectRoot"
    }

    $pathspecs = @(foreach ($folder in $Config.sources.roots) {
            foreach ($extension in $Config.sources.extensions) {
                ':(icase)' + ([string]$folder).TrimEnd('/') + '/*' + $extension
            }
        })
    $lsArguments = @('-c', 'core.quotePath=false', '-C', $ProjectRoot,
        'ls-files', '--cached', '--others', '--exclude-standard', '--') + $pathspecs
    $nativePreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    $gitOutput = & $git.Source @lsArguments 2>&1
    $ErrorActionPreference = $nativePreference
    if ($LASTEXITCODE -ne 0) {
        throw "Git could not enumerate source files.`n$($gitOutput -join [Environment]::NewLine)"
    }

    $separator = [string][System.IO.Path]::DirectorySeparatorChar
    $rootPrefix = $ProjectRoot.TrimEnd($script:PathSeparators) + $separator
    $files = New-Object 'System.Collections.Generic.List[string]'

    foreach ($entry in $gitOutput) {
        $relativePath = ([string]$entry).Trim()
        if ([string]::IsNullOrWhiteSpace($relativePath) -or (Test-ExcludedRelativePath -RelativePath $relativePath -Config $Config)) {
            continue
        }

        $fullPath = $rootPrefix + $relativePath.Replace([char]'/', [System.IO.Path]::DirectorySeparatorChar)
        if (Test-Path -LiteralPath $fullPath -PathType Leaf) {
            $files.Add($fullPath)
        }
    }

    return @($files.ToArray() | Sort-Object -Property @{ Expression = { Get-OrdinalKey $_ } } -Unique)
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
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

if (args.Length != 7)
{
    Console.Error.WriteLine("usage: <config> <root> <manifest> <version> <report> <top> <binder>");
    return 2;
}

string configPath = args[0];
string projectRoot = args[1];
string manifestPath = args[2];
string version = args[3];
string reportPath = args[4];
int top = int.Parse(args[5]);
string binderPath = args[6];

JsonElement config = JsonDocument.Parse(File.ReadAllText(configPath)).RootElement;
JsonElement thresholds = config.GetProperty("thresholds");
Limits limits = new(
    thresholds.GetProperty("parts").GetInt32(),
    thresholds.GetProperty("span").GetInt32(),
    thresholds.GetProperty("hub").GetInt32(),
    thresholds.GetProperty("weave").GetDouble(),
    thresholds.GetProperty("density").GetDouble(),
    thresholds.GetProperty("lines").GetInt32(),
    thresholds.GetProperty("members").GetInt32(),
    thresholds.GetProperty("state").GetInt32());
int generation = config.GetProperty("generation").GetInt32();
bool enforced = config.GetProperty("enforced").GetBoolean();
JsonElement ceiling = config.GetProperty("ceiling");
Dictionary<string, int> ceilings = new(StringComparer.Ordinal)
{
    ["Monolith"] = ceiling.GetProperty("monolith").GetInt32(),
    ["Hub"] = ceiling.GetProperty("hub").GetInt32(),
    ["Large"] = ceiling.GetProperty("large").GetInt32(),
};
Dictionary<string, int> partCeilings = config.GetProperty("parts").EnumerateObject()
    .ToDictionary(item => item.Name, item => item.Value.GetInt32(), StringComparer.Ordinal);

HashSet<string> chosen = File.ReadAllLines(manifestPath)
    .Where(line => line.Length > 0)
    .Select(line => Path.GetFullPath(Path.Combine(projectRoot, line)))
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
LAuditBinder binder = LAuditBinder.LAuditBinderRead(projectRoot, binderPath);
CSharpCompilation compilation = binder.LAuditCompilation;
List<SyntaxTree> trees = binder.LAuditTrees.Where(tree => chosen.Contains(Path.GetFullPath(tree.FilePath))).ToList();
if (trees.Count == 0)
{
    Console.Error.WriteLine("No listed source file is bound, so the audit cannot judge.");
    return 2;
}

Dictionary<INamedTypeSymbol, TypeRecord> types = new(SymbolEqualityComparer.Default);

// Pass 1: register every part and every member so a reference bound in pass 2 can find its
// target regardless of which file declares it.
foreach (SyntaxTree tree in trees)
{
    SemanticModel model = compilation.GetSemanticModel(tree, true);
    string relative = binder.LAuditRelativeRead(tree.FilePath);
    foreach (TypeDeclarationSyntax declaration in tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>())
    {
        if (model.GetDeclaredSymbol(declaration) is not INamedTypeSymbol typeSymbol)
        {
            continue;
        }

        if (!types.TryGetValue(typeSymbol, out TypeRecord? type))
        {
            type = new TypeRecord(typeSymbol);
            types.Add(typeSymbol, type);
        }

        FileLinePositionSpan span = declaration.GetLocation().GetLineSpan();
        PartRecord part = new(relative, span.EndLinePosition.Line - span.StartLinePosition.Line + 1);
        type.Parts.Add(part);

        foreach (MemberDeclarationSyntax member in declaration.Members)
        {
            if (member is BaseTypeDeclarationSyntax or DelegateDeclarationSyntax)
            {
                continue;
            }

            foreach ((ISymbol symbol, bool state) in DeclaredMembers(model, member))
            {
                if (type.Members.ContainsKey(symbol))
                {
                    continue;
                }

                type.Members.Add(symbol, new MemberRecord(type.Members.Count, symbol, part, state));
            }
        }
    }
}

// Pass 2: every simple name inside a member body that binds to a member of the same type is an
// edge from the enclosing member to the target. Self references are not edges.
foreach (SyntaxTree tree in trees)
{
    SemanticModel model = compilation.GetSemanticModel(tree, true);
    foreach (TypeDeclarationSyntax declaration in tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>())
    {
        if (model.GetDeclaredSymbol(declaration) is not INamedTypeSymbol typeSymbol
            || !types.TryGetValue(typeSymbol, out TypeRecord? type))
        {
            continue;
        }

        foreach (MemberDeclarationSyntax member in declaration.Members)
        {
            if (member is BaseTypeDeclarationSyntax or DelegateDeclarationSyntax)
            {
                continue;
            }

            MemberRecord[] sources = DeclaredMembers(model, member)
                .Select(pair => type.Members.TryGetValue(pair.Symbol, out MemberRecord? found) ? found : null)
                .Where(found => found is not null)
                .Select(found => found!)
                .ToArray();
            if (sources.Length == 0)
            {
                continue;
            }

            foreach (SimpleNameSyntax name in member.DescendantNodes().OfType<SimpleNameSyntax>())
            {
                SymbolInfo info = model.GetSymbolInfo(name);
                ISymbol? bound = info.Symbol ?? info.CandidateSymbols.FirstOrDefault();
                if (bound is IMethodSymbol { AssociatedSymbol: not null } accessor)
                {
                    bound = accessor.AssociatedSymbol;
                }

                if (bound is not (IMethodSymbol or IFieldSymbol or IPropertySymbol or IEventSymbol))
                {
                    continue;
                }

                bound = bound.OriginalDefinition;
                if (!type.Members.TryGetValue(bound, out MemberRecord? target))
                {
                    continue;
                }

                foreach (MemberRecord source in sources)
                {
                    if (!ReferenceEquals(source, target))
                    {
                        source.Uses.Add(target);
                        target.UsedBy.Add(source);
                    }
                }
            }
        }
    }
}

List<TypeSummary> summaries = types.Values
    .Where(type => type.Members.Count > 0)
    .Select(type => Summarize(type, limits))
    .OrderByDescending(summary => summary.Lines)
    .ThenBy(summary => summary.Name, StringComparer.Ordinal)
    .ToList();

List<TypeSummary> split = summaries.Where(summary => summary.Parts > 1).ToList();
Dictionary<string, List<string>> hits = new(StringComparer.Ordinal)
{
    ["Monolith"] = summaries
        .Where(summary => summary.Monolith)
        .Select(summary => $"{summary.Name}: {summary.Parts} parts, {summary.Lines} lines, {summary.Cross} cross references, "
            + $"weave {summary.Free:0.00} without hubs, density {summary.Density:0.00}")
        .ToList(),
    ["Hub"] = summaries
        .SelectMany(summary => summary.Hubs.Select(hub => $"{summary.Name}: {hub}"))
        .ToList(),
    ["Large"] = summaries
        .Where(summary => summary.Large)
        .Select(summary => $"{summary.Name}: {summary.Lines} lines, {summary.Members} members, {summary.State} state slots")
        .ToList(),
};
Dictionary<string, int> partCounts = split.ToDictionary(summary => summary.Name, summary => summary.Parts, StringComparer.Ordinal);
List<string> partsOver = partCounts
    .Where(pair => pair.Value > partCeilings.GetValueOrDefault(pair.Key, 1))
    .Select(pair => $"{pair.Key}: {pair.Value} part(s), ceiling {partCeilings.GetValueOrDefault(pair.Key, 1)}")
    .ToList();
List<string> above = enforced
    ? ceilings
        .Where(pair => hits[pair.Key].Count > pair.Value)
        .Select(pair => $"{pair.Key}: {hits[pair.Key].Count} hit(s), ceiling {pair.Value}")
        .Concat(partsOver)
        .ToList()
    : [];
Dictionary<string, int> counts = new(partCounts, StringComparer.Ordinal);
foreach (string kind in ceilings.Keys)
{
    counts[kind] = hits[kind].Count;
}

List<string> stale = ceilings
    .Concat(partCeilings)
    .Where(pair => counts.GetValueOrDefault(pair.Key, 1) < pair.Value)
    .Select(pair => $"{pair.Key}: {counts.GetValueOrDefault(pair.Key, 1)} hit(s), ceiling {pair.Value}")
    .ToList();

Console.WriteLine($"Scanned: {trees.Count:N0} source files, ceilings {(enforced ? "enforced" : "not enforced")}");
Console.WriteLine();
Console.WriteLine("Counters");
Console.WriteLine("--------");
Console.WriteLine($"Above ceiling   {above.Count:N0}");
Console.WriteLine($"Stale ceilings  {stale.Count:N0}");

Console.WriteLine();
Console.WriteLine("Hits by kind");
Console.WriteLine("------------");
List<string[]> kindRows = ceilings
    .Select(pair => new[] { pair.Key, hits[pair.Key].Count.ToString("N0"), pair.Value.ToString("N0") })
    .ToList();
foreach (string line in TextTable(["Kind", "Hits", "Ceiling"], kindRows))
{
    Console.WriteLine(line);
}

Console.WriteLine();
Console.WriteLine("Types by verdict");
Console.WriteLine("----------------");
List<string[]> verdictRows =
[
    ["Monolith", split.Count(summary => summary.Monolith).ToString("N0")],
    ["Split", split.Count(summary => !summary.Monolith).ToString("N0")],
    ["Large", summaries.Count(summary => summary.Large).ToString("N0")],
    ["Single", summaries.Count(summary => summary.Parts == 1 && !summary.Large).ToString("N0")],
    ["Total", summaries.Count.ToString("N0")],
];
foreach (string line in TextTable(["Verdict", "Types"], verdictRows))
{
    Console.WriteLine(line);
}

string[] header = ["Type", "Parts", "Lines", "Members", "State", "Hubs", "Cross", "Weave", "Free", "Density", "Monolith"];
if (split.Count > 0)
{
    string splitHeading = $"Types split into several parts ({split.Count:N0})";
    Console.WriteLine();
    Console.WriteLine(splitHeading);
    Console.WriteLine(new string('-', splitHeading.Length));
    foreach (string line in TextTable(header, split.Take(top).Select(Row).ToList()))
    {
        Console.WriteLine(line);
    }

    if (split.Count > top)
    {
        Console.WriteLine($"... and {split.Count - top:N0} more in the report.");
    }
}

List<(string Title, List<string> Rows)> sections = ceilings.Keys
    .Select(kind => (kind, hits[kind]))
    .Append(("Above ceiling", above))
    .Append(("Stale ceilings", stale))
    .ToList();
foreach ((string title, List<string> rows) in sections.Where(section => section.Rows.Count > 0))
{
    string heading = $"{title} ({rows.Count:N0})";
    Console.WriteLine();
    Console.WriteLine(heading);
    Console.WriteLine(new string('-', heading.Length));
    rows.Take(top).ToList().ForEach(Console.WriteLine);
    if (rows.Count > top)
    {
        Console.WriteLine($"... and {rows.Count - top:N0} more in the report.");
    }
}

Console.WriteLine();
Console.WriteLine($"Report: {reportPath}");

List<string> lines =
[
    $"# Object audit {version}",
    string.Empty,
    $"- Generation: {generation}",
    $"- Enforced: {enforced}",
    $"- Types: {summaries.Count}, split into several parts: {split.Count}",
];
lines.AddRange(ceilings.Select(pair => $"- {pair.Key}: {hits[pair.Key].Count}, ceiling {pair.Value}"));
lines.Add($"- Above ceiling: {above.Count}");
lines.Add($"- Stale ceilings: {stale.Count}");
lines.Add(string.Empty);
lines.Add($"A monolith has at least {limits.Parts} parts and {limits.Span} lines, and either its largest member component "
    + $"still spans {limits.Weave:0.00} of the parts once hub state is removed or it carries {limits.Density:0.00} cross references per "
    + $"member. A hub is a state slot reached from {limits.Hub} or more parts. A large type has one part and at least "
    + $"{limits.Lines} lines, {limits.Members} members or {limits.State} state slots. "
    + "A part is one declaration of the type, so one file may hold several parts.");
lines.Add(string.Empty);
lines.Add("## Split types");
lines.Add(string.Empty);
lines.Add("| " + string.Join(" | ", header) + " |");
lines.Add("|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|");
lines.AddRange(split.Select(summary => $"| `{summary.Name}` | {summary.Parts} | {summary.Lines} | {summary.Members} | {summary.State} "
    + $"| {summary.Hubs.Count} | {summary.Cross} | {summary.Weave:0.00} | {summary.Free:0.00} | {summary.Density:0.00} "
    + $"| {(summary.Monolith ? "yes" : "no")} |"));
foreach ((string title, List<string> rows) in sections)
{
    lines.Add(string.Empty);
    lines.Add($"## {title}");
    if (rows.Count > 0)
    {
        lines.Add(string.Empty);
        lines.AddRange(rows.Select(row => "- " + row));
    }
}

string? reportFolder = Path.GetDirectoryName(reportPath);
if (!string.IsNullOrWhiteSpace(reportFolder))
{
    Directory.CreateDirectory(reportFolder);
}

File.WriteAllText(reportPath, string.Join("\n", lines) + "\n", new UTF8Encoding(false));
return above.Count + stale.Count > 0 ? 3 : 0;

static IEnumerable<(ISymbol Symbol, bool State)> DeclaredMembers(SemanticModel model, MemberDeclarationSyntax member)
{
    switch (member)
    {
        case FieldDeclarationSyntax field:
            foreach (VariableDeclaratorSyntax variable in field.Declaration.Variables)
            {
                if (model.GetDeclaredSymbol(variable) is IFieldSymbol fieldSymbol)
                {
                    yield return (fieldSymbol, !fieldSymbol.IsConst);
                }
            }
            break;
        case EventFieldDeclarationSyntax eventField:
            foreach (VariableDeclaratorSyntax variable in eventField.Declaration.Variables)
            {
                if (model.GetDeclaredSymbol(variable) is IEventSymbol eventSymbol)
                {
                    yield return (eventSymbol, true);
                }
            }
            break;
        case PropertyDeclarationSyntax property:
            if (model.GetDeclaredSymbol(property) is ISymbol propertySymbol)
            {
                bool auto = property.ExpressionBody is null
                    && property.AccessorList is not null
                    && property.AccessorList.Accessors.All(accessor => accessor.Body is null && accessor.ExpressionBody is null);
                yield return (propertySymbol, auto);
            }
            break;
        default:
            if (model.GetDeclaredSymbol(member) is ISymbol symbol)
            {
                yield return (symbol, false);
            }
            break;
    }
}

static TypeSummary Summarize(TypeRecord type, Limits limits)
{
    MemberRecord[] members = type.Members.Values.ToArray();
    int cross = members.Sum(member => member.Uses.Count(target => !ReferenceEquals(member.Part, target.Part)));

    HashSet<MemberRecord> hubs = [];
    List<string> hubNames = [];
    foreach (MemberRecord member in members.Where(member => member.IsState))
    {
        HashSet<PartRecord> touching = [member.Part];
        touching.UnionWith(member.UsedBy.Select(user => user.Part));
        if (touching.Count >= limits.Hub)
        {
            hubs.Add(member);
            hubNames.Add($"`{member.Symbol.Name}` reaches {touching.Count} parts");
        }
    }

    int lines = type.Parts.Sum(part => part.Lines);
    int partCount = type.Parts.Count;
    double weave = Weave(members, partCount, []);
    double free = Weave(members, partCount, hubs);
    double density = members.Length == 0 ? 0 : cross / (double)members.Length;
    int state = members.Count(member => member.IsState);
    bool monolith = partCount >= limits.Parts
        && lines >= limits.Span
        && (free >= limits.Weave || density >= limits.Density);
    bool large = partCount == 1
        && (lines >= limits.Lines || members.Length >= limits.Members || state >= limits.State);

    return new TypeSummary(
        type.Symbol.ToDisplayString(),
        partCount,
        lines,
        members.Length,
        state,
        hubNames,
        cross,
        weave,
        free,
        density,
        monolith,
        large);
}

static double Weave(MemberRecord[] members, int partCount, HashSet<MemberRecord> excluded)
{
    if (partCount == 0 || members.Length == 0)
    {
        return 0;
    }

    int[] parent = Enumerable.Range(0, members.Length).ToArray();
    int Find(int index)
    {
        while (parent[index] != index)
        {
            parent[index] = parent[parent[index]];
            index = parent[index];
        }
        return index;
    }

    foreach (MemberRecord member in members.Where(member => !excluded.Contains(member)))
    {
        foreach (MemberRecord target in member.Uses.Where(target => !excluded.Contains(target)))
        {
            parent[Find(member.Index)] = Find(target.Index);
        }
    }

    int widest = members
        .Where(member => !excluded.Contains(member))
        .GroupBy(member => Find(member.Index))
        .Select(group => group.Select(member => member.Part).Distinct().Count())
        .DefaultIfEmpty(0)
        .Max();
    return widest / (double)partCount;
}

static string[] Row(TypeSummary summary) =>
[
    summary.Name,
    summary.Parts.ToString("N0"),
    summary.Lines.ToString("N0"),
    summary.Members.ToString("N0"),
    summary.State.ToString("N0"),
    summary.Hubs.Count.ToString("N0"),
    summary.Cross.ToString("N0"),
    summary.Weave.ToString("0.00"),
    summary.Free.ToString("0.00"),
    summary.Density.ToString("0.00"),
    summary.Monolith ? "yes" : "no",
];

static IEnumerable<string> TextTable(string[] header, List<string[]> rows)
{
    int[] widths = header.Select((cell, column) => Math.Max(cell.Length, rows.Count == 0 ? 0 : rows.Max(row => row[column].Length))).ToArray();
    bool[] numeric = header.Select((cell, column) => rows.Count > 0 && rows.All(row => row[column] == "-" || double.TryParse(row[column], out _))).ToArray();
    string Line(string[] cells) => string.Join("  ", cells.Select((cell, column) => numeric[column] ? cell.PadLeft(widths[column]) : cell.PadRight(widths[column]))).TrimEnd();
    yield return Line(header);
    yield return string.Join("  ", widths.Select(width => new string('-', width)));
    foreach (string[] row in rows)
    {
        yield return Line(row);
    }
}

internal sealed class TypeRecord(INamedTypeSymbol symbol)
{
    public INamedTypeSymbol Symbol { get; } = symbol;
    public List<PartRecord> Parts { get; } = [];
    public Dictionary<ISymbol, MemberRecord> Members { get; } = new(SymbolEqualityComparer.Default);
}

internal sealed class PartRecord(string path, int lines)
{
    public string Path { get; } = path;
    public int Lines { get; } = lines;
}

internal sealed class MemberRecord(int index, ISymbol symbol, PartRecord part, bool isState)
{
    public int Index { get; } = index;
    public ISymbol Symbol { get; } = symbol;
    public PartRecord Part { get; } = part;
    public bool IsState { get; } = isState;
    public HashSet<MemberRecord> Uses { get; } = [];
    public HashSet<MemberRecord> UsedBy { get; } = [];
}

internal sealed record TypeSummary(
    string Name,
    int Parts,
    int Lines,
    int Members,
    int State,
    List<string> Hubs,
    int Cross,
    double Weave,
    double Free,
    double Density,
    bool Monolith,
    bool Large);

internal sealed record Limits(int Parts, int Span, int Hub, double Weave, double Density, int Lines, int Members, int State);
'@

function Get-HelperBinary {
    param([string]$DotnetPath, [string]$ProjectName, [string]$TargetFramework)

    $nativePreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    $sdkOutput = & $DotnetPath --version 2>&1
    $ErrorActionPreference = $nativePreference
    if ($LASTEXITCODE -ne 0) {
        throw "The .NET SDK version could not be read.`n$($sdkOutput -join [Environment]::NewLine)"
    }

    $sdkVersion = ([string]($sdkOutput | Select-Object -First 1)).Trim()
    $binderSource = [System.IO.File]::ReadAllText((Join-Path $PSScriptRoot 'auditbinder.cs'))
    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        $seed = $script:HelperProject + "`n" + $script:HelperProgram + "`n" + $binderSource + "`n" + $TargetFramework + "`n" + $sdkVersion
        $digest = $sha.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($seed))
    }
    finally {
        $sha.Dispose()
    }

    $hash = ([System.BitConverter]::ToString($digest) -replace '-', '').Substring(0, 16).ToLowerInvariant()
    $cacheParent = Join-Path ([System.IO.Path]::GetTempPath()) ($ProjectName + '-AuditObject')
    $cacheFolder = Join-Path $cacheParent $hash
    $binaryPath = Join-Path (Join-Path $cacheFolder 'bin') 'AuditObject.Helper.dll'
    if (Test-Path -LiteralPath $binaryPath -PathType Leaf) {
        return $binaryPath
    }

    Write-AuditLine 'Compiling the object binder once for this SDK...' -ForegroundColor DarkGray
    if (Test-Path -LiteralPath $cacheParent) {
        Get-ChildItem -LiteralPath $cacheParent -Directory | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    }

    $helperFolder = Join-Path $cacheFolder 'helper'
    [System.IO.Directory]::CreateDirectory($helperFolder) | Out-Null
    $encoding = New-Object System.Text.UTF8Encoding($false)
    $projectPath = Join-Path $helperFolder 'AuditObject.Helper.csproj'
    [System.IO.File]::WriteAllText($projectPath, $script:HelperProject.Replace('{TARGET_FRAMEWORK}', $TargetFramework), $encoding)
    [System.IO.File]::WriteAllText((Join-Path $helperFolder 'Program.cs'), $script:HelperProgram, $encoding)
    [System.IO.File]::WriteAllText((Join-Path $helperFolder 'AuditBinder.cs'), $binderSource, $encoding)
    $nativePreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    $buildOutput = & $DotnetPath build $projectPath --configuration Release --nologo --verbosity quiet --output (Join-Path $cacheFolder 'bin') 2>&1
    $ErrorActionPreference = $nativePreference
    if ($LASTEXITCODE -ne 0) {
        Remove-Item -LiteralPath $cacheFolder -Recurse -Force -ErrorAction SilentlyContinue
        throw "The object binder could not be built.`n$($buildOutput -join [Environment]::NewLine)"
    }

    return $binaryPath
}

$projectRoot = Resolve-ProjectRoot -Path $Root
$configPath = Join-Path $PSScriptRoot 'auditobject.json'
$config = Read-AuditConfig -ConfigPath $configPath
$version = Read-ProjectVersion -ProjectRoot $projectRoot -Config $config

if ($Top -le 0) {
    $Top = [int]$config.console.top
}

$reportFolder = if ([string]::IsNullOrWhiteSpace($ReportDirectory)) { [string]$config.report.directory } else { $ReportDirectory }
if (-not [System.IO.Path]::IsPathRooted($reportFolder)) {
    $reportFolder = Join-Path $projectRoot $reportFolder
}
$reportPath = Join-Path $reportFolder ([string]$config.report.prefix + $version + '.md')

[string[]]$sourceFiles = @(Get-ProjectSourceFiles -ProjectRoot $projectRoot -Config $config)
if ($sourceFiles.Length -eq 0) {
    throw "No tracked source file was enumerated, so the audit would pass vacuously: $projectRoot"
}

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if ($null -eq $dotnet) {
    throw 'The .NET SDK is required, but dotnet was not found on PATH.'
}

$previousNoLogo = $env:DOTNET_NOLOGO
$previousTelemetry = $env:DOTNET_CLI_TELEMETRY_OPTOUT
$env:DOTNET_NOLOGO = '1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$temporaryFolder = Join-Path ([System.IO.Path]::GetTempPath()) ($config.project + '-AuditObject-' + [Guid]::NewGuid().ToString('N'))
[System.IO.Directory]::CreateDirectory($temporaryFolder) | Out-Null

try {
    $binaryPath = Get-HelperBinary -DotnetPath $dotnet.Source -ProjectName ([string]$config.project) -TargetFramework ([string]$config.helper.framework)
    $manifestPath = Join-Path $temporaryFolder 'sources.txt'
    [System.IO.File]::WriteAllLines($manifestPath, $sourceFiles, [System.Text.UTF8Encoding]::new($false))

    $arguments = @(
        $binaryPath,
        $configPath,
        $projectRoot,
        $manifestPath,
        $version,
        $reportPath,
        $Top,
        (Join-Path $PSScriptRoot 'auditbinder.json')
    )

    $nativePreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    $auditOutput = & $dotnet.Source @arguments 2>&1
    $ErrorActionPreference = $nativePreference
    $auditExitCode = $LASTEXITCODE

    if ($auditExitCode -ne 0 -and $auditExitCode -ne 3) {
        throw "The object audit failed.`n$($auditOutput -join [Environment]::NewLine)"
    }

    foreach ($line in $auditOutput) {
        $text = [string]$line
        if ($text.StartsWith('Scanned: ', [System.StringComparison]::Ordinal)) {
            Write-AuditLine $text -ForegroundColor DarkGray
        }
        else {
            Write-AuditLine $text
        }
    }
}
finally {
    $env:DOTNET_NOLOGO = $previousNoLogo
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = $previousTelemetry
    if (Test-Path -LiteralPath $temporaryFolder) {
        Remove-Item -LiteralPath $temporaryFolder -Recurse -Force -ErrorAction SilentlyContinue
    }
}

if ($Open -and (Test-Path -LiteralPath $reportPath -PathType Leaf)) {
    Invoke-Item -LiteralPath $reportPath
}

exit ([int]($auditExitCode -ne 0))
