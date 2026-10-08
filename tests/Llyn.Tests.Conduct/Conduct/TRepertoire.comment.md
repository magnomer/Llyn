# TRepertoire.cs
Hash: `9caa5ec9bcceaeec`

## `public sealed class TRepertoire`

Covers how the repertoire panel chooses and shows a Situation, end to end on a real workspace.
With a Situation chosen, a fresh start opens a linked entry that shows the link from its first paint.
An opened Situation shows on the vignette, and one hidden by the query drops the query first.
A click with nothing unsaved records the voyage and shows the Situation.
A referencing entry shows on the display, and a workspace notice clears both lists.
An entry notice keeps the display while an occurrence is chosen.
A rows read that no longer lists the shown Situation clears both lists.
A confirmed delete removes the chosen Situation, and the occurrence side deletes nothing.
The held scenario lives in `TRepertoireScribe`, and the print and export gates in `TRepertoirePortrait`.

## `internal static CRepertoire TRepertoirePrepare(CAtelier atelier, CEnvoy envoy)`

Builds the repertoire over the atelier with its own editor, and it restores both vistas itself.
Its seam answers that the tab is in front, and its marshal runs each notice at once.

## `internal static void TRepertoireTitleDefer(CRepertoire repertoire)`

Writes a title into the held scenario, so the playwright's desk holds an unsaved change.
The draft's own Situation id comes from the scenario read.

## `internal static LSituation TRepertoireSituationSave(LEngine engine, string title)`

Stores one Situation with the given title.

## `internal static LEntry TRepertoireEntrySave(LEngine engine, string headword, LSituation situation)`

Stores one English entry whose card references the Situation.
