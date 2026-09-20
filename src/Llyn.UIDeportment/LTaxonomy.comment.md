# LTaxonomy.cs

## `public sealed class LTaxonomy`

The taxonomy panel's deportment: the tag vista and the membership vista it holds, and what the panel asks of them.
The tag side finds rows, takes the query, order and kind filter, and creates a Tag.
The membership side finds the entries of the chosen Tag, loads and deletes the chosen one, and marks it edited.
Both vistas are handed in by the window, which restores every tab's vistas together.
The panel's mode, its bin and its scribe toggle stay in the veneer until the tabs share one panel deportment.

## `public LVista? LTaxonomyMembershipVista => _lTaxonomyMembership;`

The membership vista, exposed for the window's export dialog alone, which names its file after the vista.

## `public IReadOnlyList<LVistaRow> LTaxonomyMembershipRead()`

The entries under the chosen Tag, or every entry while none is chosen, as the engine narrows them.
