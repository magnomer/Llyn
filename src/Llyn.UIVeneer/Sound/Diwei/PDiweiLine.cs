using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PDiweiLine
{
    private PDiweiLine(LDiweiLine line)
    {
        PDiweiLineReading = line.LDiweiLineReading;
        PDiweiLineLabel = line.LDiweiLineLabel;
        PDiweiLineRounded = line.LDiweiLineRounded;
        PDiweiLineCharacters = line.LDiweiLineCharacters;
    }

    public string PDiweiLineReading { get; }

    public string PDiweiLineLabel { get; }

    public bool PDiweiLineRounded { get; }

    public IReadOnlyList<string> PDiweiLineCharacters { get; }

    internal static IReadOnlyList<PDiweiLine> PDiweiLineBuild(IReadOnlyList<LDiweiLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        List<PDiweiLine> built = new(lines.Count);
        foreach (LDiweiLine line in lines)
        {
            built.Add(new PDiweiLine(line));
        }

        return built;
    }
}
