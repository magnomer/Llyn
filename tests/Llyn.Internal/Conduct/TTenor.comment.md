# TTenor.cs

## `public sealed class TTenor`

Covers the tenor panel's gates end to end on a real workspace.
Before the vistas are restored the rows are empty and nothing is chosen or filtered.
A created Register answers its id, lists by its name with no usage, and reads chosen once selected.
New names a Register while nothing is chosen or shown, and starts an entry once a Register is chosen.
The empty entry list reads vacant under a blank search and unmatched under a written one.
A fresh entry under a chosen Register carries it from its first paint and lists under it once stored.
A fresh entry with no Register chosen is blank and unchanged.
Closing the panel drops the fresh draft the editor held.
A null order keeps the chosen one, the reverse order sorts, and the register vista's observer hears it.
An unmatched query lists no Register, and a hidden language marks the panel filtered until cleared.
Print and export do nothing until an entry is shown, then export writes it under its headword.
A row click records the station and toggles the Register, and an arrival chooses it and raises the opening.

## `private static CTenor TTenorPrepare(CAtelier atelier, CEnvoy envoy)`

Builds the tenor panel and restores both of its vistas, as the window does for the tab.
