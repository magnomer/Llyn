# PSentence.cs

## `internal sealed partial class PSentence`

One Example row on a card.
The row carries the id of the Example it edits, and the id of the row itself.
So a sentence the user rewrites stays the same Example.
Both ids are the engine's, read from the draft, and the row never mints one.
A row that has never been written shows an empty Example id until the engine names one.

A row standing empty is not one thing.
The sentence may never have been written.
Or it may have been written and be unknown now.
The second is marked rather than shown, and the converter reads the mark off the engine's value.
The row holds that value and nothing typed.
What is typed leaves through the editor as written text, and the draft answers with the state.
So the row never resolves a state and never holds a second copy of one.

The row's Mentions and the chip line that shows them live in `PSentenceMention.cs`.

The row also carries the frame the card reads the sentence under, a marker and a role.
Nothing offers a value for either, because nothing ships one.
Which of the two is written first is the language pack's to say and never the row's.
The row is told the two places and puts each field where it was told.

Each of the two frame fields offers what has already been saved for the language.
Nothing is shipped, so an empty store offers nothing.
The field is a plain box until something is written in it.

## `internal PSentence(`

The row for one of the engine's rows, blank or filled.
It holds the sentence, the Source it cites, the frame, and the Gloss rows as the draft holds them.
All of it stands under the ids that name it.
The language catalog is handed in for the Gloss pickers, shared with every other row.

## `internal long PSentenceRow`

The id of the row itself, which every request about the row names.

## `public ObservableCollection<PCitationItem> PSentenceCitationCatalog { get; }`

The Sources the whole form offers, shared by every row.
So a Source written on one row is on offer to the next without reloading anything.

## `public ObservableCollection<string> PSentenceParticleCatalog { get; }`

The markers already saved for the language, shared by every row.
Nothing ships one, so the list is empty until a user writes and saves one.

## `public ObservableCollection<string> PSentenceDependenceCatalog { get; }`

The roles already saved for the language, shared by every row.
Nothing ships one, so the list is empty until a user writes and saves one.

## `internal void PSentenceOrderApply(LSentenceOrder order)`

Puts the marker and the role in the places the language pack states.
The row states no order of its own, so it holds only what it was told.

## `public LStateValue PSentenceText`

The sentence as the draft holds it, set only from the draft.
The same holds for the marker and the role.

## `public LStateAnchor PSentenceCitation`

The Source the row cites, as the draft holds it.
The field shows its byline through a lookup made when the field is drawn, so the row keeps no name.

## `internal void PSentenceShow(LSentenceDraft draft)`

Redraws the row from the engine's row, field by field, only where the value changed.
The Mentions are always taken, since the row only shows them and never types into them.
The ids are always taken, because the engine is the only minter.
A field already reading what the engine holds is left alone, so the caret survives its own echo.

## `internal void PSentenceCitationShow()`

Says the cited Source again, for when the list of Sources changed underneath the row.
The field then looks the byline up afresh.

## `internal static string PSentenceCitationFind(ObservableCollection<PCitationItem> catalog, LStateAnchor anchor)`

The `Author (Year)` line of the Source an anchor names, or nothing when it names none.
The citation converter calls it when a field is drawn, and the editor when it compares a typed line.
The row fill and the citation reset call it too, where a multi-binding read it before.

## `internal static void PSentenceRowApply(FrameworkElement container, PSentence row)`

Writes every part of a sentence row from the row, where bindings and data triggers stood.
The frame switch, its sign, the frame and its gap follow whether the frame is open or written.
Each frame field takes the column the row was told to take.
The sentence and citation texts go through the same lookups the converters made.
The three icons are set here, since an icon is drawn by code.

## `private static void PSentenceChoiceApply(`

Fills one frame field: the dropdown's offers, text and hint, the ghost copy and the hint shown when empty.

## `private static void PSentenceLayoutApply(FrameworkElement container, TextBox text)`

Binds the row's layout once, where element bindings stood in the markup.
The sentence column and the citation drop to the frame's baseline through the font converter.
The citation takes the sentence's font, and each dropdown the size of its ghost copy.
A row already bound is left alone, so a refill binds nothing twice.

## `public bool PSentenceFrameVisible`

Whether the row shows its frame at all.
A row that carries a marker or a role always shows one, because the reading view draws it.
A row carrying neither shows none, for the same reason, and the card is asked for one instead.

## `public bool PSentenceFrameWritten`

Whether the frame stands on what the row holds rather than on the card being asked for one.
The card's switch stands down while it does, because a frame already written cannot be opened or closed by asking.
It gives its room back rather than keeping it.
The handles beside it are not read across a gap that holds nothing.

## `public string PSentenceFrameGap`

The separator drawn between the marker and the role inside the frame.
It is one space when both are written and nothing otherwise.
The writing view spaces the frame exactly as the reading view does.

## `private static bool PSentenceFrameCheck(LStateValue value)`

Whether a frame field says anything: text, or the unknown mark.
