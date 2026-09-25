# TAuditStrictSetting.cs

## `internal static class TAuditStrictSetting`

Hand-written and tracked: the veneer-audit switch, the report path and the classifiers live here.
No script writes this file.

## `public const bool TAuditStrictEnforced = true;`

False makes every strict fact a warning that passes.
True fails a fact when a kind counts above its ceiling.

## `public const string TAuditStrictReport = "temp/audit/Strict-{0}.md";`

Where the report lands, with the version in the name.
`temp` is ignored by Git, so a run never dirties the tree.

## `public static readonly IReadOnlyDictionary<string, int> TAuditStrictCeiling`

The hit count each kind may reach.
A count above fails the fact, a ceiling above the count is stale and fails too.
Lower a ceiling when the veneer sheds a hit, never raise one to admit a new one.

## `public static readonly string[] TAuditReachInclude`

The `git ls-files` patterns of the veneer markup.

## `public static readonly string[] TAuditVeneerInclude`

The `git ls-files` patterns of the veneer sources.
A type declared under one of these is a veneer type, whatever its base.

## `public static readonly string[] TAuditDeportmentInclude`

The `git ls-files` patterns of the deportment sources, scanned for a reach into the file system.

## `public const string TAuditDeportmentNamespace = "Llyn.UIDeportment";`

The namespace whose types and members a markup binding may name, since the deportment is the veneer's path.

## `public static readonly string[] TAuditCatalogPatterns`

A line matching one of these does file, JSON, regex or process work, or starts a task, in the veneer.
That work belongs below the shell, in the engine or in `LUsher`.

## `public static readonly string[] TAuditCatalogExempt`

The veneer files whose stream use is the framework's own: a resource stream, a bitmap decode, a browser page.

## `public static readonly string[] TAuditMarkupPatterns`

A deportment line matching one of these names the file system.

## `public static readonly string[] TAuditMarkupExempt`

The deportment files exempt from the file-system scan by name.
Empty: no deportment file names the file system.

## `public static readonly string[] TAuditReachNamespaces`

The engine namespaces a markup file may not map, since mapping one lets a binding reach the engine.

## `public static readonly string[] TAuditTriggerElements`

The markup elements that branch on a condition.

## `public static readonly string[] TAuditTriggerSlots`

The binding slots that convert, format or fall back, each a computation in the markup.
