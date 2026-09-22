using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDraftClerkEtymology
{
    private readonly LEntryVault _lDraftClerkEntries;
    private readonly LIdentity _lDraftClerkIdentity;

    public LDraftClerkEtymology(LEntryVault entries, LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(identity);
        _lDraftClerkEntries = entries;
        _lDraftClerkIdentity = identity;
    }

    public LEntryDraft? LEtymologyApply(LEntryDraft content, LRequest request)
    {
        ArgumentNullException.ThrowIfNull(content);

        return request switch
        {
            LRequestEtymologyText sent => LEtymologyChange(
                content, etymology => LEtymologyTextApply(etymology, sent.LRequestText ?? string.Empty)),
            LRequestEtymologyMention sent => LEtymologyChange(content, etymology => LEtymologyMentionApply(
                etymology, sent.LRequestOffset, sent.LRequestLength, sent.LRequestEntryId)),
            LRequestEtymonAddition sent => LEtymologyChange(
                content, etymology => LEtymonAdd(etymology, sent.LRequestEntryId, sent.LRequestPosition)),
            LRequestEtymonRemoval sent => LEtymologyChange(
                content, etymology => LEtymonRemove(etymology, sent.LRequestEntryId)),
            LRequestEtymonShift sent => LEtymologyChange(
                content, etymology => LEtymonMove(etymology, sent.LRequestEntryId, sent.LRequestPosition)),
            _ => null,
        };
    }

    private static LEtymologyDraft LEtymologyTextApply(LEtymologyDraft etymology, string text)
    {
        return etymology with
        {
            LEtymologyDraftText = text,
            LEtymologyDraftMentions = LDraftClerkMention.LMentionUpdate(
                etymology.LEtymologyDraftText, text, etymology.LEtymologyDraftMentions),
        };
    }

    private LEtymologyDraft LEtymologyMentionApply(
        LEtymologyDraft etymology, int offset, int length, long entryId)
    {
        if (offset < 0 || length <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        int end = offset + length;
        if (end > LEtymologyLengthRead(etymology.LEtymologyDraftText))
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        List<LMentionDraft> kept = [];
        foreach (LMentionDraft mention in etymology.LEtymologyDraftMentions)
        {
            if (mention.LMentionDraftOffset >= end
                || mention.LMentionDraftOffset + mention.LMentionDraftLength <= offset)
            {
                kept.Add(mention);
            }
        }

        if (entryId != 0)
        {
            LEtymologyValidate(entryId);
            kept.Add(new LMentionDraft(_lDraftClerkIdentity.LIdentityCreate(), offset, length, entryId));
        }

        return etymology with { LEtymologyDraftMentions = LMentionDraft.LMentionDraftSort(kept) };
    }

    private LEtymologyDraft LEtymonAdd(LEtymologyDraft etymology, long entryId, int position)
    {
        LEtymologyValidate(entryId);

        List<long> kept = [.. etymology.LEtymologyDraftEtymons];
        if (kept.Contains(entryId))
        {
            throw new LRefusal(LRefusal.LRefusalLink);
        }

        kept.Insert(Math.Clamp(position, 0, kept.Count), entryId);
        return etymology with { LEtymologyDraftEtymons = kept };
    }

    private static LEtymologyDraft LEtymonRemove(LEtymologyDraft etymology, long entryId)
    {
        List<long> kept = [];
        foreach (long held in etymology.LEtymologyDraftEtymons)
        {
            if (held != entryId)
            {
                kept.Add(held);
            }
        }

        return etymology with { LEtymologyDraftEtymons = kept };
    }

    private static LEtymologyDraft LEtymonMove(LEtymologyDraft etymology, long entryId, int position)
    {
        List<long> kept = [.. etymology.LEtymologyDraftEtymons];
        int held = kept.IndexOf(entryId);
        if (held < 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        kept.RemoveAt(held);
        kept.Insert(Math.Clamp(position, 0, kept.Count), entryId);
        return etymology with { LEtymologyDraftEtymons = kept };
    }

    private void LEtymologyValidate(long entryId)
    {
        if (entryId <= 0 || _lDraftClerkEntries.LEntryRead(entryId) is null)
        {
            throw new LRefusal(LRefusal.LRefusalLink);
        }
    }

    private static int LEtymologyLengthRead(string text)
    {
        int length = 0;
        foreach (Rune _ in text.EnumerateRunes())
        {
            length++;
        }

        return length;
    }

    private static LEntryDraft LEtymologyChange(
        LEntryDraft content, Func<LEtymologyDraft, LEtymologyDraft> change)
    {
        return content with { LEntryDraftEtymology = change(content.LEntryDraftEtymology) };
    }
}
