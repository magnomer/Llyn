# LDraftOutlet.cs
Hash: `bf2e26fd7d6f7135`

## `public sealed class LDraftOutlet : LDraftPort`

The engine's face for `LDraftPort`, handed by the host to the conduct in place of the engine itself.
Every member forwards to the engine part that owns it and holds no state.
A forward can later target a facade instead of the engine without changing the port.

## `public LDraftOutlet(LEngine engine)`

Rejects a null engine and keeps it for every forward.

## `public IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth, string LMeaningPath)>`

Forwards the meaning scan to the card facade.
