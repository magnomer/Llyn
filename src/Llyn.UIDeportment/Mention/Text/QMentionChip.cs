using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal static class QMentionChip
{
    internal static IReadOnlyList<PMentionChip> QMentionChipCreate(IReadOnlyList<CMentionLabel> labels)
    {
        ArgumentNullException.ThrowIfNull(labels);

        List<PMentionChip> chips = new(labels.Count);
        foreach (CMentionLabel label in labels)
        {
            chips.Add(new PMentionChip(
                label.CMentionLabelId,
                label.CMentionLabelWord,
                label.CMentionLabelKey is string key
                    ? QLocalizationCatalog.QLocalizationTextRead(key)
                    : label.CMentionLabelName,
                label.CMentionLabelSense));
        }

        return chips;
    }
}
