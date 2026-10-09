# TFaultStage.cs
Hash: `2894647223e0a2fc`

## `internal sealed class TFaultStage : IDisposable`

Carries the faulted member, the fault mode and the heard notices into a row's arrangement.
It disposes what the arrangement held in reverse order, the engine before its workspace.

## `internal TFaultStage(string member, bool thrown)`

Holds the member to fault and whether it throws or answers a faulted task.

## `internal string TFaultStageMember { get; }`

The faulted member as `Interface.Member`.

## `internal bool TFaultStageThrown { get; }`

True when the member throws before it answers any task.

## `internal List<string> TFaultStageHeard { get; } = [];`

The notice keys the envoy heard, which the sweep compares with the row's key.

## `internal TFaultKind TFaultStageAdd<TFaultKind>(TFaultKind held) where TFaultKind : IDisposable`

Holds a disposable for the stage's end and answers it, so arrangements build and hold in one step.

## `public void Dispose()`

Disposes every held item, the latest first.
