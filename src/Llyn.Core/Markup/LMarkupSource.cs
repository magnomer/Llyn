using System;
using System.Text;

namespace Llyn.Core;

public static class LMarkupSource
{
    public static void LMarkupSourceAppend(
        StringBuilder text, int depth, string key, LMarkup.LMarkupReference source)
    {
        ArgumentNullException.ThrowIfNull(text);

        LReference held = source.LMarkupReferenceValue;
        int inner = depth + 1;

        text.Append(' ', depth * 2)
            .Append("<source id=")
            .Append(LMarkupMark.LMarkupMarkNormalize(key))
            .Append(">\n");

        LMarkupLeaf.LMarkupLeafAppend(text, inner, "title", held.LReferenceTitle);

        if (held.LReferenceAuthorState == LState.LStateUnknown)
        {
            text.Append(' ', inner * 2).Append("<author/>\n");
        }
        else
        {
            foreach (string author in source.LMarkupReferenceAuthor)
            {
                text.Append(' ', inner * 2)
                    .Append("<author ref=")
                    .Append(LMarkupMark.LMarkupMarkNormalize(author))
                    .Append("/>\n");
            }
        }

        LMarkupLeaf.LMarkupLeafAppend(text, inner, "year", held.LReferenceYear);
        LMarkupLeaf.LMarkupLeafAppend(
            text, inner, "kind", LReference.LReferenceKindShow(held.LReferenceKind));
        LMarkupLeaf.LMarkupLeafAppend(text, inner, "note", held.LReferenceNote);
        LMarkupLeaf.LMarkupLeafAppend(text, inner, "url", held.LReferenceUrl);

        text.Append(' ', depth * 2).Append("</source>\n");
    }
}
