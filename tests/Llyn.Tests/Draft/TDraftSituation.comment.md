# TDraftSituation.cs

## `public sealed class TDraftSituation`

Covers the situation side of the drafts folder, the held work the repertoire panel pushes into.
A situation draft carries a Situation where a sentence draft carries an Example.

## `public void SituationSave_HeldSituation_SurvivesScan()`

A context typed and then lost with its launch is still on disk, and recovery offers it back.
This is the loss the downstream exists to prevent: the panel used to hold the context in its controls alone.

## `public void SituationCommit_HeldSituation_LeavesNothingBehind()`

A committed context becomes a Situation and leaves no file, no claim and nothing to recover.
A commit that kept its file would offer saved work back at every launch.

## `public void LeftoverSweep_SituationMatchingStoredSituation_SweepsIt()`

A draft matching the Situation it names is collected, and one differing from its own is kept.
A kill just after a commit leaves such a file.
Only the sweep can tell it from work the user would lose.
