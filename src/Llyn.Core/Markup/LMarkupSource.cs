using System;
using System.Text;

namespace Llyn.Core;

public static class LMarkupSource
{
    public static void LMarkupSourceAppend(
        StringBuilder text, LMarkup.LMarkupReference source)
    {
        ArgumentNullException.ThrowIfNull(text);

        LReference held = source.LMarkupReferenceValue;

        text.Append("  <source id=")
            .Append(LMarkupMark.LMarkupMarkNormalize(source.LMarkupReferenceId))
            .Append(">\n");

        LMarkupLeaf.LMarkupLeafAppend(text, 2, "title", held.LReferenceTitle);

        foreach (string author in source.LMarkupReferenceAuthor)
        {
            LMarkupLeaf.LMarkupLeafAppend(
                text, 2, "author", LStateValue.LStateValueRead(author));
        }

        LMarkupLeaf.LMarkupLeafAppend(text, 2, "year", held.LReferenceYear);
        LMarkupLeaf.LMarkupLeafAppend(text, 2, "url", held.LReferenceUrl);
        LMarkupLeaf.LMarkupLeafAppend(text, 2, "program", held.LReferenceProgram);
        LMarkupLeaf.LMarkupLeafAppend(text, 2, "channel", held.LReferenceChannel);

        text.Append("  </source>\n");
    }
}
