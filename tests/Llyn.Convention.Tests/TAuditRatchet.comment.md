# TAuditRatchet.cs

## `public sealed class TAuditRatchet`

Holds the audit settings against the committed copy, so a gate can only tighten inside a working tree.
A ceiling raised, a waiver added or an enforcement switched off fails until the user commits it.
Only the user commits, so the diff that loosens a gate is always seen.
The hold runs within one generation only.
A new generation changes what the walk reports, so its ceilings and waivers are baselined afresh.

## `private static readonly string TAuditSettingFolder`

The test project folder, named from the project so this file is the same in every project.

## `private static readonly string[] TAuditWaiverPaths`

The three parts the custody waivers are kept in.

## `private static readonly Regex TAuditCeilingPattern`

A ceiling entry as it stands in a settings file.

## `private static readonly Regex TAuditWaiverPattern`

A waiver line as it stands in a settings file.

## `private static readonly Regex TAuditRingPattern`

A ring waiver or exempt line as it stands in the ring settings file, a path and a namespace.

## `private static readonly Regex TAuditRolePattern`

One role row of the ring table: the assembly and the namespaces it may not name.

## `private static readonly Regex TAuditQuotedPattern`

A quoted string, used to read the names of a role row.

## `private static readonly Regex TAuditEnforcedPattern`

The enforcement switch as it stands in a settings file.

## `private static readonly Regex TAuditGenerationPattern`

The generation a settings file was written at.

## `public void AuditRatchet_TruthCeiling_NeverRises()`

No custody ceiling stands above its committed value.

## `public void AuditRatchet_StrictCeiling_NeverRises()`

No strict ceiling stands above its committed value.

## `public void AuditRatchet_RingCeiling_NeverRises()`

No ring ceiling stands above its committed value.

## `public void AuditRatchet_ObjectCeiling_NeverRises()`

No object ceiling stands above its committed value.

## `public void AuditRatchet_RingWaiver_NeverGrows()`

No ring waiver is missing from the committed waiver list.
Only the waiver block is read, so an exempt row does not stand in for a waiver.

## `public void AuditRatchet_RingExempt_NeverGrows()`

No ring exempt row is missing from the committed exempt list.
An exempt row bypasses the ring guard outright, so one never appears without a commit that shows it.

## `public void AuditRatchet_RingRoles_NeverShrink()`

Every namespace a role was forbidden at the commit is still forbidden.
A role may gain a forbidden namespace, never lose one.

## `public void AuditRatchet_TruthWaiver_NeverGrows()`

No custody waiver is missing from the committed list.
Skipped while the committed file carries no ceilings, which is the first enforcement.

## `public void AuditRatchet_Enforced_NeverFlipsOff()`

A switch committed as true is still true.

## `private static void TAuditCeilingCheck(string path, IReadOnlyDictionary<string, int> current)`

Reads the committed ceilings and lists every kind whose current value is higher.
A file not yet committed has nothing to hold against and passes.

## `private static string TAuditBlockRead(string committed, string name)`

The text of one setting from its name to the end of its initializer.

## `private static bool TAuditGenerationCheck(string committed)`

True when the committed settings were written at the running generation.

## `private static string? TAuditCommittedRead(string path)`

The file as `git show HEAD:` prints it, or null when it is not in HEAD.
