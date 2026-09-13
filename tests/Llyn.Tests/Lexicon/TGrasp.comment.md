# TGrasp.cs

## `public sealed class TGrasp`

Covers the grasp value and the engine's grasp seam.
It covers the range check and the star figure the value renders as.
An entry is rated, read back, cleared, and rated again through `LEngine`.
It covers the bulletin a save raises, which is what the reading view re-reads on.
It covers what an entry edit must never do: drop the rating the user gave.
It covers a bad value reaching neither the store nor a subscriber.
It covers the favorite catalog carrying the rating, which a later ordering may sort by.
