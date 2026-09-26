# TAuditStrictSetting.cs

## `internal static class TAuditStrictSetting`

Hand-written and tracked: the surface-audit switch, the report path and the classifiers live here.
The per-file ceilings live in the ledger named by `TAuditLedgerFile`.
No script writes this file.

## `public const bool TAuditStrictEnforced = true;`

False makes every strict fact a warning that passes.
True fails a fact on any file whose count of a kind stands above its ledger ceiling.

## `public const string TAuditStrictReport = "temp/audit/Strict-{0}.md";`

Where the report lands, with the version in the name.
`temp` is ignored by Git, so a run never dirties the tree.

## `public const string TAuditLedgerFile = "TAuditStrictLedger";`

The ledger holding the ceiling of every kind in every surface, driver and host file.

## `public static readonly string[] TAuditReachInclude`

The `git ls-files` patterns of the surface markup.

## `public static readonly string[] TAuditVeneerInclude`

The `git ls-files` patterns of the surface sources, the Veneer and UITerminal both.
A type declared under one of these is a surface type, whatever its base.

## `public static readonly string[] TAuditDeportmentInclude`

The `git ls-files` patterns of the driver sources, scanned for a reach into the file system.

## `public static readonly string[] TAuditHostInclude`

The `git ls-files` patterns of the host sources, held to construction and wiring.

## `public const string TAuditDeportmentNamespace = "Llyn.UIDeportment";`

The namespace whose types and members a markup binding may name, since the driver is the surface's path.
A name a type below the cut also declares is left out.
So a deeper binding cannot pass by coincidence.

## `public static readonly string[] TAuditQueryTypes`

The types whose extension methods are queries, each a decision a surface may not make.

## `public static readonly string[] TAuditCatalogPatterns`

A line matching one of these does file, JSON, regex or process work, or starts a task, in the surface.
That work belongs below the UI, in the engine or in `LUsher`.

## `public static readonly string[] TAuditCatalogExempt`

The surface files whose stream use is the framework's own: a resource stream, a bitmap decode, a browser page.

## `public static readonly string[] TAuditDiskPatterns`

A driver line matching one of these names a file system type.
The patterns name the types, so the console's `TextReader` and `TextWriter` pass.

## `public static readonly string[] TAuditDiskExempt`

The driver files exempt from the disk scan by name.
Empty: no driver file names the file system.

## `public static readonly string[] TAuditReachNamespaces`

The namespaces below the driver that a markup file may not map, since a mapping lets a binding reach them.

## `public static readonly string[] TAuditTriggerElements`

The markup elements that branch on a condition or switch between visual states.

## `public static readonly string[] TAuditTriggerSlots`

The binding slots that convert, format, fall back, select a template or validate.
Each is a computation in the markup, whether set as an attribute, inside an extension or as a property element.
