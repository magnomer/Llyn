using System;
using System.Text;

namespace Llyn.Core;

public static class LMarkupExample
{
    public static void LMarkupExampleAppend(StringBuilder text, LExampleDraft example)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(example);

        if (example.LExampleDraftText.LStateValueState == LState.LStateUnspecified)
        {
            return;
        }

        text.Append("    <example");
        LMarkupMark.LMarkupMarkAppend(text, "src", example.LExampleDraftReference);
        LMarkupMark.LMarkupMarkAppend(text, "par", example.LExampleDraftParticle);
        LMarkupMark.LMarkupMarkAppend(text, "dep", example.LExampleDraftDependence);
        text.Append('>');

        if (example.LExampleDraftText.LStateValueState == LState.LStateSpecified)
        {
            text.Append(LMarkupLeaf.LMarkupLeafNormalize(
                example.LExampleDraftText.LStateValueShow(), "example"));
        }

        text.Append("</example>\n");
    }
}
