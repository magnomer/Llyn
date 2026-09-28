# CFolio.cs

## `internal static class CFolio`

The one map from an engine entry draft to the shape every driver shows.
It is split from `CCard` by role, since the gates and the maps change for different reasons.
The internal maps are still called by the deportment classes that later jobs dismantle.

## `internal static CSentenceOrder CFolioOrderRead(LSentenceOrder order)`

The one map from a language's sentence order to its shape, shared with the display.

## `internal static IReadOnlyList<CTranslationTarget> CFolioTargetRead(IReadOnlyList<LTranslationTarget> targets)`

The one map from link targets to their shape, shared with the editor and the lectern.

## `internal static CEntryDraft CFolioEntryRead(LEntryDraft draft)`

Shapes the held entry for the editor view.
The first pronunciation is the primary one, and the rest are its accents.

## `internal static CStateValue CFolioStateRead(LStateValue value)`

The one map from a written value to its shape.
It carries the engine's shown text and verdicts, so no driver judges a state.
The panels and the display that later jobs dismantle still call it.

## `internal static IReadOnlyList<CCardDraft> CFolioSheetRead(IReadOnlyList<LCardDraft> cards)`

Shapes the meaning or collocation cards with everything the card parts read.
A card's situation link carries no media, so its picture and video lists stay empty.
The display's compass reads the card titles and numbers through it, so it never reads an engine card.

## `private static CExampleDraft? CFolioExampleRead(LExampleDraft? example)`

The example of a sentence, or null when the sentence holds none.

## `internal static IReadOnlyList<CGlossDraft> CFolioGlossRead(IReadOnlyList<LGlossDraft> glosses)`

The one map for glosses, shared with the display's leaf.

## `internal static IReadOnlyList<CImageDraft> CFolioImageRead(IReadOnlyList<LImageDraft> images)`

The one map for images, shared with the repertoire and the media host.

## `internal static IReadOnlyList<CVideoDraft> CFolioVideoRead(IReadOnlyList<LVideoDraft> videos)`

The one map for videos, shared with the repertoire and the media host.

## `internal static CMentionDraft CFolioMentionRead(LMentionDraft mention)`

The one map for a single Mention, which the corpus also calls.
