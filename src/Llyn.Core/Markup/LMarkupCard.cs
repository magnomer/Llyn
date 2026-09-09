using System;
using System.Collections.Generic;
using System.Text;

namespace Llyn.Core;

public static class LMarkupCard
{
    public static void LMarkupCardAppend(
        StringBuilder text,
        int depth,
        string block,
        LCardDraft card,
        IReadOnlyDictionary<string, string> keys)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(keys);

        bool collocation = string.Equals(block, "collocation", StringComparison.Ordinal);
        int inner = depth + 1;

        text.Append(' ', depth * 2).Append('<').Append(block);

        if (!collocation && keys.TryGetValue(card.LCardDraftId, out string? own))
        {
            text.Append(" id=").Append(LMarkupMark.LMarkupMarkNormalize(own));
        }

        text.Append(">\n");

        LMarkupLeaf.LMarkupLeafAppend(text, inner, "title", card.LCardDraftTitle);

        if (collocation)
        {
            LMarkupLeaf.LMarkupLeafAppend(text, inner, "expression", card.LCardDraftExpression);
            LMarkupLeaf.LMarkupLeafAppend(text, inner, "meaning", card.LCardDraftMeaning);
        }
        else
        {
            LMarkupLeaf.LMarkupLeafAppend(
                text, inner, "gloss", LStateValue.LStateValueRead(card.LCardDraftGloss));
            LMarkupLeaf.LMarkupLeafAppend(
                text, inner, "meaning", card.LCardDraftMeaning, "lang", card.LCardDraftLanguage);
            LMarkupLeaf.LMarkupLeafAppend(
                text, inner, "labels", LStateValue.LStateValueRead(card.LCardDraftLabels));
        }

        foreach (LSentenceDraft sentence in card.LCardDraftSentence)
        {
            LMarkupExample.LMarkupExampleAppend(text, inner, sentence, keys);
        }

        foreach (LSituationDraft situation in card.LCardDraftSituation)
        {
            LMarkupCardAppend(text, inner, "situation", situation.LSituationDraftId, keys);
        }

        foreach (LRegisterDraft register in card.LCardDraftRegister)
        {
            LMarkupCardAppend(text, inner, "register", register.LRegisterDraftId, keys);
        }

        foreach (LImageDraft image in card.LCardDraftImage)
        {
            LMarkupCardAppend(text, inner, "image", image.LImageDraftId, keys);
        }

        foreach (LVideoDraft video in card.LCardDraftVideo)
        {
            LMarkupCardAppend(text, inner, "video", video.LVideoDraftId, keys);
        }

        foreach (string tag in card.LCardDraftTag)
        {
            LMarkupLeaf.LMarkupLeafAppend(text, inner, "tag", LStateValue.LStateValueRead(tag));
        }

        foreach (string translation in card.LCardDraftTranslation)
        {
            if (keys.TryGetValue(translation, out string? named))
            {
                text.Append(' ', inner * 2)
                    .Append("<translation entry=")
                    .Append(LMarkupMark.LMarkupMarkNormalize(named))
                    .Append("/>\n");
            }
        }

        foreach (LRelationDraft relation in card.LCardDraftRelation)
        {
            LMarkupCardAppend(text, inner, relation, keys);
        }

        foreach (LSynonymDraft interlink in card.LCardDraftInterlink)
        {
            LMarkupCardAppend(text, inner, "synonym", interlink, keys);
        }

        if (!collocation)
        {
            foreach (LCardDraft child in card.LCardDraftChild)
            {
                LMarkupCardAppend(text, inner, "sense", child, keys);
            }
        }

        text.Append(' ', depth * 2).Append("</").Append(block).Append(">\n");
    }

    private static void LMarkupCardAppend(
        StringBuilder text,
        int depth,
        LRelationDraft relation,
        IReadOnlyDictionary<string, string> keys)
    {
        StringBuilder target = new StringBuilder();
        if (!LMarkupCardAppend(target, depth + 1, "target", new LSynonymDraft(
            relation.LRelationDraftEntry, relation.LRelationDraftMeaning), keys))
        {
            return;
        }

        text.Append(' ', depth * 2).Append("<relation");
        LMarkupMark.LMarkupMarkAppend(text, "type", relation.LRelationDraftType);
        LMarkupMark.LMarkupMarkAppend(text, "label", relation.LRelationDraftLabel);
        LMarkupMark.LMarkupMarkAppend(text, "labels", relation.LRelationDraftLabels);
        text.Append(">\n").Append(target);
        text.Append(' ', depth * 2).Append("</relation>\n");
    }

    private static bool LMarkupCardAppend(
        StringBuilder text,
        int depth,
        string name,
        LSynonymDraft target,
        IReadOnlyDictionary<string, string> keys)
    {
        bool entry = target.LSynonymDraftEntry.Length > 0;
        string row = entry ? target.LSynonymDraftEntry : target.LSynonymDraftMeaning;

        if (!keys.TryGetValue(row, out string? named))
        {
            return false;
        }

        text.Append(' ', depth * 2)
            .Append('<')
            .Append(name)
            .Append(entry ? " entry=" : " sense=")
            .Append(LMarkupMark.LMarkupMarkNormalize(named))
            .Append("/>\n");
        return true;
    }

    private static void LMarkupCardAppend(
        StringBuilder text,
        int depth,
        string name,
        string row,
        IReadOnlyDictionary<string, string> keys)
    {
        if (!keys.TryGetValue(row, out string? named))
        {
            return;
        }

        text.Append(' ', depth * 2)
            .Append('<')
            .Append(name)
            .Append(" ref=")
            .Append(LMarkupMark.LMarkupMarkNormalize(named))
            .Append("/>\n");
    }
}
