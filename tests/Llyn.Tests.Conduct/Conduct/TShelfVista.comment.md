# TShelfVista.cs
Hash: `149ab0f0dc2e3280`

## `public sealed class TShelfVista`

Covers the sources panel's vista restore and its observers on a real workspace.
A second restore carries both held queries into the fresh vistas.
After a second restore a Reference notice raises each list once, through the marshal, from the fresh vistas only.
A create alone restores once, so the same notice after only a create raises once too.
An entry notice refills the Source rows, so their tallies follow, and the footnote rows through the marshal.
A workspace notice closes the chosen Source through the marshal.
A workspace notice with a Source shown closes both sides, built through `TShelf.TShelfPrepare`.
An entry notice with the entry side closed shows the chosen Source again.
A settings notice refills the Source rows through the marshal.
Closing the atelier cancels the entry editor's desk and stops playback.
Its flag-fill load answers the same rows once the fill has run.

## `private static CShelf TShelfMarshalCreate(CAtelier atelier, System.Action counted)`

Builds the shelf with a marshal that counts each notice it runs.

## `private static LEntry TShelfEntrySave(LEngine engine, string headword)`

Stores one English entry with a single meaning.
It repeats the helper in `TShelf`, which this file keeps private.
