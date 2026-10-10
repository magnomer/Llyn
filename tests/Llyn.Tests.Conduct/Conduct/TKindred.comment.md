# TKindred.cs
Hash: `d1b75d1196aef62b`

## `public sealed class TKindred`

Covers the editor's reflex block read and its lookup start, over a real desk and ports a test may fake.
The block carries every row, the anchor labels, the held entry's stored opening and the fetching line.
An empty desk, a fresh draft or a refused anchor answers no anchor.
Opening a stored entry without reflexes starts the lookup, and a reflected or fresh draft starts none.
The rows show in the declared order the entry load gives them, here alphabetical since the language has no pack.
The write gates add a row below the pressed one with its language and kind, or a blank row last.
They remove a row, flip its main mark, and write each typed cell.
A typed language answers every row's lead with the typed language standing in for the stored one.
A typed text goes to the respelling when the row's language respells, and to the phonetic text otherwise.

## `public void KindredRead_StoredEntry_AnswersEveryRowWithItsAnchorAndTheFold()`

A stored entry answers every row with its anchor label, and the opening the engine stores for it.
The anchor check is asked once for the whole block and the format once per row.

## `public void KindredRead_EmptyDesk_AnswersNoRowsAndNoAnchor()`

An empty desk answers no rows and offers no anchor.

## `public void KindredRead_FreshDraft_OffersNoAnchor()`

A fresh draft has no stored entry, so it answers no rows and offers no anchor.

## `public void KindredRead_RefusedAnchor_AnswersTheRowsUnanchored()`

A refused anchor format leaves the rows in place and answers the whole block unanchored.

## `public async Task KindredStart_StoredEntryWithoutReflexes_StartsTheLookupWhenTheDraftOpens()`

Opening a stored entry that holds no reflexes starts the lookup once the draft is prepared.

## `public void KindredStart_ReflectedOrFreshDraft_StartsNothing()`

A draft that already holds reflexes, and a fresh draft, start no lookup.

## `public void KindredAdd_PressedRow_PlacesARowOfItsLanguageAndKindBelowIt()`

The new row copies the pressed row's language and kind, and lands directly below it inside its group.
The test picks the pressed row by its text, so the declared order never changes which row is pressed.
The gate finds that row by its id, so the kind it copies is the Go-on of that row.

## `public void KindredAdd_NoOrGoneRow_AppendsABlankRow()`

Zero or a row the draft no longer holds appends a blank row last.

## `public void KindredRemove_HeldRow_DropsIt()`

A held row goes by its id, and the rows around it stay in their shown order.

## `public void KindredToggle_HeldRow_FlipsItsMainEachTime()`

Each press flips the held row's main mark.

## `public void KindredSet_TypedLanguage_WritesItAndLeadsByTheOverlaidRows()`

A typed language is written and the leads answer the rows with the typed language standing in.
Leads follow the shown order, so a run of one language prints its name once.
The answered row holds the typed language, with the key it is labelled under.
A desk holding no entry takes nothing and answers no row.

## `public void KindredSet_TextOfARespelledRow_WritesTheRespelling()`

Text typed into a respelled row is written as its respelling, and the answered row shows it.
The phonetic text of that row stays as stored.

## `public void KindredSet_TextOfAPlainRow_WritesThePhoneticText()`

Text typed into a plain row is written as its phonetic text, and the answered row shows it.

## `public void KindredSet_OtherCells_WriteEachAndAnswerNoLeads()`

Kind, romanization, meaning and note are each written, and none answers a lead.
Each answer is the typed row, holding the typed text in its own cell.

## `public void KindredSet_WhileTheDeskFills_TakesNothingAndAnswersTheHeldText()`

An edit sent while the desk fills its view is refused.
The answer is then the row as the draft holds it, so the driver's cell drops the refused text.

## Inline notes

### `private static long TKindredSave(LEngine engine)`

Saves a Chinese entry with three reflex rows.

### `private static IReadOnlyList<CReflex> TKindredRowsRead(TEditorFixture editor)`

Reads the block's rows after the desk persists the draft.

### `private static TEditorFixture TKindredPrepare(LEngine engine, long? entry)`

Builds an editor over the library vista and opens the entry.
It answers the fixture, so a test reads only the facet it drives.

### `private static long TKindredSave(LEngine engine, string headword, string language, IReadOnlyList<LReflexDraft> reflexes)`

Saves an entry with the reflex rows the test names.

### `private static CPhonologyBundle TKindredGuisePrepare(bool respelled = false)`

Fakes the guise answer so every row is respelled or plain as the test needs.
