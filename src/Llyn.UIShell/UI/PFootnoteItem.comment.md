# PFootnoteItem.cs

## `internal sealed class PFootnoteItem`

Presentation item for one citing row in `PFootnote`.
It carries the panel it leads to rather than an entry it belongs to.
An Example citing this Source is independent data any number of Entries may quote.
It has no one entry to name.
`LUsage` is reused without a second identifier, because its entry field already means the id the row is followed by.
For a card row that is the Entry holding it, and for an Example row it is the Example itself.

## `internal PFootnoteItem(LUsage usage, string owner, string unreadable, string unnamed)`

Builds the row from the stored usage and the word for the kind of side it is.
An Example whose sentence is unwritten reads the unnamed mark, so no row stands blank beside the next.

## `public LOwner PFootnoteItemOwner { get; }`

Which panel the row leads to.
A Meaning or a Collocation leads to the library panel and an Example to the corpus panel.
