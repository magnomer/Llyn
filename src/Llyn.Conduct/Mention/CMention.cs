using System;
using System.Collections.Generic;
using System.Linq;
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

    internal CMentionOffer LMentionResultOpen(CMentionResult result, string text)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(text);

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

        return new CMentionOffer(LMentionUnitRead(text, 0, result.CMentionResultOffset), offered, "Mention.Title");
    }

    internal static IReadOnlyList<CMentionPiece> CMentionDivide(string text, IReadOnlyList<LMention> mentions)
    {
        return LMentionPieceRead(LDraftPort.LEngineMentionDivide(text, mentions));
    }

    internal static IReadOnlyList<CMentionPiece> LMentionPieceRead(IReadOnlyList<LMentionPiece> pieces)
    {
        List<CMentionPiece> read = new(pieces.Count);
        foreach (LMentionPiece piece in pieces)
        {
            read.Add(new CMentionPiece(
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

    internal static CMentionSense? LMentionSenseRead(
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
                ? new CMentionSense(
                    "Mention.Sense",
                    [
                        new CMeaning(0, settings.LEngineTextRead("Mention.Whole"), 0),
                        .. CCatalog.LCatalogMeaningRead(rows),
                    ])
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

    internal int? LMentionUnitRead(string text, int start, int offset)
    {
        LDraftPort drafts = _cMentionAtelier.CAtelierDraftPort;
        int inside = offset - start;
        return inside >= 0 && inside < drafts.LEngineOffsetRead(text, text.Length)
            ? drafts.LEngineUnitRead(text, inside)
            : null;
    }

    internal int LMentionOffsetRead(string text, int unit)
    {
        return _cMentionAtelier.CAtelierDraftPort.LEngineOffsetRead(text, unit);
    }

    public bool CMentionSpanCheck(string text, int start, int length)
    {
        return _cMentionAtelier.CAtelierDraftPort.LEngineSpanCheck(text, start, length);
    }

    internal static IReadOnlyList<CMentionMark> CMentionRead(IReadOnlyList<LMention> mentions)
    {
        ArgumentNullException.ThrowIfNull(mentions);

        List<CMentionMark> read = new(mentions.Count);
        foreach (LMention mention in mentions)
        {
            read.Add(new CMentionMark(mention.LMentionEntryId, mention.LMentionSenseId));
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

    internal static IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> LMentionLineRead(
        LEntryDraft content, IReadOnlyDictionary<long, IReadOnlyList<LMentionLabel>> lines)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(lines);

        Dictionary<long, IReadOnlyList<CMentionLabel>> read = [];
        foreach (LCardDraft card in content.LEntryDraftMeanings.Concat(content.LEntryDraftCollocations))
        {
            foreach (LSentenceDraft sentence in card.LCardDraftSentence)
            {
                read[sentence.LSentenceDraftId] =
                    lines.TryGetValue(sentence.LSentenceDraftId, out IReadOnlyList<LMentionLabel>? labels)
                        ? LMentionLabelRead(labels)
                        : [];
            }
        }

        return read;
    }

    internal static IReadOnlyList<CMentionLabel> LMentionLabelRead(IReadOnlyList<LMentionLabel> labels)
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
