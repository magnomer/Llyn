# TAuditPlatform.cs

## `public sealed class TAuditPlatform`

Holds every project under `src` to the platform table.
A layer splits into a portable half and a Windows twin, and this fact keeps that split.
The layer chain itself is the ring and chain facts' concern.
`scripts/auditplatform.ps1` reports the same kinds from its own configuration.

## `private const string TAuditProjectExtension = ".csproj";`

The extension that marks a project file among the enumerated sources.

## `private static readonly string[] TAuditImportNames`

The files MSBuild imports on its own from every folder above a project.

## `private static readonly string[] TAuditConfigNames`

The analyzer configuration files the compiler reads from every folder above a project.

## `private static readonly Lazy<IReadOnlyList<TAuditHit>> TAuditPlatformHits = new(TAuditPlatformRead);`

The walk runs once and both facts read the same hits.

## `public TAuditPlatform(ITestOutputHelper output)`

Keeps the runner's output so a passing fact can still print its counts.

## `public void AuditPlatform_Projects_HoldWithinCeiling()`

Every kind counts no more hits than its ceiling.
An unwritten ceiling is zero, and the hits of an over kind are listed under it.

## `public void AuditPlatform_Ceiling_MatchesHits()`

Every ceiling equals its count, so a ceiling left above the count is stale.

## `private static IReadOnlyList<TAuditHit> TAuditPlatformRead()`

Reads every project file and source under `src` and classifies each break of the split.
A hit carries the project as its ring and the kind's subject as its target.
An unmapped project is reported once and never checked further.
The host may reference every project, so its edges are never read.

## `private static IEnumerable<TAuditHit> TAuditSourceScan(string repoRoot, string source, string project)`

Every line of one portable source that names a Windows API.
A line is reported once even when several patterns match it.

## `private static bool TAuditWindowsCheck(string project, IReadOnlyDictionary<string, string> projects)`

True for a twin in the table, or for any project whose file targets a Windows framework.

## `private static bool TAuditAnalyzerCheck(string repoRoot, string project)`

True when the platform rule is an error for one project.
The nearest analyzer configuration that sets the rule's severity wins.
Otherwise the rule is an error when a project file lists it as one.
It is also an error when every warning is, unless the rule is spared.

## `private static bool TAuditListCheck(XDocument document, string element, string rule)`

True when one property of a project file lists the rule among its codes.

## `private static IEnumerable<string> TAuditChainRead(string repoRoot, string project, string[] names)`

The named files from the project's folder up to the repository root, nearest first.

## `private static string[] TAuditFrameworkRead(XDocument document)`

Every framework one project file targets, whether written singly or as a list.

## `private static IEnumerable<string> TAuditValueRead(XDocument document, string element)`

The trimmed text of every element with that name, in any namespace.

## `private static IEnumerable<string> TAuditIncludeRead(XDocument document, string element)`

The `Include` value of every element with that name.

## `private static string TAuditRelativeRead(string repoRoot, string path)`

A path relative to the repository root with forward slashes.
