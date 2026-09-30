# TFavoriteVista.cs

## `public sealed class TFavoriteVista`

Covers the favorites panel's vista restore, its observers, its rows failure and its close on a real workspace.
A second restore carries the held query into the fresh vista.
After a second restore a favorite notice raises the rows once, through the marshal, from the fresh vista only.
A create alone restores once, so the same notice after only a create raises once too.
A workspace notice closes the chosen entry and tells the driver once.
A settings notice raises the rows through the marshal.
A failing favorite find shows `Favorite.LoadFailed` and answers no rows.
The menu offers five orderings.
The window's exit gate closes the editor and stops the recording.
Its flag-fill load answers the same rows once the fill has run.
