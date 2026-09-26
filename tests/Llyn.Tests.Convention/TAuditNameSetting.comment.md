# TAuditNameSetting.cs

## `internal static class TAuditNameSetting`

Hand-written and tracked: every naming-audit setting lives here and nowhere else.
No script writes this file, and no registered name may appear in it.
Bases, verbs, and exemptions reach the tests only through the generated TAuditNameRegistry.cs.
The tests read no script configuration, so no json or ps1 can change what they check.
They do read the untracked sources Git lists, since an unstaged file is still source.

## `public const int TAuditPrefixCeiling`

The number of types whose prefix lies outside their ring.
It only falls, and the script holds the same number as `naming.prefixCeiling`.

## `public static readonly string[] TAuditPrefixes`

Every prefix, longest first, so `PS` is read before `P`.
`P` marks a surface, `Q` a driver, `C` Conduct, `L` every other project and `T` a test.
An `S` after the first letter marks a subwindow, and a lowercase form marks a private field.

## `public static readonly Dictionary<string, string[]> TAuditPrefixRings`

Each project folder with the prefixes its types may carry.
The script holds the same map as `naming.prefixRings`.
