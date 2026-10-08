# TTaxonomy.cs
Hash: `4e0210c8851e9917`

## `public sealed class TTaxonomy`

Covers the taxonomy panel's gates end to end on a real workspace.
A fresh area already stands on its vistas, so it lists the stored Tag with nothing chosen or filtered.
The New gates live in `TTaxonomyEntry`, and the vista restore and notices live in `TTaxonomyVista`.
An arrival empties both searches before the driver hears of it.
The empty entry list reads vacant under a blank search and unmatched under a written one.
A null order keeps the chosen one, the reverse order sorts, and the area raises its rows.
An unmatched query lists no Tag, and a hidden language marks the panel filtered until cleared.
Print and export do nothing until an entry is shown, then export writes it under its headword.
A row click records the station, toggles the Tag, and raises the Tag rows then the entry rows.
An entry row click chooses the entry and records no station.
A changed fresh entry whose user stays opens nothing.
An arrival chooses the Tag and raises the opening, then the rows.
Closing the atelier cancels the editor's held entry and stops the display's playback once.

## `internal static CTaxonomy TTaxonomyPrepare(CAtelier atelier, CEnvoy envoy)`

Builds the taxonomy, which restores both of its vistas itself, as the window does for the tab.
`TTaxonomyEntry`, `TTaxonomyVista`, `TFaultMark` and `TFaultPortrait` share it.

## `internal static LEntry TTaxonomyEntrySave(LEngine engine)`

Stores one plain entry the membership list can open.
`TTaxonomyEntry` and `TFaultPortrait` share it.

## `internal static void TTaxonomyChangePrepare(LEngine engine, CTaxonomy taxonomy)`

Leaves a changed fresh entry open with no Tag chosen, so New would name a Tag after the leave question.
`TTaxonomyEntry` shares it.
