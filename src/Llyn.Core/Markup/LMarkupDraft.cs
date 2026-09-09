using System;
using System.Collections.Generic;
using System.Text;

namespace Llyn.Core;

public static class LMarkupDraft
{
    public static string LMarkupDraftFormat(
        LEntryDraft draft, IReadOnlyList<LMarkup.LMarkupReference> sources)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(sources);

        StringBuilder text = new StringBuilder();

        text.Append("<entry>\n");
        LMarkupLeaf.LMarkupLeafAppend(
            text, 1, "headword", LStateValue.LStateValueRead(draft.LEntryDraftHeadword));
        LMarkupLeaf.LMarkupLeafAppend(
            text, 1, "lang", LStateValue.LStateValueRead(draft.LEntryDraftLanguage));
        LMarkupLeaf.LMarkupLeafAppend(
            text, 1, "ipa", LStateValue.LStateValueRead(draft.LEntryDraftPronunciation));
        LMarkupLeaf.LMarkupLeafAppend(
            text, 1, "audio", LStateValue.LStateValueRead(draft.LEntryDraftAudio));

        if (draft.LEntryDraftSpeeches is IReadOnlyList<string> speeches && speeches.Count > 0)
        {
            LMarkupLeaf.LMarkupLeafAppend(
                text, 1, "pos", LStateValue.LStateValueRead(string.Join(", ", speeches)));
        }

        LMarkupLeaf.LMarkupLeafAppend(
            text, 1, "note", LStateValue.LStateValueRead(draft.LEntryDraftNote));

        foreach (LMarkup.LMarkupReference source in sources)
        {
            LMarkupSource.LMarkupSourceAppend(text, source);
        }

        foreach (LCardDraft card in draft.LEntryDraftMeanings)
        {
            LMarkupCard.LMarkupCardAppend(text, "sense", card);
        }

        foreach (LCardDraft card in draft.LEntryDraftCollocations)
        {
            LMarkupCard.LMarkupCardAppend(text, "collocation", card);
        }

        text.Append("</entry>\n");
        return text.ToString();
    }
}
