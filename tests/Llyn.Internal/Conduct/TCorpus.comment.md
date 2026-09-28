# TCorpus.cs

## `public sealed class TCorpus`

Covers the corpus panel's gates end to end on a real workspace.
A fresh start with no row chosen opens a blank transcript, and with an Example chosen a quoting entry.
An opened Example shows on the excerpt, and one hidden by the query drops the query first.
A click records the voyage and shows the Example, and a refused leave records nothing.
A leave with nothing unsaved asks nothing, and a discard stores nothing.
Closing the transcript cancels the desk, and closing the quotation editor falls back to the chosen Example.
A quoting entry shows on the display, and a close clears both lists.
An entry notice without a chosen quotation keeps the transcript.

## `private static CCorpus TCorpusPrepare(LEngine engine, CAtelier atelier, CEnvoy envoy)`

Builds the corpus over the atelier with its own editor, and restores both vistas as the forge does.
Its seam answers that the tab is in front.

## `private static void TCorpusRowsCheck(CCorpus corpus)`

Plays the driver's rows check, which clears a shown Example the refreshed rows no longer list.

## `private static LExample TCorpusExampleSave(LEngine engine, string text)`

Stores one English Example with the given text and no cited Source.

## `private static LEntry TCorpusEntrySave(LEngine engine)`

Stores one English entry to quote.
