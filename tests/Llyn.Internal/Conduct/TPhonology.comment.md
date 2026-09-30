# TPhonology.cs

## `public sealed class TPhonology`

Covers the phonology panel's gates end to end on a real workspace.
Before the vista is restored the rows are empty, and the panel reads empty and unfiltered.
A query narrows the rows, and an unmatched one reads empty.
The chosen entry marks its row alone.
Each row copies the entry, its twin name, its empty epithet, its sound and its bracketed text.
A null order keeps the chosen one, and a hidden language marks the panel filtered until cleared.
Export writes nothing until an entry is chosen, then writes it under its headword.
An entry opened in scribe mode shows in the editor, and closing the panel empties it.
A second restore carries the held query, and its notices raise the rows once through the marshal.
A Settings notice and a filter change each raise the rows, so the filter mark repaints.
A Workspace notice lets the chosen entry go and tells the driver once.
The sequence menu offers four orders.
The atelier's close lets the editor's draft go and stops the recording.

## `private static CPhonology TPhonologyPrepare(CAtelier atelier)`

Builds the phonology panel and restores its vista, as the window does for the tab.
