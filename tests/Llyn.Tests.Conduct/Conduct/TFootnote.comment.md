# TFootnote.cs
Hash: `3586f945990c5004`

## `public sealed class TFootnote`

Covers the sources panel's entry list end to end on a real workspace.
With no Source chosen it lists every entry, and a chosen Source that nothing cites empties it.
An unmatched query empties the rows, and the empty list then reads as unmatched rather than vacant.
A fresh entry opens in the editor, citing the chosen Source, or citing nothing while none is chosen.
The create and the vista restore are internal helpers, reached through relays.
Its flag-fill load answers the same rows once the fill has run.

## `private static (CFootnote, LVista, CEditor)`

Builds the list over the atelier's ports and a fresh entry editor.
It starts a source vista as the parent and a footnote vista that the list and the editor share.
Its seams answer that the tab is in front and that every finish succeeds.

## `private static LEntry TFootnoteEntrySave(LEngine engine, string headword)`

Stores one English entry with a single meaning.
