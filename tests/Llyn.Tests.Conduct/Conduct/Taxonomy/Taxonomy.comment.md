# Taxonomy

This folder holds the taxonomy panel's gates.
They cover the workspace browsed by tags, its entry list and vista.

## `TTaxonomy`

Covers the taxonomy panel's gates end to end on a real workspace.
A fresh area already stands on its vistas, so it lists the stored Tag with nothing chosen or filtered.
The New gates live in `TTaxonomyEntry`, and the vista restore and notices live in `TTaxonomyVista`.
An arrival empties both searches before the driver hears of it.
The empty entry list reads vacant under a blank search and unmatched under a written one.
A null order keeps the chosen one.
A hidden language marks the panel filtered until cleared.
Print and export do nothing until an entry is shown, then export writes it under its headword.
A row click records the station, toggles the Tag, and raises the Tag rows then the entry rows.
An entry row click chooses the entry and records no station.
A changed fresh entry whose user stays opens nothing.
An arrival chooses the Tag and raises the opening, then the rows.
Closing the atelier cancels the editor's held entry and stops the display's playback once.

## `TTaxonomyPrepare`

Builds the taxonomy, which restores both of its vistas itself, as the window does for the tab.
`TTaxonomyEntry`, `TTaxonomyVista`, `TFaultMark` and `TFaultPortrait` share it.

## `TTaxonomyChangePrepare`

Leaves a changed fresh entry open with no Tag chosen, so New would name a Tag after the leave question.

## `TTaxonomyEntry`

Covers how the taxonomy panel starts a new Tag or a new entry on a real workspace.
It builds each taxonomy through `TTaxonomy.TTaxonomyPrepare`.
New asks the wording while nothing is chosen or shown, and the Tag opens as an arrival does.
The padded wording is listed trimmed and chosen.
A blank wording shows the refusal through the envoy, and a retreat makes nothing.
A changed fresh entry asks the leave question first, and a user who stays gets nothing more.
A user who discards is then asked the wording.
Once a Tag is chosen, New starts an entry without asking the wording.
A fresh entry under a chosen Tag carries it from its first paint and lists under it once stored.
A fresh entry from a shown entry with no Tag chosen is blank and unchanged.
Closing the panel drops the fresh draft the editor held.

## `TTaxonomyVista`

Covers the taxonomy's vista restore, its subject plan and its rows failures on a real workspace.
A second restore carries both held queries into the fresh vistas.
After it a settings notice raises the Tag rows then the entry rows, once, through the marshal.
A create alone restores once, so the same notice after only a create raises once too.
A Tag notice with an entry shown raises the rows before the shown draft is read again.
A workspace notice closes the entry, lets go of the Tag, raises the rows, and tells the driver last.
A failing Tag find shows `Tag.LoadFailed` once and raises no entry rows.
A settings notice with both finds failing still shows `Tag.LoadFailed` only once.
A failing membership read shows `Tag.LoadFailed` and answers no rows.
The rows event is answered in these tests as the driver answers it, by reading the Tag list.
The flag-fill load answers the Tag rows and their languages once the fill has run.

## Stamp

Commit: `c37ca1837ded`
- `TTaxonomy.cs` `87fb2a6087aeb763`
- `TTaxonomyEntry.cs` `07974851f6f20d2f`
- `TTaxonomyVista.cs` `5b2a371d5bdbce5c`
