<#
.SYNOPSIS
Finds the giant object hiding behind split files: merges every partial type and measures how
tightly its parts are woven together.

.DESCRIPTION
A type split over several partial files can look tidy on disk while the compiler still sees one
object. This audit parses the source with Roslyn, merges the parts of every type, and reports:

  - Parts, lines, members and state slots of the merged type.
  - State that more than one part touches (shared state).
  - Member references that cross from one part into another (cross references).
  - Connected components of the member graph: members linked by "reads, writes or calls".
    One component spanning every part means the split is cosmetic. Several components each
    inside its own parts mean the type already has seams and could become separate classes.

A verdict per type, the same rules the convention test TAuditObject applies:
  single    one part, within the size thresholds
  large     one part, at thresholds.lines, thresholds.members or thresholds.state
  split     several parts, not a monolith
  monolith  at least thresholds.parts parts and thresholds.span lines, and either the largest
            component spans thresholds.weave of the parts once hub state is removed or the
            type carries thresholds.density cross references per member
A hub is a state slot reached from thresholds.hub or more parts. The monolith, hub and large
counts must equal their ceilings, and every split type must hold exactly the parts its row in
parts names, an unnamed type holding one.

Binding goes through the shared binder of auditbinder.cs and auditbinder.json: the tracked
sources, the generated code and the host build output, with no compile error allowed. The
solution must be built first. The console prints a ranked table; a
Markdown report with per-part detail is written to {report.directory}\{prefix}{version}.md.

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
    single, large, seamed, woven, monolith. See the script header for definitions.
    The monolith and large counts are gated: each must equal its ceiling in the configuration.
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
        'generation', 'project',
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
    if (-not ($Config.sources.extensions -contains $extension)) {
        return $true
    }

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

    $lsArguments = @('-c', 'core.quotePath=false', '-C', $ProjectRoot,
        'ls-files', '--cached', '--others', '--exclude-standard', '--') + @($Config.sources.roots)
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

