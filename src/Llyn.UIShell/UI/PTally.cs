using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PTally
{
    private PTally(string language, string kind, IReadOnlyList<LTallyMark> marks)
    {
        PTallyLanguage = language;
        PTallyKind = kind;
        PTallyMarks = marks;
    }

    public string PTallyLanguage { get; }

    public string PTallyKind { get; }

    public IReadOnlyList<LTallyMark> PTallyMarks { get; }

    internal static IReadOnlyList<PTally> PTallyScan(LTally? tally, bool respelled)
    {
        if (tally is null)
        {
            return [];
        }

        List<PTally> lines = new(tally.LTallyLines.Count);
        foreach (LTallyLine line in tally.LTallyLines)
        {
            IReadOnlyList<LTallyMark> marks = respelled ? line.LTallyLineRespelling : line.LTallyLineIpa;
            if (marks.Count > 0)
            {
                lines.Add(new PTally(line.LTallyLineLanguage, line.LTallyLineKind, marks));
            }
        }

        return lines;
    }
}
