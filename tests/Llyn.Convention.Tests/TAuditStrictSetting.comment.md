# TAuditStrictSetting.cs

## `internal static class TAuditStrictSetting`

Hand-written and tracked: the strict-audit switch, the report path and the classifiers live here.
No script writes this file.

## `public const bool TAuditStrictEnforced = false;`

False makes every strict fact a warning that passes.
Flip it once the shell is split and the counts are zero.

## `public const string TAuditStrictReport = "temp/audit/Truth-{0}.md";`

Where the report lands, with the version in the name.
`temp` is ignored by Git, so a run never dirties the tree.

## `public static readonly string[] TAuditVeneerBases`

A class deriving from one of these is a veneer, as is any class with a `.xaml` beside it.

## `public static readonly string[] TAuditTreatVerbs`

Query methods that, applied to a logic value, are data treatment.