function Write-AuditHelper {
    param([string]$HelperFolder, [string]$TargetFramework)

    $projectContent = @'
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
    <Reference Include="Microsoft.CodeAnalysis">
      <HintPath>$(MSBuildSDKsPath)/../Roslyn/bincore/Microsoft.CodeAnalysis.dll</HintPath>
      <Private>true</Private>
    </Reference>
    <Reference Include="Microsoft.CodeAnalysis.CSharp">
      <HintPath>$(MSBuildSDKsPath)/../Roslyn/bincore/Microsoft.CodeAnalysis.CSharp.dll</HintPath>
      <Private>true</Private>
    </Reference>
  </ItemGroup>
</Project>
'@
    $projectContent = $projectContent.Replace('{TARGET_FRAMEWORK}', $TargetFramework)

    $programContent = @'
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
JsonElement ceiling = config.GetProperty("ceiling");
Dictionary<string, int> ceilings = new(StringComparer.Ordinal)
{
    ["monolith"] = ceiling.GetProperty("monolith").GetInt32(),
    ["hub"] = ceiling.GetProperty("hub").GetInt32(),
    ["large"] = ceiling.GetProperty("large").GetInt32(),
};
Dictionary<string, int> partCeilings = config.GetProperty("parts").EnumerateObject()
    .ToDictionary(item => item.Name, item => item.Value.GetInt32(), StringComparer.Ordinal);

HashSet<string> chosen = File.ReadAllLines(manifestPath)
    .Where(line => line.Length > 0)
    .Select(line => Path.GetFullPath(Path.Combine(projectRoot, line)))
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
AuditBinder binder = AuditBinder.Bind(projectRoot, binderPath);
CSharpCompilation compilation = binder.Compilation;
List<SyntaxTree> trees = binder.Tracked.Where(tree => chosen.Contains(Path.GetFullPath(tree.FilePath))).ToList();
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
    string relative = binder.Relative(tree.FilePath);
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
        PartRecord part = new(type.Parts.Count, relative, span.EndLinePosition.Line - span.StartLinePosition.Line + 1);
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

                MemberRecord record = new(type.Members.Count, symbol, part, state, symbol.IsStatic);
                type.Members.Add(symbol, record);
                part.Members.Add(record);
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
    .ThenBy(summary => summary.FullName, StringComparer.Ordinal)
    .ToList();

List<TypeSummary> split = summaries.Where(summary => summary.Parts.Count > 1).ToList();
List<TypeSummary> large = summaries.Where(summary => summary.Verdict == "large").ToList();

Console.WriteLine();
Console.WriteLine("Counters");
Console.WriteLine("--------");
int VerdictCount(string verdict) => verdict == "hub"
    ? summaries.Sum(summary => summary.Hubs.Count)
    : summaries.Count(summary => summary.Verdict == verdict);
Dictionary<string, int> partCounts = split.ToDictionary(summary => summary.FullName, summary => summary.Parts.Count, StringComparer.Ordinal);
List<string> partsOver = partCounts
    .Where(pair => pair.Value > partCeilings.GetValueOrDefault(pair.Key, 1))
    .Select(pair => $"{pair.Key}: {pair.Value} parts, ceiling {partCeilings.GetValueOrDefault(pair.Key, 1)}")
    .ToList();
List<string> partsStale = partCeilings
    .Where(pair => partCounts.GetValueOrDefault(pair.Key, 1) < pair.Value)
    .Select(pair => $"{pair.Key}: {partCounts.GetValueOrDefault(pair.Key, 1)} parts, ceiling {pair.Value}")
    .ToList();
int aboveCount = ceilings.Count(pair => VerdictCount(pair.Key) > pair.Value) + partsOver.Count;
int staleCount = ceilings.Count(pair => VerdictCount(pair.Key) < pair.Value) + partsStale.Count;
Console.WriteLine($"Above ceiling   {aboveCount:N0}");
Console.WriteLine($"Stale ceilings  {staleCount:N0}");

Console.WriteLine();
Console.WriteLine("Types by verdict");
Console.WriteLine("----------------");
List<string[]> verdictRows = new[] { "monolith", "hub", "large", "split", "single" }
    .Select(verdict => new[]
    {
        verdict,
        VerdictCount(verdict).ToString("N0"),
        ceilings.TryGetValue(verdict, out int limit) ? limit.ToString("N0") : "-",
    })
    .ToList();
verdictRows.Add(["Total", summaries.Count.ToString("N0"), "-"]);
foreach (string line in TextTable(["Verdict", "Types", "Ceiling"], verdictRows))
{
    Console.WriteLine(line);
}

string[] header = ["Type", "Parts", "Lines", "Members", "State", "Shared", "Hubs", "XRef", "Comp", "Weave", "Free", "Density", "Verdict"];
if (split.Count > 0)
{
    string splitHeading = $"Types declared in more than one file ({split.Count:N0})";
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

foreach ((string title, List<string> rows) in new[] { ("Parts above their ceiling", partsOver), ("Stale part ceilings", partsStale) })
{
    if (rows.Count == 0)
    {
        continue;
    }

    string heading = $"{title} ({rows.Count:N0})";
    Console.WriteLine();
    Console.WriteLine(heading);
    Console.WriteLine(new string('-', heading.Length));
    rows.ForEach(Console.WriteLine);
}

Console.WriteLine();
Console.WriteLine($"Report: {reportPath}");

WriteReport(reportPath, version, summaries, split, large, header, limits);
return aboveCount + staleCount > 0 ? 3 : 0;

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

    int crossReferences = 0;
    foreach (MemberRecord member in members)
    {
        foreach (MemberRecord target in member.Uses)
        {
            parent[Find(member.Index)] = Find(target.Index);
            if (!ReferenceEquals(member.Part, target.Part))
            {
                crossReferences++;
                member.Part.OutReferences++;
                target.Part.InReferences++;
                member.Part.DependsOn.Add(target.Part);
            }
        }
    }

    List<ComponentSummary> components = members
        .GroupBy(member => Find(member.Index))
        .Select(group => new ComponentSummary(
            group.Count(),
            group.Select(member => member.Part).Distinct().OrderBy(part => part.Index).ToArray(),
            group.Select(member => member.Symbol.Name).OrderBy(name => name, StringComparer.Ordinal).ToArray()))
        .OrderByDescending(component => component.Parts.Length)
        .ThenByDescending(component => component.MemberCount)
        .ToList();

    List<StateSummary> shared = [];
    HashSet<MemberRecord> hubs = [];
    List<string> hubNames = [];
    int stateCount = 0;
    foreach (MemberRecord member in members.Where(member => member.IsState))
    {
        stateCount++;
        HashSet<PartRecord> touching = [member.Part];
        foreach (MemberRecord user in member.UsedBy)
        {
            touching.Add(user.Part);
        }

        foreach (PartRecord part in touching)
        {
            if (touching.Count > 1)
            {
                part.SharedState++;
            }
            else
            {
                part.OwnState++;
            }
        }

        if (touching.Count > 1)
        {
            shared.Add(new StateSummary(member.Symbol.Name, member.IsStatic, touching.OrderBy(part => part.Index).ToArray()));
        }

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
    double density = members.Length == 0 ? 0 : crossReferences / (double)members.Length;
    bool monolith = partCount >= limits.Parts
        && lines >= limits.Span
        && (free >= limits.Weave || density >= limits.Density);
    bool oversized = lines >= limits.Lines || members.Length >= limits.Members || stateCount >= limits.State;
    string verdict = partCount == 1
        ? (oversized ? "large" : "single")
        : monolith ? "monolith" : "split";

    return new TypeSummary(
        type.Symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
        type.Symbol.ToDisplayString(),
        type.Parts,
        lines,
        members.Length,
        members.Count(member => !member.IsStatic),
        stateCount,
        shared,
        hubNames,
        crossReferences,
        components.Where(component => component.MemberCount > 1).ToList(),
        components.Count(component => component.MemberCount == 1),
        weave,
        free,
        density,
        verdict);
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
    summary.Parts.Count.ToString("N0"),
    summary.Lines.ToString("N0"),
    summary.MemberCount.ToString("N0"),
    summary.StateCount.ToString("N0"),
    summary.SharedState.Count.ToString("N0"),
    summary.Hubs.Count.ToString("N0"),
    summary.CrossReferences.ToString("N0"),
    summary.Components.Count.ToString("N0"),
    summary.Weave.ToString("0.00"),
    summary.Free.ToString("0.00"),
    summary.Density.ToString("0.00"),
    summary.Verdict,
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

static IEnumerable<string> MarkdownTable(string[] header, IEnumerable<string[]> rows)
{
    yield return "| " + string.Join(" | ", header) + " |";
    yield return "|" + string.Join("|", header.Select((cell, column) => column == 0 ? " --- " : " ---: ")) + "|";
    foreach (string[] row in rows)
    {
        yield return "| " + string.Join(" | ", row) + " |";
    }
}

static string PartNames(IEnumerable<PartRecord> parts) =>
    string.Join(", ", parts.Select(part => "`" + Path.GetFileName(part.Path) + "`"));

static void WriteReport(
    string reportPath,
    string version,
    List<TypeSummary> summaries,
    List<TypeSummary> split,
    List<TypeSummary> large,
    string[] header,
    Limits limits)
{
    List<string> lines =
    [
        $"# AuditObject {version}",
        string.Empty,
        $"Generated {DateTime.Now:yyyy-MM-dd HH:mm}. Types parsed: {summaries.Count}. Split over several files: {split.Count}.",
        string.Empty,
        $"A monolith has at least {limits.Parts} parts and {limits.Span} lines, and either its largest member component",
        $"still spans {limits.Weave:0.00} of the parts once hub state is removed or it carries {limits.Density:0.00} cross references",
        $"per member. A hub is a state slot reached from {limits.Hub} or more parts. A large type has one part and at least",
        $"{limits.Lines} lines, {limits.Members} members or {limits.State} state slots.",
        string.Empty,
        "Columns: Parts = files declaring the type. Lines = summed declaration lines. Members = all declared members.",
        "State = fields, field-like events and auto-properties. Shared = state touched from more than one part.",
        "XRef = member references crossing from one part into another. Comp = connected components with more than one member.",
        "Weave = parts spanned by the largest component / parts. 1.00 means one component reaches every file.",
        "Hubs = state slots reached from the hub count of parts. Free = weave once hubs are removed.",
        "Density = cross references per member.",
        string.Empty,
        "## Counters",
        string.Empty,
    ];

    foreach (string verdict in new[] { "monolith", "large", "split", "single" })
    {
        lines.Add($"- {verdict}: {summaries.Count(summary => summary.Verdict == verdict)}");
    }

    lines.Add($"- hub: {summaries.Sum(summary => summary.Hubs.Count)}");

    lines.Add(string.Empty);
    lines.Add("## Split types");
    lines.Add(string.Empty);
    lines.AddRange(MarkdownTable(header, split.Select(summary => Row(summary).Select((cell, column) => column == 0 ? "`" + cell + "`" : cell).ToArray())));

    lines.Add(string.Empty);
    lines.Add("## Detail");

    foreach (TypeSummary summary in split)
    {
        lines.Add(string.Empty);
        lines.Add($"### `{summary.FullName}` - {summary.Verdict}");
        lines.Add(string.Empty);
        lines.Add($"{summary.Parts.Count} parts, {summary.Lines} lines, {summary.MemberCount} members ({summary.InstanceMembers} instance), "
            + $"{summary.StateCount} state slots ({summary.SharedState.Count} shared), {summary.CrossReferences} cross references, "
            + $"{summary.Components.Count} components plus {summary.LooseMembers} loose members, weave {summary.Weave:0.00}.");
        lines.Add(string.Empty);
        lines.AddRange(MarkdownTable(
            ["Part", "Lines", "Members", "Own state", "Shared state", "Out", "In", "Depends on"],
            summary.Parts.Select(part => new[]
            {
                "`" + part.Path + "`",
                part.Lines.ToString(),
                part.Members.Count.ToString(),
                part.OwnState.ToString(),
                part.SharedState.ToString(),
                part.OutReferences.ToString(),
                part.InReferences.ToString(),
                PartNames(part.DependsOn.OrderBy(other => other.Index)),
            })));

        if (summary.Components.Count > 0)
        {
            lines.Add(string.Empty);
            lines.Add("Components:");
            lines.Add(string.Empty);
            foreach (ComponentSummary component in summary.Components)
            {
                string names = component.MemberCount <= 12
                    ? string.Join(", ", component.MemberNames.Select(name => "`" + name + "`"))
                    : string.Join(", ", component.MemberNames.Take(12).Select(name => "`" + name + "`")) + $", ... {component.MemberCount - 12} more";
                lines.Add($"- {component.MemberCount} members across {component.Parts.Length} parts ({PartNames(component.Parts)}): {names}");
            }
        }

        if (summary.SharedState.Count > 0)
        {
            lines.Add(string.Empty);
            lines.Add("Shared state:");
            lines.Add(string.Empty);
            foreach (StateSummary state in summary.SharedState.OrderByDescending(state => state.Parts.Length))
            {
                string flavour = state.IsStatic ? " (static)" : string.Empty;
                lines.Add($"- `{state.Name}`{flavour}: {PartNames(state.Parts)}");
            }
        }
    }

    lines.Add(string.Empty);
    lines.Add("## Large single-part types");
    lines.Add(string.Empty);
    lines.AddRange(MarkdownTable(
        ["Type", "Lines", "Members", "State", "Comp", "File"],
        large.Select(summary => new[]
        {
            "`" + summary.Name + "`",
            summary.Lines.ToString(),
            summary.MemberCount.ToString(),
            summary.StateCount.ToString(),
            summary.Components.Count.ToString(),
            "`" + summary.Parts[0].Path + "`",
        })));

    string? directory = Path.GetDirectoryName(reportPath);
    if (!string.IsNullOrWhiteSpace(directory))
    {
        Directory.CreateDirectory(directory);
    }

    File.WriteAllText(reportPath, string.Join("\n", lines) + "\n", new UTF8Encoding(false));
}

internal sealed class TypeRecord(INamedTypeSymbol symbol)
{
    public INamedTypeSymbol Symbol { get; } = symbol;
    public List<PartRecord> Parts { get; } = [];
    public Dictionary<ISymbol, MemberRecord> Members { get; } = new(SymbolEqualityComparer.Default);
}

internal sealed class PartRecord(int index, string path, int lines)
{
    public int Index { get; } = index;
    public string Path { get; } = path;
    public int Lines { get; } = lines;
    public List<MemberRecord> Members { get; } = [];
    public int OwnState { get; set; }
    public int SharedState { get; set; }
    public int OutReferences { get; set; }
    public int InReferences { get; set; }
    public HashSet<PartRecord> DependsOn { get; } = [];
}

internal sealed class MemberRecord(int index, ISymbol symbol, PartRecord part, bool isState, bool isStatic)
{
    public int Index { get; } = index;
    public ISymbol Symbol { get; } = symbol;
    public PartRecord Part { get; } = part;
    public bool IsState { get; } = isState;
    public bool IsStatic { get; } = isStatic;
    public HashSet<MemberRecord> Uses { get; } = [];
    public HashSet<MemberRecord> UsedBy { get; } = [];
}

internal sealed record ComponentSummary(int MemberCount, PartRecord[] Parts, string[] MemberNames);

internal sealed record StateSummary(string Name, bool IsStatic, PartRecord[] Parts);

internal sealed record TypeSummary(
    string Name,
    string FullName,
    List<PartRecord> Parts,
    int Lines,
    int MemberCount,
    int InstanceMembers,
    int StateCount,
    List<StateSummary> SharedState,
    List<string> Hubs,
    int CrossReferences,
    List<ComponentSummary> Components,
    int LooseMembers,
    double Weave,
    double Free,
    double Density,
    string Verdict);

internal sealed record Limits(int Parts, int Span, int Hub, double Weave, double Density, int Lines, int Members, int State);
'@

    $projectPath = Join-Path $HelperFolder 'AuditObject.Helper.csproj'
    $programPath = Join-Path $HelperFolder 'Program.cs'
    [System.IO.File]::WriteAllText($projectPath, $projectContent, [System.Text.UTF8Encoding]::new($false))
    [System.IO.File]::WriteAllText($programPath, $programContent, [System.Text.UTF8Encoding]::new($false))
    [System.IO.File]::Copy((Join-Path $PSScriptRoot 'auditbinder.cs'), (Join-Path $HelperFolder 'AuditBinder.cs'), $true)
    return $projectPath
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
    throw "No source files were found under: $projectRoot"
}
Write-AuditLine ("Scanned: {0:N0} source files" -f $sourceFiles.Length) -ForegroundColor DarkGray

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if ($null -eq $dotnet) {
    throw 'The .NET SDK is required, but dotnet was not found on PATH.'
}

$nativePreference = $ErrorActionPreference
$ErrorActionPreference = 'Continue'
$sdkOutput = & $dotnet.Source --version 2>&1
$ErrorActionPreference = $nativePreference
if ($LASTEXITCODE -ne 0) {
    throw "The .NET SDK version could not be read.`n$($sdkOutput -join [Environment]::NewLine)"
}

$sdkVersion = ([string]($sdkOutput | Select-Object -First 1)).Trim()
if ($sdkVersion -notmatch '^(\d+)\.') {
    throw "The .NET SDK version is not recognized: '$sdkVersion'"
}

$targetFramework = "net$($Matches[1]).0"
$temporaryFolder = Join-Path ([System.IO.Path]::GetTempPath()) ($config.project + '-AuditObject-' + [Guid]::NewGuid().ToString('N'))
[System.IO.Directory]::CreateDirectory($temporaryFolder) | Out-Null

try {
    $manifestPath = Join-Path $temporaryFolder 'sources.txt'
    [System.IO.File]::WriteAllLines($manifestPath, $sourceFiles, [System.Text.UTF8Encoding]::new($false))

    $helperFolder = Join-Path $temporaryFolder 'helper'
    [System.IO.Directory]::CreateDirectory($helperFolder) | Out-Null
    $projectPath = Write-AuditHelper -HelperFolder $helperFolder -TargetFramework $targetFramework

    $arguments = @(
        'run',
        '--project', $projectPath,
        '--configuration', 'Release',
        '--no-launch-profile',
        '--',
        $configPath,
        $projectRoot,
        $manifestPath,
        $version,
        $reportPath,
        $Top,
        (Join-Path $PSScriptRoot 'auditbinder.json')
    )

    $previousNoLogo = $env:DOTNET_NOLOGO
    $previousTelemetry = $env:DOTNET_CLI_TELEMETRY_OPTOUT
    $env:DOTNET_NOLOGO = '1'
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'

    try {
        $nativePreference = $ErrorActionPreference
        $ErrorActionPreference = 'Continue'
        $auditOutput = & $dotnet.Source @arguments 2>&1
        $ErrorActionPreference = $nativePreference
        $auditExitCode = $LASTEXITCODE
    }
    finally {
        $env:DOTNET_NOLOGO = $previousNoLogo
        $env:DOTNET_CLI_TELEMETRY_OPTOUT = $previousTelemetry
    }

    if ($auditExitCode -ne 0 -and $auditExitCode -ne 3) {
        throw "The object audit failed.`n$($auditOutput -join [Environment]::NewLine)"
    }

    foreach ($line in $auditOutput) {
        Write-AuditLine ([string]$line)
    }
}
finally {
    if (Test-Path -LiteralPath $temporaryFolder) {
        Remove-Item -LiteralPath $temporaryFolder -Recurse -Force -ErrorAction SilentlyContinue
    }
}

if ($Open -and (Test-Path -LiteralPath $reportPath -PathType Leaf)) {
    Invoke-Item -LiteralPath $reportPath
}

exit ([int]($auditExitCode -ne 0))
