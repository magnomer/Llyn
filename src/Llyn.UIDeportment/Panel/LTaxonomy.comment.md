# LTaxonomy.cs

## `public sealed class LTaxonomy`

The taxonomy panel's deportment: the tag vista and the membership vista it holds, and what the panel asks of them.
The tag side finds rows, takes the query, order and kind filter, and creates a Tag.
The membership side finds the entries of the chosen Tag, loads and deletes the chosen one, and marks it edited.
Both vistas are handed in by the window, which restores every tab's vistas together.
The panel's loads and clears go straight to the lectern its view hands in, so the veneer relays no draft.
Its public members name only .NET and Conduct types, and the window alone builds it.

## `public LEditor LTaxonomyEditor { get; }`

The entry editor's deportment on the membership side, which takes the membership vista when the panel's vistas are restored.

## `public CPanel LTaxonomyPanel { get; }`

The shared panel state over the membership vista: the chosen entry, the scribe mode, the bin and the leave guard.

## `public void LTaxonomyEntryCreate()`

Opens a blank entry in the editor.
When a Tag is chosen, the new entry starts inside that Tag.

## `public IReadOnlyList<CVistaRow> LTaxonomyMembershipRead()`

The entries under the chosen Tag, or every entry while none is chosen, as the engine narrows them.
Each row is handed out as its Conduct copy, chosen flag included.

## `public bool LTaxonomyCoinageCheck()`

Whether New should name a new tag rather than start an entry.
That holds while no Tag is chosen and no entry is shown.
So the emptier panel gets what it would list.
Both drivers ask this one question, so neither decides it on its own.

## `public string LTaxonomyEmptyRead(string query)`

The localization key for the empty entry list, given the text in the entry search box.
A search in the entry box says nothing matched, and an empty box says the Tag holds no entry yet.

## `internal static IReadOnlyList<CCatalogTag> LTaxonomyTagRead(IReadOnlyList<LCatalogTag> rows)`

The Conduct copies of the tag rows the engine listed, chosen flag included.

## `public long LTaxonomyTagCreate(string name)`

Makes the tag and answers only its id, which is all the view browses by.

## `public void LTaxonomyObserverAttach(CSubject subject, Action<CBulletin> observer)`

Attaches a view's observer to the tag vista for one subject.
Each engine notice is copied into its Conduct shape before the observer sees it.

## `internal void LTaxonomyVistaRestore(LVista vista, LVista membership)`

Puts the controller on the vistas the window started for this tab.
It is internal, since only the window hands vistas over.
