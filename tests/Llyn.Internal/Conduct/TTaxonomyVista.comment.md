# TTaxonomyVista.cs

## `public sealed class TTaxonomyVista`

Covers the taxonomy's vista restore, its subject plan and its rows failures on a real workspace.
A second restore carries both held queries into the fresh vistas.
After a second restore a settings notice raises the Tag rows then the entry rows, once, through the marshal.
A create alone restores once, so the same notice after only a create raises once too.
A Tag notice with an entry shown raises the rows before the shown draft is read again.
A workspace notice closes the entry, lets go of the Tag, raises the rows, and tells the driver last.
A failing Tag find shows `Tag.LoadFailed` once and raises no entry rows.
A settings notice with both finds failing still shows `Tag.LoadFailed` only once.
A failing entry find alone shows `Tag.LoadFailed` and answers no rows.
The rows event is answered in these tests as the driver answers it, by reading the Tag list.
Its flag-fill load answers the same rows once the fill has run.
