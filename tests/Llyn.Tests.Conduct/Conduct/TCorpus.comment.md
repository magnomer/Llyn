# TCorpus.cs
Hash: `0d88f7649ee45fc0`

## `public sealed class TCorpus`

Covers how the corpus panel chooses and shows an Example, end to end on a real workspace.
With an Example chosen, a fresh start opens a quoting entry that shows the quoted Example from its first paint.
An opened Example shows on the excerpt, and one hidden by the query drops the query first.
A click with nothing unsaved records the voyage and shows the Example.
A quoting entry shows on the display, and a workspace notice clears both lists and tells the driver.
An entry notice keeps the display while a quotation is chosen.
A rows read that no longer lists the shown Example clears both lists.
A confirmed delete removes the chosen Example, and the quotation side deletes nothing.
The transcript desk lives in `TCorpusScribe`.
The marshal, the held Example and the window's exit gate live in `TCorpusHold`.

## `internal static CCorpus TCorpusPrepare(CAtelier atelier, CEnvoy envoy)`

Builds the corpus over the atelier with its own editor, and the corpus restores both vistas itself.
Its seam answers that the tab is in front, and its marshal runs each desk notice at once.
Most corpus test classes, `TQuotation`, `TFaultPortrait` and `TTranscriptMention` build their corpus through it.

## `internal static LExample TCorpusExampleSave(LEngine engine, string text)`

Stores one English Example with the given text and no cited Source.

## `internal static LEntry TCorpusEntrySave(LEngine engine)`

Stores one English entry to quote.
