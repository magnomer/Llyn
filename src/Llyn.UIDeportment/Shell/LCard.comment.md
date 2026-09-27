# LCard.cs

## `public sealed class LCard`

The deportment of the editor's cards: what a card's fields ask the engine while the user types into them.
The translation field finds prospects, resolves a typed word and opens or drops a court link.
The register, situation, tag and source fields find the stored rows a typed word matches.
The sentence menu reads the frame, particles and dependences of a language.
The etymology field sends its narrative, its source links and its spans through it, over the editor's desk.
It holds no state, since the card drag and the chip picks stay in the veneer until their own plan.
The editor deportment owns one and hands it to the card views beside the desk.

## `internal LCard(CDesk desk, LDraftPort drafts, LEntryPort entries, LPhonologyPort phonology)`

Takes the editor's desk, so an etymology request lands on the held draft.
Only the editor builds one, so the constructor is internal.

## `public void LCardCitationSet(long cardId, long sentenceId, string title)`

Points the sentence's citation at the Reference the engine resolves the typed title to.
The engine reads the Reference cited now off the held draft, so an unchanged title keeps it.

## `public void LCardEtymologySet(string text)`

Defers the narrative like every typed field, so one keystroke is not one request.

## `public void LCardEtymonAdd(long entryId)`

Appends a source link at the end of the row.

## `public void LCardEtymonRemove(long entryId)`

Drops one source link.

## `public void LCardMentionSave(string text, int start, int length, long entryId)`

Links a selection of the narrative to an entry, the span measured by the engine.

## `public void LCardMentionDelete(string text, int start, int length)`

Drops the span the selection lies inside, by naming no entry over its whole length.
A selection inside no span sends nothing.

## `public bool LCardMentionCheck(string text, int start, int length)`

Whether the selection lies inside a span of the held narrative.

## `private LEtymologyDraft LCardEtymologyRead()`

The held draft's etymology, or an empty one while nothing is held.

## `private void LCardMentionSend(LMentionDraft? span, long entryId)`

Sends one span request, or nothing when there is no span.

## `public void LCardCourtDelete(long ownerId, long targetId)`

Drops the court link between the held draft and a target, and the target's fresh draft with it.
One user action is one gate, so the translation field no longer finds the link first.

## `internal static CSentenceOrder LCardOrderRead(LSentenceOrder order)`

The one map from a language's sentence order to its shape, shared with the window.

## `internal static IReadOnlyList<CTranslationTarget> LCardTargetRead(IReadOnlyList<LTranslationTarget> targets)`

The one map from link targets to their shape, shared with the editor and the lectern.

## `internal static CStateValue LCardStateRead(LStateValue value)`

The one map from a written value to its shape.
It carries the engine's shown text and verdicts, so no driver judges a state.

## `internal static CEntryDraft LCardEntryRead(LEntryDraft draft)`

Shapes the held entry for the editor view.
Every list goes through the splice builder, so no engine answer sits in a local.

## `internal static IReadOnlyList<CCardDraft> LCardSheetRead(IReadOnlyList<LCardDraft> cards)`

Shapes the meaning or collocation cards with everything the card parts read.

## `private static CExampleDraft? LCardExampleRead(LExampleDraft? example)`

The example of a sentence, or null when the sentence holds none.

## `internal static IReadOnlyList<CGlossDraft> LCardGlossRead(IReadOnlyList<LGlossDraft> glosses)`

The one map for glosses, shared with the corpus and the display.

## `internal static IReadOnlyList<CMentionDraft> LCardMentionRead(IReadOnlyList<LMentionDraft> mentions)`

The one map for a list of Mentions.

## `internal static IReadOnlyList<CImageDraft> LCardImageRead(IReadOnlyList<LImageDraft> images)`

The one map for images, shared with the repertoire and the media host.

## `internal static IReadOnlyList<CVideoDraft> LCardVideoRead(IReadOnlyList<LVideoDraft> videos)`

The one map for videos, shared with the repertoire and the media host.

## `internal static CMentionDraft LCardMentionRead(LMentionDraft mention)`

The one map for a single Mention, which the desk and the corpus also call.
