using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PDiweiItem
{
    private PDiweiItem(LDiweiSection section)
    {
        PDiweiItemLabel = section.LDiweiSectionLabel;
        PDiweiItemLines = PDiweiLine.PDiweiLineBuild(section.LDiweiSectionLines);
        PDiweiItemTallies = PTally.PTallyBuild(section.LDiweiSectionTallies, section.LDiweiSectionRespelled);
        PDiweiItemSwitched = section.LDiweiSectionSwitched;
        PDiweiItemRespelled = section.LDiweiSectionRespelled;
    }

    public string PDiweiItemLabel { get; }

    public IReadOnlyList<PDiweiLine> PDiweiItemLines { get; }

    public IReadOnlyList<PTally> PDiweiItemTallies { get; }

    public bool PDiweiItemSwitched { get; }

    public bool PDiweiItemRespelled { get; }

    internal static IReadOnlyList<PDiweiItem> PDiweiItemBuild(IReadOnlyList<LDiweiSection> sections)
    {
        ArgumentNullException.ThrowIfNull(sections);

        List<PDiweiItem> built = new(sections.Count);
        foreach (LDiweiSection section in sections)
        {
            built.Add(new PDiweiItem(section));
        }

        return built;
    }
}
