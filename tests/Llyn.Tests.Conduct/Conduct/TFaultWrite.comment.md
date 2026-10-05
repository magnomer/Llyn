# TFaultWrite.cs
Hash: `b49b0ba30da35a30`

## `public sealed partial class TFault`

The synchronous half of the fault sweep, holding one row per gate that writes.
A gate that writes must show the fault once and leave the state it reads back unchanged.
A gate that only records its fault must raise nothing and show nothing.
A settings change once landed in memory before its write failed, so the read-back guards that hole.

## `private static readonly IReadOnlyList<TFaultWrite> TFaultWriteRows =`

One row per synchronous writing gate, named `Type.Method` like the task rows.
The two workspace change overloads carry their parameter types, so each runs under its own name.
Port rows fault their port member over `TInterfaceConduct.TAtelierFaultCreate`.
Settings rows fault `LSettingsVault.LSettingsSave`, so the engine's own rollback is under test.
Their read-back is the settings field the gate writes, read from the engine.
The tab select faults `LPostureVault.LPostureSave` and shows `Layout.SaveFailed`.
The volume set faults `LMediaPort.LEngineVolumeSet` and shows `Sound.VolumeFailed` through the workspace's held envoy.
Neither reads back, since each failure keeps the state the screen shows and tells the user once.
Volume, tab select and close open the atelier first, so the workspace holds an envoy.
The close row keeps its atelier off the stage, since a second close would fault again.
The close row expects no key, since the window is closing and the fault is only recorded.
The entry, display, panel and sound rows join from `TFaultMark.cs`, keeping this file within the line limit.

## `public static TheoryData<string> TFaultWriteGates`

The gate names of every synchronous row, so the theory runs once per gate under a readable name.

## `public void EnvoyFailureShow_FailedWrite_ReceivesKeyAndKeepsState(string gate) => TFaultWriteRun(gate);`

Runs a synchronous row with its member throwing.

## `private static void TFaultWriteRun(string gate)`

Arranges the row, reads the state back, then clears what the envoy heard.
The gate call must raise nothing, and the envoy must have heard exactly the row's key, once.
A row with no key expects the envoy to have heard nothing at all.
A row with a read-back must read the same value after the failed call as before it.

## `private static LEngine TFaultVaultStart(TFaultStage stage)`

Starts an engine on a prepared workspace whose settings and posture vaults forward to the real ones.
The vault member the stage names throws a vault fault, as the real vaults wrap their failures.
A first engine stores a posture beforehand, so the faulted engine's posture load saves nothing.
Only the gate's own save then meets the fault, not a save made at load.

## `private static void TFaultVaultCheck(TFaultStage stage, string member)`

Throws a vault fault when `member` is the stage's faulted member, and does nothing otherwise.

## `private static CEnvoy TFaultFolderCreate(TFaultStage stage, string chosen) =>`

An envoy that answers the workspace folder question with `chosen` and records failure keys.
The workspace change without a path asks it before it reaches the port.

## `private sealed record TFaultWrite(string TFaultWriteGate, string TFaultWriteMember, string? TFaultWriteKey, Func<TFaultStage, (Action, Func<object?>?)> TFaultWriteArrange)`

One gate of the synchronous sweep, with the fault it meets and the notice it must show.

**Parameters**

- `TFaultWriteGate`: the gate's name as `Type.Method`.
- `TFaultWriteMember`: the faulted member as `Interface.Member`.
- `TFaultWriteKey`: the notice key the envoy must hear once, or null for a gate that only records its fault.
- `TFaultWriteArrange`: answers the gate call and an optional read-back of the state the gate writes.
