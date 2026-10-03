# TRequest.cs
Hash: `50746c3c8c590192`

## `public sealed class TRequest`

Covers the requests a form sends to edit a held draft, and what the engine answers.
Also covers a repeated text that writes and raises nothing.
Also covers the panel Situation's title, description and kind.

## `public void RequestApply_HeadwordText_PersistsInDraftFile()`

A headword request lands in the file, so a re-read sees it without the returned draft.

## `public void RequestApply_CardAddition_MintsNegativeIdAndRaisesBulletin()`

An added card comes back named with a minted id and numbered one.
One draft bulletin is raised for it, naming the draft.

## `public void RequestApply_CardShift_ReordersAndRenumbersCards()`

A shift lands the card where asked and the whole list reads 1, 2, 3 again.

## `public void RequestApply_CardRemoval_DropsCardAndRenumbersRest()`

A removal drops the one card named and closes the gap in the numbering.

## `public void RequestApply_CardTitle_ChangesOnlyThatCard()`

A field request changes the card it names and leaves its neighbour as it was.

## `public void RequestApply_UnknownCard_Refuses()`

A card the draft does not hold is refused with the card reason.

## `public void RequestApply_ZeroCardId_Refuses()`

Zero names nothing in a draft, so a request carrying it is refused before anything is searched.

## `public void RequestApply_CollocationParent_Refuses()`

A collocation cannot hold a card, so an addition naming one as parent is refused with the nesting reason.

## `public void RequestApply_DraftNotHeld_Refuses()`

A request for a draft this engine does not hold is refused with the draft reason.

## `public void RequestApply_HeadwordFreshAudio_Clears()`

A recording fetched on a draft started on nothing is audio of one spelling, and a new spelling drops it.

## `public void RequestApply_HeadwordStoredAudio_Stays()`

A recording that came with the stored entry is the entry's own, so a corrected headword keeps it.

## `public void RequestApply_HeadwordReplacedAudio_Clears()`

A recording fetched over the stored one differs from the entry's, so it goes like any fresh one.

## `public void RequestApply_LanguageFreshAudio_Clears()`

A language sent again as it stands drops nothing, and a new language drops the fetched recording.

## `public void RequestApply_LanguageStoredAudio_Clears()`

A stored recording is audio in the old language, so a new language drops it like a fetched one.

## `private static string TRequestRecordingSave(TWorkspace workspace)`

Writes one byte of audio under the workspace, so a saved entry resolves it back to the same path.

## `public void RequestApply_UnchangedText_WritesNothingAndRaisesNothing()`

A text sent again exactly as the Example holds it changes nothing.
The draft version stays and no bulletin is raised, so a repeated send does not wake every surface.

## `public void RequestApply_SituationFields_ChangesThePanelSituation()`

Title, description and kind requests each change the situation the panel holds.
The situation keeps its id throughout, so the panel never swaps to another record.
