# LTenureGauge.cs
Hash: `05df6162da764173`

## `public sealed class LTenureGauge`

The state of one tenure's draft, read in one go and announced when it moves.
The panel settles its buttons from that announcement and keeps no dirty or halted flag of its own.
It keeps the last reading while the engine revision stands, since one editor refresh asks several times.
No engine call is made under the gate, because the engine raises bulletins that read the state back.

## `private static readonly LTenureState LTenureGaugeHalted = new(false, null, false, false, true);`

The state answered when the draft check itself fails, since the engine that just failed cannot be asked.

## `private static readonly LTenureState LTenureGaugeEnded = new(false, null, false, false, false);`

The one state an ended tenure answers, since its draft is gone.

## `private readonly LEngine _lEngine;`

The engine whose draft is read and whose bulletin announces a moved state.

## `private readonly long _lTenureGaugeId;`

The draft id, which every check and every tenure bulletin names.

## `private readonly object _lTenureGaugeGate;`

The tenure's own gate, so a kept state and the queue's flags are read under one hold.

## `private readonly LTenureQueue _lTenureGaugeQueue;`

The tenure's queue, which knows whether the tenure ended or halted.
A failing check halts the tenure through it.

## `private LTenureState _lTenureGaugeLast;`

The state last announced, so the bulletin is raised only when the state moved.

## `private LTenureState? _lTenureGaugeState;`

The state last read, kept because one editor refresh asks for it several times over.
An ended or halted tenure never answers it, so the end and the halt need not drop it.
Cancel and finish both announce through the engine as well, which moves the revision past it.

## `private long _lTenureGaugeRevision;`

The engine revision read before the kept state was computed.
A change landing mid-reading moves the revision past it, so the next reading computes again.

## `internal LTenureGauge(LEngine engine, long id, object gate, LTenureQueue queue)`

Made by the tenure alone, after its queue, since the first reading asks the queue whether the tenure lives.
That first reading becomes the last announced state, so a fresh tenure raises nothing.

## `public LTenureState LTenureGaugeRead()`

The state in one reading, made of changed, refusal, undo, redo and halted.
An ended tenure answers a fixed state without touching the engine.
A halted one still asks whether the draft changed, so a closing window can still warn.
It can neither undo nor redo, since nothing further applies.
A check that fails halts the tenure, because the draft can no longer be reached.
A live tenure reuses its kept state while the engine revision stands, sparing the disk and the database.
Every write to a draft, a stored record or the chronicle moves the revision.
The revision is read under `LEngineGate` before computing, so a change landing mid-reading forces the next reading to compute.
A halted tenure neither keeps nor reuses a state, and a reading that fails keeps nothing.

## `public bool LTenureGaugeStorable`

Whether the draft has changed and carries no refusal, read without writing the queue.
A store button asks it on every repaint, so it must not write.

## `internal static bool LTenureGaugeCheck(LTenureState state)`

The one owner of the no-refusal rule, shared by the tenure's ready check and the storable verdict.

## `internal void LTenureGaugeRaise()`

Reads the state and raises the tenure bulletin when it differs from the last one raised.
`LTenureQueue` calls it through the delegate the tenure hands over, after each apply, restore and halt.
The tenure's sweep, cancel and successful finish call it directly.
