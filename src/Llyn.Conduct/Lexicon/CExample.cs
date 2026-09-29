using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CExample(
    string CExampleLanguage,
    CStateValue CExampleText,
    long? CExampleSource,
    string CExampleCitation,
    string CExampleTally,
    IReadOnlyList<CGlossDraft> CExampleGloss,
    IReadOnlyList<CMentionMark> CExampleExcerpt)
{
    public string CExampleTextHint => LExampleHintRead(CExampleText.CStateValueUncertain);

    internal static string LExampleHintRead(bool uncertain)
    {
        return uncertain ? "Display.Unknown" : "Example.Text";
    }

    internal static CExample LExampleBlankRead(string tally)
    {
        return new CExample(string.Empty, CStateValue.CStateValueEmpty, null, string.Empty, tally, [], []);
    }
}
