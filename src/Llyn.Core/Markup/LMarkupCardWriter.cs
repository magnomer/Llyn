using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

internal static class LMarkupCardWriter
{
    internal static LMarkupNode LMarkupCardFormat(LMarkupCard card, string name)
    {
        List<LMarkupNode> element = [];
        LMarkup.LMarkupValueFormat(element, "title", card.LMarkupCardTitle);
        LMarkup.LMarkupValueFormat(element, "expression", card.LMarkupCardExpression);
        LMarkup.LMarkupValueFormat(element, "definition", card.LMarkupCardMeaning);

        foreach (LMarkupSentence sentence in card.LMarkupCardSentence)
        {
            element.Add(LMarkupSentenceFormat(sentence));
        }

        foreach (LSituationDraft situation in card.LMarkupCardSituation)
        {
            element.Add(LMarkupSituationFormat(situation));
        }

        foreach (LRegisterDraft register in card.LMarkupCardRegister)
        {
            LMarkup.LMarkupValueFormat(element, "register", register.LRegisterDraftName);
        }

        foreach (LMarkupTranslation translation in card.LMarkupCardTranslation)
        {
            element.Add(LMarkupTranslationFormat(translation));
        }

        foreach (LTagDraft tag in card.LMarkupCardTag)
        {
            element.Add(LMarkupNode.LMarkupNodeCreate("tag", tag.LTagDraftText));
        }

        foreach (LImageDraft image in card.LMarkupCardImage)
        {
            List<LMarkupNode> written = [];
            LMarkup.LMarkupValueFormat(written, "location", image.LImageDraftLocation);
            element.Add(LMarkupNode.LMarkupNodeCreate("image", written));
        }

        foreach (LVideoDraft video in card.LMarkupCardVideo)
        {
            List<LMarkupNode> written = [];
            LMarkup.LMarkupValueFormat(written, "location", video.LVideoDraftLocation);
            LMarkup.LMarkupValueFormat(written, "span", video.LVideoDraftSpan);
            element.Add(LMarkupNode.LMarkupNodeCreate("video", written));
        }

        foreach (LMarkupCard child in card.LMarkupCardChild)
        {
            element.Add(LMarkupCardFormat(child, "meaning"));
        }

        return LMarkupNode.LMarkupNodeCreate(name, element);
    }

    private static LMarkupNode LMarkupSentenceFormat(LMarkupSentence sentence)
    {
        List<LMarkupNode> element = [];
        LMarkup.LMarkupValueFormat(element, "particle", sentence.LMarkupSentenceParticle);
        LMarkup.LMarkupValueFormat(element, "dependence", sentence.LMarkupSentenceDependence);

        if (sentence.LMarkupSentenceExample is LMarkupExample example)
        {
            element.Add(LMarkupExampleFormat(example));
        }

        return LMarkupNode.LMarkupNodeCreate("sentence", element);
    }

    private static LMarkupNode LMarkupExampleFormat(LMarkupExample example)
    {
        List<LMarkupNode> element = [];
        LMarkup.LMarkupValueFormat(element, "text", example.LMarkupExampleText);
        LMarkup.LMarkupTextFormat(element, "language", example.LMarkupExampleLanguage);

        foreach (LGlossDraft gloss in example.LMarkupExampleGloss)
        {
            List<LMarkupNode> written = [];
            LMarkup.LMarkupTextFormat(written, "language", gloss.LGlossDraftLanguage);
            LMarkup.LMarkupValueFormat(written, "text", gloss.LGlossDraftText);
            element.Add(LMarkupNode.LMarkupNodeCreate("gloss", written));
        }

        foreach (LMarkupMention mention in example.LMarkupExampleMention)
        {
            element.Add(LMarkupMentionFormat(mention));
        }

        if (example.LMarkupExampleReference is LMarkupReference reference)
        {
            element.Add(LMarkupReferenceFormat(reference));
        }

        return LMarkupNode.LMarkupNodeCreate("example", element);
    }

    private static LMarkupNode LMarkupMentionFormat(LMarkupMention mention)
    {
        List<LMarkupNode> element = [];
        string offset = mention.LMarkupMentionOffset.ToString(CultureInfo.InvariantCulture);
        element.Add(LMarkupNode.LMarkupNodeCreate("offset", offset));
        string length = mention.LMarkupMentionLength.ToString(CultureInfo.InvariantCulture);
        element.Add(LMarkupNode.LMarkupNodeCreate("length", length));
        LMarkup.LMarkupTextFormat(element, "headword", mention.LMarkupMentionHeadword);
        LMarkup.LMarkupTextFormat(element, "language", mention.LMarkupMentionLanguage);
        LMarkup.LMarkupTextFormat(element, "sense", mention.LMarkupMentionSense);
        return LMarkupNode.LMarkupNodeCreate("mention", element);
    }

    private static LMarkupNode LMarkupReferenceFormat(LMarkupReference reference)
    {
        List<LMarkupNode> element = [];
        LMarkup.LMarkupValueFormat(element, "title", reference.LMarkupReferenceTitle);
        LMarkup.LMarkupValueFormat(element, "year", reference.LMarkupReferenceYear);
        LMarkup.LMarkupValueFormat(element, "kind", LReference.LReferenceKindShow(reference.LMarkupReferenceKind));
        LMarkup.LMarkupValueFormat(element, "url", reference.LMarkupReferenceUrl);
        LMarkup.LMarkupValueFormat(element, "note", reference.LMarkupReferenceNote);

        foreach (string author in reference.LMarkupReferenceAuthor)
        {
            element.Add(LMarkupNode.LMarkupNodeCreate("author", author));
        }

        return LMarkupNode.LMarkupNodeCreate("reference", element);
    }

    private static LMarkupNode LMarkupTranslationFormat(LMarkupTranslation translation)
    {
        List<LMarkupNode> element = [];
        LMarkup.LMarkupTextFormat(element, "headword", translation.LMarkupTranslationHeadword);
        LMarkup.LMarkupTextFormat(element, "language", translation.LMarkupTranslationLanguage);
        return LMarkupNode.LMarkupNodeCreate("translation", element);
    }

    private static LMarkupNode LMarkupSituationFormat(LSituationDraft situation)
    {
        List<LMarkupNode> element = [];
        LMarkup.LMarkupValueFormat(element, "title", situation.LSituationDraftTitle);
        LMarkup.LMarkupValueFormat(element, "description", situation.LSituationDraftDescription);
        LMarkup.LMarkupValueFormat(element, "kind", situation.LSituationDraftKind);
        return LMarkupNode.LMarkupNodeCreate("situation", element);
    }
}
