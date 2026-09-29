# TCorpus.cs

## `public sealed class TCorpus`

Covers the corpus panel's gates end to end on a real workspace.
A fresh start with no row chosen opens a blank transcript, and with an Example chosen a quoting entry.
The quoting entry shows the citation from its first paint.
An opened Example shows on the excerpt, and one hidden by the query drops the query first.
A click records the voyage and shows the Example, and a refused leave records nothing.
A leave with nothing unsaved asks nothing, and a discard stores nothing.
A kept leave stays on the transcript, and a stored leave keeps the Example.
A saved fresh transcript shows the stored Example on the excerpt outside the scribe.
Closing the transcript cancels the desk, and closing the quotation editor falls back to the chosen Example.
A quoting entry shows on the display, and a workspace notice clears both lists and tells the driver.
An entry notice keeps the transcript without a chosen quotation, and keeps the display with one.
A rows read that no longer lists the shown Example clears both lists.
A confirmed delete removes the chosen Example, and the quotation side deletes nothing.
The window's exit gate cancels the entry editor's desk and stops its display's playback once.
The marshal the corpus is built with hands each transcript edit on as the held Example.
A held stored Example carries its tally and text placeholder, and a cancel raises the blank Example with both.

## `internal static CCorpus TCorpusPrepare(CAtelier atelier, CEnvoy envoy)`

Builds the corpus over the atelier with its own editor, and restores both vistas as the forge does.
Its seam answers that the tab is in front, and its marshal runs each desk notice at once.
`TQuotation` builds its corpus the same way.

## `internal static LExample TCorpusExampleSave(LEngine engine, string text)`

Stores one English Example with the given text and no cited Source.

## `internal static LEntry TCorpusEntrySave(LEngine engine)`

Stores one English entry to quote.
