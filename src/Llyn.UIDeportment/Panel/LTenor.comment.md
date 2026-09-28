# LTenor.cs

## `public sealed class LTenor`

The tenor panel's deportment: the register vista and the cohort vista it holds, and what the panel asks of them.
The register side finds rows, takes the query, order and kind filter, and creates a Register.
The cohort side finds the entries of the chosen Register, loads and deletes the chosen one, and marks it edited.
Both vistas are handed in by the window, which restores every tab's vistas together.
The panel's loads and clears go straight to the lectern its view hands in, so the veneer relays no draft.
Its public members name only .NET and Conduct types, and the window alone builds it.

## `public LEditor LTenorEditor { get; }`

The entry editor's deportment on the cohort side, which takes the cohort vista when the panel's vistas are restored.

## `public CPanel LTenorPanel { get; }`

The shared panel state over the cohort vista: the chosen entry, the scribe mode, the bin and the leave guard.

## `public void LTenorEntryCreate()`

Opens a blank entry in the editor.
When a Register is chosen, the new entry starts inside that Register.

## `public IReadOnlyList<CVistaRow> LTenorCohortRead()`

The entries under the chosen Register, or every entry while none is chosen, as the engine narrows them.
Each row is handed out as its Conduct copy, chosen flag included.

## `public bool LTenorCoinageCheck()`

Whether New should name a new register rather than start an entry.
That holds while no Register is chosen and no entry is shown.
So the emptier panel gets what it would list.
Both drivers ask this one question, so neither decides it on its own.

## `public string LTenorEmptyRead(string query)`

The localization key for the empty entry list, given the text in the entry search box.
A search in the entry box says nothing matched, and an empty box says the Register holds no entry yet.

## `internal static IReadOnlyList<CCatalogRegister> LTenorRegisterRead(IReadOnlyList<LCatalogRegister> rows)`

The Conduct copies of the register rows the engine listed, chosen flag included.

## `public long LTenorRegisterCreate(string name)`

Makes the register and answers only its id, which is all the view browses by.

## `public void LTenorObserverAttach(CSubject subject, Action<CBulletin> observer)`

Attaches a view's observer to the register vista for one subject.
Each engine notice is copied into its Conduct shape before the observer sees it.

## `internal void LTenorVistaRestore(LVista vista, LVista cohort)`

Puts the controller on the vistas the window started for this tab.
It is internal, since only the window hands vistas over.
