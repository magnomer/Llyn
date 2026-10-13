using Llyn.Core;

namespace Llyn.Tests;

internal static class TInterfaceMentionSpan
{
    internal static LMention TMentionCreate(long id, int start, int length, long entryId, long senseId = 0) =>
        new(id, start, length, entryId, senseId);

    internal static (int LMentionSpanOffset, int LMentionSpanLength) TMentionSpanResolve(
        string text, int offset, bool separated) =>
        LMentionSpan.LMentionSpanResolve(text, offset, separated);

    internal static IReadOnlyList<LMentionPiece> TMentionSpanDivide(string text, IReadOnlyList<LMention> mentions) =>
        LMentionSpan.LMentionSpanDivide(text, mentions);

    internal static int TMentionOffsetRead(string text, int unit) =>
        LMentionSpan.LMentionOffsetRead(text, unit);

    internal static LMentionDraft TMentionSpanRead(string text, int start, int length) =>
        LMentionSpan.LMentionSpanRead(text, start, length);

    internal static LMentionDraft? TEtymologyDraftFind(LEtymologyDraft etymology, LMentionDraft span) =>
        etymology.LEtymologyDraftFind(span);

    internal static LMentionDraft? TExampleDraftFind(LExampleDraft example, LMentionDraft span) =>
        example.LExampleDraftFind(span);

    internal static LExampleDraft? TDraftExampleRead(LDraft draft, long cardId, long sentenceId) =>
        draft.LDraftExampleRead(cardId, sentenceId);

    internal static int TMentionUnitRead(string text, int offset) =>
        LMentionSpan.LMentionUnitRead(text, offset);

    internal static LMentionDraft TMentionDraftCreate(long id, int start, int length, long entryId, long senseId = 0) =>
        new(id, start, length, entryId, senseId);

    internal static LMention TMentionDraftResolve(LMentionDraft draft) =>
        draft.LMentionDraftResolve();
}
