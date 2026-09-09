using System;
using System.Text;

namespace Llyn.Core;

public static class LMarkupExample
{
    public static void LMarkupExampleAppend(StringBuilder text, LSentenceDraft sentence)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(sentence);

        LStateValue written = sentence.LSentenceDraftExample is LExampleDraft example
            ? example.LExampleDraftText
            : LStateValue.LStateValueUnspecified;

        if (written.LStateValueState == LState.LStateUnspecified)
        {
            return;
        }

        text.Append("    <example");
        LMarkupMark.LMarkupMarkAppend(
            text,
            "src",
            sentence.LSentenceDraftExample?.LExampleDraftReference
                ?? LStateValue.LStateValueUnspecified);
        LMarkupMark.LMarkupMarkAppend(text, "par", sentence.LSentenceDraftParticle);
        LMarkupMark.LMarkupMarkAppend(text, "dep", sentence.LSentenceDraftDependence);
        text.Append('>');

        if (written.LStateValueState == LState.LStateSpecified)
        {
            text.Append(LMarkupLeaf.LMarkupLeafNormalize(written.LStateValueShow(), "example"));
        }

        text.Append("</example>\n");
    }
}
