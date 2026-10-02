# TTimbreReflex.cs
Hash: `c4e2ecaefd58876c`

## `public sealed class TTimbreReflex`

Covers the editor's reflex block read and its lookup start, over a real desk and ports a test may fake.
The block carries every row, the anchor labels, the shared fold and the fetching line.
An empty desk, a fresh draft or a refused anchor answers no anchor.
Opening a stored entry without reflexes starts the lookup, and a reflected or fresh draft starts none.
The write gates add a row below the pressed one with its language and kind, or a blank row last.
They remove a row, flip its main mark, and write each typed cell.
A typed language answers every row's lead with the typed language standing in for the stored one.
A typed text goes to the respelling when the row's language respells, and to the phonetic text otherwise.

## `public void TimbreReflexRead_StoredEntry_AnswersEveryRowWithItsAnchorAndTheFold()`

A stored entry answers every row with its anchor label, and the fold the display shares.
The anchor check is asked once for the whole block and the format once per row.

## `public void TimbreReflexRead_EmptyDesk_AnswersNoRowsAndNoAnchor()`

An empty desk answers no rows and offers no anchor.

## `public void TimbreReflexRead_FreshDraft_OffersNoAnchor()`

A fresh draft has no stored entry, so it answers no rows and offers no anchor.

## `public void TimbreReflexRead_RefusedAnchor_AnswersTheRowsUnanchored()`

A refused anchor format leaves the rows in place and answers the whole block unanchored.

## `public async Task TimbreReflexStart_StoredEntryWithoutReflexes_StartsTheLookupWhenTheDraftOpens()`

Opening a stored entry that holds no reflexes starts the lookup once the draft is prepared.

## `public void TimbreReflexStart_ReflectedOrFreshDraft_StartsNothing()`

A draft that already holds reflexes, and a fresh draft, start no lookup.

## `public void TimbreReflexAdd_PressedRow_PlacesARowOfItsLanguageAndKindBelowIt()`

The new row copies the pressed row's language and kind, and lands directly below it.

## `public void TimbreReflexAdd_NoOrGoneRow_AppendsABlankRow()`

Zero or a row the draft no longer holds appends a blank row last.

## `public void TimbreReflexRemove_HeldRow_DropsIt()`

A held row goes, and the rows around it stay.

## `public void TimbreReflexToggle_HeldRow_FlipsItsMainEachTime()`

Each press flips the held row's main mark.

## `public void TimbreReflexSet_TypedLanguage_WritesItAndLeadsByTheOverlaidRows()`

A typed language is written and the leads answer the rows with the typed language standing in.
The answer names the typed language as the text the cell now holds.
A desk holding no entry takes nothing and answers an empty cell.

## `public void TimbreReflexSet_TextOfARespelledRow_WritesTheRespelling()`

Text typed into a respelled row is written as its respelling.

## `public void TimbreReflexSet_TextOfAPlainRow_WritesThePhoneticText()`

Text typed into a plain row is written as its phonetic text.

## `public void TimbreReflexSet_OtherCells_WriteEachAndAnswerNoLeads()`

Kind, romanization, meaning and note are each written, and none answers a lead.
Each answer names its own cell and the typed text.

## `public void TimbreReflexSet_WhileTheDeskFills_TakesNothingAndAnswersTheHeldText()`

An edit sent while the desk fills its view is refused.
The answer then names the text the held block shows, so the driver's cell drops the refused text.

## Inline notes

### `private static long TTimbreReflexSave(LEngine engine)`

Saves a Chinese entry with three reflex rows.

### `private static IReadOnlyList<CReflex> TTimbreReflexRead(CEditor editor)`

Reads the block's rows after the desk persists the draft.

### `private static CEditor TTimbreReflexPrepare(LEngine engine, long? entry)`

Builds an editor over the library vista and opens the entry.

### `private static long TTimbreReflexSave(LEngine engine, string headword, string language, IReadOnlyList<LReflexDraft> reflexes)`

Saves an entry with the reflex rows the test names.

### `private static LPhonologyPort TTimbreGuisePrepare(bool respelled = false)`

Fakes the guise answer so every row is respelled or plain as the test needs.
