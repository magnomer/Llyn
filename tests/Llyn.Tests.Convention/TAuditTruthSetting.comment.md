# TAuditTruthSetting.cs

## `internal static class TAuditTruthSetting`

Hand-written and tracked: the driver-audit scope, the binder inputs and the handle list live here.
The per-file ceilings live in the ledger named by `TAuditLedgerFile`.
No script writes this file.

## `public const bool TAuditTruthEnforced = true;`

False makes every driver fact a warning that passes.
True fails a fact on any file whose count of a kind stands above its ledger ceiling.

## `public const string TAuditTruthReport = "temp/audit/Custody-{0}.md";`

Where the report lands, with the version in the name.
`temp` is ignored by Git, so a run never dirties the tree.

## `public const string TAuditStateSuffix = "State";`

A field or property whose name ends in this is a state the engine should own.

## `public const string TAuditBulletinType = "LBulletin";`

The engine's notice type.
A handler or lambda taking one and never reading it is a deaf observer, in either medium.

## `public const string TAuditConfiguration = "Debug";`

The build configuration whose output and generated files the binder reads.

## `public const string TAuditReferenceRoot = "src/Llyn.Host";`

The project whose build output carries every package any source references.
Host names every project, so its output holds the packages of both media and of Infrastructure.

## `public const string TAuditConductRoot = "src/Llyn.Conduct";`

The folder whose types are Conduct's.
A driver may hold, implement and reshape them, since Conduct's types stop at the driver.

## `public const string TAuditLedgerFile = "TAuditTruthLedger";`

The ledger holding the ceiling of every kind in every driver file.

## `public static readonly string[] TAuditShellInclude`

The `git ls-files` patterns of every UI source, both surfaces and both drivers.
A type declared under one of these is UI.
A type declared in Conduct is a gate type, and a type declared under any other source is engine.

## `public static readonly string[] TAuditTruthInclude`

The patterns of the driver sources the driver walks audit, Deportment and Demeanor alike.
The surfaces are left to the surface audit, where holding anything is already a hit.

## `public static readonly string[] TAuditFrameworkPacks`

The shared frameworks referenced beside the test runtime, so framework types resolve.

## `public static readonly string[] TAuditControlBases`

A type deriving from one of these is a GUI control, whose state may not decide a request.

## `public static readonly string[] TAuditOrderVerbs`

The collection calls that change an order, which only the engine may decide.

## `public static readonly string[] TAuditFillVerbs`

The calls that write into a collection in place.
A `readonly` field filled through one is not a fixture.

## `public const string TAuditRequestPrefix = "LRequest";`

The prefix every request record carries.

## `public static readonly string[] TAuditSendRoots`

The tenure calls a request finally goes through.
A member that reaches one, calls a Conduct method or builds a request is a sender.

## `public static readonly string[] TAuditClockTypes`

The types that fire on time rather than on a user act.
A clock drives a request through an event, a callback it is built with or a loop waiting on it.

## `public static readonly string[] TAuditConsoleInput`

The console calls that carry what the user typed, the CUI's control input.
Each is written as the full name of its declaring type and member.

## `public static readonly string[] TAuditDialogTypes`

The types whose answer confirms a request.
A confirm belongs in the gate behind a port, so a dialog answer deciding a request is a guard.

## `public static readonly string[] TAuditDelayMembers`

The waits that turn a loop into a clock.

## `public static readonly string[] TAuditInputMembers`

The members of a GUI control that carry what the user typed or chose.

## `public static readonly string[] TAuditTruthHandles`

Driver types a driver field may hold as a handle rather than as a value.
A handle is called on and passed on, so the field is skipped before any rule reads it.
A Conduct type is a handle by rule and is never listed.
A type from below Conduct is never a handle, so holding one is a mirror hit counted in the ledger.
`LWindow` is the window deportment the veneer helpers take.
A type is matched as the binder shows it, with any nullable mark dropped.

## `public static readonly string[] TAuditTreatVerbs`

Query methods that, applied to a value from below Conduct, are data treatment.
