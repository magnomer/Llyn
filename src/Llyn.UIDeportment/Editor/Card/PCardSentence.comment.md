# PCardSentence.cs
Hash: `ae3ddd5aef584c1f`

## `internal sealed class PCardSentence`

The Example rows one card shows.
The engine holds the rows, and this list renders them by id and reports what the user does to them.
A row belongs to the card it was written under and moves with it.
It knows no card, so its owner is named by whoever subscribes to its notice.

## `internal PCardSentence(ObservableCollection<QCitationItem> catalog, ObservableCollection<string> particles, ObservableCollection<string> dependences, ObservableCollection<PLanguageItem> languages)`

The catalog, particle, dependence and language lists are the editor's own, shared by every card.
Each row reads them, so one write reaches every card at once.

## `public ObservableCollection<PSentence> PCardSentenceRow { get; }`

The rows in the engine's order, which the card's sentence list draws.

## `internal event Action<PSentence, PGloss, string>? PCardSentenceNotice;`

Where a language picked for a Gloss goes, with its sentence row and the raw language.
The editor subscribes when it builds the card, closing over that card, and hands the pick to one gate.
The list cannot send, because it holds no engine.
Typed text never comes this way, since a row holds no copy of what is typed.

## `internal void PCardSentenceApply(CSentenceOrder order)`

Hands the language pack's field order to every row the card holds.
The card keeps no order of its own.
A row opened later is built under the order the editor hands it.

## `internal void PCardSentenceShow(IReadOnlyList<CSentenceDraft> drafts, CSentenceOrder? order)`

Makes the rows show the engine's rows, matched by sentence id.
A field already reading what the engine holds is left alone.
The order is the one `CSentenceFrameRead` last answered, and a new row is built under it.
The engine keeps a blank row, so a card always offers somewhere to write without the card adding one.

## `internal int PCardSentenceFind(PSentence row)`

Where the row stands, which is what an addition beneath it is asked at.

## Inline notes

### `private PSentence PCardSentenceCreate(CSentenceDraft draft, CSentenceOrder? order)`

Builds one row from the engine's row and subscribes its Gloss notice to raise the list's.
The frame's order is applied at birth, so a row never draws in the default order first.
No order yet means no frame was read, and the row's frame keeps its default order.
