# TDraftExample.cs

## `public sealed class TDraftExample`

Covers the sentence side of the drafts folder, the held work the corpus panel pushes into.
A sentence draft carries an Example where an entry draft carries a form.
The folder holds both the same way.

## `public void ExampleSave_HeldSentence_SurvivesScan()`

A sentence typed and then lost with its launch is still on disk, and recovery offers it back.
This is the loss the downstream exists to prevent: the panel used to hold the sentence in its controls alone.

## `public void ExampleCommit_HeldSentence_LeavesNothingBehind()`

A committed sentence becomes an Example and leaves no file, no claim and nothing to recover.
A commit that kept its file would offer saved work back at every launch.

## `public void LeftoverSweep_SentenceMatchingStoredExample_SweepsIt()`

A draft matching the Example it names is collected, and one differing from its own is kept.
A kill just after a commit leaves such a file.
Only the sweep can tell it from work the user would lose.
