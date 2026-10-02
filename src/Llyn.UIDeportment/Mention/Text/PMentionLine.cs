using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Llyn.UIDeportment;

internal sealed class PMentionLine
{
    public ObservableCollection<PMentionChip> PMentionLineChip { get; } = [];

    internal void PMentionLineRefine(IReadOnlyList<PMentionChip> wanted)
    {
        ArgumentNullException.ThrowIfNull(wanted);

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
}
