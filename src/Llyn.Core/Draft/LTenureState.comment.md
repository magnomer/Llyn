# LTenureState.cs

## `public sealed record LTenureState(`

What a panel holding a draft needs to know to light its buttons, read in one call.
The engine owns every answer in it, so the panel keeps no dirty flag, halted flag or chronicle.
It is a record, so two readings compare by value and a tenure raises its bulletin only when something moved.

**Parameters**

- `LTenureStateChanged` — Whether the held draft differs from what it was opened on, by the engine's measure.
- `LTenureStateRefusal` — The reason a commit would be refused right now, or `null` when it would go through.
- `LTenureStateBackward` — Whether an undo has a snapshot to step back to.
- `LTenureStateForward` — Whether a redo has a snapshot to step forward to.
- `LTenureStateHalted` — Whether a request failed to apply, after which nothing further applies.
