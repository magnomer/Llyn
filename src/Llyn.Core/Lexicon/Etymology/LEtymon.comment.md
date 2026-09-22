# LEtymon.cs

## `public sealed record LEtymon(`

One source entry an etymology links to directly.
It is the bare link shape, drawn as a chip beside the headword and carrying no prose.
The target is held as the Entry's id, never as text, so a rename follows by itself.
An entry holds its Etymons in the order the user put them in.
A span inside a narrative is an `LMention` instead, never an Etymon.

**Parameters**

- `LEtymonId` — Opaque, program-generated stable id, zero for a row the store has not written.
- `LEtymonEntryId` — The Entry that declares this link as part of its own origin.
- `LEtymonPosition` — The place of the link among the entry's links, counted from zero.
- `LEtymonTargetId` — The source Entry the link points at.
