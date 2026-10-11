# TGrasp.cs
Hash: `fc971b3834a60e34`

## `public sealed class TGrasp`

Covers the grasp value and the engine's grasp seam.
It covers the range check.
An entry is rated, read back, cleared, and rated again through `LEngine`.
It covers the bulletin a save raises, which is what the reading view re-reads on.
An entry edit must never drop the rating the user gave.
It covers a bad value reaching neither the store nor a subscriber.
It covers a rating for a missing entry throwing.
It covers the favorite catalog carrying the rating, which a later ordering may sort by.
It covers the star scan, which turns a rating into full, half and empty stars for the sheet.
