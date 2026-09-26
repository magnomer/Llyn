# TAuditParity.cs

## `public sealed class TAuditParity`

Holds every convention-test setting to the value its audit script reads from `scripts`.
A script and its test run two engines, so the same values make them report the same findings.
A value changed on one side only fails here and names the key on both sides.

## `private const string TAuditParityName = "AUDITPARITY";`

The audit name every failure of this class is reported under.

## `private const string TAuditScriptFolder = "scripts";`

The folder that holds the script configurations, relative to the repository root.

## `private const string TAuditScriptPattern = "audit*.json";`

The audit configurations, which share the generation of the convention tests.
The other script configurations count generations of their own tools.

## `public void AuditParity_Scripts_ShareGeneration()`

Every audit configuration names the generation the convention tests run at.

## `public void AuditParity_Binder_MatchesScript()`

The shared binder of the scripts compiles what `TAuditBinder` compiles.
Its packs, configuration, host root and exclusions match the test settings.

## `public void AuditParity_Lines_MatchScript()`

The line limits, roots, extensions, excluded segments and width limits match scripts/auditlines.json.

## `public void AuditParity_Comments_MatchScript()`

The comment roots, files, rules, markers, closers and exemptions match scripts/auditcomments.json.

## `public void AuditParity_Platform_MatchesScript()`

The platform table, frameworks, analyzer, domain exemptions and Windows markers match scripts/auditplatform.json.
A portable row is its own column, a twin row takes its half, and the host row has none.

## `public void AuditParity_Structure_MatchesScript()`

The rings, their reach, frames and cut, the ambient members and the chain rules match scripts/auditstructure.json.
A pure ring's script frame is the shared test frame plus the ring's extra frame.

## `public void AuditParity_Object_MatchesScript()`

The object thresholds, kind ceilings and part ceilings match scripts/auditobject.json.
A kind ceiling is matched without regard to case.

## `public void AuditParity_Fake_MatchesScript()`

The implicit usings and the test roots the fake audit binds match scripts/auditfake.json.

## `private static JsonElement TAuditScriptRead(string name)`

The root of one script configuration, detached from its document.

## `private static string[] TAuditListRead(JsonElement parent, string key)`

One string array of a configuration.

## `private static Dictionary<string, string[]> TAuditTableRead(JsonElement map)`

One configuration object as a map of names to string lists.
A scalar value becomes a list of its text, so numbers and strings compare alike.

## `private static void TAuditValueMatch<TAuditValue>(List<string> drift, string key, TAuditValue script, TAuditValue test)`

Records a drift when one value differs between the script and the test.

## `private static void TAuditListMatch(List<string> drift, string key, IEnumerable<string> script, IEnumerable<string> test)`

Records a drift when two lists differ once sorted, since order carries no meaning in these settings.

## `private static void TAuditMapMatch(List<string> drift, string key, IReadOnlyDictionary<string, string[]> script, IReadOnlyDictionary<string, string[]> test)`

Records a drift per name whose list differs, a name missing on one side included.

## `private static void TAuditDriftCheck(List<string> drift, string summary)`

Fails with every recorded drift.
