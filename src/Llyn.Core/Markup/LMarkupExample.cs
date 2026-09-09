using System;
using System.Collections.Generic;
using System.Text;

namespace Llyn.Core;

public static class LMarkupExample
{
    public static void LMarkupExampleAppend(
        StringBuilder text,
        int depth,
        LSentenceDraft sentence,
        IReadOnlyDictionary<string, string> keys)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(sentence);
        ArgumentNullException.ThrowIfNull(keys);

        if (sentence.LSentenceDraftEmpty)
        {
            return;
        }

        text.Append(' ', depth * 2).Append("<use");

        if (sentence.LSentenceDraftExample is LExampleDraft quoted)
        {
            LMarkupMark.LMarkupMarkAppend(
                text,
                "ref",
                LStateValue.LStateValueCreate(
                    keys.TryGetValue(quoted.LExampleDraftId, out string? named)
                        ? named
                        : string.Empty));
        }

        LMarkupMark.LMarkupMarkAppend(text, "par", sentence.LSentenceDraftParticle);
        LMarkupMark.LMarkupMarkAppend(text, "dep", sentence.LSentenceDraftDependence);
        text.Append("/>\n");
    }

    public static void LMarkupExampleAppend(
        StringBuilder text, int depth, string key, LExample example)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(example);

        text.Append(' ', depth * 2)
            .Append("<example id=")
            .Append(LMarkupMark.LMarkupMarkNormalize(key));

        if (example.LExampleLanguage.Length > 0)
        {
            text.Append(" lang=")
                .Append(LMarkupMark.LMarkupMarkNormalize(example.LExampleLanguage));
        }

        LMarkupMark.LMarkupMarkAppend(text, "src", example.LExampleSource);
        text.Append(">\n");

        LMarkupLeaf.LMarkupLeafAppend(text, depth + 1, "text", example.LExampleText);
        LMarkupLeaf.LMarkupLeafAppend(text, depth + 1, "trans", example.LExampleTranslation);

        text.Append(' ', depth * 2).Append("</example>\n");
    }
}
