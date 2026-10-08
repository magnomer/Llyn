# CDeskChronicle.cs
Hash: `fc6b9a85af7d4b71`

## `public sealed class CDeskChronicle`

The undo and redo steps of one desk's held draft, and the halt the tenure reports.
It reads the tenure through the desk's `CDeskDraft`, so the draft keeps the only hold.
It shows its hold failures through `CEnvoy`, so no driver can lose one.

## `private bool _cDeskChronicleHalted;`

Whether the tenure was halted at the last bulletin.
A start clears it.

## `internal CDeskChronicle(CDeskDraft draft, LSettingsPort settings, string scope, CEnvoy envoy)`

Only the desk builds one, over its own draft holder, settings, scope and envoy.

## `internal event Action? CDeskChronicleChanged;`

A step ran or a bulletin arrived, so the chronicle buttons are read again.
The desk raises its `CDeskStateChanged` on it, so drivers hear one event.

## `public bool CDeskChronicleHalted`

Whether the tenure has stopped taking requests, so the edit area should go dead.

## `public bool CDeskChronicleRunning`

A tenure is held and still takes requests, so its edit area stays live.

## `private bool CDeskChronicleStalling`

The tenure halted since the last bulletin.

## `internal void LDeskChronicleClear()`

Forgets the last bulletin's halt, called by the desk on every start.

## `public void CDeskChronicleResonate()`

Announces the change of state, after showing the scope's `HoldFailed` notice if the tenure just halted.
The notice shows once per halt.

## `public (bool CDeskBackward, bool CDeskForward) CDeskChronicleRead()`

Whether the held draft can step back and forward, or neither while nothing is held.

## `public void CDeskChronicleUndo()`

Steps the held draft back, then announces the change of state.
A failed step is shown under the scope's `HoldFailed` key and still announces.
`CDeskChronicleRedo` steps it forward the same way.

## `public void CDeskChronicleRedo()`

Steps the held draft forward, then announces the change of state.
A failed step is shown under the scope's `HoldFailed` key and still announces.
