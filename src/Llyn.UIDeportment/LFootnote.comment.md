# LFootnote.cs

## `public sealed class LFootnote`

The deportment of the sources panel's entry list: the shared panel state over the footnote vista and its rows.
The rows are the entries citing the chosen Source, or every entry while none is chosen.
It keeps a handle on the parent vista too, because the engine narrows the rows by the parent's choice.
The panel state is held rather than inherited, because a shell type deriving from logic is a custody hit.
Its delete seam always refuses, because an entry is never deleted from this list.

## `public event Action<long>? LFootnoteCreated;`

A fresh entry was started under the chosen Source, named by that Source's id.
The veneer hands the id to the editor as a citation, so the new entry starts already citing it.

## `public LEditor LFootnoteEditor { get; }`

The entry editor's deportment: its desk answers the leave and it opens the edited row.
A source created here is cited into it.

## `public string LFootnoteEmptyKey`

The key of the empty text: unmatched while the list is being searched, else vacant.

## `public void LFootnoteEntryCreate()`

The fresh step over this list, asking nothing, for a deportment that already asked for the whole tab.
The chosen Source is announced after the editor is open, so the citation lands in the new draft.
