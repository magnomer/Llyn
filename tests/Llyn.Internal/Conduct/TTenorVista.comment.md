# TTenorVista.cs

## `public sealed class TTenorVista`

Covers the tenor's vista restore, its subject plan and its rows failures on a real workspace.
A second restore carries both held queries into the fresh vistas.
After a second restore a settings notice raises the Register rows then the entry rows, once, through the marshal.
A Register notice with an entry shown raises the rows before the shown draft is read again.
A workspace notice closes the entry, lets go of the Register, raises the rows, and tells the driver last.
A failing Register find shows `Register.LoadFailed` once and raises no entry rows.
A settings notice with both finds failing still shows `Register.LoadFailed` only once.
A failing entry find alone shows `Register.LoadFailed` and answers no rows.
The rows event is answered in these tests as the driver answers it, by reading the Register list.
