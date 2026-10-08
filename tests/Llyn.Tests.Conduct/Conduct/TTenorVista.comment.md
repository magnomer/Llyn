# TTenorVista.cs
Hash: `f236ea567d99d759`

## `public sealed class TTenorVista`

Covers the tenor's rows load, vista restore, notice handling and rows failures on a real workspace.
A second restore carries both held queries into the fresh vistas.
After a second restore a settings notice raises the Register rows then the entry rows, once, through the marshal.
A create without a restore raises the same notice rows once too.
A Register notice with an entry shown raises the rows before the shown draft is read again.
A workspace notice closes the entry, lets go of the Register, raises the rows, and tells the driver last.
A failing Register find shows `Register.LoadFailed` once and raises no entry rows.
A settings notice with both finds failing still shows `Register.LoadFailed` only once.
A failing cohort read shows `Register.LoadFailed` and answers no rows.
The rows event is answered in these tests as the driver answers it, by reading the Register list.
The rows load answers the stored rows and the languages once the fill has run.
