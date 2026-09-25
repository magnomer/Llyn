# LTaxonomy.cs

## `public sealed class LTaxonomy`

The taxonomy panel's deportment: the tag vista and the membership vista it holds, and what the panel asks of them.
The tag side finds rows, takes the query, order and kind filter, and creates a Tag.
The membership side finds the entries of the chosen Tag, loads and deletes the chosen one, and marks it edited.
Both vistas are handed in by the window, which restores every tab's vistas together.
The panel's loads and clears go straight to the editor's lectern, so the veneer relays no draft.

## `public LEditor LTaxonomyEditor { get; }`

The entry editor's deportment on the membership side, which takes the membership vista when the panel's vistas are restored.

## `public LPanel LTaxonomyPanel { get; }`

The shared panel state over the membership vista: the chosen entry, the scribe mode, the bin and the leave guard.

## `public void LTaxonomyEntryCreate()`

Opens a blank entry in the editor.
When a Tag is chosen, the new entry starts inside that Tag.

## `public IReadOnlyList<LVistaRow> LTaxonomyMembershipRead()`

The entries under the chosen Tag, or every entry while none is chosen, as the engine narrows them.
