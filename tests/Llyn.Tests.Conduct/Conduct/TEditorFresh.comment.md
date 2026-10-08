# TEditorFresh.cs
Hash: `d085429fc639bcb1`

## `public sealed class TEditorFresh`

Covers how the entry editor opens a fresh draft, bare or from a linked object, on a real workspace.
It builds each editor through `TEditor.TEditorPrepare` and its stored entry through `TEditor.TEditorEntryPrepare`.
A null open holds a fresh draft, and a missing entry falls back to one.
A fresh draft holds one card of each kind.
A fresh occurrence start opens a draft already linked to its Situation, and a blank one without.
A fresh quotation start replaces the held draft with one already citing its Example.
A fresh footnote start opens a draft already citing its Source in the first sentence.
A fresh membership start replaces the held draft with one already carrying its Tag.
A fresh cohort start replaces the held draft with one already carrying its Register.
