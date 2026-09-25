# LAtlas.cs

## `public sealed class LAtlas`

The deportment of the repertoire panel's situation list: the panel state over the situation vista and its rows.
The list finds rows and their usage, and takes the inquest, tier and mesh.
Its panel loads, edits and deletes the chosen Situation.
The delete seam asks the removal seam with how many entries reference the chosen Situation.

## `public bool LAtlasNarrowed`

Whether the inquest or the filter hides any row, as the vista answers it.

## `private int LAtlasUsageRead(long? id)`

How many entries reference the given Situation, read fresh from the engine.
Zero when no Situation is given.

## `private bool LAtlasDeleteConfirm()`

Asks the removal seam whether to delete the chosen Situation, given its usage.
