using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Llyn.UIDeportment;

internal sealed class PMentionLine
{
    public ObservableCollection<PMentionChip> PMentionLineChip { get; } = [];

    internal void PMentionLineRefine(IReadOnlyList<PMentionChip> wanted)
    {
        ArgumentNullException.ThrowIfNull(wanted);

        if (PMentionLineChip.SequenceEqual(wanted))
        {
            return;
        }

        PMentionLineChip.Clear();
        foreach (PMentionChip chip in wanted)
        {
            PMentionLineChip.Add(chip);
        }
    }
}
