# CFolio.cs
Hash: `f0f520a06bf38b25`

## `internal static class CFolio`

The one map from an engine entry draft to the shape every driver shows.
It is split from `CCard` by role, since the gates and the maps change for different reasons.

## `internal static IReadOnlyList<CTranslationTarget> CFolioTargetRead(IReadOnlyList<LTranslationTarget> targets)`

The one map from link targets to their shape, shared with the editor and the lectern.

## `internal static CEntryDraft CFolioEntryRead(LEntryDraft draft, IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> targets, LMediaPort media)`

Shapes the held entry for the editor view.
The link targets come keyed by card from the tenure, so each card carries its links ready.
The image addresses come through `media`, so each picture row carries its address ready.

## `internal static CStateValue CFolioStateRead(LStateValue value)`

The one map from a written value to its shape.
It carries the engine's plain text and verdicts, so no driver judges a state.
The empty text for a value with nothing legible is the engine's own, so the map keeps no fallback.

## `private static IReadOnlyList<CCardDraft> CFolioSheetRead(IReadOnlyList<LCardDraft> cards, IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> targets, LMediaPort media, string meaning)`

Shapes the meaning or collocation cards with everything the card parts read.
A card's situation link carries no media, so its picture and video lists stay empty.
A card's links are its own entry of the tenure's target map, mapped by the one target map.
The tenure answers an entry for every card, so a missing one is a fault and throws.
The title, expression and meaning arrive worded, and `meaning` names the meaning field's hint for the sheet.
The cards come in the order `CFolioOrderRead` sets, so a driver's list index is a Conduct place.

## `internal static int? CFolioPlaceRead(LEntryDraft content, long cardId, int place)`

Turns a place in the card list Conduct handed out into the engine's index for the same card.
A place is the index of a card in `CEntryDraftMeanings` or `CEntryDraftCollocations` of the last read.
It reads the list holding `cardId` in the order `CFolioOrderRead` sets, so it matches the handed list.
It answers null for a place outside that list, so a stale place sends nothing.

## `internal static int? CFolioOrdinalRead(LEntryDraft content, long cardId, string ordinal)`

Turns the number typed in a card's badge into a place in the handed list.
The number counts from one, and a number past either end is pulled to that end.
Text that is not a whole number answers null, so it moves nothing.
It answers a place, not an engine index, so the typed move goes through `CFolioPlaceRead` like a drag.
So the badge number and the shown order agree even when the engine list is not in position order.

## `internal static IReadOnlyList<LCardDraft> CFolioOrderRead(IReadOnlyList<LCardDraft> cards)`

The one order of a card list, by the engine's position, with ties kept in the engine's order.
Every Conduct path that numbers or finds a card reads it, so all of them agree.
Those are the sheet map, the place map, the reading view's cards and the contents.
It is internal so the display reads the same order rather than a copy of the sort.

## `private static IReadOnlyList<LCardDraft> CFolioListRead(LEntryDraft content, long cardId)`

The list that holds `cardId`, the collocations when no meaning holds it.
The place map and the ordinal map share it, so both read the same list.

## `private static CSentenceDraft CFolioSentenceRead(LSentenceDraft sentence)`

Shapes one sentence row with its example and its worded text, marker and role.
A row holding no example yet words the empty text, so its field shows the example hint.

## `private static CExampleDraft? CFolioExampleRead(LExampleDraft? example)`

The example of a sentence, or null when the sentence holds none.

## `internal static IReadOnlyList<CGlossDraft> CFolioGlossRead(IReadOnlyList<LGlossDraft> glosses)`

The one map for glosses, shared with the reading view's sentence rows.

## `internal static IReadOnlyList<CImageDraft> CFolioImageRead(IReadOnlyList<LImageDraft> images, LMediaPort media)`

The one map for images, shared with the repertoire and the reading view's cards.
It carries the engine's empty verdict, so a reading card folds an image nobody located.
It carries the address `media` resolves from the location, so a row loads its preview without asking.
The engine settles what may be reached, and the map only hands it the plain text.

## `internal static IReadOnlyList<CVideoDraft> CFolioVideoRead(IReadOnlyList<LVideoDraft> videos, LMediaPort media)`

The one map for videos, shared with the repertoire and the reading view's cards.
It carries the engine's empty verdict, so a reading card folds a video nobody located.
It carries the screen `media` resolves from the location, so a row plays without asking.
The engine settles the address and the film id, and the map only pairs them.
It carries the moments the engine reads from the span, so the screen parses no timestamp.
