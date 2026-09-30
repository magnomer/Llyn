# TRepertoireVista.cs

## `public sealed class TRepertoireVista`

Covers the repertoire panel's vista restore and its observers on a real workspace.
A second restore carries both held queries into the fresh vistas.
After a second restore a Situation notice raises each list once, through the marshal, from the fresh vistas only.
A create alone restores once, so the same notice after only a create raises once too.
An entry notice refills the situation rows, so their tallies follow.
A workspace notice closes the chosen Situation and tells the driver.
A settings notice refills the situation rows through the marshal.
Its flag-fill load answers the same rows once the fill has run.
