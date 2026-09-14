using System.Globalization;
using System.Xml.Linq;

namespace Llyn.Core;

internal static class LMarkupCardWriter
{
    internal static XElement LMarkupCardFormat(LMarkupCard card, string name)
    {
        XElement element = new(name);
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
            element.Add(new XElement("tag", LMarkup.LMarkupTextNormalize(tag.LTagDraftText)));
        }

        foreach (LImageDraft image in card.LMarkupCardImage)
        {
            XElement written = new("image");
            LMarkup.LMarkupValueFormat(written, "location", image.LImageDraftLocation);
            element.Add(written);
        }

        foreach (LVideoDraft video in card.LMarkupCardVideo)
        {
            XElement written = new("video");
            LMarkup.LMarkupValueFormat(written, "location", video.LVideoDraftLocation);
            LMarkup.LMarkupValueFormat(written, "span", video.LVideoDraftSpan);
            element.Add(written);
        }

        foreach (LMarkupCard child in card.LMarkupCardChild)
        {
            element.Add(LMarkupCardFormat(child, "meaning"));
        }

        return element;
    }

    private static XElement LMarkupSentenceFormat(LMarkupSentence sentence)
    {
        XElement element = new("sentence");
        LMarkup.LMarkupValueFormat(element, "particle", sentence.LMarkupSentenceParticle);
        LMarkup.LMarkupValueFormat(element, "dependence", sentence.LMarkupSentenceDependence);

        if (sentence.LMarkupSentenceExample is LMarkupExample example)
        {
            element.Add(LMarkupExampleFormat(example));
        }

        return element;
    }

    private static XElement LMarkupExampleFormat(LMarkupExample example)
    {
        XElement element = new("example");
        LMarkup.LMarkupValueFormat(element, "text", example.LMarkupExampleText);
        LMarkup.LMarkupTextFormat(element, "language", example.LMarkupExampleLanguage);

        foreach (LGlossDraft gloss in example.LMarkupExampleGloss)
        {
            XElement written = new("gloss");
            LMarkup.LMarkupTextFormat(written, "language", gloss.LGlossDraftLanguage);
            LMarkup.LMarkupValueFormat(written, "text", gloss.LGlossDraftText);
            element.Add(written);
        }

        foreach (LMarkupMention mention in example.LMarkupExampleMention)
        {
            element.Add(LMarkupMentionFormat(mention));
        }

        if (example.LMarkupExampleReference is LMarkupReference reference)
        {
            element.Add(LMarkupReferenceFormat(reference));
        }

        return element;
    }

    private static XElement LMarkupMentionFormat(LMarkupMention mention)
    {
        XElement element = new("mention");
        element.Add(new XElement("offset", mention.LMarkupMentionOffset.ToString(CultureInfo.InvariantCulture)));
        element.Add(new XElement("length", mention.LMarkupMentionLength.ToString(CultureInfo.InvariantCulture)));
        LMarkup.LMarkupTextFormat(element, "headword", mention.LMarkupMentionHeadword);
        LMarkup.LMarkupTextFormat(element, "language", mention.LMarkupMentionLanguage);
        LMarkup.LMarkupTextFormat(element, "sense", mention.LMarkupMentionSense);
        return element;
    }

    private static XElement LMarkupReferenceFormat(LMarkupReference reference)
    {
        XElement element = new("reference");
        LMarkup.LMarkupValueFormat(element, "title", reference.LMarkupReferenceTitle);
        LMarkup.LMarkupValueFormat(element, "year", reference.LMarkupReferenceYear);
        LMarkup.LMarkupValueFormat(element, "kind", LReference.LReferenceKindShow(reference.LMarkupReferenceKind));
        LMarkup.LMarkupValueFormat(element, "url", reference.LMarkupReferenceUrl);
        LMarkup.LMarkupValueFormat(element, "note", reference.LMarkupReferenceNote);

        foreach (string author in reference.LMarkupReferenceAuthor)
        {
            element.Add(new XElement("author", LMarkup.LMarkupTextNormalize(author)));
        }

        return element;
    }

    private static XElement LMarkupTranslationFormat(LMarkupTranslation translation)
    {
        XElement element = new("translation");
        LMarkup.LMarkupTextFormat(element, "headword", translation.LMarkupTranslationHeadword);
        LMarkup.LMarkupTextFormat(element, "language", translation.LMarkupTranslationLanguage);
        return element;
    }

    private static XElement LMarkupSituationFormat(LSituationDraft situation)
    {
        XElement element = new("situation");
        LMarkup.LMarkupValueFormat(element, "title", situation.LSituationDraftTitle);
        LMarkup.LMarkupValueFormat(element, "description", situation.LSituationDraftDescription);
        LMarkup.LMarkupValueFormat(element, "kind", situation.LSituationDraftKind);
        return element;
    }
}
