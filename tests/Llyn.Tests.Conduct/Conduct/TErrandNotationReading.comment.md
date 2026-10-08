# TErrandNotationReading.cs
Hash: `d64260cf4771f331`

## `public sealed class TErrandNotationReading`

Covers how the notation popup writes a picked reading into the draft, on a real editor.
It takes its pack from `TErrandNotation.TErrandNotationPack`.
It builds its editors through `TErrandNotation.TErrandNotationPrepare`, except the schemed one, which opens a stored entry.
A pick writes the primary IPA and its variety at once, before any persist.
An accent row takes the pick, and a row removed since the start takes nothing.
A schemed search writes the transcription text at once and names no variety.
No search, or a desk still filling, writes nothing.
