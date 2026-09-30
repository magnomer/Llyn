using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CMention
{
    private readonly CAtelier _cMentionAtelier;

    internal CMention(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cMentionAtelier = atelier;
    }

    public event Action<long>? CMentionSenseChosen;

    internal CMentionOffer LMentionResultOpen(CMentionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        CNavigation navigation = _cMentionAtelier.CAtelierNavigation;
        IReadOnlyList<CTranslationTarget> offered = [];
        if (result.CMentionResultStored is CMentionMark stored)
        {
            if (stored.CMentionMarkEntry != 0
                && navigation.CNavigationEntryOpen(stored.CMentionMarkEntry)
                && stored.CMentionMarkSense != 0)
            {
                CMentionSenseChosen?.Invoke(stored.CMentionMarkSense);
            }
        }
        else if (result.CMentionResultSingle)
        {
            navigation.CNavigationEntryOpen(result.CMentionResultFirst);
        }
        else if (result.CMentionResultMany)
        {
            offered = result.CMentionResultEntry;
        }

        return new CMentionOffer(result.CMentionResultOffset, offered);
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

        IReadOnlyList<CMentionLabel> labels =
            LMentionLabelRead(_cMentionAtelier.CAtelierEntryPort.LEngineMentionResolve(text, drafts));
        List<CMentionLabel> named = new(labels.Count);
        foreach (CMentionLabel label in labels)
        {
            named.Add(label.CMentionLabelKey is null ? label : label with { CMentionLabelName = silent });
        }

        return named;
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

    internal static CProspect LMentionProspectRead(CDesk desk, string word, CEnvoy envoy, LSettingsPort settings)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(word);

        try
        {
            return desk.CDeskChip?.LQuillMentionFind(word) is LTranslationOffer offer
                ? CCard.LCardProspectRead(offer)
                : new CProspect(word, string.Empty, [], false, false, []);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(envoy, settings, "Mention.FindFailed", exception);
            return new CProspect(word, string.Empty, [], false, false, []);
        }
    }

    internal static IReadOnlyList<CMeaning>? LMentionMeaningRead(
        LDraftPort drafts, CDesk desk, long cardId, long sentenceId, string text, int start, int length,
        CEnvoy envoy, LSettingsPort settings)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(desk);

        if (desk.CDeskTenure is not LTenure held)
        {
            return null;
        }

        try
        {
            return drafts.LEngineSenseRead(held, cardId, sentenceId, text, start, length, "Display.Unknown") is { } rows
                ? CCatalog.LCatalogMeaningRead(rows)
                : null;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(envoy, settings, "Mention.FindFailed", exception);
            return null;
        }
    }

    internal static IReadOnlyList<CMentionLabel> LMentionChipRead(
        LDraftPort drafts, CDesk desk, long cardId, long sentenceId, CEnvoy envoy, LSettingsPort settings)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(desk);

        if (desk.CDeskTenure is not LTenure held)
        {
            return [];
        }

        try
        {
            return LMentionLabelRead(drafts.LEngineMentionResolve(held, cardId, sentenceId));
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(envoy, settings, "Mention.FindFailed", exception);
            return [];
        }
    }

    public int CMentionUnitRead(string text, int offset)
    {
        return _cMentionAtelier.CAtelierDraftPort.LEngineUnitRead(text, offset);
    }

    public int CMentionOffsetRead(string text, int unit)
    {
        return _cMentionAtelier.CAtelierDraftPort.LEngineOffsetRead(text, unit);
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

    internal static IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> LMentionLineRead(
        IReadOnlyDictionary<long, IReadOnlyList<LMentionLabel>> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        Dictionary<long, IReadOnlyList<CMentionLabel>> read = new(lines.Count);
        foreach ((long sentence, IReadOnlyList<LMentionLabel> labels) in lines)
        {
            read[sentence] = LMentionLabelRead(labels);
        }

        return read;
    }

    private static IReadOnlyList<CMentionLabel> LMentionLabelRead(IReadOnlyList<LMentionLabel> labels)
    {
        ArgumentNullException.ThrowIfNull(labels);

        List<CMentionLabel> read = new(labels.Count);
        foreach (LMentionLabel label in labels)
        {
            read.Add(new CMentionLabel(
                label.LMentionLabelId,
                label.LMentionLabelWord,
                label.LMentionLabelName,
                label.LMentionLabelSense,
                label.LMentionLabelLinked));
        }

        return read;
    }
}
