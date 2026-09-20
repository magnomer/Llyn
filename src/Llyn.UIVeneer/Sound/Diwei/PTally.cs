using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PTally
{
    private PTally(LTallyLine line, bool respelled)
    {
        PTallyLanguage = line.LTallyLineLanguage;
        PTallyKind = line.LTallyLineKind;
        PTallyMarks = line.LTallyLineRead(respelled);
    }

    public string PTallyLanguage { get; }

    public string PTallyKind { get; }

    public IReadOnlyList<LTallyMark> PTallyMarks { get; }

    internal static IReadOnlyList<PTally> PTallyBuild(IReadOnlyList<LTallyLine> lines, bool respelled)
    {
        ArgumentNullException.ThrowIfNull(lines);

        List<PTally> built = new(lines.Count);
        foreach (LTallyLine line in lines)
        {
            built.Add(new PTally(line, respelled));
        }

        return built;
    }
}
