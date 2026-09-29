using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PMentionLine
{
    public ObservableCollection<PMentionChip> PMentionLineChip { get; } = [];

    internal void PMentionLineShow(CAtelier atelier, string text, IReadOnlyList<CMentionDraft> mentions, string silent)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(mentions);

        PMentionLineRefine(atelier.CAtelierMention.CMentionResolve(text, mentions, silent));
    }

    internal void PMentionLineRefine(IReadOnlyList<CMentionLabel> labels)
    {
        ArgumentNullException.ThrowIfNull(labels);

        List<PMentionChip> wanted = new(labels.Count);
        foreach (CMentionLabel label in labels)
        {
            wanted.Add(new PMentionChip(
                label.CMentionLabelId,
                label.CMentionLabelWord,
                label.CMentionLabelKey is string key
                    ? QLocalizationCatalog.QLocalizationTextRead(key)
                    : label.CMentionLabelName,
                label.CMentionLabelSense));
        }

        for (int index = 0; index < wanted.Count; index++)
        {
            if (index >= PMentionLineChip.Count)
            {
                PMentionLineChip.Add(wanted[index]);
            }
            else if (!PMentionLineChip[index].Equals(wanted[index]))
            {
                PMentionLineChip[index] = wanted[index];
            }
        }

        while (PMentionLineChip.Count > wanted.Count)
        {
            PMentionLineChip.RemoveAt(PMentionLineChip.Count - 1);
        }
    }

    internal void PMentionLineClear()
    {
        PMentionLineChip.Clear();
    }
}
