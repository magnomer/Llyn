# TAuditTruthSetting.cs

## `internal static partial class TAuditTruthSetting`

Hand-written and tracked: the truth-audit scope, the ceilings and the handle types live here.
The waivers live in their own parts, one per kind of hit.
No script writes this file.

## `public const bool TAuditTruthEnforced = true;`

False makes every custody fact a warning that passes.
True fails a fact on any hit that is not waived and on any kind above its ceiling.

## `public const string TAuditTruthReport = "temp/audit/Custody-{0}.md";`

Where the report lands, with the version in the name.
`temp` is ignored by Git, so a run never dirties the tree.

## `public const string TAuditStateSuffix = "State";`

A field or property whose name ends in this is a state the engine should own.

## `public const string TAuditBulletinType = "LBulletin";`

The engine's notice type, so a handler taking one and never reading it is a deaf handler.

## `public const string TAuditObserverType = "PObserver";`

The shell's subscription type, so a lambda handed to one that ignores its bulletin is deaf too.

## `public static readonly string[] TAuditTruthInclude`

The `git ls-files` patterns of the shell sources.

## `public static readonly string[] TAuditOrderVerbs`

The collection calls that change an order, which only the engine may decide.

## `public static readonly string[] TAuditFillVerbs`

The calls that write into a collection in place.
A `readonly` field filled through one is not a fixture.

## `public const string TAuditRequestPrefix`

The prefix every request record carries.

## `public static readonly string[] TAuditSendRoots`

The tenure calls a request finally goes through.
A member that reaches one, or builds a request, is a sender.

## `public static readonly string[] TAuditClockTypes`

The types whose events fire on time rather than on a user act.

## `public static readonly string[] TAuditInputMembers`

The members of a control that carry what the user typed or chose.

## `public static readonly string[] TAuditTruthHandles`

Types a shell field may hold as a handle to the engine rather than as a value.
A handle is called on and passed back, so the argument and guard rules skip it.
`PObserver` is the shell's subscription and is attached and detached by the engine.
`LListener`, `LReceiver` and `LObserver` are the contracts a panel derives from to be called back.

## `public static readonly IReadOnlyDictionary<string, int> TAuditTruthCeiling`

The hit count each kind may reach, waived hits included.
A count above fails the fact, a ceiling above the count is stale and fails too.
Lower a ceiling when the shell sheds a hit, never raise one to admit a new one.


## `public static string[] TAuditTruthWaiver => [.. TAuditWaiverArgument, .. TAuditWaiverGuard, .. TAuditWaiverField];`

Every waiver, joined from the argument, guard and field parts on each read.
A property, since static fields across partial files initialise in no fixed order.
A new field, or a new kind on a waived field, is not waived and fails.
A waiver matching no hit is reported as stale and is removed, never kept.
