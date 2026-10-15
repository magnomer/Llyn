using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Llyn.UIDeportment;

internal sealed class QMentionLine
{
    public ObservableCollection<QMentionChip> QMentionLineChip { get; } = [];

    internal void QMentionLineRefine(IReadOnlyList<QMentionChip> wanted)
    {
        ArgumentNullException.ThrowIfNull(wanted);

        if (QMentionLineChip.SequenceEqual(wanted))
        {
            return;
        }

        QMentionLineChip.Clear();
        foreach (QMentionChip chip in wanted)
        {
            QMentionLineChip.Add(chip);
        }
    }
}
