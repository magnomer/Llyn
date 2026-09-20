# TAuditBoundarySetting.cs

## `internal static class TAuditBoundarySetting`

Hand-written and tracked: the names and files the boundary facts hold the shell to.
No script writes this file.

## `public static readonly string[] TAuditBoundaryForbidden`

The calls that read text or an id into a state, and a `using static` hiding a logic name.
Each is a regular expression, so a space before the parenthesis does not slip past.

## `public static readonly string[] TAuditBoundaryState`

The three state names a panel used to compare against, before the converter alone read them.
Each is matched bare, so a qualified read, markup and a `using static` are caught alike.
A bare name followed by `:` or `=` is a declaration or a named argument and is not a read.

## `public static readonly string[] TAuditBoundaryConverter`

The two files that may read a state apart, because showing a mark is their whole job.

## `public static readonly string[] TAuditBoundaryHidden`

The constructs that keep code out of a syntax walk: a preprocessor branch, reflection, `dynamic`, inline markup code.
Also an enum parsed from text and a `using` alias, which each give a logic name a second spelling.
The walkers parse without symbols, so a branch would be skipped, and reflection names nothing the walk can follow.

## `public static readonly string[] TAuditBoundaryLoader`

The two files that read embedded resources through the assembly, and so may name reflection.

## `public const string TAuditBoundaryReflection`

The namespace that reaches a member by its name as text.

## `public const string TAuditBoundaryTimer`

The type a panel used to hold its own debounce timer in, before the tenure owned the quiet.

## `public static readonly string[] TAuditBoundaryHold`

The file name patterns of the sources that hold a draft and may not keep a timer.

## `public const string TAuditBoundaryPanel`

A panel type declaration, by its prefix, which only the shell may hold.
