using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal static class CFolio
{
    internal static IReadOnlyList<CTranslationTarget> CFolioTargetRead(IReadOnlyList<LTranslationTarget> targets)
    {
        ArgumentNullException.ThrowIfNull(targets);

        return targets
            .Select(static target => new CTranslationTarget(
                target.LTranslationTargetId,
                target.LTranslationTargetHeadword,
                target.LTranslationTargetLanguage))
            .ToList();
    }

    internal static CEntryDraft CFolioEntryRead(
        LEntryDraft draft,
        IReadOnlyDictionary<(LCardKind, long), IReadOnlyList<LTranslationTarget>> targets,
        IReadOnlySet<(LCardKind, long)> folds,
        LMediaPort media,
        IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth, string LMeaningPath)> senses,
        LDraftPort drafts)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(targets);
        ArgumentNullException.ThrowIfNull(folds);
        ArgumentNullException.ThrowIfNull(media);
        ArgumentNullException.ThrowIfNull(senses);
        ArgumentNullException.ThrowIfNull(drafts);

        return new CEntryDraft(
            draft.LEntryDraftHeadword,
            draft.LEntryDraftNote,
            CFolioSheetRead(
                draft, draft.LEntryDraftMeanings, targets, folds, media, "Card.DefinitionHint", false, senses, drafts),
            CFolioSheetRead(
                draft, draft.LEntryDraftCollocations, targets, folds, media, "Card.MeaningHint", true, senses, drafts));
    }

    internal static CStateValue CFolioStateRead(LStateValue value)
    {
        return new CStateValue(value.LStateValuePlain, value.LStateValueUncertain || value.LStateValueUnreadable);
    }

    private static IReadOnlyList<CCardDraft> CFolioSheetRead(
        LEntryDraft draft,
        IReadOnlyList<LCardDraft> cards,
        IReadOnlyDictionary<(LCardKind, long), IReadOnlyList<LTranslationTarget>> targets,
        IReadOnlySet<(LCardKind, long)> folds,
        LMediaPort media,
        string meaning,
        bool expressed,
        IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth, string LMeaningPath)> senses,
        LDraftPort drafts)
    {
        return CFolioOrderRead(cards)
            .Select(card => new CCardDraft(
                card.LCardDraftId,
                card.LCardDraftPosition,
                CStateWording.LStateWordingRead(CFolioStateRead(card.LCardDraftTitle), null),
                CStateWording.LStateWordingRead(
                    CFolioStateRead(expressed ? card.LCardDraftExpression : LStateValue.LStateValueUnspecified),
                    null,
                    "Card.ExpressionHint"),
                CStateWording.LStateWordingRead(CFolioStateRead(card.LCardDraftMeaning), null, meaning),
                card.LCardDraftSentence.Select(sentence => CFolioSentenceRead(draft, sentence, drafts)).ToList(),
                card.LCardDraftSituation
                    .Select(static row => new CSituationDraft(
                        row.LSituationDraftId,
                        CFolioStateRead(row.LSituationDraftTitle),
                        CFolioStateRead(row.LSituationDraftKind),
                        CFolioStateRead(row.LSituationDraftDescription),
                        [],
                        []))
                    .ToList(),
                card.LCardDraftRegister
                    .Select(static row => new CRegisterDraft(
                        row.LRegisterDraftId,
                        CStateWording.LStateWordingRead(CFolioStateRead(row.LRegisterDraftName), null)))
                    .ToList(),
                CFolioTargetRead(targets[
                    (expressed ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, card.LCardDraftId)]),
                card.LCardDraftTag.Select(static row => new CTagDraft(row.LTagDraftId, row.LTagDraftText)).ToList(),
                CFolioImageRead(card.LCardDraftImage, media),
                CFolioVideoRead(card.LCardDraftVideo, media),
                folds.Contains(
                    (expressed ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, card.LCardDraftId)),
                card.LCardDraftStored,
                card.LCardDraftSense
                    .Join(
                        senses,
                        static id => id,
                        static row => row.LMeaningId,
                        static (_, row) => new CMeaning(row.LMeaningId, row.LMeaningName, 0, row.LMeaningPath))
                    .ToList()))
            .ToList();
    }

    internal static int? CFolioPlaceRead(LEntryDraft content, bool collocated, int place)
    {
        ArgumentNullException.ThrowIfNull(content);

        IReadOnlyList<LCardDraft> cards = CFolioListRead(content, collocated);
        IReadOnlyList<LCardDraft> shown = CFolioOrderRead(cards);
        if (place < 0 || place >= shown.Count)
        {
            return null;
        }

        long target = shown[place].LCardDraftId;
        for (int index = 0; index < cards.Count; index++)
        {
            if (cards[index].LCardDraftId == target)
            {
                return index;
            }
        }

        return null;
    }

    internal static int? CFolioOrdinalRead(LEntryDraft content, bool collocated, string ordinal)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(ordinal);

        if (!int.TryParse(ordinal, NumberStyles.Integer, CultureInfo.InvariantCulture, out int wanted))
        {
            return null;
        }

        int count = Math.Max(CFolioListRead(content, collocated).Count, 1);
        return Math.Clamp(wanted, 1, count) - 1;
    }

    internal static int CFolioCountRead(LEntryDraft content, bool collocated)
    {
        ArgumentNullException.ThrowIfNull(content);

        return CFolioListRead(content, collocated).Count;
    }

    internal static IReadOnlyList<LCardDraft> CFolioOrderRead(IReadOnlyList<LCardDraft> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        return cards.OrderBy(static card => card.LCardDraftPosition).ToList();
    }

    private static IReadOnlyList<LCardDraft> CFolioListRead(LEntryDraft content, bool collocated)
    {
        return collocated ? content.LEntryDraftCollocations : content.LEntryDraftMeanings;
    }

    private static CSentenceDraft CFolioSentenceRead(LEntryDraft draft, LSentenceDraft sentence, LDraftPort drafts)
    {
        CExampleDraft? example = CFolioExampleRead(sentence.LSentenceDraftExample);
        return new CSentenceDraft(
            sentence.LSentenceDraftId,
            drafts.LEngineLineFind(draft, sentence),
            example,
            sentence.LSentenceDraftCited,
            CStateWording.LStateWordingRead(
                example?.CExampleDraftText ?? CStateValue.CStateValueEmpty, null, "Card.ExampleHint"),
            CStateWording.LStateWordingRead(
                CFolioStateRead(sentence.LSentenceDraftParticle), null, "Card.ParticleHint"),
            CStateWording.LStateWordingRead(
                CFolioStateRead(sentence.LSentenceDraftDependence), null, "Card.DependenceHint"));
    }

    private static CExampleDraft? CFolioExampleRead(LExampleDraft? example)
    {
        return example is null
            ? null
            : new CExampleDraft(
                CFolioStateRead(example.LExampleDraftText),
                example.LExampleDraftReference.LStateAnchorShown,
                CFolioGlossRead(example.LExampleDraftGloss));
    }

    internal static IReadOnlyList<CGlossDraft> CFolioGlossRead(IReadOnlyList<LGlossDraft> glosses)
    {
        return glosses
            .Select(static gloss => new CGlossDraft(
                gloss.LGlossDraftId,
                gloss.LGlossDraftLanguage,
                CFolioStateRead(gloss.LGlossDraftText),
                gloss.LGlossDraftNamed))
            .ToList();
    }

    internal static IReadOnlyList<CImageDraft> CFolioImageRead(IReadOnlyList<LImageDraft> images, LMediaPort media)
    {
        ArgumentNullException.ThrowIfNull(media);

        return images
            .Select(image => new CImageDraft(
                image.LImageDraftId,
                CFolioStateRead(image.LImageDraftLocation),
                image.LImageDraftEmpty,
                media.LEngineLocationRead(image.LImageDraftLocation.LStateValuePlain)))
            .ToList();
    }

    internal static string CFolioNameRead(Uri? address, string written)
    {
        ArgumentNullException.ThrowIfNull(written);

        string path = address is null ? written : address.IsFile ? address.LocalPath : address.AbsolutePath;
        int start = path.LastIndexOfAny(['/', '\\']) + 1;
        if (start == 0 && path.Length >= 2 && path[1] == ':' && char.IsAsciiLetter(path[0]))
        {
            start = 2;
        }

        string named = path[start..];
        return named.Length > 0 ? named : address?.ToString() ?? written;
    }

    internal static IReadOnlyList<CVideoDraft> CFolioVideoRead(IReadOnlyList<LVideoDraft> videos, LMediaPort media)
    {
        ArgumentNullException.ThrowIfNull(media);

        return videos
            .Select(video =>
            {
                (TimeSpan from, TimeSpan? until) = LMediaPort.LEngineSpanRead(video);
                return new CVideoDraft(
                    video.LVideoDraftId,
                    CFolioStateRead(video.LVideoDraftLocation),
                    CFolioStateRead(video.LVideoDraftSpan),
                    video.LVideoDraftEmpty,
                    media.LEngineScreenRead(video.LVideoDraftLocation.LStateValuePlain) is (Uri address, var film)
                        ? new CScreen(address, film)
                        : null,
                    from,
                    until);
            })
            .ToList();
    }
}
