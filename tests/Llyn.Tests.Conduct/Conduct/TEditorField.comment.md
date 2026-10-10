# TEditorField.cs
Hash: `262b64eb0a2d9323`

## `public sealed class TEditorField`

Covers the editor's typed field gates over an input editor on a real workspace, with no delay.
A card's title, expression and definition land as typed once the deferred requests are flushed.
A card's links read back in the card's order.
Stored link targets retain their resolved headwords and card order.
A card without links reads none.
An empty desk reads nothing.
An image's location, a video's location and span, and a gloss's text land as typed.
A pressed gloss button appends a gloss in the engine's gloss language, after the glosses held.
A removed gloss leaves, and a picked language retags the gloss that stays.
A gloss with no language carries Conduct's hint key, and a named one carries none.
The close gate lets the held draft and every search go.
An editor closed and then reopened on an entry hears a box stored in another editor again, once.
The editor reads its draft's language, and an opened workspace opens a fresh draft in it.
The observers the editor attaches show the draft through the marshal the driver hands.

## `internal static TEditorFixture TEditorFieldPrepare(LEngine engine)`

Builds an input editor whose marshal runs each action at once.

## `internal static TEditorFixture TEditorFieldPrepare(LEngine engine, Action<Action> marshal)`

Builds an input editor with no delay and the given marshal, and opens a fresh draft in it.
It answers the fixture, so a test reads only the facets it needs.

## `internal static long TEditorSheetAdd(TEditorFixture editor)`

Appends a Meaning card to the held draft and answers its id.

## `internal static CCardDraft TEditorCardRead(TEditorFixture editor, long sheet)`

The card as the editor's draft read shows it.

## `private static long TEditorSentenceAdd(TEditorFixture editor, long sheet)`

Gives the card one written sentence and answers its id, since a gloss needs the sentence's example.

## `private static IReadOnlyList<CGlossDraft> TEditorGlossRead(TEditorFixture editor, long sheet)`

The glosses of the card's first sentence, as the editor's draft read shows them.

## `public void CardFieldSet_TypedTexts_WritesTitleExpressionAndDefinition()`

Deferred field requests persist title, expression and definition after flushing.

## `public void DraftRead_LinkedEntries_CarriesThemOnTheCardInItsOrder()`

Resolved stored targets retain their headwords in card order.

## `public void DraftChanged_LinkedEntry_CarriesTheLinkOnItsCard()`

The changed-draft notification carries the resolved linked target.

## `public void DraftRead_EmptyDesk_ReadsNothing()`

An empty desk has no mapped entry draft.

## `public void ImageLocationSet_TypedLocation_WritesTheRow()`

The typed image location persists on its row.

## `public void VideoSet_TypedLocationAndSpan_WritesTheRow()`

The typed video location and span persist on their row.

## `public void GlossSet_TypedText_WritesTheGloss()`

The typed gloss text persists on its gloss.

## `public void GlossAdd_PressedRow_AppendsAGlossInTheGlossLanguage()`

Adding a gloss appends it with the configured gloss language.

## `public void GlossRead_BlankAndNamedLanguage_CarriesTheHintOnlyForTheBlankOne()`

Only a gloss with blank language carries the language hint.

## `public void GlossRemoveAndLanguageSet_PickedGloss_DropsOneAndRetagsTheOther()`

Removal drops the selected gloss, and language selection retags the survivor.

## `public async Task EditorClose_HeldDraftAndSearch_LetsBothGo()`

Closing releases the held draft and running searches.

## `public void EditorEntryOpen_ClosedThenReopened_HearsTheBoxFoldAgain()`

A reopened editor receives one box-fold refresh from another editor holding the same entry.

## `public void EditorLanguage_HeldDraft_ReadsTheDraftsLanguage()`

The editor reports its held draft language.

## `public void EditorCreate_WorkspaceOpened_OpensAFreshDraft()`

An opened workspace gives the new input editor a fresh draft.

## `public void ObserverAttach_DraftEdited_ShowsTheDraftThroughTheMarshal()`

Draft notifications use the supplied marshal.
