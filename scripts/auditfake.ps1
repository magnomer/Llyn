<#
.SYNOPSIS
Finds fake members: source methods and values that nothing live reads, or that only tests read.

.DESCRIPTION
A member can compile, pass its tests and still do nothing for the application. This audit binds
the sources with Roslyn, binds the tests against them, and records who reads every member.

  - A read is a call, a get, a handler subscription or any use as a value.
    Assigning a field or auto-property, incrementing it or passing it as `out` only writes it.
  - A member is live when a live reader reads it. Constructors, overrides, interface
    implementations, generated markup code, markup words and JSON-serialized properties are
    live by themselves.
  - Every member left over is fake:
      Orphan  nothing live and no test reads it
      Tested  only tests read it

A member read only by other fake members is fake too, so a whole dead chain is reported, not
just its head. The console prints the counters and the first rows of each kind; a Markdown
report with every hit is written to {report.directory}\{prefix}{version}.md.

Binding uses the .NET SDK's own Roslyn, the shared frameworks listed in the configuration and
the package assemblies of the latest build output. Build the solution first so the generated
markup classes and the package assemblies exist.

Everything project-specific lives in auditfake.json next to this script. No project source is
modified. Git and the .NET SDK are required.

.PARAMETER Root
Project root to audit. Defaults to the directory containing this script's parent.

.PARAMETER ReportDirectory
Overrides report.directory for this run. Relative paths resolve against the project root.

.PARAMETER Top
Overrides console.top: how many rows of each kind the console shows.

.PARAMETER Open
Open the report after the audit finishes.

.PARAMETER NoPause
Do not stop at each console page. Paging is also off when output or input is redirected.

.PARAMETER Help
Display this help and exit. The alias -? is supported.

.EXAMPLE
auditfake
Audit the current checkout.

.EXAMPLE
auditfake -ReportDirectory D:\temp\audit -Top 10 -Open
Write the report elsewhere, show ten rows of each kind, open the report.
#>
#requires -Version 5.1
# AUDITFAKE GENERATION 11 - auditfake.ps1.
# A generation is not a revision count. It names functionality, not edits, so editing one of these
# files is never on its own a reason to raise it. Raise it only when the audited outcome changes.
# A generation names the set of checks the audit applies. Two projects on the same generation audit
# the same things and their reports compare directly, whatever else differs between the files. A check
# added, removed, or changed in what it reports is a new generation; wording, plumbing, and refactoring
# leave it alone. Every audit script shares one generation number with the convention-test settings,
# and each refuses a configuration written at another generation.
# Generation 11: the first generation of this audit. It binds the sources and the tests with Roslyn
# and reports every member nothing live reads as Orphan, or only tests read as Tested.
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
    auditfake.ps1

SYNOPSIS
    Find source members that nothing live reads, or that only tests read.

SYNTAX
    auditfake [-Root <path>] [-ReportDirectory <path>] [-Top <n>] [-Open] [-NoPause] [-Help]

OPTIONS
    -Root <path>             Project root. Defaults to the parent of the scripts folder.
    -ReportDirectory <path>  Overrides report.directory for this run.
    -Top <n>                 Overrides console.top.
    -Open                    Open the report when done.
    -NoPause                 No console paging.
    -Help                    Show this help.

KINDS
    Orphan  nothing live and no test reads the member.
    Tested  only tests read the member.
'@ | Write-Host
    exit 0
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$script:AuditGeneration = 11

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

