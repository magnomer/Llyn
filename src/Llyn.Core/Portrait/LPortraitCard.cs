using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPortraitCard(
    int LPortraitCardPosition,
    string LPortraitCardTitle,
    string LPortraitCardKind,
    string LPortraitCardExpression,
    string LPortraitCardMeaning,
    IReadOnlyList<string> LPortraitCardSituation,
    IReadOnlyList<string> LPortraitCardRegister,
    IReadOnlyList<LPortraitLink> LPortraitCardTranslation,
    IReadOnlyList<LPortraitExample> LPortraitCardExample,
    IReadOnlyList<string> LPortraitCardTag,
    IReadOnlyList<LPortraitMedia> LPortraitCardImage,
    IReadOnlyList<LPortraitMedia> LPortraitCardVideo)
{
    public static IReadOnlyList<LPortraitCard> LPortraitCardCreate(
        IReadOnlyList<LCardDraft> cards,
        string kind,
        LSentenceOrder order,
        string mark,
        IReadOnlyDictionary<long, LPortraitLink> targets)
    {
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(targets);

        List<LPortraitCard> shown = new List<LPortraitCard>();

        foreach (LCardDraft card in cards)
        {
            List<string> situations = new List<string>();
            foreach (LSituationDraft situation in card.LCardDraftSituation)
            {
                situations.Add(
                    LPortraitText.LPortraitTextRead(situation.LSituationDraftTitle, mark));
            }

            List<string> registers = new List<string>();
            foreach (LRegisterDraft register in card.LCardDraftRegister)
            {
                registers.Add(
                    LPortraitText.LPortraitTextRead(register.LRegisterDraftName, mark));
            }

            List<LPortraitLink> links = new List<LPortraitLink>();
            foreach (long id in card.LCardDraftTranslation)
            {
                if (targets.TryGetValue(id, out LPortraitLink? link))
                {
                    links.Add(link);
                }
            }

            List<LPortraitExample> examples = new List<LPortraitExample>();
            foreach (LSentenceDraft sentence in card.LCardDraftSentence)
            {
                examples.Add(new LPortraitExample(
                    LPortraitFrame.LPortraitFrameRead(sentence, order, mark),
                    LPortraitText.LPortraitTextRead(
                        sentence.LSentenceDraftExample?.LExampleDraftText
                            ?? LStateValue.LStateValueUnspecified,
                        mark)));
            }

            List<string> tags = new List<string>();
            foreach (LTagDraft tag in card.LCardDraftTag)
            {
                tags.Add(tag.LTagDraftText);
            }

            List<LPortraitMedia> images = new List<LPortraitMedia>();
            foreach (LImageDraft image in card.LCardDraftImage)
            {
                if (!image.LImageDraftEmpty)
                {
                    images.Add(new LPortraitMedia(
                        LPortraitText.LPortraitTextRead(image.LImageDraftLocation, mark),
                        string.Empty,
                        false));
                }
            }

            List<LPortraitMedia> videos = new List<LPortraitMedia>();
            foreach (LVideoDraft video in card.LCardDraftVideo)
            {
                if (!video.LVideoDraftEmpty)
                {
                    videos.Add(new LPortraitMedia(
                        LPortraitText.LPortraitTextRead(video.LVideoDraftLocation, mark),
                        LPortraitText.LPortraitTextRead(video.LVideoDraftSpan, mark),
                        true));
                }
            }

            shown.Add(new LPortraitCard(
                card.LCardDraftPosition,
                LPortraitText.LPortraitTextRead(card.LCardDraftTitle, mark),
                kind,
                LPortraitText.LPortraitTextRead(card.LCardDraftExpression, mark),
                LPortraitText.LPortraitTextRead(card.LCardDraftMeaning, mark),
                situations,
                registers,
                links,
                examples,
                tags,
                images,
                videos));
        }

        return shown;
    }
}
