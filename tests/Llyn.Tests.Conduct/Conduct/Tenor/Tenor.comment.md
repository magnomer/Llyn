# Tenor

This folder holds the tenor panel's gates.
They cover the workspace browsed by registers, its entry list and vista.

## `TTenor`

Covers the tenor panel's gates end to end on a real workspace.
A fresh area already stands on its vistas, so it lists the stored Register with nothing chosen or filtered.
The empty entry list reads vacant under a blank search and unmatched under a written one.
A null order keeps the chosen one.
A hidden language marks the panel filtered until cleared.
Print and export do nothing until an entry is shown, then export writes it under its headword.
A row click records the station, toggles the Register and raises the rows.
An arrival empties both searches, chooses the Register and raises the opening, then the rows.
Closing the atelier cancels the editor's entry and stops the playback once.
Creating and opening an entry lives in `TTenorEntry`.
`TTenorPrepare` builds the panel, which restores both of its vistas itself, as the window does for the tab.

## `TTenorEntry`

Covers how the tenor panel starts, closes and opens an entry on a real workspace.
It builds its panel through `TTenor.TTenorPrepare`.
New asks the wording under the Register key.
A padded wording opens the trimmed Register as an arrival does, chosen with no usage.
A blank wording shows the refusal through the envoy and opens nothing, and a retreat makes nothing.
A changed fresh entry that stays gets only the leave question.
One that is discarded goes on to the wording.
New starts an entry without asking the wording once a Register is chosen.
A fresh entry under a chosen Register carries it from its first paint and lists under it once stored.
A fresh entry with an entry shown and no Register chosen is blank and unchanged.
Closing the panel drops the fresh draft the editor held.
A cohort row select opens the entry with no station, and a stayed leave question opens nothing.

## `TTenorVista`

Covers the tenor's rows load, vista restore, notice handling and rows failures on a real workspace.
A second restore carries both held queries into the fresh vistas.
After it a settings notice raises the Register rows then the entry rows, once, through the marshal.
A create without a restore raises the same notice rows once too.
A Register notice with an entry shown raises the rows before the shown draft is read again.
A workspace notice closes the entry, lets go of the Register, raises the rows, and tells the driver last.
A failing Register find shows `Register.LoadFailed` once and raises no entry rows.
A settings notice with both finds failing still shows `Register.LoadFailed` only once.
A failing cohort read shows `Register.LoadFailed` and answers no rows.
The rows event is answered in these tests as the driver answers it, by reading the Register list.

## Stamp

Commit: `c2f335c331fe`
- `TTenor.cs` `738f8586d18e5c2e`
- `TTenorEntry.cs` `b070e44ef3675130`
- `TTenorVista.cs` `f236ea567d99d759`
