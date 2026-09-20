using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

internal sealed class PMentionLine
{
    public ObservableCollection<PMentionChip> PMentionLineChip { get; } = [];

    internal void PMentionLineShow(LWindow window, string text, IReadOnlyList<LMentionDraft> mentions, string silent)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(mentions);

        List<PMentionChip> wanted = new(mentions.Count);
        foreach (LMentionLabel label in window.LWindowMentionResolve(text, mentions))
        {
            wanted.Add(new PMentionChip(
                label.LMentionLabelId,
                label.LMentionLabelWord,
                label.LMentionLabelLinked ? label.LMentionLabelName : silent,
                label.LMentionLabelSense));
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
