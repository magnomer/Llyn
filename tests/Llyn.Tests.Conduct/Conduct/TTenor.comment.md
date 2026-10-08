# TTenor.cs
Hash: `738f8586d18e5c2e`

## `public sealed class TTenor`

Covers the tenor panel's gates end to end on a real workspace.
A fresh area already stands on its vistas, so it lists the stored Register with nothing chosen or filtered.
The empty entry list reads vacant under a blank search and unmatched under a written one.
A null order keeps the chosen one, the reverse order sorts, and the rows are raised.
An unmatched query lists no Register, and a hidden language marks the panel filtered until cleared.
Print and export do nothing until an entry is shown, then export writes it under its headword.
A row click records the station, toggles the Register and raises the rows.
An arrival chooses the Register and raises the opening, then the rows.
An arrival empties both searches before the opening is raised.
Closing the atelier cancels the editor's entry and stops the playback once.
Creating and opening an entry lives in `TTenorEntry`.

## `internal static CTenor TTenorPrepare(CAtelier atelier, CEnvoy envoy)`

Builds the tenor panel, which restores both of its vistas itself, as the window does for the tab.
