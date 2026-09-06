# TDraftReference.cs

## `public sealed class TDraftReference`

Covers the source side of the drafts folder, the held work the imprint panel pushes into.
A source draft carries a Reference where a situation draft carries a Situation.

## `public void ReferenceSave_HeldReference_SurvivesScan()`

A source typed and then lost with its launch is still on disk, and recovery offers it back.
This is the loss the downstream exists to prevent: the panel used to hold the source in its controls alone.

## `public void ReferenceCommit_HeldReference_LeavesNothingBehind()`

A committed source becomes a Reference and leaves no file, no claim and nothing to recover.
A commit that kept its file would offer saved work back at every launch.

## `public void LeftoverSweep_ReferenceMatchingStoredReference_SweepsIt()`

A draft matching the Reference it names is collected, and one differing from its own is kept.
A kill just after a commit leaves such a file, and only the sweep can tell it from work the user would lose.
