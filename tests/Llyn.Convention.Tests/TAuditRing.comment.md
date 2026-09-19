# TAuditRing.cs

## `public sealed class TAuditRing`

Keeps every project on its ring.
A ring names only the rings inside it, never one outside.
The project edges, the source namespaces and the engine's reach into the veneer are each held.

## `private const string TAuditRingSource = "src/";`

The folder every ring lives under.

## `private static readonly Regex TAuditRingReference`

A `ProjectReference` as it stands in a `.csproj`, with the referenced project name captured.

## `private static readonly string[] TAuditRingField`

A veneer field holding the engine.
Plans 09 and 10 leave one in `PWindow.xaml.cs` and one in `App.xaml.cs`.

## `private static readonly string[] TAuditRingHelper`

The tenure, vista and foray type names, wherever the veneer spells one.

## `private static readonly string[] TAuditRingAdapter`

An archive, loader or database session constructed by the engine itself.
Plans 06 to 08 move each construction into the infrastructure.

## `private static readonly string[] TAuditRingDatabase`

A member read off the engine's database field, which only the constructor and the workspace open may touch.

## `public void AuditRing_Projects_ReferenceInward()`

Reads every `.csproj` under `src` and holds its edges to the ring table exactly.
An edge added or dropped without an edit to the table fails.

## `public void AuditRing_Sources_UseOnlyInnerRings()`

Scans every source of every project for a namespace its ring may not name.
A hit that is neither exempt, waived nor a reader-only `System.IO` fails.

## `public void AuditRing_Waiver_MatchesSource()`

Every waiver row still matches a break, so a fixed break deletes its row.

## `public void AuditRing_Veneer_HoldsEngineWithinCeiling()`

The engine fields and the engine helpers in the veneer each stay within their ceiling.

## `public void AuditRing_Engine_ConstructsNoAdapter()`

The adapters the engine constructs stay within their ceiling.

## `public void AuditRing_Engine_ReadsNoDatabase()`

No engine part outside `LEngine.cs` reads the database field, so every row reaches the engine through a vault.

## `public void AuditRing_Ceiling_MatchesHits()`

No ceiling sits above its count, so a shed hit lowers its ceiling.

## `private static void TAuditRingCheck(string kind, string role, string[] patterns, string summary)`

Counts the patterns over one project and fails when the count is above the ceiling of the kind.

## `private static List<TViolation> TAuditRingFind()`

Every forbidden namespace named in every project, with its waiver row as the reason or an empty reason.
An exempt row and a reader-only `System.IO` are dropped before the waivers are read.

## `private static bool TAuditWaiverMatch(string waiver, string path, string name)`

True when the row names this namespace and its path is the file or a `/*` prefix of it.

## `private static bool TAuditStreamCheck(string path, Dictionary<string, bool> streamed)`

True when the file names a file system type, read once per file.

## `private static Func<string, bool> TAuditRoleSelect(string role)`

A chooser admitting the sources under one project folder.

## `private static List<TViolation> TAuditRingScan(Func<string, bool> chosen, IReadOnlyList<string> patterns)`

The one walk the facts share, over the sources under `src` the chooser admits.
Each match is one hit naming the file, the line, the text matched and the pattern.
An empty enumeration fails rather than passing vacuously.

## `private static string[] TAuditEdgeRead(string csproj)`

The project names one `.csproj` references, sorted.
