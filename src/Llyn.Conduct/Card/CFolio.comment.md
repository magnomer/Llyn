# CFolio.cs

## `internal static class CFolio`

The one map from an engine entry draft to the shape every driver shows.
It is split from `CCard` by role, since the gates and the maps change for different reasons.

## `internal static IReadOnlyList<CTranslationTarget> CFolioTargetRead(IReadOnlyList<LTranslationTarget> targets)`

The one map from link targets to their shape, shared with the editor and the lectern.

## `internal static CEntryDraft CFolioEntryRead(`

Shapes the held entry for the editor view.
The first pronunciation is the primary one, and the rest are its accents.
The link targets come keyed by card from the tenure, so each card carries its links ready.
The image addresses come through `media`, so each picture row carries its address ready.

## `internal static CStateValue CFolioStateRead(LStateValue value)`

The one map from a written value to its shape.
It carries the engine's plain text and verdicts, so no driver judges a state.
The empty text for a value with nothing legible is the engine's own, so the map keeps no fallback.

## `private static IReadOnlyList<CCardDraft> CFolioSheetRead(`

Shapes the meaning or collocation cards with everything the card parts read.
A card's situation link carries no media, so its picture and video lists stay empty.
A card's links are its own entry of the tenure's target map, mapped by the one target map.
The tenure answers an entry for every card, so a missing one is a fault and throws.
The title, expression and meaning arrive worded, and `meaning` names the meaning field's hint for the sheet.

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

## `internal static IReadOnlyList<CVideoDraft> CFolioVideoRead(IReadOnlyList<LVideoDraft> videos)`

The one map for videos, shared with the repertoire and the reading view's cards.
It carries the engine's empty verdict, so a reading card folds a video nobody located.

## `internal static CMentionDraft CFolioMentionRead(LMentionDraft mention)`

The one map for a single Mention, which the corpus also calls.
