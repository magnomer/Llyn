using System;
using System.Text;

namespace Llyn.Core;

public static class LMarkupCard
{
    public static void LMarkupCardAppend(StringBuilder text, string block, LCardDraft card)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(card);

        text.Append("  <").Append(block).Append(">\n");

        LMarkupLeaf.LMarkupLeafAppend(text, 2, "title", card.LCardDraftTitle);
        LMarkupLeaf.LMarkupLeafAppend(text, 2, "expression", card.LCardDraftExpression);
        LMarkupLeaf.LMarkupLeafAppend(text, 2, "meaning", card.LCardDraftMeaning);
        LMarkupLeaf.LMarkupLeafAppend(
            text, 2, "synonym", LStateValue.LStateValueRead(card.LCardDraftSynonym));

        foreach (LSituationDraft situation in card.LCardDraftSituation)
        {
            LMarkupLeaf.LMarkupLeafAppend(text, 2, "situation", situation.LSituationDraftTitle);
        }

        foreach (LRegisterDraft register in card.LCardDraftRegister)
        {
            LMarkupLeaf.LMarkupLeafAppend(text, 2, "register", register.LRegisterDraftName);
        }

        foreach (LSentenceDraft sentence in card.LCardDraftSentence)
        {
            LMarkupExample.LMarkupExampleAppend(text, sentence);
        }

        foreach (string tag in card.LCardDraftTag)
        {
            LMarkupLeaf.LMarkupLeafAppend(text, 2, "tag", LStateValue.LStateValueRead(tag));
        }

        foreach (LStateValue image in card.LCardDraftImage)
        {
            LMarkupLeaf.LMarkupLeafAppend(text, 2, "image", image);
        }

        foreach (LVideoDraft video in card.LCardDraftVideo)
        {
            LMarkupLeaf.LMarkupLeafAppend(text, 2, "video", video.LVideoDraftLocation);
        }

        text.Append("  </").Append(block).Append(">\n");
    }
}
