# TRepertoire.cs

## `public sealed class TRepertoire`

Covers the repertoire panel's gates end to end on a real workspace.
A fresh start with no row chosen opens a blank scenario, and with a Situation chosen a linked entry.
The linked entry shows the link from its first paint.
A saved fresh scenario shows the stored Situation on the vignette, outside the scribe.
A stored leave on a click stores the scenario and shows the clicked Situation.
An opened Situation shows on the vignette, and one hidden by the query drops the query first.
A click records the voyage and shows the Situation, and a refused leave records nothing.
A leave with nothing unsaved asks nothing, and a stored leave keeps the Situation.
A kept leave stays on the scenario, from the leave question and from an occurrence click alike.
Closing the scenario cancels the desk, and closing the occurrence editor falls back to the chosen Situation.
A referencing entry shows on the display, and a close clears both lists.
An entry notice keeps the scenario without a chosen occurrence, and keeps the display with one.
A rows read that no longer lists the shown Situation clears both lists.
A confirmed delete removes the chosen Situation, and the occurrence side deletes nothing.
An export writes only the entry on display, and a print with nothing chosen prints nothing.

## `private static void TRepertoireTitleDefer(CRepertoire repertoire)`

Writes a title into the held scenario, so the desk holds an unsaved change.
The draft's own Situation id comes from the scenario read.

## `internal static CRepertoire TRepertoirePrepare(CAtelier atelier, CEnvoy envoy)`

Builds the repertoire over the atelier with its own editor, and restores both vistas as the forge does.
Its seam answers that the tab is in front.

## `internal static LSituation TRepertoireSituationSave(LEngine engine, string title)`

Stores one Situation with the given title.

## `internal static LEntry TRepertoireEntrySave(LEngine engine, string headword, LSituation situation)`

Stores one English entry whose card references the Situation.
