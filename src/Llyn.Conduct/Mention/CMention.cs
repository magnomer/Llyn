using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed class CMention
{
    private readonly CAtelier _cMentionAtelier;

    internal CMention(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cMentionAtelier = atelier;
    }

    public IReadOnlyList<CMentionLabel> CMentionResolve(
        string text, IReadOnlyList<CMentionDraft> mentions, string silent)
    {
        ArgumentNullException.ThrowIfNull(mentions);

        List<LMentionDraft> drafts = new(mentions.Count);
        foreach (CMentionDraft mention in mentions)
        {
            drafts.Add(new LMentionDraft(
                mention.CMentionDraftId,
                mention.CMentionDraftOffset,
                mention.CMentionDraftLength,
                mention.CMentionDraftEntry,
                mention.CMentionDraftSense));
        }

        return CMentionLabelRead(_cMentionAtelier.CAtelierEntryPort.LEngineMentionResolve(text, drafts), silent);
    }

    public IReadOnlyList<CMentionPiece> CMentionDivide(string text, IReadOnlyList<CMentionMark> mentions)
    {
        IReadOnlyList<LMentionPiece> pieces =
            _cMentionAtelier.CAtelierDraftPort.LEngineMentionDivide(text, CMentionRead(mentions));
        List<CMentionPiece> read = new(pieces.Count);
        foreach (LMentionPiece piece in pieces)
        {
            read.Add(new CMentionPiece(
                piece.LMentionPieceOffset,
                piece.LMentionPieceEnd,
                piece.LMentionPieceText,
                piece.LMentionPieceStored?.LMentionLinked));
        }

        return read;
    }

    public int CMentionUnitRead(string text, int offset)
    {
        return _cMentionAtelier.CAtelierDraftPort.LEngineUnitRead(text, offset);
    }

    public int CMentionOffsetRead(string text, int unit)
    {
        return _cMentionAtelier.CAtelierDraftPort.LEngineOffsetRead(text, unit);
    }

    public (int CMentionSpanOffset, int CMentionSpanLength) CMentionSpanRead(string text, int start, int length)
    {
        LMentionDraft span = _cMentionAtelier.CAtelierDraftPort.LEngineSpanRead(text, start, length);
        return (span.LMentionDraftOffset, span.LMentionDraftLength);
    }

    public CMentionDraft? CMentionFind(
        CDesk desk, long cardId, long sentenceId, string text, int start, int length, bool settled)
    {
        ArgumentNullException.ThrowIfNull(desk);

        if (settled)
        {
            desk.CDeskPersist();
        }

        return CMentionRead(
            desk.CDeskTenure?.LTenureMentionFind(
                cardId, sentenceId, _cMentionAtelier.CAtelierDraftPort.LEngineSpanRead(text, start, length)));
    }

    private static CMentionDraft? CMentionRead(LMentionDraft? found)
    {
        return found is null
            ? null
            : new CMentionDraft(
                found.LMentionDraftId,
                found.LMentionDraftEntry,
                found.LMentionDraftOffset,
                found.LMentionDraftLength,
                found.LMentionDraftSense);
    }

    public bool CMentionSpanCheck(string text, int start, int length)
    {
        return _cMentionAtelier.CAtelierDraftPort.LEngineSpanCheck(text, start, length);
    }

    internal static IReadOnlyList<CMentionMark> CMentionMarkRead(IReadOnlyList<LMentionDraft>? drafts)
    {
        if (drafts is null)
        {
            return [];
        }

        List<CMentionMark> read = new(drafts.Count);
        foreach (LMentionDraft draft in drafts)
        {
            read.Add(new CMentionMark(
                draft.LMentionDraftId,
                draft.LMentionDraftOffset,
                draft.LMentionDraftLength,
                draft.LMentionDraftEntry,
                draft.LMentionDraftSense));
        }

        return read;
    }

    internal static IReadOnlyList<CMentionMark> CMentionRead(IReadOnlyList<LMention> mentions)
    {
        ArgumentNullException.ThrowIfNull(mentions);

        List<CMentionMark> read = new(mentions.Count);
        foreach (LMention mention in mentions)
        {
            read.Add(new CMentionMark(
                mention.LMentionId,
                mention.LMentionOffset,
                mention.LMentionLength,
                mention.LMentionEntryId,
                mention.LMentionSenseId));
        }

        return read;
    }

    internal static CMentionResult CMentionResultRead(LMentionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return new CMentionResult(
            result.LMentionResultOffset,
            result.LMentionResultStored is LMention stored ? CMentionRead([stored])[0] : null,
            CFolio.CFolioTargetRead(result.LMentionResultEntry));
    }

    internal static IReadOnlyList<LMention> CMentionRead(IReadOnlyList<CMentionMark> mentions)
    {
        ArgumentNullException.ThrowIfNull(mentions);

        List<LMention> read = new(mentions.Count);
        foreach (CMentionMark mention in mentions)
        {
            read.Add(new LMention(
                mention.CMentionMarkId,
                mention.CMentionMarkOffset,
                mention.CMentionMarkLength,
                mention.CMentionMarkEntry,
                mention.CMentionMarkSense));
        }

        return read;
    }

    private static IReadOnlyList<CMentionLabel> CMentionLabelRead(IReadOnlyList<LMentionLabel> labels, string silent)
    {
        List<CMentionLabel> read = new(labels.Count);
        foreach (LMentionLabel label in labels)
        {
            read.Add(new CMentionLabel(
                label.LMentionLabelId,
                label.LMentionLabelWord,
                label.LMentionLabelLinked ? label.LMentionLabelName : silent,
                label.LMentionLabelSense));
        }

        return read;
    }
}