Write-AuditLine "AUDITFAKE GENERATION $script:AuditGeneration" -ForegroundColor Cyan
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
        'sources.roots', 'sources.tests', 'sources.extensions', 'sources.markup',
        'sources.excludeSegments', 'sources.excludeSuffixes', 'sources.excludePrefixes',
        'generated.roots', 'generated.configuration',
        'references.frameworks', 'references.build',
        'implicitUsings',
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
        throw "The fake-audit configuration is generation $($config.generation) but this tooling is generation $script:AuditGeneration.`nA generation names the set of checks applied, so the two must match: $ConfigPath"
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
    param([string]$RelativePath, $Config, [string[]]$Extensions, [switch]$KeepGenerated)

    $segments = $RelativePath -split '[\\/]'
    foreach ($segment in $segments) {
        if ($Config.sources.excludeSegments -contains $segment) {
            return $true
        }
    }

    $fileName = $segments[$segments.Length - 1]
    $extension = [System.IO.Path]::GetExtension($fileName)
    if (-not ($Extensions -contains $extension)) {
        return $true
    }

    if ($KeepGenerated) {
        return $false
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

function Get-TrackedFiles {
    param([string]$ProjectRoot, $Config, [string[]]$Roots, [string[]]$Extensions)

    $git = Get-Command git -ErrorAction SilentlyContinue
    if ($null -eq $git) {
        throw 'Git is required to enumerate project source files, but git was not found on PATH.'
    }

    if (-not (Test-Path -LiteralPath (Join-Path $ProjectRoot '.git'))) {
        throw "The project root is not a Git working tree: $ProjectRoot"
    }

    $lsArguments = @('-c', 'core.quotePath=false', '-C', $ProjectRoot,
        'ls-files', '--cached', '--others', '--exclude-standard', '--') + @($Roots)
    $gitOutput = & $git.Source @lsArguments 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Git could not enumerate source files.`n$($gitOutput -join [Environment]::NewLine)"
    }

    $separator = [string][System.IO.Path]::DirectorySeparatorChar
    $rootPrefix = $ProjectRoot.TrimEnd($script:PathSeparators) + $separator
    $files = New-Object 'System.Collections.Generic.List[string]'

    foreach ($entry in $gitOutput) {
        $relativePath = ([string]$entry).Trim()
        if ([string]::IsNullOrWhiteSpace($relativePath) -or
            (Test-ExcludedRelativePath -RelativePath $relativePath -Config $Config -Extensions $Extensions)) {
            continue
        }

        $fullPath = $rootPrefix + $relativePath.Replace([char]'/', [System.IO.Path]::DirectorySeparatorChar)
        if (Test-Path -LiteralPath $fullPath -PathType Leaf) {
            $files.Add($fullPath)
        }
    }

    return @($files.ToArray() | Sort-Object -Unique)
}

function Get-LatestFolder {
    param([string]$Folder)

    if (-not (Test-Path -LiteralPath $Folder -PathType Container)) {
        return $null
    }

    return Get-ChildItem -LiteralPath $Folder -Directory |
        Sort-Object LastWriteTimeUtc -Descending |
        Select-Object -First 1
}

function Get-GeneratedFiles {
    param([string]$ProjectRoot, $Config)

    $files = New-Object 'System.Collections.Generic.List[string]'
    foreach ($root in $Config.generated.roots) {
        $folder = Join-Path (Join-Path (Join-Path $ProjectRoot $root) 'obj') $Config.generated.configuration
        $target = Get-LatestFolder -Folder $folder
        if ($null -eq $target) {
            continue
        }

        Get-ChildItem -LiteralPath $target.FullName -Filter '*.cs' -File -Recurse |
            Where-Object { $_.Name -notlike '*_wpftmp*' -and $_.Name -notlike '*.g.i.cs' } |
            ForEach-Object { $files.Add($_.FullName) }
    }

    return @($files.ToArray())
}

function Get-BuildReferences {
    param([string]$ProjectRoot, $Config)

    $folder = Join-Path (Join-Path (Join-Path $ProjectRoot $Config.references.build) 'bin') $Config.generated.configuration
    $target = Get-LatestFolder -Folder $folder
    if ($null -eq $target) {
        throw "No build output under $folder; build the solution before the audit."
    }

    $project = [string]$Config.project
    return @(Get-ChildItem -LiteralPath $target.FullName -Filter '*.dll' -File |
        Where-Object {
            $name = [System.IO.Path]::GetFileNameWithoutExtension($_.Name)
            -not ($name -ieq $project -or $name.StartsWith($project + '.', [System.StringComparison]::OrdinalIgnoreCase))
        } |
        ForEach-Object { $_.FullName })
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
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

if (args.Length != 6)
{
    Console.Error.WriteLine("usage: <config> <root> <manifest> <version> <report> <top>");
    return 2;
}

string configPath = args[0];
string projectRoot = args[1];
string manifestPath = args[2];
string version = args[3];
string reportPath = args[4];
int top = int.Parse(args[5]);

JsonElement config = JsonDocument.Parse(File.ReadAllText(configPath)).RootElement;
string[] implicitUsings = config.GetProperty("implicitUsings").EnumerateArray().Select(item => item.GetString()!).ToArray();
string[] frameworks = config.GetProperty("references").GetProperty("frameworks").EnumerateArray().Select(item => item.GetString()!).ToArray();

Dictionary<char, List<string>> manifest = File.ReadAllLines(manifestPath)
    .Where(line => line.Length > 2)
    .GroupBy(line => line[0])
    .ToDictionary(group => group.Key, group => group.Select(line => line[2..]).ToList());
List<string> Take(char tag) => manifest.TryGetValue(tag, out List<string>? list) ? list : [];

CSharpParseOptions parseOptions = new(LanguageVersion.Preview, DocumentationMode.None, SourceCodeKind.Regular);
SyntaxTree Parse(string path) => CSharpSyntaxTree.ParseText(File.ReadAllText(path), parseOptions, path);

// The shared frameworks sit beside the runtime this helper runs on, under the same version folder.
Dictionary<string, string> chosen = new(StringComparer.OrdinalIgnoreCase);
string runtime = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
string shared = Path.GetDirectoryName(Path.GetDirectoryName(runtime)!)!;
foreach (string pack in frameworks)
{
    string folder = Path.Combine(shared, pack, Path.GetFileName(runtime));
    if (!Directory.Exists(folder))
    {
        Console.Error.WriteLine($"The shared framework '{pack}' is not installed at {folder}; its members stay unbound.");
        continue;
    }

    foreach (string path in Directory.EnumerateFiles(folder, "*.dll"))
    {
        chosen.TryAdd(Path.GetFileNameWithoutExtension(path), path);
    }
}

foreach (string path in Take('R'))
{
    chosen[Path.GetFileNameWithoutExtension(path)] = path;
}

List<MetadataReference> references = [];
foreach (string path in chosen.Values)
{
    try
    {
        references.Add(MetadataReference.CreateFromFile(path));
    }
    catch (Exception exception) when (exception is IOException or BadImageFormatException)
    {
    }
}

List<SyntaxTree> tracked = Take('S').AsParallel().AsOrdered().Select(Parse).ToList();
List<SyntaxTree> generated = Take('G').AsParallel().AsOrdered().Select(Parse).ToList();
CSharpCompilation source = CSharpCompilation.Create(
    "AuditFake",
    tracked.Concat(generated),
    references,
    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true, nullableContextOptions: NullableContextOptions.Enable));

List<SyntaxTree> testTrees = Take('T').AsParallel().AsOrdered().Select(Parse).ToList();
testTrees.Add(CSharpSyntaxTree.ParseText(string.Concat(implicitUsings.Select(space => $"global using {space};\n")), parseOptions));
CSharpCompilation tests = CSharpCompilation.Create(
    "AuditFakeTests",
    testTrees,
    references.Append(source.ToMetadataReference()),
    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true, nullableContextOptions: NullableContextOptions.Enable));

Regex word = new("[A-Za-z_][A-Za-z0-9_]*", RegexOptions.Compiled);
HashSet<string> markup = Take('M')
    .SelectMany(path => word.Matches(File.ReadAllText(path)).Select(match => match.Value))
    .ToHashSet(StringComparer.Ordinal);

Dictionary<string, Member> members = new(StringComparer.Ordinal);
foreach (SyntaxTree tree in tracked)
{
    Fake.ScanMembers(source.GetSemanticModel(tree, true), projectRoot, members);
}

HashSet<string> names = new(members.Values.Select(member => member.Name), StringComparer.Ordinal);
HashSet<string> serialized = new(StringComparer.Ordinal);
foreach (SyntaxTree tree in source.SyntaxTrees)
{
    SemanticModel model = source.GetSemanticModel(tree, true);
    Fake.ScanUses(model, members, names, false);
    Fake.ScanPersisted(model, serialized);
}

foreach (SyntaxTree tree in tests.SyntaxTrees)
{
    Fake.ScanUses(tests.GetSemanticModel(tree, true), members, names, true);
}

Fake.MarkLive(members, markup, serialized);
List<Hit> hits = members.Values
    .Where(member => !member.Live)
    .Select(member => Fake.CreateHit(member, members))
    .OrderBy(hit => hit.Path, StringComparer.Ordinal)
    .ThenBy(hit => hit.Line)
    .ToList();

string[] kinds = ["Orphan", "Tested"];
foreach (string kind in kinds)
{
    List<Hit> ofKind = hits.Where(hit => hit.Kind == kind).ToList();
    Console.WriteLine();
    Console.WriteLine($"{kind}: {ofKind.Count}");
    foreach (Hit hit in ofKind.Take(top))
    {
        Console.WriteLine($"  {hit.Path}:{hit.Line} {hit.Name}: {hit.Reason}");
    }

    if (ofKind.Count > top)
    {
        Console.WriteLine($"  ... {ofKind.Count - top} more in the report.");
    }
}

Console.WriteLine();
Console.WriteLine("Counters");
foreach (string kind in kinds)
{
    Console.WriteLine($"  {kind,-7} {hits.Count(hit => hit.Kind == kind)}");
}

Console.WriteLine();
Console.WriteLine($"Report: {reportPath}");

List<string> lines =
[
    $"# AuditFake {version}",
    string.Empty,
    $"Generated {DateTime.Now:yyyy-MM-dd HH:mm}. Members judged: {members.Count}.",
    string.Empty,
    "A member is live when a live reader, a constructor, an override, generated code, markup or the serializer reads it.",
    "Orphan is read by nothing live. Tested is read only by tests.",
    string.Empty,
    "## Counters",
    string.Empty,
];
lines.AddRange(kinds.Select(kind => $"- {kind}: {hits.Count(hit => hit.Kind == kind)}"));
foreach (string kind in kinds)
{
    lines.Add(string.Empty);
    lines.Add($"## {kind}");
    lines.Add(string.Empty);
    lines.AddRange(hits.Where(hit => hit.Kind == kind).Select(hit => $"- `{hit.Path}:{hit.Line}` `{hit.Name}`: {hit.Reason}"));
}

string? reportFolder = Path.GetDirectoryName(reportPath);
if (!string.IsNullOrWhiteSpace(reportFolder))
{
    Directory.CreateDirectory(reportFolder);
}

File.WriteAllText(reportPath, string.Join("\n", lines) + "\n", new UTF8Encoding(false));
return 0;

internal static class Fake
{
    private const string RootReader = "";

    public static void ScanMembers(SemanticModel model, string projectRoot, Dictionary<string, Member> members)
    {
        string path = Path.GetRelativePath(projectRoot, model.SyntaxTree.FilePath).Replace('\\', '/');
        foreach (TypeDeclarationSyntax declaration in model.SyntaxTree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>())
        {
            if (model.GetDeclaredSymbol(declaration) is not INamedTypeSymbol type || type.TypeKind == TypeKind.Enum)
            {
                continue;
            }

            if (declaration is RecordDeclarationSyntax { ParameterList: { } parameters })
            {
                foreach (ParameterSyntax parameter in parameters.Parameters)
                {
                    if (type.GetMembers(parameter.Identifier.ValueText).OfType<IPropertySymbol>().FirstOrDefault() is IPropertySymbol property)
                    {
                        Add(members, property, parameter, path, true);
                    }
                }
            }

            foreach (MemberDeclarationSyntax member in declaration.Members)
            {
                foreach (ISymbol symbol in Declared(model, member))
                {
                    if (IsCandidate(symbol))
                    {
                        Add(members, symbol, member, path, IsStored(member));
                    }
                }
            }
        }
    }

    public static void ScanUses(SemanticModel model, Dictionary<string, Member> members, HashSet<string> names, bool test)
    {
        SyntaxNode root = model.SyntaxTree.GetRoot();
        foreach (SimpleNameSyntax name in root.DescendantNodes().OfType<SimpleNameSyntax>())
        {
            if (!names.Contains(name.Identifier.ValueText))
            {
                continue;
            }

            SymbolInfo info = model.GetSymbolInfo(name);
            IEnumerable<ISymbol> bound = info.Symbol is ISymbol symbol ? [symbol] : info.CandidateSymbols;
            foreach (ISymbol target in bound)
            {
                AddUse(model, name, target, members, test);
            }
        }

        foreach (CommonForEachStatementSyntax loop in root.DescendantNodes().OfType<CommonForEachStatementSyntax>())
        {
            ForEachStatementInfo info = model.GetForEachStatementInfo(loop);
            ISymbol?[] targets = [info.GetEnumeratorMethod, info.MoveNextMethod, info.CurrentProperty];
            foreach (ISymbol target in targets.OfType<ISymbol>())
            {
                AddUse(model, loop, target, members, test);
            }
        }
    }

    public static void ScanPersisted(SemanticModel model, HashSet<string> serialized)
    {
        foreach (InvocationExpressionSyntax call in model.SyntaxTree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (model.GetSymbolInfo(call).Symbol is not IMethodSymbol method
                || method.ContainingType?.ToDisplayString() != "System.Text.Json.JsonSerializer")
            {
                continue;
            }

            IEnumerable<ITypeSymbol?> types = method.TypeArguments
                .Concat(call.ArgumentList.Arguments.Select(argument => model.GetTypeInfo(argument.Expression).Type));
            foreach (ITypeSymbol? type in types)
            {
                AddPersisted(type, serialized, new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default));
            }
        }
    }

    public static void MarkLive(Dictionary<string, Member> members, HashSet<string> markup, HashSet<string> serialized)
    {
        Dictionary<string, List<Member>> dependents = new(StringComparer.Ordinal);
        Queue<Member> queue = new();
        foreach (Member member in members.Values)
        {
            foreach (string reader in member.Readers.Where(members.ContainsKey))
            {
                if (!dependents.TryGetValue(reader, out List<Member>? list))
                {
                    list = [];
                    dependents.Add(reader, list);
                }

                list.Add(member);
            }

            if (markup.Contains(member.Name)
                || serialized.Contains(member.Key)
                || member.Readers.Any(reader => !members.ContainsKey(reader)))
            {
                member.Live = true;
                queue.Enqueue(member);
            }
        }

        while (queue.TryDequeue(out Member? live))
        {
            foreach (Member member in dependents.GetValueOrDefault(live.Key) ?? [])
            {
                if (!member.Live)
                {
                    member.Live = true;
                    queue.Enqueue(member);
                }
            }
        }
    }

    public static Hit CreateHit(Member member, Dictionary<string, Member> members)
    {
        List<string> readers = member.Readers.Select(reader => members[reader].Name).Order(StringComparer.Ordinal).ToList();
        List<string> testers = member.Testers.Order(StringComparer.Ordinal).ToList();
        List<string> reasons = [];
        if (testers.Count > 0)
        {
            reasons.Add("by tests " + FormatList(testers));
        }

        if (readers.Count > 0)
        {
            reasons.Add("by fake members " + FormatList(readers));
        }

        string reason = reasons.Count == 0 ? "read by nothing" : "read only " + string.Join(" and ", reasons);
        return new Hit(member.Path, member.Line, member.Name, testers.Count > 0 ? "Tested" : "Orphan", reason);
    }

    private static void Add(Dictionary<string, Member> members, ISymbol symbol, SyntaxNode node, string path, bool stored)
    {
        string key = Key(symbol);
        if (!members.ContainsKey(key))
        {
            members.Add(key, new Member(key, symbol.Name, path, node.GetLocation().GetLineSpan().StartLinePosition.Line + 1, stored));
        }
    }

    private static IEnumerable<ISymbol> Declared(SemanticModel model, MemberDeclarationSyntax member) => member switch
    {
        BaseTypeDeclarationSyntax or DelegateDeclarationSyntax => [],
        BaseFieldDeclarationSyntax field => field.Declaration.Variables.Select(variable => model.GetDeclaredSymbol(variable)).OfType<ISymbol>(),
        _ => model.GetDeclaredSymbol(member) is ISymbol symbol ? [symbol] : [],
    };

    private static bool IsStored(MemberDeclarationSyntax member) => member switch
    {
        BaseFieldDeclarationSyntax => true,
        PropertyDeclarationSyntax { ExpressionBody: null, AccessorList: { } accessors } =>
            accessors.Accessors.All(accessor => accessor.Body is null && accessor.ExpressionBody is null),
        _ => false,
    };

    private static bool IsCandidate(ISymbol symbol)
    {
        if (symbol.IsImplicitlyDeclared || symbol.IsOverride || symbol.IsExtern)
        {
            return false;
        }

        bool kind = symbol switch
        {
            IMethodSymbol method => method.MethodKind == MethodKind.Ordinary && !(method.IsStatic && method.Name == "Main"),
            IPropertySymbol property => !property.IsIndexer && property.ExplicitInterfaceImplementations.Length == 0,
            IEventSymbol eventSymbol => eventSymbol.ExplicitInterfaceImplementations.Length == 0,
            IFieldSymbol field => field.AssociatedSymbol is null,
            _ => false,
        };
        return kind && Contracts(symbol).Count == 0;
    }

    private static List<ISymbol> Contracts(ISymbol symbol)
    {
        if (symbol.ContainingType is not INamedTypeSymbol type || symbol is IFieldSymbol)
        {
            return [];
        }

        return type.AllInterfaces
            .SelectMany(face => face.GetMembers())
            .Where(member => SymbolEqualityComparer.Default.Equals(
                type.FindImplementationForInterfaceMember(member)?.OriginalDefinition, symbol.OriginalDefinition))
            .ToList();
    }

    private static void AddUse(SemanticModel model, SyntaxNode site, ISymbol target, Dictionary<string, Member> members, bool test)
    {
        ISymbol normal = Normal(target);
        List<Member> reached = new[] { normal }
            .Concat(Contracts(normal))
            .Select(symbol => members.GetValueOrDefault(Key(symbol)))
            .OfType<Member>()
            .ToList();
        if (reached.Count == 0)
        {
            return;
        }

        string? owner = Owner(model, site);
        foreach (Member member in reached)
        {
            if (owner == member.Key || !IsRead(site, normal, member))
            {
                continue;
            }

            if (test)
            {
                member.Testers.Add(Label(model, site));
            }
            else
            {
                member.Readers.Add(owner ?? RootReader);
            }
        }
    }

    // A stored slot that is only assigned, incremented or passed out is written, not read.
    // A field-like event is read only when a handler is added or removed.
    private static bool IsRead(SyntaxNode site, ISymbol target, Member member)
    {
        if (site is not ExpressionSyntax expression || target is IMethodSymbol)
        {
            return true;
        }

        while (expression.Parent is MemberAccessExpressionSyntax access && access.Name == expression)
        {
            expression = access;
        }

        if (expression.Parent is MemberBindingExpressionSyntax binding && binding.Name == expression)
        {
            expression = binding;
        }

        SyntaxNode? parent = expression.Parent;
        if (target is IEventSymbol)
        {
            return !member.Stored
                || parent is AssignmentExpressionSyntax
                {
                    RawKind: (int)SyntaxKind.AddAssignmentExpression or (int)SyntaxKind.SubtractAssignmentExpression,
                } subscription && subscription.Left == expression;
        }

        if (!member.Stored)
        {
            return true;
        }

        return parent switch
        {
            AssignmentExpressionSyntax assignment when assignment.Left == expression =>
                assignment.IsKind(SyntaxKind.CoalesceAssignmentExpression),
            PostfixUnaryExpressionSyntax or PrefixUnaryExpressionSyntax => parent.Kind() is not (
                SyntaxKind.PostIncrementExpression or SyntaxKind.PostDecrementExpression
                or SyntaxKind.PreIncrementExpression or SyntaxKind.PreDecrementExpression),
            ArgumentSyntax argument when argument.RefKindKeyword.IsKind(SyntaxKind.OutKeyword) => false,
            ArgumentSyntax { Parent: TupleExpressionSyntax tuple } =>
                !(tuple.Parent is AssignmentExpressionSyntax deconstruction && deconstruction.Left == tuple),
            _ => true,
        };
    }

    private static string? Owner(SemanticModel model, SyntaxNode site)
    {
        foreach (SyntaxNode ancestor in site.Ancestors())
        {
            switch (ancestor)
            {
                case BaseMethodDeclarationSyntax or BasePropertyDeclarationSyntax:
                case VariableDeclaratorSyntax { Parent.Parent: BaseFieldDeclarationSyntax }:
                    return model.GetDeclaredSymbol(ancestor) is ISymbol owner ? Key(owner) : null;
                case BaseTypeDeclarationSyntax:
                    return null;
            }
        }

        return null;
    }

    private static string Label(SemanticModel model, SyntaxNode site)
    {
        foreach (SyntaxNode ancestor in site.Ancestors())
        {
            if (ancestor is BaseMethodDeclarationSyntax or BasePropertyDeclarationSyntax
                && model.GetDeclaredSymbol(ancestor) is ISymbol owner)
            {
                return $"{owner.ContainingType?.Name}.{owner.Name}";
            }
        }

        return Path.GetFileName(site.SyntaxTree.FilePath);
    }

    private static void AddPersisted(ITypeSymbol? type, HashSet<string> serialized, HashSet<ITypeSymbol> seen)
    {
        if (type is null || !seen.Add(type))
        {
            return;
        }

        if (type is IArrayTypeSymbol array)
        {
            AddPersisted(array.ElementType, serialized, seen);
            return;
        }

        if (type is not INamedTypeSymbol named)
        {
            return;
        }

        foreach (ITypeSymbol argument in named.TypeArguments)
        {
            AddPersisted(argument, serialized, seen);
        }

        if (named.Locations.All(location => !location.IsInSource))
        {
            return;
        }

        foreach (IPropertySymbol property in named.GetMembers().OfType<IPropertySymbol>())
        {
            serialized.Add(Key(property));
            AddPersisted(property.Type, serialized, seen);
        }
    }

    private static string FormatList(List<string> names)
    {
        string shown = string.Join(", ", names.Take(3));
        return names.Count > 3 ? $"{shown} and {names.Count - 3} more" : shown;
    }

    private static ISymbol Normal(ISymbol symbol)
    {
        if (symbol is IMethodSymbol method)
        {
            method = method.ReducedFrom ?? method;
            method = method.PartialDefinitionPart ?? method;
            return method.AssociatedSymbol is ISymbol associated ? associated.OriginalDefinition : method.OriginalDefinition;
        }

        return symbol.OriginalDefinition;
    }

    // The documentation id is the same in the source and the test compilation.
    private static string Key(ISymbol symbol)
    {
        ISymbol normal = Normal(symbol);
        return normal.GetDocumentationCommentId() ?? normal.ToDisplayString();
    }
}

internal sealed class Member(string key, string name, string path, int line, bool stored)
{
    public string Key { get; } = key;
    public string Name { get; } = name;
    public string Path { get; } = path;
    public int Line { get; } = line;
    public bool Stored { get; } = stored;
    public HashSet<string> Readers { get; } = new(StringComparer.Ordinal);
    public HashSet<string> Testers { get; } = new(StringComparer.Ordinal);
    public bool Live { get; set; }
}

internal sealed record Hit(string Path, int Line, string Name, string Kind, string Reason);
'@

    $projectPath = Join-Path $HelperFolder 'AuditFake.Helper.csproj'
    $programPath = Join-Path $HelperFolder 'Program.cs'
    [System.IO.File]::WriteAllText($projectPath, $projectContent, [System.Text.UTF8Encoding]::new($false))
    [System.IO.File]::WriteAllText($programPath, $programContent, [System.Text.UTF8Encoding]::new($false))
    return $projectPath
}

$projectRoot = Resolve-ProjectRoot -Path $Root
$configPath = Join-Path $PSScriptRoot 'auditfake.json'
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

[string[]]$sourceFiles = @(Get-TrackedFiles -ProjectRoot $projectRoot -Config $config -Roots $config.sources.roots -Extensions $config.sources.extensions)
[string[]]$testFiles = @(Get-TrackedFiles -ProjectRoot $projectRoot -Config $config -Roots $config.sources.tests -Extensions $config.sources.extensions)
[string[]]$markupFiles = @(Get-TrackedFiles -ProjectRoot $projectRoot -Config $config -Roots $config.sources.roots -Extensions $config.sources.markup)
[string[]]$generatedFiles = @(Get-GeneratedFiles -ProjectRoot $projectRoot -Config $config)
[string[]]$referenceFiles = @(Get-BuildReferences -ProjectRoot $projectRoot -Config $config)
if ($sourceFiles.Length -eq 0 -or $testFiles.Length -eq 0) {
    throw "No source or test files were found under: $projectRoot"
}
Write-AuditLine ("Source files: $($sourceFiles.Length), tests: $($testFiles.Length), markup: $($markupFiles.Length), " +
    "generated: $($generatedFiles.Length), references: $($referenceFiles.Length)") -ForegroundColor DarkGray

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if ($null -eq $dotnet) {
    throw 'The .NET SDK is required, but dotnet was not found on PATH.'
}

$sdkOutput = & $dotnet.Source --version 2>&1
if ($LASTEXITCODE -ne 0) {
    throw "The .NET SDK version could not be read.`n$($sdkOutput -join [Environment]::NewLine)"
}

$sdkVersion = ([string]($sdkOutput | Select-Object -First 1)).Trim()
if ($sdkVersion -notmatch '^(\d+)\.') {
    throw "The .NET SDK version is not recognized: '$sdkVersion'"
}

$targetFramework = "net$($Matches[1]).0"
$temporaryFolder = Join-Path ([System.IO.Path]::GetTempPath()) ($config.project + '-AuditFake-' + [Guid]::NewGuid().ToString('N'))
[System.IO.Directory]::CreateDirectory($temporaryFolder) | Out-Null

try {
    $manifestPath = Join-Path $temporaryFolder 'sources.txt'
    $manifest = @($sourceFiles | ForEach-Object { "S|$_" }) +
        @($testFiles | ForEach-Object { "T|$_" }) +
        @($markupFiles | ForEach-Object { "M|$_" }) +
        @($generatedFiles | ForEach-Object { "G|$_" }) +
        @($referenceFiles | ForEach-Object { "R|$_" })
    [System.IO.File]::WriteAllLines($manifestPath, [string[]]$manifest, [System.Text.UTF8Encoding]::new($false))

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
        $Top
    )

    $previousNoLogo = $env:DOTNET_NOLOGO
    $previousTelemetry = $env:DOTNET_CLI_TELEMETRY_OPTOUT
    $env:DOTNET_NOLOGO = '1'
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'

    try {
        $auditOutput = & $dotnet.Source @arguments 2>&1
        $auditExitCode = $LASTEXITCODE
    }
    finally {
        $env:DOTNET_NOLOGO = $previousNoLogo
        $env:DOTNET_CLI_TELEMETRY_OPTOUT = $previousTelemetry
    }

    if ($auditExitCode -ne 0) {
        throw "The fake audit failed.`n$($auditOutput -join [Environment]::NewLine)"
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
